#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using Macro.Common.Utilities;
using Macro.Common;

namespace Macro.Web.Services
{
    /// <summary>
    /// Can only be used on the "UI" thread by casting SynchronizationContext.Current to this interface.
    /// </summary>
    public interface IWebSynchronizationContext
    {
        void RunModal();
        void BreakModal();
    }

    public class WebSynchronizationContext : SynchronizationContext, IWebSynchronizationContext, IDisposable
    {
        private class Command
        {
            public Command(SendOrPostCallback callback, object state)
            {
                Callback = callback;
                State = state;
            }

            private SendOrPostCallback Callback { get; set; }
            private object State { get; set; }

            private readonly object _waitLock = new object();
            private volatile bool _executed;

            public void Wait()
            {
                if (_executed)
                    return;

                lock (_waitLock)
                {
                    if (!_executed)
                        Monitor.Wait(_waitLock);
                }
            }

            public void Execute()
            {
                try
                {
                    Callback(State);
                }
                finally
                {
                    ReleaseWait();
                }
            }

            public void ReleaseWait()
            {
                lock (_waitLock)
                {
                    _executed = true;
                    Monitor.Pulse(_waitLock);
                }
            }
        }

        private Application _application;
        private BlockingQueue<Command> _queue;
        private Thread _thread;

        private Command _currentCommand;
        private bool _breakModal;

        public WebSynchronizationContext(Application application)
        {
            Platform.CheckForNullReference(application, "application");

            _application = application;
            _queue = new BlockingQueue<Command>();
            _thread = new Thread(RunThread)
                          {
                              CurrentCulture = application.Culture, 
                              CurrentUICulture = application.Culture
                          };
            _thread.Name = String.Format("Web Simulated UI Thread [{0}]", _thread.ManagedThreadId);
            _thread.Start();
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _queue.Enqueue(new Command(callback, state));
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread.Equals(_thread))
            {
                try
                {
                	//TODO (CR May 2010): should process everything in the queue first, then call this.
                    callback(state);
                }
                catch (Exception e)
                {
                    _application.Stop(e);
                }
            }
            else
            {
                // Exception occurred here in WebPortal testing, just ignoring send if the queue doesn't exist, assume its a garbage 
                // collection/cleanup issue.
                if (_queue != null)
                {
                    Command command = new Command(callback, state);
                    _queue.Enqueue(command);
                    command.Wait();
                }
            }
        }

        public void RunModal()
        {
            if (!Thread.CurrentThread.Equals(_thread))
                throw new InvalidOperationException("RunModal must be called from the \"UI\" Thread.");

            _currentCommand.ReleaseWait();
            RunCommandPump();
        }

        public void BreakModal()
        {
            if (!Thread.CurrentThread.Equals(_thread))
                throw new InvalidOperationException("BreakModal must be called from the \"UI\" Thread.");

            _breakModal = true;
        }

        private void RunThread()
        {
            Application.Current = _application;
            SetSynchronizationContext(this);

            RunCommandPump();

			Application.Current = null;
			SetSynchronizationContext(null);

			_application = null;
			_queue = null;
			_thread = null;
        }

        private void RunCommandPump()
        {
            while (_queue.ContinueBlocking && !_breakModal)
            {
                _currentCommand = null;
                bool queueEmpty = !_queue.Dequeue(out _currentCommand);
                if (!queueEmpty && _currentCommand != null)
                {
                    try
                    {
                        _currentCommand.Execute();
                    }
                    catch (Exception e)
                    {
                        //TODO: Review this
                        // Should we break out of the loop?
                        _application.Stop(e);
                        
                    }
                }
            }

            //reset this for the next one (RunCommandPump) down in the stack
            _breakModal = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
        	try
        	{
				Thread thread = _thread;

				_queue.ContinueBlocking = false;
				if (Thread.CurrentThread.Equals(thread))
				{
					Platform.Log(LogLevel.Debug, "Disposing WebSynchronizationContext from it's own thread.");
					return;
				}

				if (thread.IsAlive)
					thread.Join(TimeSpan.FromSeconds(30));
        	}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Unexpected error disposing WebSynchronizationContext.");
			}
		}

        #endregion
    }
}
