using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Windows.Browser;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight
{
    internal class ServerMessageSender : IDisposable
    {
        #region Members
        
        private ApplicationServiceClient _proxy;
        private bool _connectionOpened;
        private Binding _binding;
        private readonly object _sync = new object();
        private bool _disposed = false;

        #endregion

        #region Public Events

        public event EventHandler<ServerEventReceivedEventArgs> MessageReceived;
        public event EventHandler<ServerChannelFaultEventArgs> ChannelFaulted;
        public event EventHandler<StartApplicationCompletedEventArgs> ApplicationStarted;
        public event EventHandler<GetPendingEventCompletedEventArgs> PollCompleted;
        public event EventHandler ChannelOpening;
        public event EventHandler ChannelOpened;
        #endregion

        #region Public Properties
        public bool Faulted
        {
            get
            {
                lock (_sync)
                {
                    if (_proxy != null && _proxy.State != CommunicationState.Faulted && _proxy.InnerChannel != null &&
                        _proxy.InnerChannel.State != CommunicationState.Faulted)
                        return false;
                }
                return true;
            }
        }

        public Guid ApplicationId { get; private set; }

        #endregion

        #region Public Methods

        public void Intialize()
        {
            SetupChannel();
        }

        public void StartApplication(StartApplicationRequest request)
        {
            if (_proxy != null)
            {
                var rq = request as StartViewerApplicationRequest;
                //_startRequest = request;
                if (rq!=null && rq.StudyInstanceUid.Count > 0 )
                    Platform.Log(LogLevel.Info, "Sending Start Application request to server for Study: {0}",rq.StudyInstanceUid[0]);
                else
                    Platform.Log(LogLevel.Info, "Sending Start Application request to server");

                request.MetaInformation = new MetaInformation
                                              {
                                                  Language = Thread.CurrentThread.CurrentUICulture.Name
                                              };
                _proxy.StartApplicationAsync(request);
            }
            else
            {
                Platform.Log(LogLevel.Error,"Request for Start Application when no proxy client was created");
            }
        }

        public void StopApplication()
        {
            if (_connectionOpened)
            {
                _connectionOpened = false;

                if (!Faulted)
                {
                    try
                    {
                        Platform.Log(LogLevel.Info, "Sending Stop Application request to server for application: {0}",
                                     ApplicationId);
                        _proxy.StopApplicationAsync(new StopApplicationRequest {ApplicationId = ApplicationId});
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception stopping connection.");
                    }
                }
                else
                {
                    Platform.Log(LogLevel.Info,
                                 "Received Stop Application request on faulted channel for application: {0}",
                                 ApplicationId);
                    Disconnect("StopApplication Request");
                }
            }
        }

        public void PublishPerformance(PerformanceData data)
        {
            if (!Faulted)
                _proxy.ReportPerformanceAsync(data);
        }

        public void SendMessages(MessageSet set)
        {
            if (!Faulted)
                _proxy.ProcessMessagesAsync(set, set);
        }

        public void CheckForPendingEvents(int maxWaitTime)
        {
            if (!Faulted && _connectionOpened)
                _proxy.GetPendingEventAsync(new GetPendingEventRequest { ApplicationId = ApplicationId, MaxWaitTime = maxWaitTime });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ThrottleSettings.Default.PropertyChanged -= ThrottleSettingsPropertyChanged;

                Disconnect(SR.Disposing);
                _disposed = true;
            }
        }

        public void Disconnect(string reason)
        {
            if (_proxy != null)
            {
                
                lock (_sync)
                {
                    if (_proxy != null)
                    {
                        ReleaseChannel();
                        _proxy = null;
                    }
                }
            }
        }

        #endregion

        #region Proxy Events

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            if (_connectionOpened)
            {
                var args = new ServerChannelFaultEventArgs
                               {
                                   ErrorMessage = _connectionOpened
                                                      ? ErrorMessages.ConnectionLost
                                                      : String.Format(ErrorMessages.UnableToConnectTo,
                                                                      _proxy.InnerChannel.RemoteAddress.Uri)
                               };
                if (ChannelFaulted != null) ChannelFaulted(this, args);
            }
            _connectionOpened = false;
        }

        private void StartApplicationCompleted(object sender, StartApplicationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnError(e.Error);
                return;
            }
            ApplicationId = e.Result.AppIdentifier;

            Platform.Log(LogLevel.Info, "Application start completed: {0}", ApplicationId);

            ThrottleSettings.Default.PropertyChanged += ThrottleSettingsPropertyChanged;

            //TODO: put the key in some assembly that can be shared with the server-side code
            _proxy.SetPropertyAsync(new SetPropertyRequest
            {
                ApplicationId = ApplicationId,
                Key = "DynamicImageQualityEnabled",
                Value = ThrottleSettings.Default.EnableDynamicImageQuality.ToString()
            });

            if (ApplicationStarted != null) ApplicationStarted(this, e);
        }
       
        private void OnChannelOpening(object sender, EventArgs e)
        {
            if (ChannelOpening != null) ChannelOpening(this, EventArgs.Empty);            
        }

        private void ConnectionOpened(object sender, EventArgs e)
        {
            _connectionOpened = true;

            if (ChannelOpened != null) ChannelOpened(this, EventArgs.Empty);
        }

        private void StopApplicationCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    OnError(e.Error);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Received Stop Application Completed message for application: {0}", ApplicationId);
                }
                Disconnect("Application Shut Down");
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unexpected exception cleanup up connection.");
            }
        }

        private void MessageSent(object sender, ProcessMessagesCompletedEventArgs e)
        {
            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
            var msgs = e.UserState as MessageSet;
            if (msgs!=null)
                PerformanceMonitor.CurrentInstance.DecrementMouseWheelMsgCount(msgs.Messages.Count(i => i is MouseWheelMessage));

            if (e.Error != null)
            {
                OnError(e.Error);
                return;
            }

            if (e.Result != null)
            {
                if (e.Result.EventSet != null && e.Result.EventSet.Events != null)
                {
                    bool isMoveMoveMsg = msgs != null && msgs.Messages.Any(i => i is MouseMoveMessage);
                    if (isMoveMoveMsg)
                    {

                        long dt = Environment.TickCount - msgs.Tick;
                        var p = PerformanceMonitor.CurrentInstance;

                        //bool tileUpdateEventReturned = e.Result.EventSet != null && e.Result.EventSet.Events.Any((i) => i is TileUpdatedEvent);

                        p.LogMouseMoveRTTWithResponse(msgs.Number, dt);
                    }

                    if (e.Result.EventSet != null)
                    {
                        if (!_connectionOpened)
                        {
                            Platform.Log(LogLevel.Error, "Received messages after connection has been closed!");
                        }
                        else if (MessageReceived != null)
                        {
                            var args = new ServerEventReceivedEventArgs
                                           {
                                               EventSet = e.Result.EventSet
                                           };
                            MessageReceived(this, args);
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error, "Received messages without message event registered!");
                        }
                    }
                }
            }
        }

        private void OnGetPendingEventCompleted(object sender, GetPendingEventCompletedEventArgs e)
        {
            if (PollCompleted != null)
                PollCompleted(this, e);

            if (e.Error != null)
            {
                OnError(e.Error);
            }
            else
            {
                if (e.Result != null)
                {
                    if (e.Result.EventSet != null)
                    {
                        if (!_connectionOpened)
                        {
                            Platform.Log(LogLevel.Error, "Received messages after connection has been closed!");
                        }
                        else if (MessageReceived != null)
                        {
                            MessageReceived(this, new ServerEventReceivedEventArgs {EventSet = e.Result.EventSet});
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error, "Received messages without message event registered!");
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Methods

        private void ThrottleSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("EnableDynamicImageQuality"))
            {
                _proxy.SetPropertyAsync(new SetPropertyRequest
                {
                    ApplicationId = ApplicationId,
                    Key = "DynamicImageQualityEnabled",
                    Value = ThrottleSettings.Default.EnableDynamicImageQuality.ToString()
                });
            }
        }

        private void OnError(Exception exception)
        {
            if (!_connectionOpened)
            {
                Platform.Log(LogLevel.Error, exception, "Received exception after the connection has closed.");
                return;
            }
          
            var errorArgs = new ServerChannelFaultEventArgs
                                {
                                    Error = exception
                                };

            if (exception is FaultException<SessionValidationFault>)
            {
                var fault = exception as FaultException<SessionValidationFault>;
                errorArgs.ErrorMessage = fault.Detail.ErrorMessage;
            }
            else if (exception is FaultException<OutOfResourceFault>)
            {
                var fault = exception as FaultException<OutOfResourceFault>;
                errorArgs.ErrorMessage = fault.Detail.ErrorMessage;
            }
            else if (exception is CommunicationException)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    errorArgs.ErrorMessage = ErrorMessages.ConnectionLost;
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(String.Format("{0}: {1}", exception.GetType(), exception.Message));
                    sb.AppendLine(SR.StackTrace + ":");
                    sb.AppendLine(exception.StackTrace);


                    if (exception.InnerException != null)
                    {
                        sb.AppendLine(String.Format("{0}: {1}", exception.InnerException.GetType(), exception.InnerException.Message));
                        sb.AppendLine(SR.StackTrace + ":");
                        sb.AppendLine(exception.InnerException.StackTrace);
                    }

                    errorArgs.ErrorMessage = sb.ToString();
                }
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine(String.Format("{0}", exception.Message));
                sb.AppendLine(SR.StackTrace + ":");
                sb.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    sb.AppendLine(String.Format("{0}: {1}", exception.InnerException.GetType(), exception.InnerException.Message));
                    sb.AppendLine(SR.StackTrace + ":");
                    sb.AppendLine(exception.InnerException.StackTrace);
                }

                errorArgs.ErrorMessage = sb.ToString();
            }

            Platform.Log(LogLevel.Error,"Received error on channel: {0}", errorArgs.ErrorMessage);

            if (ChannelFaulted != null) ChannelFaulted(this, errorArgs);

            _connectionOpened = false;
        }

        private void SetupChannel()
        {
            var binding = GetChannelBinding();
            var remoteAddress = GetServerAddress();
            _proxy = new ApplicationServiceClient(binding, remoteAddress);

            _proxy.InnerChannel.Faulted += OnChannelFaulted;
            _proxy.InnerChannel.Opening += OnChannelOpening;
            _proxy.InnerChannel.Opened += ConnectionOpened;

            _proxy.ProcessMessagesCompleted += MessageSent;
            _proxy.StartApplicationCompleted += StartApplicationCompleted;
            _proxy.StopApplicationCompleted += StopApplicationCompleted;
            _proxy.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
            _proxy.GetPendingEventCompleted += OnGetPendingEventCompleted;

            _connectionOpened = true;
        }


        private EndpointAddress GetServerAddress()
        {
            switch (ApplicationStartupParameters.Current.Mode)
            {
                case ApplicationServiceMode.BasicHttp:
                    const string uri = "../Services/ApplicationService.svc/basicHttp";
                    var address = new EndpointAddress(new Uri(uri, UriKind.RelativeOrAbsolute));
                    return address;
            }

            // This should never occur
            throw new Exception();
        }

        private Binding GetChannelBinding()
        {
            switch (ApplicationStartupParameters.Current.Mode)
            {
                case ApplicationServiceMode.BasicHttp:
                    var binaryMessageEncoding = new BinaryMessageEncodingBindingElement();

                    if (HtmlPage.Document.DocumentUri.Scheme.Equals(Uri.UriSchemeHttp))
                    {
                        var http = new HttpTransportBindingElement
                                       {
                                           MaxReceivedMessageSize = int.MaxValue,
                                           MaxBufferSize = int.MaxValue,
                                           TransferMode = TransferMode.Buffered
                                       };
                        _binding = new CustomBinding(binaryMessageEncoding, http);
                        return _binding;
                    }
                    var https = new HttpsTransportBindingElement
                                    {
                                        MaxReceivedMessageSize = int.MaxValue,
                                        MaxBufferSize = int.MaxValue,
                                        TransferMode = TransferMode.Buffered
                                    };
                    _binding = new CustomBinding(binaryMessageEncoding, https);
                    return _binding;
            }

            return null;
        }

        private void ReleaseChannel()
        {
            if (_proxy != null)
            {
                _proxy.InnerChannel.Opened -= ConnectionOpened;
                _proxy.InnerChannel.Opening -= OnChannelOpening;
                _proxy.ProcessMessagesCompleted -= MessageSent;
                _proxy.StartApplicationCompleted -= StartApplicationCompleted;
                _proxy.StopApplicationCompleted -= StopApplicationCompleted;
                _proxy.GetPendingEventCompleted -= OnGetPendingEventCompleted;
                _proxy.InnerChannel.Faulted -= OnChannelFaulted;
                _proxy.CloseAsync();
                _proxy = null;
                _connectionOpened = false;
            }
        }
        #endregion
    }
}
