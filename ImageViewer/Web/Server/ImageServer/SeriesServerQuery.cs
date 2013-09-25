#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Common;
using Macro.Dicom;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    class SeriesServerQuery : ServerQuery
    {
        public SeriesServerQuery()
            : base(null)
        {            
        }
        public SeriesServerQuery(ServerPartition partition)
            : base(partition)
        {
        }

        /// <summary>
        /// Load the values for the sequence <see cref="DicomTags.RequestAttributesSequence"/>
        /// into a response message for a specific series.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the values into.</param>
        /// <param name="row">The <see cref="Series"/> entity to load the related <see cref="RequestAttributes"/> entity for.</param>
        private static void LoadRequestAttributes(IPersistenceContext read, DicomMessageBase response, Series row)
        {
            var select = read.GetBroker<IRequestAttributesEntityBroker>();

            var criteria = new RequestAttributesSelectCriteria();

            criteria.SeriesKey.EqualTo(row.GetKey());

            IList<RequestAttributes> list = select.Find(criteria);

            if (list.Count == 0)
            {
                response.DataSet[DicomTags.RequestAttributesSequence].SetNullValue();
                return;
            }

            foreach (RequestAttributes request in list)
            {
                var item = new DicomSequenceItem();
                item[DicomTags.ScheduledProcedureStepId].SetStringValue(request.ScheduledProcedureStepId);
                item[DicomTags.RequestedProcedureId].SetStringValue(request.RequestedProcedureId);

                response.DataSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);
            }
        }

        /// <summary>
        /// Populate the data from a <see cref="Series"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row">The <see cref="Series"/> table to populate the row from.</param>
        private void PopulateSeries(IPersistenceContext read, DicomAttributeCollection request, DicomMessageBase response, IEnumerable<uint> tagList,
                                    Series row)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            Study theStudy = Study.Load(read, row.StudyKey);
            StudyStorage storage = StudyStorage.Load(read, theStudy.ServerPartitionKey, theStudy.StudyInstanceUid);
            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(ServerPartitionMonitor.Instance.FindPartition(row.ServerPartitionKey).AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue(storage.StudyStatusEnum == StudyStatusEnum.Nearline
                                                                       ? "NEARLINE"
                                                                       : "ONLINE");

            if (false == String.IsNullOrEmpty(theStudy.SpecificCharacterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(theStudy.SpecificCharacterSet);
                dataSet.SpecificCharacterSet = theStudy.SpecificCharacterSet; // this will ensure the data is encoded using the specified character set
            }

            foreach (uint tag in tagList)
            {
                try
                {
                    switch (tag)
                    {
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(request[DicomTags.PatientId].ToString());
                            break;
                        case DicomTags.StudyInstanceUid:
                            dataSet[DicomTags.StudyInstanceUid].SetStringValue(
                                request[DicomTags.StudyInstanceUid].ToString());
                            break;
                        case DicomTags.SeriesInstanceUid:
                            dataSet[DicomTags.SeriesInstanceUid].SetStringValue(row.SeriesInstanceUid);
                            break;
                        case DicomTags.Modality:
                            dataSet[DicomTags.Modality].SetStringValue(row.Modality);
                            break;
                        case DicomTags.SeriesNumber:
                            dataSet[DicomTags.SeriesNumber].SetStringValue(row.SeriesNumber);
                            break;
                        case DicomTags.SeriesDescription:
                            dataSet[DicomTags.SeriesDescription].SetStringValue(row.SeriesDescription);
                            break;
                        case DicomTags.PerformedProcedureStepStartDate:
                            dataSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue(
                                row.PerformedProcedureStepStartDate);
                            break;
                        case DicomTags.PerformedProcedureStepStartTime:
                            dataSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue(
                                row.PerformedProcedureStepStartTime);
                            break;
                        case DicomTags.NumberOfSeriesRelatedInstances:
                            dataSet[DicomTags.NumberOfSeriesRelatedInstances].AppendInt32(row.NumberOfSeriesRelatedInstances);
                            break;
                        case DicomTags.RequestAttributesSequence:
                            LoadRequestAttributes(read, response, row);
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("SERIES");
                            break;
                        default:
                            dataSet[tag].SetNullValue();
                            break;
                        // Meta tags that should have not been in the RQ, but we've already set
                        case DicomTags.RetrieveAeTitle:
                        case DicomTags.InstanceAvailability:
                        case DicomTags.SpecificCharacterSet:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error setting tag {0} in C-FIND-RSP",
                                 dataSet[tag].Tag.ToString());
                    dataSet[tag].SetNullValue();
                }
            }
        }


        /// <summary>
        /// Method for processing Series level queries.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public override void Query(DicomAttributeCollection message, ServerQueryResultDelegate del)
        {
            //Read context for the query.
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var tagList = new List<uint>();

                var selectSeries = read.GetBroker<ISeriesEntityBroker>();

                //TODO (CR May 2010): Should change so that the Partition AE Title is passed in the RetrieveAeTitle tag in the query message.
                var criteria = new SeriesSelectCriteria();
                if (Partition!=null)
                    criteria.ServerPartitionKey.EqualTo(Partition.Key);

                DicomAttributeCollection data = message;
                foreach (DicomAttribute attrib in message)
                {
                    tagList.Add(attrib.Tag.TagValue);
                    if (!attrib.IsNull)
                        switch (attrib.Tag.TagValue)
                        {
                            case DicomTags.StudyInstanceUid:
                                List<ServerEntityKey> list =
                                    LoadStudyKey(read, (string[])data[DicomTags.StudyInstanceUid].Values);
                                QueryHelper.SetKeyCondition(criteria.StudyKey, list.ToArray());
                                break;
                            case DicomTags.SeriesInstanceUid:
                                QueryHelper.SetStringArrayCondition(criteria.SeriesInstanceUid,
                                                        (string[])data[DicomTags.SeriesInstanceUid].Values);
                                break;
                            case DicomTags.Modality:
                                QueryHelper.SetStringCondition(criteria.Modality, data[DicomTags.Modality].GetString(0, string.Empty));
                                break;
                            case DicomTags.SeriesNumber:
                                QueryHelper.SetStringCondition(criteria.SeriesNumber, data[DicomTags.SeriesNumber].GetString(0, string.Empty));
                                break;
                            case DicomTags.SeriesDescription:
                                QueryHelper.SetStringCondition(criteria.SeriesDescription,
                                                   data[DicomTags.SeriesDescription].GetString(0, string.Empty));
                                break;
                            case DicomTags.PerformedProcedureStepStartDate:
                                QueryHelper.SetRangeCondition(criteria.PerformedProcedureStepStartDate,
                                                  data[DicomTags.PerformedProcedureStepStartDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.PerformedProcedureStepStartTime:
                                QueryHelper.SetRangeCondition(criteria.PerformedProcedureStepStartTime,
                                                  data[DicomTags.PerformedProcedureStepStartTime].GetString(0, string.Empty));
                                break;
                            case DicomTags.RequestAttributesSequence: // todo
                                break;
                        }
                }

                // Open a second read context, in case other queries are required.
                using (IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    selectSeries.Find(criteria, delegate(Series row)
                    {
                        if (CancelReceived)
                            throw new DicomException("DICOM C-Cancel Received");

                        var response = new DicomMessage();
                        PopulateSeries(subRead, message, response, tagList, row);
                        del(response.DataSet);
                    });
                }

                return;
            }
        }

    }
}
