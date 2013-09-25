#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Web.Common;

namespace Macro.Web.Services
{

    [ServiceBehavior( IncludeExceptionDetailInFaults = true, 
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        AddressFilterMode = AddressFilterMode.Prefix)]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    class ApplicationService : IApplicationService
    {
        static ApplicationService()
        {
            PerformanceMonitor.Initialize();
        }

        private static string GetClientAddress()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return endpoint!=null? endpoint.Address : "Unknown";
        }

        private static Application FindApplication(Guid applicationId)
		{
			Application application = Application.Find(applicationId);
			if (application == null)
			{
                string reason = string.Format("Could not find the specified app id {0}", applicationId);
                throw new FaultException(reason);
			}

			return application;
		}

        /// <summary>
        /// Ensure number of applicatin
        /// </summary>
        private static void CheckNumberOfApplications()
        {
            ApplicationServiceSettings settings = new ApplicationServiceSettings();
            if (settings.MaximumSimultaneousApplications <= 0)
                return;

            Cache cache = Cache.Instance;
            lock (cache.SyncLock)
            {
                // Note: because app is added into the cache ONLY when it has successfully started, there's a small chance that 
                // this check will fail and # of app actually will exceed the max allowed. Decided to live with it for now.
                if (cache.Count >= settings.MaximumSimultaneousApplications)
                {
                    Platform.Log(LogLevel.Warn, "Refuse to start: Max # of Simultaneous Applications ({0}) has been reached.", settings.MaximumSimultaneousApplications);
                    throw new FaultException<OutOfResourceFault>(new OutOfResourceFault { ErrorMessage = SR.MessageMaxApplicationsAllowedExceeded });
                }
            }
        }
        
        private static void CheckMemoryAvailable()
        {
            ApplicationServiceSettings settings = new ApplicationServiceSettings();

            if (settings.MinimumFreeMemoryMB <= 0)
                return;

            bool memoryAvailable = SystemResources.GetAvailableMemory(SizeUnits.Megabytes) > settings.MinimumFreeMemoryMB;

            if (!memoryAvailable)
            {
                string error = String.Format(
                    "Application server out of resources.  Minimum free memory not available ({0}MB required, {1}MB available).",
                    settings.MinimumFreeMemoryMB,
                    SystemResources.GetAvailableMemory(SizeUnits.Megabytes));

                Platform.Log(LogLevel.Warn, error);
                throw new FaultException<OutOfResourceFault>(new OutOfResourceFault { ErrorMessage = error });
            }
        }

        public StartApplicationRequestResponse StartApplication(StartApplicationRequest request)
        {
            CheckNumberOfApplications();
            CheckMemoryAvailable();

            try
            {
                OperationContext operationContext = OperationContext.Current;
                // 5 minute timeout, mostly for debugging.
                operationContext.Channel.OperationTimeout = TimeSpan.FromMinutes(5);

                Application application = Application.Start(request);

                //TODO: when we start allowing application recovery, remove these lines.
                // NOTE: These events are fired only if the underlying connection is permanent (eg, duplex http or net tcp).

                // Commented out per CR 3/22/2011, don't want the contenxt to reference the application
                //operationContext.Channel.Closed += delegate { application.Stop(); };
                //operationContext.Channel.Faulted += delegate { application.Stop(); };

                return new StartApplicationRequestResponse { AppIdentifier = application.Identifier };
            }
            catch (Enterprise.Common.InvalidUserSessionException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (Enterprise.Common.PasswordExpiredException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (Enterprise.Common.UserAccessDeniedException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (Enterprise.Common.RequestValidationException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (Exception ex)
            {
                throw new FaultException(ExceptionTranslator.Translate(ex));
            } 
        }
        
        public ProcessMessagesResult ProcessMessages(MessageSet messageSet)
		{
            IApplication application = FindApplication(messageSet.ApplicationId);
			if (application == null)
				return null;

            try
			{
				return application.ProcessMessages(messageSet);
			}
			catch (Enterprise.Common.InvalidUserSessionException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.PasswordExpiredException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.UserAccessDeniedException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.RequestValidationException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Exception ex)
			{
			    Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages");
				throw new FaultException(ExceptionTranslator.Translate(ex));
			}
		}

		public void StopApplication(StopApplicationRequest request)
		{			
			try
			{
                IApplication application = FindApplication(request.ApplicationId);

                Platform.Log(LogLevel.Info, "Received application shutdown request from {0}", GetClientAddress());
                   
                application.Shutdown();
			}
			catch (Exception ex)
			{
				throw new FaultException(ExceptionTranslator.Translate(ex)); 
			}
		}

        public void ReportPerformance(PerformanceData data)
        {
            PerformanceMonitor.Report(data);
        }

        public GetPendingEventRequestResponse GetPendingEvent(GetPendingEventRequest request)
        {
            IApplication application = Application.Find(request.ApplicationId);

            if (application!=null)
            {
                try
                {
                    var response = new GetPendingEventRequestResponse
                                       {
                                           ApplicationId = application.Identifier,
                                           EventSet = application.GetPendingOutboundEvent(Math.Max(0, request.MaxWaitTime))
                                       };
                
                    return response;
                }
                catch (Exception)
                {
                    // This happens on shutdown, just return an empty response.
                    return new GetPendingEventRequestResponse
                               {
                                   ApplicationId = application.Identifier,
                               };
                }
            }

            // Without a permanent connection, there's a chance the client is polling even when the application has stopped on the server.
            // Throw fault exception to tell the client to stop.
            string reason = string.Format("Could not find the specified ApplicationId: {0}", request.ApplicationId);
            Platform.Log(LogLevel.Error, reason);
            throw new FaultException<InvalidOperationFault>(new InvalidOperationFault(), reason);
        }

        public void SetProperty(SetPropertyRequest request)
        {
            IApplication application = Application.Find(request.ApplicationId);
            if (application != null)
            {
                application.SetProperty(request.Key, request.Value);
            }
        }
    }
}
