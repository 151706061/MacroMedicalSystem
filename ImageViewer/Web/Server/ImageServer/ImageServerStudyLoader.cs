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
using Macro.Dicom.Iod;
using Macro.Dicom.Utilities.Xml;
using Macro.ImageServer.Common;
using Macro.ImageServer.Common.Exceptions;
using Macro.ImageServer.Model;
using Macro.ImageViewer.Common;
using Macro.ImageViewer.Common.Auditing;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    //TODO: Delete this since we are now using streaming study loader only?
    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
    public class ImageServerStudyLoader : StudyLoader
    {
        private IEnumerator<InstanceXml> _instances;
        private IDicomServiceNode _ae;
        private StudyStorageLocation _location;
        public ImageServerStudyLoader()
            : base("CC_ImageServer")
        {
            PrefetchingStrategy = new ImageServerPrefetchingStrategy();
        }

        protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
        {
            var ae = studyLoaderArgs.Server as IDicomServiceNode;
            _ae = ae;

            EventResult result = EventResult.Success;
            AuditedInstances loadedInstances = new AuditedInstances();
            try
            {
                ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(_ae.AETitle);
                if (!partition.Enabled)
                    throw new OfflineLoadStudyException(studyLoaderArgs.StudyInstanceUid);    

                FilesystemMonitor.Instance.GetReadableStudyStorageLocation(partition.Key,
                                                                           studyLoaderArgs.StudyInstanceUid,
                                                                           StudyRestore.False,
                                                                           StudyCache.True,
                                                                           out _location);

                StudyXml studyXml = _location.LoadStudyXml();

                _instances = GetInstances(studyXml).GetEnumerator();

                loadedInstances.AddInstance(studyXml.PatientId, studyXml.PatientsName, studyXml.StudyInstanceUid);

                return studyXml.NumberOfStudyRelatedInstances;

            }
            catch (StudyIsNearlineException e)
            {
                throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            finally
            {
                AuditHelper.LogOpenStudies(new[] {ae.AETitle}, loadedInstances, EventSource.CurrentUser, result);
            }
        }

        private IEnumerable<InstanceXml> GetInstances(StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                foreach (InstanceXml instanceXml in seriesXml)
                {
                    yield return instanceXml;
                }
            }
        }

        protected override SopDataSource LoadNextSopDataSource()
        {
            if (!_instances.MoveNext())
                return null;

            return new ImageServerSopDataSource(_instances.Current, _location.GetSopInstancePath(
                                              _instances.Current.Collection[DicomTags.SeriesInstanceUid].ToString(),
                                              _instances.Current.SopInstanceUid));
        }
    }
}
