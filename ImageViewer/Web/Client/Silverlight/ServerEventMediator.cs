#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.Web.Client.Silverlight;
using Macro.Web.Client.Silverlight.Utilities;
using Message = Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference.Message;
using System.Windows.Media;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;

namespace Macro.ImageViewer.Web.Client.Silverlight
{
    /// <summary>
    /// Mediator of Events and Messages sent between the client and server
    /// </summary>
    /// <remarks>
    /// This class is the mediator between the <see cref="ServerMessageSender"/> and <see cref="ServerMessagePoller"/> classes and the core application.
    /// 
    /// </remarks>
	public class ServerEventMediator : IDisposable
	{
        #region Events
        /// <summary>
        /// Event called on a UI Thread when the application stops
        /// </summary>
        public event EventHandler<ServerApplicationStopEventArgs> ServerApplicationStopped;
        /// <summary>
        /// Event called on a UI Thread when a Critical Error occurs
        /// </summary>
        public event EventHandler CriticalError;
        /// <summary>
        /// Event called on a UI Thread when a Critical Error occurs
        /// </summary>
        public event EventHandler WarningEvent;
        /// <summary>
        /// Event called on a UI Thread when a channel is being opened
        /// </summary>
        public event EventHandler ChannelOpening;
        /// <summary>
        /// Event called on a UI Thread when a channel completed opening
        /// </summary>
        public event EventHandler ChannelOpened;
        /// <summary>
        /// Occurs when a tile "HasCapture" property is changed.
        /// </summary>
        public event EventHandler TileHasCaptureChanged;
        #endregion

        #region Members

        ///TODO (CR May 2010): we should be able to get rid of these "type handlers" entirely (I have a design change in mind).
        private readonly Dictionary<Type, EventHandler<ServerEventArgs>> _typeHandlers = new Dictionary<Type, EventHandler<ServerEventArgs>>();
        private readonly Dictionary<Guid, EventHandler<ServerEventArgs>> _sourceHandlers = new Dictionary<Guid, EventHandler<ServerEventArgs>>();
		private readonly object _sync = new object();
		private MessageQueue _queue;
        private Thread _outboundThread;
		private int _nextMessageId = 1;

        private readonly Queue<MessageSet> _outboundQueue = new Queue<MessageSet>();
        private readonly object _outboundQueueSync = new object();

        private readonly Dictionary<int, EventSet> _incomingEventSets = new Dictionary<int, EventSet>();
        private int _nextEventSetNumber = 1;

        private readonly object _incomingEventSync = new object();
        private readonly Dictionary<Guid, long> _timePrevTileUpdateEvent = new Dictionary<Guid, long>();
        private long _renderLoopCount;

        // Poller & Senders
        private ServerMessagePoller _poller;
        private ServerMessageSender _sender;

        private bool _disposed = false;

        #endregion

        #region Public Properties

        public bool Faulted { 
            get
            {
                return _sender == null || _sender.Faulted;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(ApplicationStartupParameters appParameters)
		{
            try
            {
                if (_queue == null)
                    _queue = new MessageQueue(DoSend);

                CompositionTarget.Rendering += CompositionTargetRendering;
				
                _sender = new ServerMessageSender();
                _sender.ApplicationStarted += StartApplicationCompleted;
                _sender.ChannelFaulted += ChannelFaulted;
                _sender.MessageReceived += OnServerEventReceived;
                _sender.ChannelOpening += OnChannelOpening;
                _sender.ChannelOpened += OnChannelOpened;
                _sender.Intialize();
            }
            catch (Exception ex)
            {
                var args = new ServerChannelFaultEventArgs
                               {
                                   Error = ex
                               };
                ChannelFaulted(this,args);
            }
        }


	    /// <summary>
        /// Register an event handler for a specific type of event from the server
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RegisterEventHandler(Type eventType, EventHandler<ServerEventArgs> handler)
        {
            lock (_sync)
            {
                if (_typeHandlers.ContainsKey(eventType))
                {
                    var existingHandlers = _typeHandlers[eventType];
                    handler += existingHandlers;
                    // strange but must replace what's in the list or we will lose it.. 
                    _typeHandlers[eventType] = handler;
                }
                else
                    _typeHandlers.Add(eventType, handler);

                Platform.Log(LogLevel.Debug, "Register event handler for type {0}", eventType);
            }
        }

        //TODO: this doesn't seem to work even though we kind of need it to
        public void UnregisterEventHandler(Type eventType, EventHandler<ServerEventArgs> handler)
        {
            lock (_sync)
            {
                if (!_typeHandlers.ContainsKey(eventType)) return;

                EventHandler<ServerEventArgs> h = _typeHandlers[eventType];
                h -= handler;
                _typeHandlers[eventType] = h;
                Platform.Log(LogLevel.Debug, "Release event handler for type {0}", eventType);
            }
        }

        /// <summary>
        /// Register an event handler from a specific sender/resource from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="handler"></param>
        public void RegisterEventHandler(Guid sender, EventHandler<ServerEventArgs> handler)
        {
            lock (_sync)
            {
                _sourceHandlers.Add(sender, handler);
                Platform.Log(LogLevel.Debug, "Register event handler for {0}", sender);
            }
        }

        /// <summary>
        /// Release an event handler from a specific sender/resource from the server
        /// </summary>
        /// <param name="sender"></param>
        public void UnregisterEventHandler(Guid sender)
        {
            lock (_sync)
            {
                _sourceHandlers.Remove(sender);
                Platform.Log(LogLevel.Debug, "Release event handler for {0}", sender);
            }
        }

        /// <summary>
        /// Start the application
        /// </summary>
        /// <param name="request">The start request.</param>
        public void StartApplication(StartApplicationRequest request)
        {
            if (!Faulted)
                _sender.StartApplication(request);        
        }

        /// <summary>
        /// Publish performance data to the server.
        /// </summary>
        /// <param name="data"></param>
        internal void PublishPerformance(PerformanceData data)
        {
            if (!Faulted)
                _sender.PublishPerformance(data);
        }

        /// <summary>
        /// Stop the Application
        /// </summary>
        public void StopApplication()
        {
            if (!Faulted)
                _sender.StopApplication();
        }

        public void OnTileHasCaptureChanged(Tile tileEntity)
        {
            if (TileHasCaptureChanged != null)
                TileHasCaptureChanged(tileEntity, EventArgs.Empty);
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>true on success, false if the connection is fauled or an exception occurs</returns>
        public bool DispatchMessage(Message message)
        {
            try
            {
                if (!Faulted)
                {
                    if (message is MouseMoveMessage)
                    {
                        PerformanceMonitor.CurrentInstance.IncrementSendLag(1);
                    }
                    else if (message is MouseWheelMessage)
                    {
                        PerformanceMonitor.CurrentInstance.IncrementMouseWheelMsgCount(1);
                    }

                    ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                    _queue.Enqueue(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return false;
        }

        /// <summary>
        /// Handle an exception, the connection to the server will be closed and a message will be displayed on the GUI.
        /// </summary>
        /// <param name="exception"></param>
        public void HandleException(Exception exception)
        {
            string formattedMessage = exception.Message;
            if (!Faulted)
                _sender.StopApplication();

            UIThread.Execute(() =>
            {
                if (CriticalError != null)
                    CriticalError(formattedMessage, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Handle a critical error, the connection to the server will be closed and a message will be displayed on the GUI.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void HandleCriticalError(string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);

            if (!Faulted)
                _sender.StopApplication();

            UIThread.Execute(() =>
            {
                if (CriticalError != null)
                    CriticalError(formattedMessage, EventArgs.Empty);
            });
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

                if (_queue != null)
                {
                    _queue.Stop();

                    if (disposing)
                        _queue.Dispose();
                    _queue = null;
                } 
                
                if (_poller != null)
                {
                    _poller.Dispose();
                    _poller = null;
                } 
                
                if (_sender != null)
                {
                    _sender.ApplicationStarted -= StartApplicationCompleted;
                    _sender.ChannelFaulted -= ChannelFaulted;
                    _sender.MessageReceived -= OnServerEventReceived;
                    
                    if (disposing)
                        _sender.Dispose();

                    _sender = null;
                }

                // outbound thread should shut down after sender is shut down
                if (_outboundThread != null)
                {
                    lock (_outboundQueueSync)
                        Monitor.Pulse(_outboundQueueSync);
                    if (_outboundThread.IsAlive)
                    {
                        _outboundThread.Join(500);
                    }
                    _outboundThread = null;
                }


                CompositionTarget.Rendering -= CompositionTargetRendering;

                _disposed = true;
            }
        }

        #endregion

        #region Private Methods

        private void StartPolling()
        {
            _poller = new ServerMessagePoller(_sender);
            _poller.Start();
        }

        private void StopThreads()
        {
            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
            }

            if (_outboundThread != null)
            {
                if (_outboundThread.IsAlive)
                {
                    _outboundThread.Abort();
                    _outboundThread.Join();
                }
                _outboundThread = null;
            }

            if (_poller != null)
            {
                _poller.Dispose();
                _poller = null;
            }
        }

	    private void StartApplicationCompleted(object sender, StartApplicationCompletedEventArgs e)
        {
            StartPolling();

            _outboundThread = new Thread(ignore => ProcessOutboundQueue());
            _outboundThread.Name = String.Format("Outbound Thread[{0}]", _outboundThread.ManagedThreadId);
            _outboundThread.Start();
        }
   
        private void OnApplicationNotFoundEventReceived(ApplicationNotFoundEvent applicationNotFoundEvent)
        {
            //TODO:  Think about if this is the right solution
            // NOTE: _startRequest contains the sessionid which is probably expired by this time.
            if (_sender.ApplicationId.Equals(applicationNotFoundEvent.ApplicationId))
            {
                HandleCriticalError(ErrorMessages.ApplicationIDNotFound, applicationNotFoundEvent.ApplicationId);
            }
        }

        private void OnApplicationStoppedEventReceived(ApplicationStoppedEvent applicationStoppedEvent)
        {
            try
            {
                UIThread.Execute(() =>
                                     {

                                         if (ServerApplicationStopped != null)
                                         {
                                             ServerApplicationStopped(this,
                                                                      new ServerApplicationStopEventArgs
                                                                          {ServerEvent = applicationStoppedEvent});
                                         }
                                         else
                                         {
                                             HandleCriticalError(applicationStoppedEvent.Message);
                                         }
                                     });
            }
            finally
            {
                if (!Faulted)
                    _sender.Disconnect(SR.ApplicationHasStopped);
                StopThreads();
            }
        }

        private void ChannelFaulted(object sender, ServerChannelFaultEventArgs e)
        {
            string formattedMessage = e.ErrorMessage ?? e.Error.Message;
            UIThread.Execute(() =>
            {
                if (CriticalError != null)
                    CriticalError(formattedMessage, EventArgs.Empty);
            });
        }

        private void OnChannelOpening(object sender, EventArgs e)
        {
            UIThread.Execute(() =>
            {
                if (ChannelOpening != null) ChannelOpening(this, e);
            });
        }

        private void OnChannelOpened(object sender, EventArgs e)
        {
            UIThread.Execute(() =>
            {
                if (ChannelOpened != null) ChannelOpened(this, e);
            });
        }

        private void OnServerEventReceived(object sender, ServerEventReceivedEventArgs eventSet)
        {
            Platform.Log(LogLevel.Debug, "==> {0}: IN MSG # {1}", Environment.TickCount, eventSet.EventSet.Number);

            if (ThrottleSettings.Default.LagDetectionStrategy == LagDetectionStrategy.WhenMouseMoveIsProcessed)
                PerformanceMonitor.CurrentInstance.DecrementSendLag(eventSet.EventSet.Events.Count(i => i is MouseMoveProcessedEvent));

            int tileUpdateEvCount = eventSet.EventSet.Events.Count(i => i is TileUpdatedEvent);
            if (tileUpdateEvCount > 0)
            {
                //if (ThrottleSettings.Default.LagDetectionStrategy == LagDetectionStrategy.WhenTileUpdateReturn)
                //    PerformanceMonitor.CurrentInstance.DecrementSendLag(eventSet.Events.Count((i) => i is TileUpdatedEvent));

                PerformanceMonitor.CurrentInstance.RenderingLag += tileUpdateEvCount;
                if (tileUpdateEvCount > 1)
                    Platform.Log(LogLevel.Debug, "########## {0} tile update events received ###########", tileUpdateEvCount);
            }


            lock (_incomingEventSync)
            {
                _incomingEventSets[eventSet.EventSet.Number] = eventSet.EventSet;
                Monitor.Pulse(_incomingEventSync);
            }
        }

        private void ProcessOutboundQueue()
        {
            while(!Faulted)
            {
                lock (_outboundQueueSync)
                {
                    if (_outboundQueue.Count == 0)
                    {
                        Monitor.Wait(_outboundQueueSync);
                    }
                }


                if (_outboundQueue.Count == 0)
                {
                    continue;
                }

                if (_outboundQueue.Count > 0)
                {
                    MessageSet msgset = _outboundQueue.Dequeue();
                    msgset.Tick = Environment.TickCount;
                    msgset.Timestamp = DateTime.Now;

                    Platform.Log(LogLevel.Debug, "<== Background {0}: MSG # {1}: Count: {2}", msgset.Tick, msgset.Number, msgset.Messages.Count);
                    if (!Faulted)
                        _sender.SendMessages(msgset);
                    Platform.Log(LogLevel.Debug, "<Complete Background {0}: MSG # {1}: Count: {2}", msgset.Tick, msgset.Number, msgset.Messages.Count);

                    ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                }
            }
        }

        private void DoSend(List<Message> msgs)
        {
            if (Faulted)
                return;

            var msgset = new MessageSet
                             {
                                 Messages = new System.Collections.ObjectModel.ObservableCollection<Message>(),
                                 ApplicationId = _sender.ApplicationId,
                                 Number = _nextMessageId++
                             };

            foreach (Message msg in msgs)
            {
                if (msg != null)
                    msgset.Messages.Add(msg);
            }
            try
            {

                // TODO: REVIEW THIS
                // Per MSDN:
                // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                msgset.Tick = Environment.TickCount;
                msgset.Timestamp = DateTime.Now;

                if (ThrottleSettings.Default.SimulateNetworkTrafficOrder)
                {
                    lock (_outboundQueueSync)
                    {
                        _outboundQueue.Enqueue(msgset);
                        Monitor.Pulse(_outboundQueueSync);
                    }
                }
                else
                {
                    Platform.Log(LogLevel.Debug, "<== {0}: MSG # {1}: Count: {2}", msgset.Tick, msgset.Number, msgset.Messages.Count);
                    if (!Faulted)
                        _sender.SendMessages(msgset);
                    Platform.Log(LogLevel.Debug, "<Complete {0}: MSG # {1}: Count: {2}", msgset.Tick, msgset.Number, msgset.Messages.Count);
                  
                    // TODO: REVIEW THIS
                    // Per MSDN:
                    // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                    // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                    ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;
                }
            }
            catch (Exception e)
            {
                // happens on timeout of connection
                HandleException(e);

                // In some case, this is not necessary because the connection is already faulted.
                // But let's do it anyway.
                if (!Faulted) 
                    _sender.Disconnect(e.Message);
            }
        }

		private void ProcessEventSet(EventSet eventSet)
        {
            if (eventSet == null)
                return;

            if (eventSet.Events == null)
                return;

#if DEBUG
            if (eventSet.Events!=null)
            {
                int updateEvents =  eventSet.Events.Count(i => i is TileUpdatedEvent);
                if (updateEvents>1)
                {
                    //UIThread.Execute(()=> System.Windows.MessageBox.Show(String.Format("Multiple tile update events in message set #{0}", eventSet.Number)));
                }
            }
#endif
            
            foreach (Event @event in eventSet.Events)
			{
                var ev = @event;

			    try
				{
                    if (ev is ApplicationNotFoundEvent)
                    {
                        OnApplicationNotFoundEventReceived(ev as ApplicationNotFoundEvent);
                    }
                    else if (ev is ApplicationStoppedEvent)
                    {
                        OnApplicationStoppedEventReceived(ev as ApplicationStoppedEvent);
                    }
                    else
                    {
                        EventHandler<ServerEventArgs> handler;
                        if (_sourceHandlers.TryGetValue(ev.SenderId, out handler))
                        {
                            handler.Invoke(ev.SenderId, new ServerEventArgs { ServerEvent = ev });
                        }
                        else if (_typeHandlers.TryGetValue(ev.GetType(), out handler))
                        {
                            handler.Invoke(ev.SenderId, new ServerEventArgs { ServerEvent = ev });
                        }
                        else
                        {
                            string msg;
                            if (ev is PropertyChangedEvent)
                            {
                                var propChangedEvent = ev as PropertyChangedEvent;
                                var sb = new StringBuilder();
                                sb.AppendLine(String.Format("EventID:{0}", propChangedEvent.Identifier));
                                sb.AppendLine(String.Format("Source: {0} [ID={1}]", propChangedEvent.Sender, propChangedEvent.SenderId));
                                sb.AppendLine(String.Format("Property: {0}", propChangedEvent.PropertyName));
                                sb.AppendLine(String.Format("Value: {0}", propChangedEvent.Value));

                                if (propChangedEvent.DebugInfo != null)
                                {
                                    sb.AppendLine("--------------------------------------");
                                    foreach (string m in propChangedEvent.DebugInfo)
                                    {
                                        sb.AppendLine(m);
                                    }
                                }
                                msg = sb.ToString();
                            }
                            else
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine(String.Format("EventID:{0}", ev.Identifier));
                                sb.AppendLine(String.Format("Source: {0} [ID={1}]", ev.Sender, ev.SenderId));
                                msg = sb.ToString();                                
                            }

                            UIThread.Execute(() =>
                                                 {
                                                     if (WarningEvent != null)
                                                         WarningEvent(msg, EventArgs.Empty);
                                                 });
                        }
                    }
				}
				catch (Exception ex)
				{
					if (!Faulted)
                        HandleException(ex);
                }
			}
		}

        private void CompositionTargetRendering(Object sender, EventArgs e)
        {
            ProcessIncomingQueue();
        }
        
        private void ProcessIncomingQueue()
        {
            lock (_incomingEventSync)
            {
                //TODO: will renderLoopCount overflow?
                _renderLoopCount++;

                EventSet current;
                if (!_incomingEventSets.TryGetValue(_nextEventSetNumber, out current))
                {
                    return;
                }
                    
                bool hasTileUpdateEvent = current.Events.Any(i => i is TileUpdatedEvent);
                TileUpdatedEvent tileUpdateEv = hasTileUpdateEvent ? current.Events.First(i => i is TileUpdatedEvent) as TileUpdatedEvent : null;
                    
                if (hasTileUpdateEvent)
                {
                    long prevTileUpdate;
                    if (_timePrevTileUpdateEvent.TryGetValue(tileUpdateEv.SenderId, out prevTileUpdate))
                    {
                        // For some reason, image on the screen is not updated if it is changed too fast
                        // My guess is it takes another iteration to refresh the UI.
                        //
                        // NOTE: From MSDN http://msdn.microsoft.com/en-us/library/system.windows.media.compositiontarget.rendering.aspx
                        // This event handler gets called once per frame. Each time that Windows Presentation Foundation (WPF) marshals the persisted rendering data in the visual tree across to the composition tree, 
                        // your event handler is called. In addition, if changes to the visual tree force updates to the composition tree, your event handler is also called. 
                        // Note that your event handler is called after layout has been computed. 
                        // However, you can modify layout in your event handler, which means that layout will be computed once more before rendering.
                        //
                        if (ThrottleSettings.Default.EnableFPSCap)
                            if (_renderLoopCount - prevTileUpdate < 3)
                                return;
                    }                   
                }

                _incomingEventSets.Remove(_nextEventSetNumber);
                _nextEventSetNumber = current.Number + 1;
                var ev = current;

                ProcessEventSet(ev); 

                if (hasTileUpdateEvent)
                {
                    _timePrevTileUpdateEvent[tileUpdateEv.SenderId] = _renderLoopCount;
                }
            }
        }

        #endregion
    }

	class MessageQueue : IDisposable
    {
        public delegate void SendDelegate(List<Message> msgs);

		private Thread _thread;
		private volatile bool _stop;
		private readonly object _syncLock = new object();
        private readonly Queue<Message> _queue = new Queue<Message>();
	    private bool _disposed = false;

        public MessageQueue(SendDelegate del)
        {
            _thread = new Thread(ProcessQueue);
			_thread.Name = String.Format("Outbound Message Queue [{0}]", _thread.ManagedThreadId);
			_thread.Start(del);
        }

        public void Enqueue(Message msg)
        {
			lock (_syncLock)
			{
				_queue.Enqueue(msg);
				Monitor.Pulse(_syncLock);
			}
        }

        public int Count
        {
			get
			{
				lock (_syncLock)
				{
					return _queue.Count;
				}
			}
        }

        private void ProcessQueue(object del)
        {
            var sendDelegate = del as SendDelegate;
            while (!_stop)
            {
				const int maxMessages = 2;
                var msgs = new List<Message>();
				lock (_syncLock)
				{
					if (_queue.Count == 0)
						Monitor.Wait(_syncLock);

					while(_queue.Count > 0 && msgs.Count < maxMessages)
						msgs.Add(_queue.Dequeue());

					if (msgs.Count == 0)
						continue;

				}

                // send is called outside the lock block to avoid deadlock when polling duplex http _binding is used
                // it happens when the server for some reason decides to wait for the client to finish processing the "app started" message,
                // and the client attempts to send the "client rect size" msg to the server when it processes he "app started" message.
                //
                // Basic http _binding appears to have the same problem too
                try
                {
                    if (sendDelegate != null) sendDelegate(msgs);
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Error,x,"Unexpected exception with sendDelegate");
                }
            }
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
                Stop();

                _thread = null;

                _disposed = true;
            }
        }

        public void Stop()
        {
            if (_stop)
                return;

            _stop = true;

            lock (_syncLock)
            {
                Monitor.Pulse(_syncLock);
            }

            if (_thread.IsAlive)
                _thread.Join(500);
        }
    }

}


namespace Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference
{

    public partial class MessageSet
    {
        public int Tick;
    }
}
