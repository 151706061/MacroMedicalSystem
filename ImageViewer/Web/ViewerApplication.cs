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
using System.ServiceModel.Channels;
using System.Text;
using Macro.Common;
using Macro.Common.Configuration;
using Macro.Desktop;
using Macro.Dicom.ServiceModel;
using Macro.Dicom.ServiceModel.Query;
using Macro.ImageViewer.Common;
using Macro.ImageViewer.Common.DicomServer;
using Macro.ImageViewer.StudyManagement;
using Macro.ImageViewer.Web.Common.Messages;
using Macro.Web.Common;
using Macro.Web.Common.Events;
using Macro.Web.Services;
using Macro.ImageViewer.Web.Common;
using Macro.ImageViewer.Web.EntityHandlers;
using Macro.ImageViewer.Web.Common.Entities;
using Application=Macro.Desktop.Application;
using Macro.Common.Utilities;
using Message=Macro.Web.Common.Message;

namespace Macro.ImageViewer.Web
{
	//TODO (CR May 2010): resource strings.

	[ExtensionOf(typeof(ExceptionTranslatorExtensionPoint))]
	internal class ExceptionTranslator : IExceptionTranslator
	{
		#region IExceptionTranslator Members

		public string Translate(Exception e)
		{
			//TODO (CR April 2011): Figure out how to share the Exception Policies for these messages ...
			//Current ExceptionHandler/Policy design just doesn't work for this at all.
			if (e.GetType().Equals(typeof(InUseLoadStudyException)))
				return ImageViewer.SR.MessageLoadStudyFailedInUse;
			if (e.GetType().Equals(typeof(NearlineLoadStudyException)))
			{
				return ((NearlineLoadStudyException)e).IsStudyBeingRestored
                    ? ImageViewer.SR.MessageLoadStudyFailedNearline : ImageViewer.SR.MessageLoadStudyFailedNearlineNoRestore;
			}
			if (e.GetType().Equals(typeof(OfflineLoadStudyException)))
                return ImageViewer.SR.MessageLoadStudyFailedOffline;
			if (e.GetType().Equals(typeof(NotFoundLoadStudyException)))
                return ImageViewer.SR.MessageLoadStudyFailedNotFound;
			if (e.GetType().Equals(typeof(LoadStudyException)))
				return SR.MessageStudyCouldNotBeLoaded;
			if (e is LoadMultipleStudiesException)
				return ((LoadMultipleStudiesException)e).GetUserMessage();

			if (e.GetType().Equals(typeof(NoVisibleDisplaySetsException)))
				return ImageViewer.SR.MessageNoVisibleDisplaySets;

			if (e.GetType().Equals(typeof(PatientStudiesNotFoundException)))
				return SR.MessagePatientStudiesNotFound;
			if (e.GetType().Equals(typeof(AccessionStudiesNotFoundException)))
				return SR.MessageAccessionStudiesNotFound;
			if (e.GetType().Equals(typeof(InvalidRequestException)))
				return e.Message;
            if (e.GetType().Equals(typeof(PermissionDeniedException)))
                return e.Message;
			return null;
		}

		#endregion
	}

    public class PermissionDeniedException : Exception
    {
        public PermissionDeniedException(string detail)
            : base(detail)
		{
		}
    }

	internal class PatientStudiesNotFoundException : Exception
	{
		public PatientStudiesNotFoundException(string patientId)
			:base(String.Format("Studies for patient '{0}' could not be found.", patientId))
		{
		}
	}

	internal class AccessionStudiesNotFoundException : Exception
	{
		public AccessionStudiesNotFoundException(string accession)
			: base(String.Format("Studies matching accession '{0}' could not be found.", accession))
		{
		}
	}

	internal class InvalidRequestException : Exception
	{
		public InvalidRequestException()
			: base("The request must contain at least one valid study instance uid, patient id, or accession#.")
		{
		}
	}

    internal class RemoteClientInformation
    {
        public string IPAddress {get;set;}
    }

	[Application(typeof(StartViewerApplicationRequest))]
	[ExtensionOf(typeof(ApplicationExtensionPoint))]
	public class ViewerApplication : Macro.Web.Services.Application
	{
        private static readonly object _syncLock = new object();
		private Common.ViewerApplication _app;
		private ImageViewerComponent _viewer;
		private EntityHandler _viewerHandler;

	    private readonly RemoteClientInformation _client;
        
        public  ViewerApplication()
        {
            _client = new RemoteClientInformation
                          {
                              IPAddress = GetClientAddress(OperationContext.Current)
                          };

        }

        public override string InstanceName
        {
            get
            {
                return String.Format("WebStation (user={0}, ip={1})",
                                         Principal != null ? Principal.Identity.Name : "Unknown",
                                         _client.IPAddress);   

            }
        }

		private static IList<StudyRootStudyIdentifier> FindStudies(StartViewerApplicationRequest request)
		{
			bool invalidRequest = true;
			List<StudyRootStudyIdentifier> results = new List<StudyRootStudyIdentifier>();

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				if (request.StudyInstanceUid != null && request.StudyInstanceUid.Length > 0)
				{
					foreach (string studyUid in request.StudyInstanceUid)
					{
						//TODO (CR May 2010): can actually trigger a query of all studies
						if (!String.IsNullOrEmpty(studyUid))
							invalidRequest = false;

                        //TODO (CR May 2010): if request.AeTitle is set, assign RetrieveAeTitle parameter in 
                        // StudyRootStudyIndentifer to this value.  Update the query code to then only
                        // search this specified partition and remove the loop code below that looks
                        // for matching AeTitles.
						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              StudyInstanceUid = studyUid
						                                          };
					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);

					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new NotFoundLoadStudyException(studyUid);
                    }
				}
				if (request.PatientId != null && request.PatientId.Length > 0)
				{
					foreach (string patientId in request.PatientId)
					{
						if (!String.IsNullOrEmpty(patientId))
							invalidRequest = false;

						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              PatientId = patientId
						                                          };

					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new PatientStudiesNotFoundException(patientId);
                    }
				}
				if (request.AccessionNumber != null && request.AccessionNumber.Length > 0)
				{
					foreach (string accession in request.AccessionNumber)
					{
						if (!String.IsNullOrEmpty(accession))
							invalidRequest = false;

						StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier
						                                          {
						                                              AccessionNumber = accession
						                                          };

					    IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);

					    bool found = false;
						foreach (StudyRootStudyIdentifier study in studies)
						{
						    if (!string.IsNullOrEmpty(request.AeTitle) && !study.RetrieveAeTitle.Equals(request.AeTitle)) continue;

						    results.Add(study);
						    found = true;
						}

                        if (!found)
                            throw new AccessionStudiesNotFoundException(accession);
                    }
				}
			}

			if (invalidRequest)
				throw new InvalidRequestException();

			return results;
		}

		private static bool AnySopsLoaded(IImageViewer imageViewer)
		{
			foreach (Patient patient in imageViewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					foreach (Series series in study.Series)
					{
						foreach (Sop sop in series.Sops)
						{
							return true;
						}
					}
				}
			}

			return false;
		}


	    protected override EventSet OnGetPendingOutboundEvent(int wait)
	    {
            if (_context == null)
            {
                string reason = string.Format("Application context no longer exists");
                throw new Exception(reason);
            }

            return _context.GetPendingOutboundEvent(wait);
	    }


	    protected override ProcessMessagesResult OnProcessMessageEnd(MessageSet messageSet, bool messageWasProcessed)
	    {
            if (!messageWasProcessed)
            {
                return new ProcessMessagesResult { EventSet = null, Pending = true };
            }
            
            bool hasMouseMoveMsg = false;
            foreach (Message m in messageSet.Messages)
            {
                if (m is MouseMoveMessage)
                {
                    hasMouseMoveMsg = true;
                    break;
                }
            }
            EventSet ev = GetPendingOutboundEvent(hasMouseMoveMsg ? 100 : 0);

            return new ProcessMessagesResult { EventSet = ev, Pending = false };
	    }

	    protected override void OnStart(StartApplicationRequest request)
		{
			lock (_syncLock)
			{
                Platform.Log(LogLevel.Info, "Viewer Application is starting...");
                if (Application.Instance == null)
					Platform.StartApp("Macro.Desktop.Application",new string[] {"-r"});
			}

            


            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Debug, "Finding studies...");
			var startRequest = (StartViewerApplicationRequest)request;
			IList<StudyRootStudyIdentifier> studies = FindStudies(startRequest);

			List<LoadStudyArgs> loadArgs = CollectionUtils.Map(studies, (StudyRootStudyIdentifier identifier) => CreateLoadStudyArgs(identifier));

		    DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("", Identifier.ToString());
            WebDesktopWindow window = new WebDesktopWindow(args, Application.Instance);
            window.Open();

            _viewer = CreateViewerComponent(startRequest);

			try
			{
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Loading study...");
                _viewer.LoadStudies(loadArgs);
			}
			catch (Exception e)
			{
				if (!AnySopsLoaded(_viewer)) //end the app.
                    throw;

				//Show an error and continue.
				ExceptionHandler.Report(e, window);
			}

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Info, "Launching viewer...");
			
			ImageViewerComponent.Launch(_viewer, window, "");

			_viewerHandler = EntityHandler.Create<ViewerEntityHandler>();
			_viewerHandler.SetModelObject(_viewer);
		    _app = new Common.ViewerApplication
		               {
		                   Identifier = Identifier,
		                   Viewer = (Viewer) _viewerHandler.GetEntity(),

                           VersionString = GetProductVersionString()
                                
			           };
            

            // Push the ViewerApplication object to the client
            Event @event = new PropertyChangedEvent
            {
                PropertyName = "Application",
                Value = _app,
                Identifier = Guid.NewGuid(),
                SenderId = request.Identifier
            };

            ApplicationContext.Current.FireEvent(@event);
		}

        private static string GetProductVersionString()
        {
            if (ProductInformation.Name.Equals(ProductInformation.Component))
                return ProductInformation.GetNameAndVersion(false, false);

            return string.Format("{0}\n{1}\n{2}", ProductInformation.Name,
                            Concatenate(ProductInformation.Component, String.Format("v{0}", ProductInformation.GetVersion(false, true))),
                            Concatenate(ProductInformation.Edition, ProductInformation.Release));
        }

        private static string Concatenate(params string[] strings)
        {
            if (strings == null || strings.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var s in strings)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static LoadStudyArgs CreateLoadStudyArgs(StudyRootStudyIdentifier identifier)
        {

            // TODO: Need to think about this more. What's the best way to swap different loader?
            // Do we need to support loading studies from multiple servers? 

            string host = WebViewerServices.Default.ArchiveServerHostname;
            int port = WebViewerServices.Default.ArchiveServerPort;

            int headerPort = WebViewerServices.Default.ArchiveServerHeaderPort;
            int wadoPort = WebViewerServices.Default.ArchiveServerWADOPort;

            var serverAe = new ApplicationEntity
                               {
                                   ScpParameters = new ScpParameters(host, port),
                                   StreamingParameters = new StreamingParameters(headerPort, wadoPort),
                                   AETitle = identifier.RetrieveAeTitle
                               };


            // TODO (Marmot) - Need to figure out how this works with the changes for Marmot
            return new LoadStudyArgs(identifier.StudyInstanceUid, ServiceNodeExtensions.ToServiceNode(serverAe));
        }

	    private ImageViewerComponent CreateViewerComponent(StartViewerApplicationRequest request)
        {
            var keyImagesOnly = request.LoadStudyOptions != null && request.LoadStudyOptions.KeyImagesOnly;
            var excludePriors = request.LoadStudyOptions != null && request.LoadStudyOptions.ExcludePriors;

            if (keyImagesOnly)
            {
                var layoutManager = new ImageViewer.Layout.Basic.LayoutManager() { LayoutHook = new KeyImageLayoutHook() };
                //override the KO options
                const string ko = "KO";
                var realOptions = new KeyImageDisplaySetCreationOptions(layoutManager.DisplaySetCreationOptions[ko]);
                layoutManager.DisplaySetCreationOptions[ko] = realOptions;


                if (excludePriors)
                {
                    return new ImageViewerComponent(layoutManager, PriorStudyFinder.Null); 
                }
                else
                {
                    return new ImageViewerComponent(layoutManager);
                }
            }
            else
            {
                ImageViewer.Layout.Basic.LayoutManager layoutManager;

                layoutManager = new ImageViewer.Layout.Basic.LayoutManager()
                {
                    LayoutHook = (request.LoadStudyOptions!=null && request.LoadStudyOptions.PreferredLayout != null)
                                ? new CustomLayoutHook(request.LoadStudyOptions.PreferredLayout.Rows, request.LoadStudyOptions.PreferredLayout.Columns)
                                : new CustomLayoutHook()
                };
                
                if (excludePriors) 
                {
                    return new ImageViewerComponent(layoutManager, PriorStudyFinder.Null);
                }
                else
                {
                    return new ImageViewerComponent(layoutManager); 
                } 
            }


        }

	    protected override void OnStop()
		{
			if (_viewerHandler != null)
			{
				_viewerHandler.Dispose();
				_viewerHandler = null;
			}

			//TODO (CR May 2010): WebDesktopWindow shouldn't hang around, but we can check.
			if (_viewer != null)
			{
				_viewer.Stop();
				_viewer.Dispose();
				_viewer = null;
			}
		}

		protected override Macro.Web.Common.Application GetContractObject()
		{
			return _app;
		}


        private static string GetClientAddress(OperationContext context)
        {
            if (context == null)
                return "Unknown";

            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return endpoint != null ? endpoint.Address : "Unknown";
        }
        
	}

    [ExtensionOf(typeof(SettingsStoreExtensionPoint))]
    public class StandardSettingsProvider : ISettingsStore
    {
		public bool IsOnline
		{
			get { return true; }
		}
		
		public bool SupportsImport
        {
            get { return false; }
        }

        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            return new List<SettingsGroupDescriptor>();
        }

        public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
        {
            return null;
        }

        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            return new List<SettingsPropertyDescriptor>();
        }

        public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            throw new NotSupportedException();
        }

        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            return new Dictionary<string, string>();
        }

        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {

        }

        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            //throw new NotSupportedException();
        }
    }

    
}
