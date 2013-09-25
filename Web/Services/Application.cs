#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;
using System.Threading;
using Macro.Enterprise.Common;
using Macro.Web.Common;
using Macro.Common;
using Macro.Web.Common.Events;
using Macro.Common.Utilities;
using System.Globalization;

namespace Macro.Web.Services
{
	
	[ExtensionOf(typeof(ExceptionTranslatorExtensionPoint))]
	internal class UserSessionExceptionTranslator : IExceptionTranslator
	{
		#region IExceptionTranslator Members

		public string Translate(Exception e)
		{
			if (e.GetType().Equals(typeof(UserAccessDeniedException)))
				return SR.MessageAccessDenied;
			if (e.GetType().Equals(typeof(PasswordExpiredException)))
				return SR.MessagePasswordExpired;
			if (e.GetType().Equals(typeof(InvalidUserSessionException)))
				return SR.MessageSessionEnded;
			if (e.GetType().Equals(typeof(RequestValidationException)))
				return SR.MessageUnexpectedError;

			return default(string);
		}

		#endregion
	}

	public class ApplicationAttribute : Attribute
	{
		public ApplicationAttribute(Type startRequestType)
		{
			StartRequestType = startRequestType;
		}

		public Type StartRequestType { get; private set; }

		/// <summary>
		/// Determines whether or not this instance is the same as <paramref name="obj"/>, which is itself an <see cref="Attribute"/>.
		/// </summary>
		public override bool Match(object obj)
		{
			var attribute = obj as ApplicationAttribute;
			return attribute != null && attribute.StartRequestType == StartRequestType;
		}
	}

	[ExtensionPoint]
	public class ApplicationExtensionPoint : ExtensionPoint<IApplication>
	{
	}

	public interface IApplication
	{
		Guid Identifier { get; }
		IPrincipal Principal { get; }
		IApplicationContext Context { get; }

        /// <summary>
        /// Starts the application
        /// </summary>
        void Start(StartApplicationRequest request);

        /// <summary>
        /// Processes the specific <see cref="MessageSet"/>
        /// </summary>
        ProcessMessagesResult ProcessMessages(MessageSet messages);

        /// <summary>
        /// Stops the application. <see cref="Shutdown"/> will be called later
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the application. <see cref="Shutdown"/> will be called later
        /// </summary>
        void Stop(string message);

        /// <summary>
        /// Gets pending events
        /// </summary>
	    EventSet GetPendingOutboundEvent(int wait);

        /// <summary>
        /// Updates property
        /// </summary>
        void SetProperty(string key, object value);

        /// <summary>
        /// Shuts down the application
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Name of the application instance for logging purpose.
        /// </summary>
        string InstanceName { get; }
	}

    public enum MessageBatchMode
    {
        /// <summary>
        /// Allow only one single message of the same type to be sent in a batch
        /// </summary>
        PerType,
        
        /// <summary>
        /// Allow messages of the same type to be sent in the same batch if they are sent by different entities.
        /// </summary>
        PerTarget
    }

	public abstract partial class Application : IApplication
	{
		[ThreadStatic]
		private static Application _current;

		protected ApplicationContext _context;

		private string _userName;
	    private volatile UserSessionInfo _session;
		private const int SessionRenewalOffsetMinutes = 1; // renew the session 1 min before it is expired.
		private TimeSpan _sessionPollingIntervalSeconds;
		private volatile int _lastSessionCheckTicks;

        private DateTime _lastClientMessage = DateTime.Now;
		private volatile bool _timedOut;

		private readonly object _syncLock = new object();
		private bool _stop;
		private event EventHandler _stopped;
		internal WebSynchronizationContext _synchronizationContext;

		private readonly IncomingMessageQueue _incomingMessageQueue;

		private static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(5);
		private System.Threading.Timer _timer;
		private bool _timerMethodExecuting;

        public CultureInfo Culture
        {
            private set;
            get;
        }

		protected Application()
		{
		    BatchMode = MessageBatchMode.PerTarget;

		    Identifier = Guid.NewGuid();
			_incomingMessageQueue = new IncomingMessageQueue(
				messageSet => _synchronizationContext.Send(nothing => DoProcessMessages(messageSet), null));
		}

		internal static Application Current
		{
			get { return _current; }
			set
			{
				_current = value;
				if (_current != null && _current.Principal != null)
					Thread.CurrentPrincipal = _current.Principal;
			}
		}

		public Guid Identifier { get; private set; }

		public IApplicationContext Context
		{
			get { return _context; }
		}

		#region Authentication

		public IPrincipal Principal { get; private set; }

		private bool IsSessionShared { get; set; }


        public abstract string InstanceName { get;  }

        public MessageBatchMode BatchMode { get; protected set; }

	    private void AuthenticateUser(StartApplicationRequest request)
		{
			_userName = request.Username;
			if (!String.IsNullOrEmpty(request.SessionId))
			{
				IsSessionShared = request.IsSessionShared;
			}

			_session = UserAuthentication.ValidateSession(request.Username, request.SessionId);
            if (_session == null)
				return;

            if (_session.Principal != null)
                Thread.CurrentPrincipal = Principal = _session.Principal;

		}

		private void Logout()
		{
            if (IsSessionShared || _session == null)
				return;

            UserAuthentication.Logout(_session);
		}

		protected void EnsureSessionIsValid()
		{
            if (_session == null)
				return;

            bool nearExpiry = Platform.Time.Add(TimeSpan.FromMinutes(SessionRenewalOffsetMinutes)) > _session.SessionToken.ExpiryTime;
			TimeSpan timeSinceLastCheck = TimeSpan.FromMilliseconds(Environment.TickCount - _lastSessionCheckTicks);
			if (nearExpiry || timeSinceLastCheck > _sessionPollingIntervalSeconds)
			{
				_lastSessionCheckTicks = Environment.TickCount;
                _session = UserAuthentication.RenewSession(_session);
				OnSessionRenewed();
			}
		}

        protected void CheckIfSessionIsStillValid()
        {
            _session = UserAuthentication.ValidateSession(_session.Principal.Identity.Name, _session.SessionToken.Id);
            _lastSessionCheckTicks = Environment.TickCount;
            
            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Debug, "Session {0} for user {1} is still valid. Will expire on {2}", _session.SessionToken.Id, _session.Principal.Identity.Name, _session.SessionToken.ExpiryTime);                
        }

		private void OnSessionRenewed()
		{
            if (_session == null)
				return;

		    _context.FireEvent(new SessionUpdatedEvent
			{
				Identifier = Guid.NewGuid(),
				SenderId = Identifier,
                ExpiryTimeUtc = _session.SessionToken.ExpiryTime.ToUniversalTime(),
				Username = _userName
			});
		}

		#endregion

		#region IApplication Members

		void IApplication.Start(StartApplicationRequest request)
		{
			throw new InvalidOperationException("Start must be called internally.");
		}

        protected void ProcessMetaInfo(MetaInformation info)
        {
            if (info == null)
            {
                return;
            }

            if (false == string.IsNullOrEmpty(info.Language))
            {
                try
                {
                    Culture = new CultureInfo(info.Language);
                    if (Culture.IsNeutralCulture)
                        Culture = CultureInfo.CreateSpecificCulture(info.Language);

                    Thread.CurrentThread.CurrentCulture = Culture;
                    Thread.CurrentThread.CurrentUICulture = Culture;
                }
                catch (ArgumentException ex)
                {
                    Platform.Log(LogLevel.Warn, "Unable to use language ({0}) requested by the client : {1}", info.Language, ex.Message);
                }
            }
        }

		internal void InternalStart(StartApplicationRequest request)
		{
            try
			{
                ProcessMetaInfo(request.MetaInformation);
				AuthenticateUser(request);
                _synchronizationContext = new WebSynchronizationContext(this);
				_synchronizationContext.Send(nothing => DoStart(request), null);
			}
			catch (Exception)
			{
				Logout();
				DisposeMembers();
				throw;
			}

			lock (_syncLock)
			{
				//DoStart can call Stop.
				if (_stop)
					return;
			}

			_timer = new System.Threading.Timer(OnTimer, null, TimerInterval, TimerInterval);
		}

        private void DoStart(StartApplicationRequest request)
        {
            _context.FireEvent(new ApplicationStartedEvent
                                   {
                                       Identifier = Guid.NewGuid(),
                                       SenderId = Identifier,
                                       StartRequestId = request.Identifier
                                   });

            OnStart(request);

            string logMessage = _session == null
                                    ? String.Format("Application {0} has started.", Identifier)
                                    : String.Format("Application {0} has started (user={1}, session={2}, expiry={3}).",
                                                    Identifier, _userName, _session.SessionToken.Id,
                                                    _session.SessionToken.ExpiryTime);

            ConsoleHelper.Log(LogLevel.Info, ConsoleColor.Green, logMessage);

            OnSessionRenewed();
        }

	    public void Stop()
		{
			Stop("");
		}

        public void Shutdown()
        {
            Stop();
            DisposeMembers();
            //TODO: Remove from cache immediately?
        }


	    public void Stop(string message)
		{
			lock (_syncLock)
			{
				if (_stop)
					return;

				_stop = true;
			}

			_synchronizationContext.Send(ignore => DoStop(message), null);

			try
			{
				lock (_syncLock)
				{
					EventsHelper.Fire(_stopped, this, EventArgs.Empty);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}

			Logout();

			// DisposeMembers(); // Commented out for non-duplex binding since app is now managed by the cache
		}
        
	    public void SetProperty(string key, object value)
	    {
	        Platform.Log(LogLevel.Debug, "Setting {0}={1}", key, value);
	        _context.SetProperty(key, value);
	    }

	    internal void Stop(Exception e)
		{
            Platform.Log(LogLevel.Error, e, "An error has occurred and the application is stopping");
			Stop(ExceptionTranslator.Translate(e));
		}

		private void DoStop(string message)
		{
            try
            {
                OnStop();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
            finally
            {
                _context.FireEvent(new ApplicationStoppedEvent
                                       {
                                           Identifier = Guid.NewGuid(),
                                           SenderId = Identifier,
                                           Message = message,
                                           IsTimedOut = _timedOut
                                       });

                var stopMessage = String.IsNullOrEmpty(message) ? "<none>" : message;
                if (_session == null)
                {
                    Platform.Log(LogLevel.Info, "Application {0} has stopped (message={1}).", Identifier, stopMessage);
                }
                else
                    Platform.Log(LogLevel.Info, "Application {0} has stopped (user={1}, session={2}, message={3}).",
                                      Identifier, _userName, _session.SessionToken.Id, stopMessage);
            }
		}

		private void OnTimer(object nothing)
		{
			try
			{
                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                {
                    Thread.CurrentThread.Name = String.Format("{0} [{1}]", InstanceName, Thread.CurrentThread.ManagedThreadId);
                }

				lock (_syncLock)
				{
					if (_stop || _timerMethodExecuting)
						return;

					_timerMethodExecuting = true;
				}

                Thread.CurrentThread.CurrentCulture = Culture;
                Thread.CurrentThread.CurrentUICulture = Culture;
                
                CheckIfSessionIsStillValid();

                // TODO: REVIEW THIS
                // This is a temporary workaround to ensure the app shuts down when the connection is lost.
                // Although the app will eventually timed out after 11 min (_inactivityTimeoutMinutes),
                // for security reason it's better to detect this situation asap. 
                // Without a permanent connection, we now rely on the timeout and GetPendingEvent 
                // to guess if the client is still alive.
                //
                // Assumption: The client is supposed to send a GetPendingEvent request repeatedly while
                // it is idle. Assume max waiting time for GetPendingEvent is 10 seconds,
                // we can assume the client browser is closed or connection is lost if we don't receive 
                // one for 20 seconds
                if (DateTime.Now - _lastClientMessage > TimeSpan.FromSeconds(20))
                {
                    Stop(String.Format(SR.MessageNoCommunicationFromClientError, _lastClientMessage));
                }
			}
            catch(SessionDoesNotExistException)
            {
                _timedOut = true;
                Stop(SR.MessageSessionEnded);
            }
            catch(SessionExpiredException)
            {
                _timedOut = true;
                Stop(SR.MessageSessionEnded);
            }
			catch (Exception e)
			{
				Stop(e);
			}
			finally
			{
				lock (_syncLock)
				{
					_timerMethodExecuting = false;
				}
			}
		}


	    internal void DisposeMembers()
		{
			//NOTE: This class has purposely been designed such that this method (and these members) do not need to be synchronized.
            //If you were to synchronize this method, it could deadlock with the other objects that can call
			//back into this class.

			try
			{
				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}

				if (_synchronizationContext != null)
				{
					_synchronizationContext.Dispose();
					_synchronizationContext = null;
				}

				if (_context == null)
					return;

				_context.Dispose();
				_context = null;

			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		internal event EventHandler Stopped
		{
			add { lock (_syncLock) { _stopped += value; } }
			remove { lock (_syncLock) { _stopped -= value; } }
		}

		ProcessMessagesResult IApplication.ProcessMessages(MessageSet messageSet)
		{
			lock (_syncLock)
			{
				if (_stop)
					return null;
			}

            //TODO: do this here (which will fault the channel), or inside DoProcessMessages and stop the app?
			EnsureSessionIsValid();

			bool processed = _incomingMessageQueue.ProcessMessageSet(messageSet);

		    return OnProcessMessageEnd(messageSet, processed);
		}


	    private void DoProcessMessages(MessageSet messageSet)
		{
            foreach (var message in messageSet.Messages)
                ProcessMessage(message);
		}

        protected abstract ProcessMessagesResult OnProcessMessageEnd(MessageSet messageSet, bool processed);
		protected abstract void OnStart(StartApplicationRequest request);
		protected abstract void OnStop();
	    protected abstract EventSet OnGetPendingOutboundEvent(int wait);
        

        public EventSet GetPendingOutboundEvent(int wait)
        {
            _lastClientMessage = DateTime.Now;
            EventSet result = OnGetPendingOutboundEvent(wait);
            return result;
        }

		protected virtual void ProcessMessage(Message message)
		{
			IEntityHandler handler = Context.EntityHandlers[message.TargetId];
			if (handler != null)
			{
				handler.ProcessMessage(message);
                _lastClientMessage = DateTime.Now;
            }
			else
			{
				string msg = String.Format("Invalid message target: {0}", message.TargetId);
				Platform.Log(LogLevel.Debug, msg);
				//throw new ArgumentException(msg);
			}
		}

		protected abstract Common.Application GetContractObject();

		#endregion

		#region Static Helpers

		internal static Application Start(StartApplicationRequest request)
		{
			var filter = new AttributeExtensionFilter(new ApplicationAttribute(request.GetType()));
			var app = new ApplicationExtensionPoint().CreateExtension(filter) as IApplication;
			if (app == null)
				throw new NotSupportedException(String.Format("No application extension exists for start request type {0}", request.GetType().FullName));

			if (!(app is Application))
				throw new NotSupportedException("Application class must derive from Application base class.");

			#region Setup

			var application = (Application)app;
			var context = new ApplicationContext(application);
			application._context = context;

			application._sessionPollingIntervalSeconds = TimeSpan.FromSeconds(ApplicationServiceSettings.Default.SessionPollingIntervalSeconds);

			#endregion

			//NOTE: must call start before adding to the cache; we want the app to be fully initialized before it can be accessed from outside this method.
            
            application.InternalStart(request);
			
            Cache.Instance.Add(application);

			return application;
		}

		public static Application Find(Guid identifier)
		{
            var application = Cache.Instance.Find(identifier);
            if (application != null && String.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = String.Format("{0} [{1}]", application.InstanceName, Thread.CurrentThread.ManagedThreadId);
            }

		    return application;
		}

		public static void StopAll(string message)
		{
		    Cache.Instance.StopAndClearAll(message);
		}

		#endregion
	}

}