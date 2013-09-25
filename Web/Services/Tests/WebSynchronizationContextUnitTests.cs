#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using System.Threading;
using ClearCanvas.Web.Common;
using NUnit.Framework;


namespace ClearCanvas.Web.Services.Tests
{
	[TestFixture]
	public class WebSynchronizationContextUnitTests
	{
        #region Constructor Tests
        [Test(Description = "Test Constructor with null arguments")]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void TestConstructor()
        {
            WebSynchronizationContext broker = new WebSynchronizationContext(null);
        }

        [Test(Description = "Test valid Constructor")]
        public void TestConstructor2()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());
            WebSynchronizationContext broker = new WebSynchronizationContext(ap);
            broker.Dispose();
            ap.Stop();
        }
        #endregion

        private readonly object _waitSyncLock = new object();
	    private volatile int _callCount = 0;
        private volatile int _messageCount = 1;
	    private volatile bool _fault = false;

        private void Wait(object o)
        {
            lock (_waitSyncLock)
                Monitor.Wait(_waitSyncLock);
            Console.WriteLine("Wait released");
        }

        private void Release(object o)
        {
            lock (_waitSyncLock)
                Monitor.Pulse(_waitSyncLock);
        }

        private void Sleep(object o)
        {
            int? i = o as int?;
            if (i.HasValue)
                Thread.Sleep(i.Value);
        }

        private void Send(object o)
        {
            int? i = o as int?;
            if (!i.HasValue)
            {
                _fault = true;
            }
            else
            {
                if(i.Value != _callCount)
                    _fault = true;

                _callCount++;                
            }
            Console.WriteLine("Completed send");
        }

        private void DoModal(object o)
        {
            SyncMockApplication ap = o as SyncMockApplication;
            if (ap == null)
            {
                _fault = true;
                return;
            }

            ap.SyncContext.RunModal();
        }

        private void ReleaseModal(object o)
        {
            SyncMockApplication ap = o as SyncMockApplication;
            if (ap == null)
            {
                _fault = true;
                return;
            }

            ap.SyncContext.BreakModal();
        }

        private void PostMessage(object o)
        {
            SyncMockApplication ap = o as SyncMockApplication;
            if (ap == null)
            {
                _fault = true;
                return;
            }

            ap.SyncContext.Post(Send, ++_messageCount);
        }

        private void SendMessage(object o)
        {
            SyncMockApplication ap = o as SyncMockApplication;
            if (ap == null)
            {
                _fault = true;
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
        }
          
        [Test(Description = "Test Send() queueing")]
        public void TestSend()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _messageCount = 0;
            _callCount = 1;
            _fault = false;

            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 ap.SyncContext.Send(Wait, null);
                                             });
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 ap.SyncContext.Send(Send, ++_messageCount);
                                             });
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 ap.SyncContext.Send(Send, ++_messageCount);
                                             });
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 ap.SyncContext.Send(Send, ++_messageCount);
                                             });
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 ap.SyncContext.Send(Send, ++_messageCount);
                                             });

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Sleep, 100);
            });


            while (_messageCount < 4)
                Thread.Sleep(1000);

            lock (_waitSyncLock)
            {
                Monitor.Pulse(_waitSyncLock);
            }

            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault,"Fault during test, out of order entries");

            ap.Stop();
            
        }


        [Test(Description = "Test Send() queueing with calls to RunModal")]
        public void TestSendWithModal()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _messageCount = 0;
            _callCount = 1;
            _fault = false;

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Wait, null);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(DoModal, ap);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(ReleaseModal, ap);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Sleep, 100);
            });


            while (_messageCount < 5)
                Thread.Sleep(1000);

            lock (_waitSyncLock)
            {
                Monitor.Pulse(_waitSyncLock);
            }

            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();

        }

        [Test(Description = "Test Post() queueing")]
        public void TestPost()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _callCount = 1;
            _messageCount = 0;
            _fault = false;

            ap.SyncContext.Post(Wait, null);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Sleep, 100);

            //broker.Post(Release, null);

            Thread.Sleep(1000);

            lock (_waitSyncLock)
                Monitor.Pulse(_waitSyncLock);


            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();
            
        }

        [Test(Description = "Test Post() queueing with several calls to RunModal")]
        public void TestPostWithModal()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _callCount = 1;
            _messageCount = 0;
            _fault = false;

            ap.SyncContext.Post(Wait, null);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Sleep, 100);

            //broker.Post(Release, null);

            Thread.Sleep(1000);

            lock (_waitSyncLock)
                Monitor.Pulse(_waitSyncLock);


            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();

        }

        [Test(Description = "Test exiting within in a modal")]
        public void TestStopInModal()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _callCount = 1;
            _messageCount = 0;
            _fault = false;

            ap.SyncContext.Post(Wait, null);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Sleep, 100);

            Thread.Sleep(1000);

            lock (_waitSyncLock)
                Monitor.Pulse(_waitSyncLock);

            ap.SyncContext.Send(Sleep, 100);

            ap.Stop();

            Assert.IsFalse(_fault, "Fault during test, out of order entries");
        }

        [Test(Description = "Test Post() with nested calls to RunModel")]
        public void NestedDoModal()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _callCount = 1;
            _messageCount = 0;
            _fault = false;

            ap.SyncContext.Post(Wait, null);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Sleep, 100);

            //broker.Post(Release, null);

            Thread.Sleep(1000);

            lock (_waitSyncLock)
                Monitor.Pulse(_waitSyncLock);


            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();

        }

        [Test(Description = "Test combined Send() and Post queueing")]
        public void TestMixedSendAndPost()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _messageCount = 0;
            _callCount = 1;
            _fault = false;

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Wait, null);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);            
            });

            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Sleep, 100);
            });

            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);


            while (_messageCount < 12)
                Thread.Sleep(1000);

            lock (_waitSyncLock)
            {
                Monitor.Pulse(_waitSyncLock);
            }

            Thread.Sleep(1000);

            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();

        }

        [Test(Description = "Test nested posts and send queueing")]
        public void TestNestedSendAndPost()
        {
            SyncMockApplication ap = new SyncMockApplication(new MockClientCallback());

            _messageCount = 0;
            _callCount = 1;
            _fault = false;

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Wait, null);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });
            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ap.SyncContext.Post(DoModal, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(PostMessage, ap);
            ap.SyncContext.Post(PostMessage, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(ReleaseModal, ap);
            ap.SyncContext.Post(SendMessage, ap);
            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(SendMessage, ap);
            ap.SyncContext.Post(Send, ++_messageCount);

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Send, ++_messageCount);
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                ap.SyncContext.Send(Sleep, 100);
            });

            ap.SyncContext.Post(Send, ++_messageCount);
            ap.SyncContext.Post(Send, ++_messageCount);


            while (_messageCount < 12)
                Thread.Sleep(1000);

            lock (_waitSyncLock)
            {
                Monitor.Pulse(_waitSyncLock);
            }

            Thread.Sleep(1000);

            ap.SyncContext.Send(Sleep, 100);

            Assert.IsFalse(_fault, "Fault during test, out of order entries");

            ap.Stop();

        }
	}

    class SyncMockApplication : Application
    {
        private Common.Application _app;
        public bool IsRunning { get; set; }

        public WebSynchronizationContext SyncContext
        {
            get { return _synchronizationContext; }
        }
        public EventBroker EventBroker
        {
            get
            {
                return _context._eventBroker;
            }
        }

        public SyncMockApplication(IApplicationServiceCallback callback)
        {
            // for it to start so the web sync context is created
            this._context = new ApplicationContext(this, callback);
            Start(new MockStartApplicationRequest());

        }

        protected override void OnStart(StartApplicationRequest request)
        {
            //NOOP
            IsRunning = true;
        }

        protected override void OnStop()
        {
            //NOOP
            IsRunning = false;

        }

        protected override Common.Application GetContractObject()
        {
            return _app;
        }
    }
}

#endif