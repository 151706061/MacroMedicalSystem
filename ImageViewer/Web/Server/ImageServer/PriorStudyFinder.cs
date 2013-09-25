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
using System.Globalization;
using System.ServiceModel;
using System.Text;
using Macro.Common;
using Macro.Dicom.Iod;
using Macro.Dicom.ServiceModel;
using Macro.Dicom.ServiceModel.Query;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageViewer.Common;
using Macro.ImageViewer.Layout.Basic;
using Macro.ImageViewer.StudyManagement;
using Patient=Macro.ImageViewer.StudyManagement.Patient;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(PriorStudyFinderExtensionPoint))]
	public class PriorStudyFinder : ImageViewer.PriorStudyFinder
	{
		private volatile bool _cancel;

        public PriorStudyFinder()
        {
        }

        public override PriorStudyFinderResult FindPriorStudies()
		{
			_cancel = false;

            var priorsServerQueries = new List<IStudyRootQuery>();
            foreach (ServerPartition partition in ServerPartitionMonitor.Instance)
            {
                using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    IDeviceEntityBroker broker = ctx.GetBroker<IDeviceEntityBroker>();
                    DeviceSelectCriteria criteria = new DeviceSelectCriteria();
                    criteria.DeviceTypeEnum.EqualTo(DeviceTypeEnum.PriorsServer);
                    criteria.ServerPartitionKey.EqualTo(partition.Key);
                    IList<Device> list = broker.Find(criteria);
                    foreach (Device theDevice in list)
                    {
                        // Check the settings and log for debug purposes
                        if (!theDevice.Enabled)
                        {
                            Platform.Log(LogLevel.Debug, "Prior Server '{0}' on partition '{1}' is disabled in the device setting",
                                    theDevice.AeTitle, partition.AeTitle);
                            continue;
                        }

                        DicomStudyRootQuery remoteQuery = new DicomStudyRootQuery(partition.AeTitle, theDevice.AeTitle,
                                                                                  theDevice.IpAddress, theDevice.Port);
                        priorsServerQueries.Add(remoteQuery);
                    }
                }
            }

            // Log the prior servers for debug purpose
            if (Platform.IsLogLevelEnabled(LogLevel.Debug) && priorsServerQueries.Count > 0)
            {
                StringBuilder log = new StringBuilder();
                log.Append("Searching for priors on the following servers:");

                StringBuilder serverList = new StringBuilder();
                foreach (DicomStudyRootQuery server in priorsServerQueries)
                {
                    if (serverList.Length > 0)
                        serverList.Append(",");
                    serverList.AppendFormat("{0}", server);
                }

                log.Append(serverList.ToString());
                Platform.Log(LogLevel.Debug, log.ToString());
            }

            StudyItemList results = new StudyItemList();

			DefaultPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();
			List<string> patientIds = new List<string>();
			foreach (Patient patient in Viewer.StudyTree.Patients)
			{
				if (_cancel)
					break;

				IPatientData reconciled = reconciliationStrategy.ReconcileSearchCriteria(patient);
				if (!patientIds.Contains(reconciled.PatientId))
					patientIds.Add(reconciled.PatientId);
			}

            //Note: we don't catch the exception for this one because it's just querying the other
            //partitions, so if it fails we want an outright failure to occur.  Besides, it should never happen
            //if the server is functioning correctly.
			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				foreach (string patientId in patientIds)
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier {PatientId = patientId};

				    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					foreach (StudyRootStudyIdentifier study in studies)
					{
						if (_cancel)
							break;

						StudyItem studyItem = ConvertToStudyItem(study);
						if (studyItem != null)
							results.Add(studyItem);
					}
				}
			}

            bool complete = true;
            foreach (IStudyRootQuery query in priorsServerQueries)
            {
                foreach (string patientId in patientIds)
                {

                    try
                    {
                        var identifier = new StudyRootStudyIdentifier
                                             {
                                                 PatientId = patientId
                                             };

                        IList<StudyRootStudyIdentifier> list = query.StudyQuery(identifier);
                        foreach (StudyRootStudyIdentifier i in list)
                        {
                            if (_cancel)
                                break;

                            StudyItem studyItem = ConvertToStudyItem(i);
                            if (studyItem != null)
                                results.Add(studyItem);
                        }
                    }
                    catch (FaultException<DataValidationFault> ex)
                    {
                        complete = false;
                        Platform.Log(LogLevel.Error, ex, "An error has occurred when searching for prior studies on server '{0}'", query.ToString());
                    }
                    catch (FaultException<QueryFailedFault> ex)
                    {
                        complete = false;
                        Platform.Log(LogLevel.Error, ex, "An error has occurred when searching for prior studies on server '{0}'", query.ToString());
                    }
                }
            }

            return new PriorStudyFinderResult(results, complete);
		}

		public override void Cancel()
		{
			_cancel = true;
		}

		private StudyItem ConvertToStudyItem(StudyRootStudyIdentifier study)
		{
			string studyLoaderName;
		    ApplicationEntity applicationEntity;

		    ServerPartition partiton = ServerPartitionMonitor.Instance.GetPartition(study.RetrieveAeTitle);
			if (partiton != null)
			{
                studyLoaderName = WebViewerServices.Default.StudyLoaderName;
                string host = WebViewerServices.Default.ArchiveServerHostname;
                int port = WebViewerServices.Default.ArchiveServerPort;
                int headerPort = WebViewerServices.Default.ArchiveServerHeaderPort;
                int wadoPort = WebViewerServices.Default.ArchiveServerWADOPort;

			    applicationEntity = new ApplicationEntity()
			                            {
                                            AETitle = study.RetrieveAeTitle,
			                                ScpParameters = new ScpParameters(host, port),
			                                StreamingParameters = new StreamingParameters(headerPort, wadoPort)
			                            };

			}
            else
			{
			    Device theDevice = FindServer(study.RetrieveAeTitle);

                if (theDevice != null)
			    {
		            // TODO (Marmot) - Need to get this to work with changes in marmot
			        applicationEntity = new ApplicationEntity() { ScpParameters = new ScpParameters(theDevice.IpAddress, theDevice.Port), AETitle = theDevice.AeTitle };
			    }
			    else // (node == null)
			    {
			        Platform.Log(LogLevel.Warn,
			                     String.Format("Unable to find server information '{0}' in order to load study '{1}'",
			                                   study.RetrieveAeTitle, study.StudyInstanceUid));

			        return null;
			    }
			}

		    var item = new StudyItem(study, ServiceNodeExtensions.ToServiceNode(applicationEntity));
			if (String.IsNullOrEmpty(item.InstanceAvailability))
				item.InstanceAvailability = "ONLINE";

			return item;
		}

		private static Device FindServer(string retrieveAeTitle)
		{
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var broker = ctx.GetBroker<IDeviceEntityBroker>();
                var criteria = new DeviceSelectCriteria();
                criteria.AeTitle.EqualTo(retrieveAeTitle);
                IList<Device> list = broker.Find(criteria);
                foreach (Device theDevice in list)
                {
                    if (string.Compare(theDevice.AeTitle, retrieveAeTitle, false, CultureInfo.InvariantCulture) == 0)
                        return theDevice;
                }
            }

			return null;
		}
	}
}
