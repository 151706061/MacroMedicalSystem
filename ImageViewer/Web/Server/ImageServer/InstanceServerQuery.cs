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
using System.Text.RegularExpressions;
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.Utilities.Xml;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    class InstanceServerQuery : ServerQuery
    {
        public InstanceServerQuery()
            : base(null)
        {            
        }
        public InstanceServerQuery(ServerPartition partition)
            : base(partition)
        {
        }

        /// <summary>
        /// Populate at the IMAGE level a response message.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="theInstanceStream"></param>
        private void PopulateInstance(DicomAttributeCollection request, DicomMessage response, List<uint> tagList,
                                      InstanceXml theInstanceStream)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            DicomAttributeCollection sourceDataSet = theInstanceStream.Collection;

            if (false == sourceDataSet.Contains(DicomTags.SpecificCharacterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(sourceDataSet[DicomTags.SpecificCharacterSet].ToString());
                dataSet.SpecificCharacterSet = sourceDataSet[DicomTags.SpecificCharacterSet].ToString(); // this will ensure the data is encoded using the specified character set
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
                            dataSet[DicomTags.SeriesInstanceUid].SetStringValue(
                                request[DicomTags.SeriesInstanceUid].ToString());
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                            break;
                        default:
                            if (sourceDataSet.Contains(tag))
                                dataSet[tag] = sourceDataSet[tag].Copy();
                            else
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
        /// Compare at the IMAGE level if a query matches the data in an <see cref="InstanceXml"/> file.
        /// </summary>
        /// <param name="queryMessage"></param>
        /// <param name="matchTagList"></param>
        /// <param name="instanceStream"></param>
        /// <returns></returns>
        private static bool CompareInstanceMatch(DicomAttributeCollection queryMessage, IEnumerable<uint> matchTagList,
                                                 InstanceXml instanceStream)
        {
            foreach (uint tag in matchTagList)
            {
                if (!instanceStream.Collection.Contains(tag))
                    continue;

                DicomAttribute sourceAttrib = queryMessage[tag];
                DicomAttribute matchAttrib = instanceStream.Collection[tag];

                if (sourceAttrib.Tag.VR.Equals(DicomVr.SQvr))
                    continue; // TODO

                if (sourceAttrib.IsNull)
                    continue;

                string sourceString = sourceAttrib.ToString();
                if (sourceString.Contains("*") || sourceString.Contains("?"))
                {
                    sourceString = sourceString.Replace("*", "[\x21-\x7E]");
                    sourceString = sourceString.Replace("?", ".");
                    if (!Regex.IsMatch(matchAttrib.ToString(), sourceString))
                        return false;
                }
                else if (!sourceAttrib.Equals(matchAttrib))
                    return false;
            }

            return true;
        }

        private void LoadStudyPartition(string studyInstanceUid)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyEntityBroker selectStudy = read.GetBroker<IStudyEntityBroker>();

                StudySelectCriteria criteria = new StudySelectCriteria();
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
                Study theStudy = selectStudy.FindOne(criteria);

                Partition = ServerPartition.Load(read, theStudy.ServerPartitionKey);
            }
        }

        /// <summary>
        /// Method for processing Image level queries.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public override void Query(DicomAttributeCollection message, ServerQueryResultDelegate del)
        {
            List<uint> tagList = new List<uint>();
            List<uint> matchingTagList = new List<uint>();

            DicomAttributeCollection data = message;
            string studyInstanceUid = data[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string seriesInstanceUid = data[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);

            //TODO (CR May 2010): Should change so that the Partition AE Title is passed in the RetrieveAeTitle tag in the query message.
            LoadStudyPartition(studyInstanceUid);

            StudyStorageLocation location;
            FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, studyInstanceUid, StudyRestore.True,
                                                                           StudyCache.True, out location);

            // Will always return a value, although it may be an empty StudyXml file
            StudyXml studyXml = location.LoadStudyXml();

            SeriesXml seriesXml = studyXml[seriesInstanceUid];
            if (seriesXml == null)
            {
                throw new DicomException("Unable to find series");
            }

            foreach (DicomAttribute attrib in message)
            {
                if (attrib.Tag.TagValue == DicomTags.QueryRetrieveLevel)
                    continue;

                tagList.Add(attrib.Tag.TagValue);
                if (!attrib.IsNull)
                    matchingTagList.Add(attrib.Tag.TagValue);
            }

            foreach (InstanceXml theInstanceStream in seriesXml)
            {
                if (CompareInstanceMatch(message, matchingTagList, theInstanceStream))
                {
                    DicomMessage response = new DicomMessage();
                    PopulateInstance(message, response, tagList, theInstanceStream);
                    del(response.DataSet);
                }
            }

            return;
        }
    }
}
