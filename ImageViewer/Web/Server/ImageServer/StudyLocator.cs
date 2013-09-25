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
using System.ServiceModel;
using System.Threading;
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.ServiceModel.Query;
using Macro.ImageServer.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Core.ModelExtensions;
using Macro.Web.Enterprise.Authentication;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConfigurationName = "StudyLocator", Namespace = QueryNamespace.Value)]
    public class StudyLocator : IStudyRootQuery
    {
        private class ImageServerQuery
        {
            public ServerPartition Partition { get; private set; }
            public StudyServerQuery StudyQuery { get; private set; }
            public SeriesServerQuery SeriesQuery { get; private set; }
            public InstanceServerQuery InstanceQuery { get; private set; }

            public ImageServerQuery(ServerPartition partition)
            {
                Partition = partition;
                StudyQuery = new StudyServerQuery(partition);
                SeriesQuery = new SeriesServerQuery(partition);
                InstanceQuery = new InstanceServerQuery(partition);
            }
        }

        private readonly List<ImageServerQuery> _partitionQueryList = new List<ImageServerQuery>();
        public StudyLocator()
        {
            foreach (ServerPartition partition in ServerPartitionMonitor.Instance)
            {
                var accessAllowed = false;
                var webUser = Thread.CurrentPrincipal as CustomPrincipal;
                if (webUser != null)
                {
                    accessAllowed = partition.IsUserAccessAllowed(webUser);
                }

                if (webUser==null || accessAllowed)
                    _partitionQueryList.Add(new ImageServerQuery(partition));
            }
        }

        #region IStudyRootQuery Members


        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            CheckMinimumPermission();

            if (!string.IsNullOrEmpty(queryCriteria.RetrieveAeTitle))
            {
                // AE Title is specificed but is the user allowed?
                CheckPartitionPermission(queryCriteria.RetrieveAeTitle);
            }

            Dictionary<string, StudyRootStudyIdentifier> combinedResults = new Dictionary<string, StudyRootStudyIdentifier>();

            //TODO (CR May 2010): Should change so that the Partition AE Title is passed in the RetrieveAeTitle tag in the query message.

            foreach (ImageServerQuery query in _partitionQueryList)
            {
                IList<StudyRootStudyIdentifier> list = Query(queryCriteria, query.StudyQuery);
                foreach (StudyRootStudyIdentifier i in list)
                {
                	//TODO (CR May 2010): is it ok to include the same study multiple times in a study query response?
                    
                    // Note: Include entries from different partitions if they have the same Study Instance Uid.
                    // The caller will filter the list again using the AE title.
                    string key = string.Format("{0}/{1}", i.RetrieveAeTitle, i.StudyInstanceUid);
                    if (!combinedResults.ContainsKey(key))
                    {
                        if (!query.Partition.Enabled)
                            i.InstanceAvailability = "OFFLINE";
                        combinedResults.Add(key, i);
                    }
                }
            }

            return new List<StudyRootStudyIdentifier>(combinedResults.Values);
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            CheckMinimumPermission();


            Dictionary<string, SeriesIdentifier> combinedResults = new Dictionary<string, SeriesIdentifier>();

            foreach (ImageServerQuery query in _partitionQueryList)
            {
                IList<SeriesIdentifier> list = Query(queryCriteria, query.SeriesQuery);
                foreach (SeriesIdentifier i in list)
                {
                	//TODO (CR May 2010): should this be keyed on the RetrieveAE/UID as well?
                    if (!combinedResults.ContainsKey(i.SeriesInstanceUid))
                    {
                        if (!query.Partition.Enabled)
                            i.InstanceAvailability = "OFFLINE";

                        combinedResults.Add(i.SeriesInstanceUid, i);
                    }
                }
            }

            return new List<SeriesIdentifier>(combinedResults.Values);
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            CheckMinimumPermission();

            Dictionary<string, ImageIdentifier> combinedResults = new Dictionary<string, ImageIdentifier>();

            foreach (ImageServerQuery query in _partitionQueryList)
            {
                IList<ImageIdentifier> list = Query(queryCriteria, query.InstanceQuery);
                foreach (ImageIdentifier i in list)
                {
					//TODO (CR May 2010): should this be keyed on the RetrieveAE/UID as well?
                    // There's a bug in InstanceServerQuery which causes results only to be returned
                    // from one partition.  This bug should be fixed also.
					if (!combinedResults.ContainsKey(i.SopInstanceUid))
                    {
                        if (!query.Partition.Enabled)
                            i.InstanceAvailability = "OFFLINE";
                        combinedResults.Add(i.SopInstanceUid, i);
                    }
                }
            }

            return new List<ImageIdentifier>(combinedResults.Values);
        }

        #endregion

        #region Private Methods

        private void CheckMinimumPermission()
        {
            if (_partitionQueryList == null || _partitionQueryList.Count == 0)
                throw new PermissionDeniedException(SR.ExceptionPermissionDenied);
        }

        private void CheckPartitionPermission(string aeTitle)
        {
            var webUser = Thread.CurrentPrincipal as CustomPrincipal;
            if (webUser == null)
                return;

            foreach(var partition in ServerPartitionMonitor.Instance)
            {
                if (partition.AeTitle.Equals(aeTitle, StringComparison.InvariantCulture))
                {
                    if (!partition.IsUserAccessAllowed(webUser))
                    {
                        throw new PermissionDeniedException(string.Format("User {0} does not have permission to access partition {1}", webUser.DisplayName, partition.AeTitle));
                    }
                }
            }
        }

        #endregion

        private static IList<TIdentifier> Query<TIdentifier, TFindScu>(TIdentifier queryCriteria, TFindScu scu)
            where TIdentifier : Identifier, new()
            where TFindScu : ServerQuery
        {
            if (queryCriteria == null)
            {
                const string message = "The query identifier cannot be null.";
                Platform.Log(LogLevel.Error, message);
                throw new FaultException(message);
            }

            IList<DicomAttributeCollection> scuResults;
            DicomAttributeCollection criteria;
            try
            {
                criteria = queryCriteria.ToDicomAttributeCollection();
            }
            catch (DicomException e)
            {
                DataValidationFault fault = new DataValidationFault
                                                {
                                                    Description =
                                                        "Failed to convert contract object to DicomAttributeCollection."
                                                };
                Platform.Log(LogLevel.Error, e, fault.Description);
                throw new FaultException<DataValidationFault>(fault, fault.Description);
            }
            catch (Exception e)
            {
                DataValidationFault fault = new DataValidationFault
                                                {
                                                    Description =
                                                        "Unexpected exception when converting contract object to DicomAttributeCollection."
                                                };
                Platform.Log(LogLevel.Error, e, fault.Description);
                throw new FaultException<DataValidationFault>(fault, fault.Description);
            }

            try
            {
                scuResults = new List<DicomAttributeCollection>();
                scu.Query(criteria, result => scuResults.Add(result));
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                QueryFailedFault fault = new QueryFailedFault
                                             {
                                                 Description = String.Format("An unexpected error has occurred ({0})",
                                                                             e.Message)
                                             };
                Platform.Log(LogLevel.Error, e, fault.Description);
                throw new FaultException<QueryFailedFault>(fault, fault.Description);
            }


            List<TIdentifier> results = new List<TIdentifier>();
            foreach (DicomAttributeCollection result in scuResults)
            {
                TIdentifier identifier = Identifier.FromDicomAttributeCollection<TIdentifier>(result);
                if (String.IsNullOrEmpty(identifier.RetrieveAeTitle))
                    identifier.RetrieveAeTitle = scu.Partition.AeTitle;

                results.Add(identifier);
            }

            return results;
        }

        public override string ToString()
        {
            return "ImageServerPartitions";
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (ImageServerQuery query in _partitionQueryList)
            {
                query.SeriesQuery.Dispose();
                query.InstanceQuery.Dispose();
                query.StudyQuery.Dispose();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion
    }
}