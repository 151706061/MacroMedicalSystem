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
using Macro.Web.Common;

namespace Macro.Web.Services
{
    internal class EventQueue : IDisposable
    {
		private readonly Application _application;

      	private readonly object _syncLock = new object();
		private readonly Queue<Event> _queue;
		private int _nextEventSetNumber = 1;
        
		public EventQueue(Application application)
        {
		    Platform.CheckForNullReference(application, "application");
            _application = application;

            _queue = new Queue<Event>();
		}

		public void Send(Event @event)
		{
		    Platform.CheckForNullReference(@event, "event");
			lock (_syncLock)
			{
				_queue.Enqueue(@event);
				Monitor.Pulse(_syncLock);
			}
		}

        /// <summary>
        /// Check for pending events
        /// </summary>
        /// <param name="wait">Milliseconds</param>
        /// <returns></returns>
        internal EventSet GetPendingEvent(int wait)
        {
            bool debugging = Platform.IsLogLevelEnabled(LogLevel.Debug);

            lock (_syncLock)
            {
                if (debugging)
                    Platform.Log(LogLevel.Debug, "Polling Request received.. {0} in the queue", _queue.Count);

                if (_queue.Count == 0)
                {
                    if (wait == 0)
                    {
                        return null;
                    }

                    if (debugging)
                        Platform.Log(LogLevel.Debug, "Nothing in the queue... waiting for {0} ms", wait);

                    //TODO: What happens if the browser is closed while waiting?
                    if (!Monitor.Wait(_syncLock, wait))
                    {
                        if (debugging)
                            Platform.Log(LogLevel.Debug, "Waiting ends. Still nothing in the queue");

                        return null;
                    }
                }


                const int maxBatchSize = 20;
                Dictionary<Guid, List<Event>> markedEvents = new Dictionary<Guid, List<Event>>();

                List<Event> events = new List<Event>();
                try
                {
                    while (_queue.Count > 0 && events.Count < maxBatchSize)
                    {
                        Event @event = _queue.Peek();


                        // In theory this should never happen
                        if (@event == null)
                        {
                            break;
                        }

                        if (!@event.AllowSendInBatch)
                        {
                            List<Event> speciallyMarkedEvents;
                            Event theEvent = @event;

                            if (_application.BatchMode == MessageBatchMode.PerType)
                            {
                                // Only include the event if there's no other event of the same type to be sent.
                                if (events.Exists(i => i.GetType().Equals(theEvent.GetType())))
                                    break;
                            }
                            else if (_application.BatchMode == MessageBatchMode.PerTarget)
                            {
                                // Note: We can send messages of the same type but targetted to different
                                // entities in the same response instead of separated ones to improve performance. 
                                // One example in the web station is during sync stacking
                                if (markedEvents.TryGetValue(@event.SenderId, out speciallyMarkedEvents))
                                {
                                    if (speciallyMarkedEvents.Find(i => i.GetType() == @theEvent.GetType()) != null)
                                        break;

                                    markedEvents[@event.SenderId].Add(@event);
                                }
                                else
                                {
                                    markedEvents.Add(@event.SenderId, new List<Event>(new[] {@event}));
                                }
                            }
                        }

                        @event = _queue.Dequeue();
                        events.Add(@event);
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }

                if (debugging)
                {
                    Platform.Log(LogLevel.Debug, "Sending Polling Response with {0} events", events.Count);

                    foreach(var e in events)
                    {
                        Platform.Log(LogLevel.Debug, "Event Sent: {0} [{1}]", 
                            e.GetType().Name, e.ToString());
                    }
                }
                if (events.Count > 0)
                {
                    return new EventSet
                    {
                        Events = events.ToArray(),
                        ApplicationId = _application.Identifier,
                        Number = _nextEventSetNumber++,
                        HasMorePending = _queue.Count > 0,
                    };

                }
            }
            return null;
        }

        #region IDisposable Members

		public void Dispose()
		{
			try
			{
				lock(_syncLock)
				{
                    // Release any calls to GetPendingEvent() that may be in a Monitor.Wait()
                	Monitor.Pulse(_syncLock);

                    // CR 3-22-2011, is this necessary and enough time?
                    if (_queue.Count > 0)
                        Monitor.Wait(_syncLock, 5);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Unexpected error disposing EventBroker.");
			}
		}

		#endregion
	}   
}