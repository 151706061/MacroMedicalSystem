#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using System.Windows.Threading;

namespace Macro.Web.Client.Silverlight
{
    /// <summary>
    /// Uses a <see cref="Timer"/> internally to delay-publish an event.
    /// </summary>
    /// <remarks>
    /// This class <B>must</B> be instantiated from within a UI thread; see <see cref="Timer"/> for more details.
    /// </remarks>
    /// <seealso cref="Timer"/>
    public class DelayedEventPublisher<TEventArgs> : IDisposable
        where TEventArgs : EventArgs
    {
        private DispatcherTimer _timer;
        private readonly EventHandler<TEventArgs> _eventHandler;
        private object _lastSender;
        private TEventArgs _lastEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventHandler">The 'true' event handler; calls to which are delayed until 
        /// the timeout has elapsed with no calls to <see cref="Publish(object, TEventArgs)"/>.</param>
        /// <param name="timeout">The timeout</param>
        public DelayedEventPublisher(EventHandler<TEventArgs> eventHandler, TimeSpan timeout)
        {
            _timer = new DispatcherTimer { Interval = timeout };
            _timer.Tick += OnTimeout;
            _eventHandler = eventHandler;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventHandler">The 'true' event handler; calls to which are delayed until 
        /// <paramref name="timeoutMilliseconds"/> has elapsed with no calls to <see cref="Publish(object, TEventArgs)"/>.</param>
        /// <param name="timeoutMilliseconds">The time after which, if <see cref="Publish(object, TEventArgs)"/> has not been called, 
        /// to publish the delayed event via <paramref name="eventHandler"/>.</param>
        public DelayedEventPublisher(EventHandler<TEventArgs> eventHandler, double timeoutMilliseconds)
            : this(eventHandler, TimeSpan.FromMilliseconds(timeoutMilliseconds))
        {
        }

        /// <summary>
        /// Cancels the currently pending delay-published event, if one exists.
        /// </summary>
        public void Cancel()
        {
            _timer.Stop();
            _lastSender = null;
            _lastEvent = null;
        }

        /// <summary>
        /// Delay-publishes an event with the input parameters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Repeated calls to <see cref="Publish(object, TEventArgs)"/> will cause
        /// only the most recent event parameters to be remembered until the delay
        /// timeout has expired, at which time only those event parameters will
        /// be used to publish the delayed event.
        /// </para>
        /// <para>
        /// When a delayed event is published, the <see cref="DelayedEventPublisher{TEventArgs}"/>
        /// goes into an idle state.  The next call to <see cref="Publish(object, TEventArgs)"/>
        /// starts the delayed publishing process over again.
        /// </para>
        /// </remarks>
        public void Publish(object sender, TEventArgs @event)
        {
            lock (_timer)
            {
                _lastSender = sender;
                _lastEvent = @event;

                if (_timer.IsEnabled)
                {
                    //Logger.Write("Delayed event\n");
                    return; //ignore it
                }

                _timer.Start();
            }

        }

        private void OnTimeout(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                lock (_timer)
                {
                    PublishNow();
                }
            }
        }

        /// <summary>
        /// Called to immediately publish the currently pending
        /// delay-published event; the method does nothing if there
        /// is no event pending.
        /// </summary>
        private void PublishNow()
        {
            if (_eventHandler == null || _timer == null)
                return;

            if (_timer.IsEnabled)
            {
                _timer.Stop();

                _eventHandler(_lastSender, _lastEvent);
            }
        }


        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern.
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                lock (_timer)
                {
                    _timer.Stop();
                    _timer = null;
                }
            }
        }

        /// <summary>
        /// True if a delayed event is active
        /// </summary>
        public bool DelayedEvent
        {
            get { return _timer != null && _timer.IsEnabled; }
        }
    }
}
