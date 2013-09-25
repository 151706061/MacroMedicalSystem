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
using System.Threading;
using Macro.Common;
using Macro.Dicom;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;
using Macro.Web.Enterprise.Authentication;
using AuthorityTokens = Macro.ImageServer.Enterprise.Authentication;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    class StudyServerQuery : ServerQuery
    {
        public StudyServerQuery() : base(null)
        {            
        }

        public StudyServerQuery(ServerPartition partition) : base(partition)
        {
        }

        /// <summary>
        /// Load the values for the tag <see cref="DicomTags.ModalitiesInStudy"/> into a response
        /// message for a specific <see cref="Study"/>.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the value into.</param>
        /// <param name="key">The <see cref="ServerEntityKey"/> for the <see cref="Study"/>.</param>
        private static void LoadModalitiesInStudy(IPersistenceContext read, DicomMessageBase response, ServerEntityKey key)
        {
            var select = read.GetBroker<IQueryModalitiesInStudy>();

            var parms = new ModalitiesInStudyQueryParameters { StudyKey = key };

            IList<Series> list = select.Find(parms);

            string value = "";
            foreach (Series series in list)
            {
                value = value.Length == 0
                    ? series.Modality
                    : String.Format("{0}\\{1}", value, series.Modality);
            }
            response.DataSet[DicomTags.ModalitiesInStudy].SetStringValue(value);
        }

        
        /// <summary>
        /// Populate the data from a <see cref="Study"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row">The <see cref="Study"/> table to populate the response from.</param>
        /// <param name="availability">Instance availability string.</param>
        private void PopulateStudy(IPersistenceContext read, DicomMessageBase response, IEnumerable<uint> tagList, Study row, string availability)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(ServerPartitionMonitor.Instance.FindPartition(row.ServerPartitionKey).AeTitle);

            dataSet[DicomTags.InstanceAvailability].SetStringValue(availability);

            if (false == String.IsNullOrEmpty(row.SpecificCharacterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(row.SpecificCharacterSet);
                dataSet.SpecificCharacterSet = row.SpecificCharacterSet; // this will ensure the data is encoded using the specified character set
            }
            foreach (uint tag in tagList)
            {
                try
                {
                    switch (tag)
                    {
                        case DicomTags.StudyInstanceUid:
                            dataSet[DicomTags.StudyInstanceUid].SetStringValue(row.StudyInstanceUid);
                            break;
                        case DicomTags.PatientsName:
                            dataSet[DicomTags.PatientsName].SetStringValue(row.PatientsName);
                            break;
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(row.PatientId);
                            break;
                        case DicomTags.PatientsBirthDate:
                            dataSet[DicomTags.PatientsBirthDate].SetStringValue(row.PatientsBirthDate);
                            break;
                        case DicomTags.PatientsAge:
                            dataSet[DicomTags.PatientsAge].SetStringValue(row.PatientsAge);
                            break;
                        case DicomTags.PatientsSex:
                            dataSet[DicomTags.PatientsSex].SetStringValue(row.PatientsSex);
                            break;
                        case DicomTags.StudyDate:
                            dataSet[DicomTags.StudyDate].SetStringValue(row.StudyDate);
                            break;
                        case DicomTags.StudyTime:
                            dataSet[DicomTags.StudyTime].SetStringValue(row.StudyTime);
                            break;
                        case DicomTags.AccessionNumber:
                            dataSet[DicomTags.AccessionNumber].SetStringValue(row.AccessionNumber);
                            break;
                        case DicomTags.StudyId:
                            dataSet[DicomTags.StudyId].SetStringValue(row.StudyId);
                            break;
                        case DicomTags.StudyDescription:
                            dataSet[DicomTags.StudyDescription].SetStringValue(row.StudyDescription);
                            break;
                        case DicomTags.ReferringPhysiciansName:
                            dataSet[DicomTags.ReferringPhysiciansName].SetStringValue(row.ReferringPhysiciansName);
                            break;
                        case DicomTags.NumberOfStudyRelatedSeries:
                            dataSet[DicomTags.NumberOfStudyRelatedSeries].AppendInt32(row.NumberOfStudyRelatedSeries);
                            break;
                        case DicomTags.NumberOfStudyRelatedInstances:
                            dataSet[DicomTags.NumberOfStudyRelatedInstances].AppendInt32(
                                row.NumberOfStudyRelatedInstances);
                            break;
                        case DicomTags.ModalitiesInStudy:
                            LoadModalitiesInStudy(read, response, row.Key);
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
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
        /// Method for processing Study level queries.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public override void Query(DicomAttributeCollection message, ServerQueryResultDelegate del)
        {
            var tagList = new List<uint>();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var find = read.GetBroker<IStudyEntityBroker>();

                //TODO (CR May 2010): Should change so that the Partition AE Title is passed in the RetrieveAeTitle tag in the query message.

                var criteria = new StudySelectCriteria();
                if (Partition !=null)
                    criteria.ServerPartitionKey.EqualTo(Partition.Key);

                if (!Thread.CurrentPrincipal.IsInRole(Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies))
                {
                    var principal = Thread.CurrentPrincipal as CustomPrincipal;
                    if (principal != null)
                    {
                        var oidList = new List<ServerEntityKey>();
                        foreach (var oid in principal.Credentials.DataAccessAuthorityGroups)
                            oidList.Add(new ServerEntityKey("OID", oid));
                        var dataAccessGroupSelectCriteria = new DataAccessGroupSelectCriteria();
                        dataAccessGroupSelectCriteria.AuthorityGroupOID.In(oidList);
                        IList<DataAccessGroup> groups;
                        using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            var broker = context.GetBroker<IDataAccessGroupEntityBroker>();
                            groups = broker.Find(dataAccessGroupSelectCriteria);
                        }

                        var entityList = new List<ServerEntityKey>();
                        foreach (DataAccessGroup group in groups)
                        {
                            entityList.Add(group.Key);
                        }

                        var dataAccessSelectCriteria = new StudyDataAccessSelectCriteria();
                        dataAccessSelectCriteria.DataAccessGroupKey.In(entityList);

                        criteria.StudyDataAccessRelatedEntityCondition.Exists(dataAccessSelectCriteria);
                    }
                }



                DicomAttributeCollection data = message;
                foreach (DicomAttribute attrib in message)
                {
                    tagList.Add(attrib.Tag.TagValue);
                    if (!attrib.IsNull)
                        switch (attrib.Tag.TagValue)
                        {
                            case DicomTags.StudyInstanceUid:
                                QueryHelper.SetStringArrayCondition(criteria.StudyInstanceUid,
                                                        (string[]) data[DicomTags.StudyInstanceUid].Values);
                                break;
                            case DicomTags.PatientsName:
                                QueryHelper.SetStringCondition(criteria.PatientsName,
                                                   data[DicomTags.PatientsName].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientId:
                                QueryHelper.SetStringCondition(criteria.PatientId,
                                                   data[DicomTags.PatientId].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientsBirthDate:
                                QueryHelper.SetRangeCondition(criteria.PatientsBirthDate,
                                                  data[DicomTags.PatientsBirthDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientsSex:
                                QueryHelper.SetStringCondition(criteria.PatientsSex,
                                                   data[DicomTags.PatientsSex].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyDate:
                                QueryHelper.SetRangeCondition(criteria.StudyDate,
                                                  data[DicomTags.StudyDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyTime:
                                QueryHelper.SetRangeCondition(criteria.StudyTime,
                                                  data[DicomTags.StudyTime].GetString(0, string.Empty));
                                break;
                            case DicomTags.AccessionNumber:
                                QueryHelper.SetStringCondition(criteria.AccessionNumber,
                                                   data[DicomTags.AccessionNumber].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyId:
                                QueryHelper.SetStringCondition(criteria.StudyId, data[DicomTags.StudyId].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyDescription:
                                QueryHelper.SetStringCondition(criteria.StudyDescription,
                                                   data[DicomTags.StudyDescription].GetString(0, string.Empty));
                                break;
                            case DicomTags.ReferringPhysiciansName:
                                QueryHelper.SetStringCondition(criteria.ReferringPhysiciansName,
                                                   data[DicomTags.ReferringPhysiciansName].GetString(0, string.Empty));
                                break;
                            case DicomTags.ModalitiesInStudy:
                                // Specify a subselect on Modality in series
                                var seriesSelect = new SeriesSelectCriteria();
                                QueryHelper.SetStringArrayCondition(seriesSelect.Modality,
                                                        (string[]) data[DicomTags.ModalitiesInStudy].Values);
                                criteria.SeriesRelatedEntityCondition.Exists(seriesSelect);
                                break;
                        }
                }

                // Open another read context, in case additional queries are required.
                using (IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    // First find the Online studies
                    var storageCriteria = new StudyStorageSelectCriteria();
                    storageCriteria.StudyStatusEnum.NotEqualTo(StudyStatusEnum.Nearline);
                    storageCriteria.QueueStudyStateEnum.NotIn(new[]
                                                                  {
                                                                      QueueStudyStateEnum.DeleteScheduled,
                                                                      QueueStudyStateEnum.WebDeleteScheduled,
                                                                      QueueStudyStateEnum.EditScheduled
                                                                  });
                    criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);

                    find.Find(criteria, delegate(Study row)
                                            {
                                                var response = new DicomMessage();

                                            	//TODO (CR May 2010): should the availability be NEARLINE?  The criteria above was for ONLINE studies.
                                                PopulateStudy(subRead, response, tagList, row, "NEARLINE");
                                                del(response.DataSet);
                                            });

                    // Now find the Nearline studies
                    storageCriteria = new StudyStorageSelectCriteria();
                    storageCriteria.StudyStatusEnum.EqualTo(StudyStatusEnum.Nearline);
                    storageCriteria.QueueStudyStateEnum.NotIn(new[]
                                                                  {
                                                                      QueueStudyStateEnum.DeleteScheduled,
                                                                      QueueStudyStateEnum.WebDeleteScheduled,
                                                                      QueueStudyStateEnum.EditScheduled
                                                                  });
                    criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);

                    find.Find(criteria, delegate(Study row)
                    {
                        var response = new DicomMessage();
                        PopulateStudy(subRead, response, tagList, row, "NEARLINE");
                        del(response.DataSet);
                    });

                }
            }

            return;
        }

    }
}
