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
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight
{
    internal class ServerEventReceivedEventArgs : EventArgs
    {
        public EventSet EventSet { get; set; }
    }

    internal class ServerChannelFaultEventArgs : EventArgs
    {
        public Exception Error { get; set; }

        public String ErrorMessage { get; set; }
    }

    internal class ServerMessagePoller : IDisposable
    {
        const int MinPollDelaySinceLastActivity = 100; // 100 ms since last activity

        private ServerMessageSender _service;
        private Thread _pollingThread;
        private bool _stop;
        private readonly object _syncLock = new object();
        private int _pendingPollingCount;
        private bool _disposed = false;

        public ServerMessagePoller(ServerMessageSender service)
        {
            _service = service;
            _service.PollCompleted += OnGetPendingEventCompleted;
        }

        public void Start()
        {
            _pollingThread = new Thread(ThreadStart);
            _pollingThread.Name = String.Format("Polling Thread[{0}]", _pollingThread.ManagedThreadId);
            _pollingThread.Start();
        }

        private void ThreadStart(object ignore)
        {
            while (true)
            {
                if (_stop || _service.Faulted)
                {
                    return;
                }

                long now = Environment.TickCount;

                // TimeSpan used to deal with roll over of TickCount
                // Note: Environment.TickCount unit is in ms
                if (TimeSpan.FromMilliseconds(now - ApplicationActivityMonitor.Instance.LastActivityTick) < TimeSpan.FromMilliseconds(MinPollDelaySinceLastActivity))
                {
                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, 50);
                        continue;
                    }
                }

                if (!DoPoll())
                {
                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, 50);
                    }
                }
            }
        }

        private void OnGetPendingEventCompleted(object sender, GetPendingEventCompletedEventArgs e)
        {
            Interlocked.Decrement(ref _pendingPollingCount);

            lock (_syncLock)
            {
                Monitor.PulseAll(_syncLock);
            }          
        }

        private bool DoPoll()
        {
            if (_pendingPollingCount == 0 && _service != null)
            {
                Interlocked.Increment(ref _pendingPollingCount);

                int maxWaitTime = 10000; //ms

                try
                {
                    _service.CheckForPendingEvents(maxWaitTime);

                    lock (_syncLock)
                    {
                        Monitor.Wait(_syncLock, maxWaitTime - 100); // -100 so that another one will go out while the prev one is coming back. -100 = RTT/2
                    } 
                }
                catch (Exception x)
                {
                    // catch exception to prevent crashing
                    Platform.Log(LogLevel.Error, x, "Unexpected exception polling for server data");
                }

                return true;
            }

            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _stop = true;
                if (_service != null)
                {
                    _service.PollCompleted -= OnGetPendingEventCompleted;
                    _service = null;
                }

                if (_pollingThread != null)
                {
                    lock (_syncLock)
                        Monitor.PulseAll(_syncLock);

                    _pollingThread.Join(500);
                    _pollingThread = null;
                }

                _disposed = true;
            }
        }

        #endregion
    }

}
