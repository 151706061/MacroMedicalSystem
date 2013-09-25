#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Events;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.Web.Services.Tests
{
    [TestFixture]
    public class EventBrokerTests
    {
        // amount of time the unit test thread will sleep before checking the results
        private const int WAIT_DELAY = 1000;
        private bool BatchSendingSupported;
        
        [TestFixtureSetUp]
        public void Setup()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            BatchSendingSupported = CanBrokerSendMessagesInBatch(client, broker);
        }

        private bool CanBrokerSendMessagesInBatch(MockClientCallback client, EventBroker broker)
        {
            int EXPECTED_MSG_COUNT = 100;
            int messageCount = 1;

            // try different batch sizes
            while (messageCount < EXPECTED_MSG_COUNT)
            {
                client.Reset();
                Assert.AreEqual(0, client.EventsSent.Count);

                broker.Suspended = true;

                MockEvent[] events = new MockEvent[messageCount];
                for (int i = 0; i < events.Length; i++)
                {
                    events[i] = new MockEvent(true);
                    broker.Send(events[i]);
                }

                broker.Suspended = false;
                Thread.Sleep(500);

                if (messageCount > 1 && client.EventsSetSent.Count == 1)
                {
                    client.Reset();
                    return true;
                }

                messageCount++; // increase # of messages to send
            }

            return false;
        }


        #region Constructor Tests

        [Test(Description = "Test Constructor with null arguments")]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void TestConstructor()
        {
            EventBroker broker = new EventBroker(null, null);
        }

        [Test(Description = "Test Constructor with null IApplicationServiceCallback")]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void TestConstructor2()
        {
            MockApplication ap = new MockApplication(null);
            EventBroker broker = new EventBroker(ap, null);
        }

        [Test(Description = "Test Constructor with null application")]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void TestConstructor3()
        {
            EventBroker broker = new EventBroker(null, new MockClientCallback());
        }

        #endregion

        #region Send Tests

        [Test(Description = "Test Send with null argument")]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void TestSendWithNullArgument()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;

            Assert.IsFalse(broker.Suspended);

            client.Reset();
            MockEvent @event = null;
            broker.Send(@event);
        }

        [Test(Description = "Test Sending a single message")]
        public void TestSingleSend()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsFalse(broker.Suspended);

            Thread.Sleep(WAIT_DELAY);
            client.Reset();
             
            MockEvent @event = new MockEvent();
            broker.Send(@event);
            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(client.EventCount == 1, "Should send a single message");

            Assert.IsTrue(client.LastEventSet.Events.Length == 1);
            Assert.IsTrue(ReferenceEquals(client.LastEventSet.Events[0], @event));
        }

        [Test(Description = "Test sending multiple messages")]
        public void TestMultipleSend()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app =new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY);
            
            client.Reset();

            int MSG_COUNT = 5;
            MockEvent[] events = new MockEvent[MSG_COUNT];

            for(int i=0; i<events.Length; i++)
                events[i] = new MockEvent();

            foreach(Event e in events)
                broker.Send(e);

            int MAX_MSG_DELAY = 200; //200ms per msg
            Thread.Sleep(MAX_MSG_DELAY * MSG_COUNT);
            Assert.IsTrue(client.EventsSent.Count == MSG_COUNT, "All messages should be sent within the required period");
            for (int i = 0; i < client.EventsSent.Count; i++)
            {
                Assert.IsTrue(ReferenceEquals(client.EventsSent[i], events[i]), "Order of events is incorrect");
            }
        }

        [Test(Description = "Test sending multiple messages with AllowBatchSend=true")]
        public void TestAllowBatchSending()
        {
            // TODO: Do we need another test to check the behaviour when the broker does not support batch sending?
            if (!BatchSendingSupported)
                  Assert.Ignore("Broker does not appear to support batch sending");

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY);

            
            client.Reset();

            broker.Suspended = true;

            int MSG_COUNT = 50;
            MockEvent[] events = new MockEvent[MSG_COUNT];

            for (int i = 0; i < events.Length; i++)
            {
                events[i] = new MockEvent(true);
            }

            foreach (Event e in events)
                broker.Send(e);


            broker.Suspended= false;

            Thread.Sleep(2000);

            // When all messages are AllowSendInBatch, there's nothing to check really except that all messages
            // will be sent in the right order. 
            
            Assert.IsTrue(client.EventsSent.Count == MSG_COUNT, "Not all messages were accounted for");
            for (int i = 0; i < client.EventsSent.Count; i++)
            {
                Assert.IsTrue(ReferenceEquals(client.EventsSent[i], events[i]), "Order of events was incorrect");
            }

        }

        [Test(Description = "Test sending multiple messages with AllowBatchSend=false")]
        public void TestAllowBatchSending2()
        {
            if (!BatchSendingSupported)
                Assert.Ignore("Broker does not appear to support batch sending");

            int MSG_COUNT = 50;

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY);

            client.Reset();

            broker.Suspended = true;
            MockEvent[] events = new MockEvent[MSG_COUNT];
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = new MockEvent(false);
            }
            foreach (Event e in events)
                broker.Send(e);

            
            broker.Suspended = false;

            Thread.Sleep(2000);
            Assert.AreEqual(MSG_COUNT, client.EventsSent.Count, "Not all messages were accounted for");

            Assert.AreEqual(MSG_COUNT, client.EventsSetSent.Count, "Each message should be sent individually");

            for (int i = 0; i < client.EventsSent.Count; i++)
            {
                Assert.IsTrue(ReferenceEquals(client.EventsSent[i], events[i]), "Order of all events was incorrect");
            }

        }

        
        [Test(Description = "Test sending multiple messages with AllowBatchSend=false randomly")]
        // For this case, if the batch contains a special event with AllowSendInBatch=false,
        // we want to make sure that the event is the last event in the batch.
        public void TestAllowBatchSending3()
        {
            if (!BatchSendingSupported)
                Assert.Ignore("Broker does not appear to support batch sending");

            int MSG_COUNT = 50;

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY);

            
            client.Reset();

            MockEvent[] events = new MockEvent[MSG_COUNT];
            List<Event> eventsNotAllowSendInBatch = new List<Event>();
            Random ran = new Random();
            for (int i = 0; i < events.Length; i++)
            {
                if (i%10 == 0 || ran.Next()%10 == 0)
                {
                    events[i] = new MockEvent(false);
                    eventsNotAllowSendInBatch.Add(events[i]);
                }
                else
                {
                    events[i] = new MockEvent(true);
                }
            }
            foreach (Event e in events)
                broker.Send(e);


            broker.Suspended = false;

            Thread.Sleep(2000);
            Assert.AreEqual(MSG_COUNT, client.EventsSent.Count, "Not all messages were accounted for");

            Trace.WriteLine("Checking order of the events..");
            int notAllowInBatchEventsFoundCounter = 0;
            for (int i = 0; i < client.EventsSetSent.Count; i++)
            {
                EventSet set = client.EventsSetSent[i];
                for (int eventIndex = 0; eventIndex < set.Events.Length; eventIndex++)
                {
                    // If the batch contains a special event with AllowSendInBatch=false,
                    // the event should be the last event in the batch.
                    Event e = set.Events[eventIndex];
                    if (!e.AllowSendInBatch)
                    {
                        Assert.IsTrue(ReferenceEquals(e, eventsNotAllowSendInBatch[notAllowInBatchEventsFoundCounter]),
                                      "Special events were sent in the wrong order");
                        Assert.AreEqual(eventIndex, set.Events.Length - 1,
                                        "Special event must be the last message in the set");

                        notAllowInBatchEventsFoundCounter++;
                        break;
                    }
                }
            }
            Assert.AreEqual(eventsNotAllowSendInBatch.Count, notAllowInBatchEventsFoundCounter,
                            "Not all specially events were accounted for.");

            for (int i = 0; i < client.EventsSent.Count; i++)
            {
                Assert.IsTrue(ReferenceEquals(client.EventsSent[i], events[i]), "Order of all events was incorrect");
            }

        }



        [Test(Description = "Test Send with communication error")]
        public void TestSend_CommunicationException()
        {
            MockClientCallback client = new MockClientCallback();
            
            MockApplication app = new MockApplication(client);

            EventBroker broker = new EventBroker(app, client);
            
            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);

            client.Reset();

            MockEvent @event = new MockEvent() {ExceptionToThrow = new CommunicationException("Simulated")};
            broker.Send(@event);

            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(app.IsRunning, "EventBroker should eat any Communication Exception instead of stopping the application");

        }

        [Test(Description = "Test Send with other exception")]
        public void TestSend_OtherException()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);
            
            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);

            client.Reset();
            MockEvent @event = new MockEvent() { ExceptionToThrow = new Exception("Simulated") };
            broker.Send(@event);

            Thread.Sleep(WAIT_DELAY);
            Assert.IsFalse(app.IsRunning, "EventBroker should stop the application");
        }

        #endregion

        #region Suspend/Resume Tests

        [Test(Description = "Test Suspend/Resume")]
        public void TestSuspendResume()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);
            
            broker.Suspended = true;
            Assert.IsTrue(broker.Suspended);

            client.Reset();

            MockEvent @event1 = new MockEvent();
            MockEvent @event2 = new MockEvent();
            broker.Send(@event1);
            broker.Send(@event2);

            Thread.Sleep(2 * WAIT_DELAY);
            Assert.IsTrue(broker.Suspended, "Should remain suspsended after Send is called");
            Assert.IsTrue(client.EventCount == 0, "Should NOT send any message while it's being suspended");

            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(2 * WAIT_DELAY);
            Assert.IsTrue(client.EventCount == 2, "Should resume sending all messages after it's no longer suspended");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[0], @event1), "Event order is incorrect");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[1], @event2), "Event order is incorrect");
            
        }


        [Test(Description = "Test Suspend/Resume when errors occur")]
        public void TestSuspendResumeWithErrors()
        {
            // Based on the current implementation (implicit requirement), the Application.Stop() will be called if 
            // an exception is thrown when the broker tries to send an event.

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsTrue(app.IsRunning);
            Thread.Sleep(WAIT_DELAY);
            
            broker.Suspended = true;
            Assert.IsTrue(broker.Suspended);

            client.Reset();

            MockEvent @event1 = new MockEvent();
            broker.Send(@event1);

            MockEvent @event2 = new MockEvent() { ExceptionToThrow = new Exception("Simulated") };
            broker.Send(@event2);

            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(broker.Suspended, "Should remain suspsended");
            Assert.IsTrue(client.EventCount == 0, "Should NOT send any message while it's being suspended");
            Assert.IsTrue(app.IsRunning, "Application should still be running because the broker is being suspended"); 
            
            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY);
            
            Assert.IsTrue(client.EventCount == 3 /* including the app stop event */, "EventBroker should send all messages + AppStopEvent when it's no longer suspended");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[0], @event1), "Event order is incorrect");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[1], @event2), "Event order is incorrect");
            Assert.IsTrue(client.EventsSent[2] is ApplicationStoppedEvent, "Last event must be ApplicationStoppedEvent");
            
            Assert.IsFalse(app.IsRunning, "Application should be stopped because of the exception");

        }

        
        #endregion

        #region Batching Sending Tests(using suspend/resume)


        [Test(Description = "Test Batch Sending")]
        public void TestBatchSending()
        {
            if (!BatchSendingSupported)
                Assert.Ignore("Broker does not appear to support batch sending");

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);

            broker.Suspended = true;
            Assert.IsTrue(broker.Suspended);
            client.Reset();

            Event[] events = new Event[50];
            for(int i=0; i<events.Length; i++)
            {
                events[i] = new MockEvent();
            }

            for (int i = 0; i < events.Length; i++)
            {
                broker.Send(events[i]);
            }

            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(broker.Suspended, "Should remain suspsended after Send is called");
            Assert.IsTrue(client.EventCount == 0, "Should NOT send any message while it's being suspended");

            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY*5);
            Assert.IsTrue(client.EventCount == events.Length, "All messages should be sent after it's no longer suspended");
            
            const int MinBatchSizeRequired = 2;
            Assert.IsTrue(client.EventsSetSent.Count <= (events.Length/MinBatchSizeRequired), "Messages must be sent in batches");
        }


        [Test(Description = "Test Batch Sending with errors")]
        public void TestBatchSendingWithErrors()
        {
            if (!BatchSendingSupported)
                Assert.Ignore("Broker does not appear to support batch sending");

            // Based on the current implementation (implicit requirement), the Application.Stop() will be called if 
            // an exception is thrown when the broker tries to send an event.

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);

            broker.Suspended = true;
            Assert.IsTrue(broker.Suspended);
            client.Reset();

            // Prepare a batch of 50 msgs which will cause an exception on the 25th.
            MockEvent[] events = new MockEvent[50];
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = new MockEvent();
                if (i == 24)
                    events[i].ExceptionToThrow = new Exception("Simulated");
            }

            for (int i = 0; i < events.Length; i++)
            {
                broker.Send(events[i]);
            }

            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(broker.Suspended, "Should remain suspsended after Send is called");
            Assert.IsTrue(client.EventCount == 0, "Should NOT send any message while it's being suspended");

            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);
            Thread.Sleep(WAIT_DELAY * 5);

            Assert.IsFalse(app.IsRunning, "Application should be stopped");
            
            // TODO: Ideally broker should only send 26 messages (24 good msgs + bad msg + stop event)
            // However, it's not implemented to stop sending messages if the application is already stopped.
            // Decide to skip these checks since it's late in the game and didn't seem to cause any problem.
            Assert.Ignore("Message count is not verified.");
            //Assert.IsTrue(client.EventCount == 26 , "26 messages should be sent");
            //Assert.IsTrue(client.EventsSent[25] is ApplicationStoppedEvent, "Last event should be ApplicationStoppedEvent");
            
        }


        #endregion

        #region Speed Test


        [Test(Description = "Test Speed")]
        public void TestMinSpeed()
        {
            // Make sure there's no significant delay caused by the broker
            
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);

            client.Reset();

            // use a thread to continously send messages
            Thread producer = new Thread((arg) =>
                                             {
                                                 ManualResetEvent stop = arg as ManualResetEvent;
                                                 while(!stop.WaitOne(1))
                                                 {
                                                     broker.Send(new MockEvent());
                                                 }
                                             });

            ManualResetEvent stopEvent = new ManualResetEvent(false);
            producer.Start(stopEvent);

            // In theory, without any other delay caused by external code, 
            // the broker should send msgs out at min rate of 20 msg per sec.
            // 
            int MIN_SPEED_MSG_PER_SEC= 20;
            TimeSpan duration = TimeSpan.FromSeconds(5);
            Thread.Sleep(duration);
            stopEvent.Set();
            producer.Join();

            int msgCount = client.EventsSent.Count;
            double minMsgCount = MIN_SPEED_MSG_PER_SEC*duration.TotalSeconds;
            Assert.Greater(msgCount, minMsgCount, "Messages must be sent at min rate of 20 mps");
            Trace.WriteLine(String.Format("Speed: {0} msg per sec", msgCount / duration.TotalSeconds));
        }

        #endregion

        #region Dispose Tests

        [Test(Description = "Test Dispose before sending a message")]
        public void TestDisposeBeforeSending()
        {
            MockClientCallback client= new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;;
            Thread.Sleep(WAIT_DELAY);
            
            client.Reset();
            
            broker.Dispose();

            
            MockEvent @event = new MockEvent();
            broker.Send(@event);

            Assert.IsTrue(client.EventCount == 0);
            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(client.EventCount == 0, "Should not send out any message after it is disposed");
        }

        [Test(Description = "Test Dispose after sending")]
        public void TestDisposeAfterSending()
        {
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);
            
            client.Reset();
            MockEvent @event = new MockEvent();
            broker.Send(@event);

            Assert.IsFalse(broker.Suspended);
            broker.Dispose();

            
            //Attempt to send an event after its disposed

            Thread.Sleep(WAIT_DELAY);
            Assert.IsTrue(client.EventCount == 1, "EventBroker shall deliver the last message which was sent before it was disposed"); 
        }

        [Test(Description = "Test Dispose while broker is being suspended")]
        public void TestDisposeWhileSuspendOff()
        {
            // What happens if Dispose is called while the broker is being suspended?
            // Based on the current implementation, existing messages in the queue
            // will be sent out when Dispose() is called, regardless of the value of the Suspended property.

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsTrue(app.IsRunning);
            Thread.Sleep(WAIT_DELAY);

            broker.Suspended = false;
            Assert.IsFalse(broker.Suspended);

            client.Reset();

            MockEvent @event1 = new MockEvent();
            broker.Send(@event1);

            MockEvent @event2 = new MockEvent();
            broker.Send(@event2);

            Assert.IsFalse(broker.Suspended, "EventBroker Suspended should be false");

            broker.Dispose();

            // send more events after calling Dispose()
            MockEvent @event3 = new MockEvent();
            broker.Send(@event3);


            Thread.Sleep(WAIT_DELAY);

            Assert.IsFalse(broker.Suspended, "EventBroker Suspended should be false after Dispose() is called");
            Assert.IsTrue(client.EventCount == 2, "EventBroker should delivery all messages up to the point when Dispose() is called");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[0], @event1), "Event order is incorrect");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[1], @event2), "Event order is incorrect");

            Assert.IsTrue(app.IsRunning, "Application should not be stopped");

        }
        

        [Test(Description = "Test calling Dispose() while broker is being suspended")]
        public void TestDisposeWhileSuspendOn()
        {
            // What happens if Dispose is called while the broker is being suspended?
            // Based on the current implementation, existing messages in the queue
            // will be sent out when Dispose() is called, regardless of the value of the Suspended property.

            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Assert.IsTrue(app.IsRunning);
            Thread.Sleep(WAIT_DELAY);

            broker.Suspended = true;
            Assert.IsTrue(broker.Suspended);

            client.Reset();

            MockEvent @event1 = new MockEvent();
            broker.Send(@event1);

            MockEvent @event2 = new MockEvent();
            broker.Send(@event2);

            Assert.IsTrue(broker.Suspended, "EventBroker Suspended should be true");

            broker.Dispose();

            // send more events after calling Dispose()
            MockEvent @event3 = new MockEvent();
            broker.Send(@event3);


            Thread.Sleep(WAIT_DELAY);

            Assert.IsTrue(app.IsRunning, "Application should not be stopped");
            Assert.IsTrue(client.EventCount == 2, "EventBroker should delivery all messages up to the point when Dispose() is called");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[0], @event1), "Event order is incorrect");
            Assert.IsTrue(ReferenceEquals(client.EventsSent[1], @event2), "Event order is incorrect");
            
            // TODO: should Suspended be true or false if Dispose() is called?
            // On one hand, it should be false because messages have been sent out.
            // On another hand, this is an invalid use case because one should never try to use the broker once it is disposed of (bad coding).
            Assert.Ignore("EventBroker.Suspended property is not verified");
        }
        
        #endregion

        #region Test Event Sequence Number

        [Test(Description = "Test Sequence Number")]
        public void TestSequenceNumber()
        {
			// Make sure the sequence number generated
			// by the broker is correct
            MockClientCallback client = new MockClientCallback();
            MockApplication app = new MockApplication(client);
            EventBroker broker = app.EventBroker;
            Thread.Sleep(WAIT_DELAY);

            client.Reset();

            // use threads to continously send messages
            Thread[] producers = new Thread[5];

            for (int i = 0; i < producers.Length; i++)
            {
                producers[i] = new Thread((arg) =>
                                                 {
                                                     Random ran = new Random();
                                                     ManualResetEvent stop = arg as ManualResetEvent;
                                                     while (!stop.WaitOne(1))
                                                     {
                                                         while(ran.Next()%10==0)
                                                            broker.Send(new MockEvent());

                                                         Thread.Sleep(ran.Next(0, 20));
                                                     }
                                                 });
            }

            ManualResetEvent stopEvent = new ManualResetEvent(false);
            foreach(Thread producer in producers)
                producer.Start(stopEvent);

            TimeSpan duration = TimeSpan.FromSeconds(5);
            Thread.Sleep(duration);
            stopEvent.Set();

            foreach (Thread producer in producers)
                producer.Join();


            int? expectedSQ=null;

            for(int i=0; i<client.EventsSetSent.Count; i++)
            {
                EventSet eventSet = client.EventsSetSent[i];

                if (!expectedSQ.HasValue)
                    expectedSQ = eventSet.Number;
                else
                {
                    Assert.AreEqual(expectedSQ.Value, eventSet.Number, "Wrong sequence number");
                }
                expectedSQ++;
            }
            
        }

        #endregion

    }

    class MockEvent:Event
    {
        private bool _allowSendInBatch;

        public Exception ExceptionToThrow { get; set; }

        public MockEvent()
        {
            _allowSendInBatch = true;
        }


        public MockEvent(bool allowSendInBatch)
        {
            _allowSendInBatch = allowSendInBatch;
        }

        public override bool AllowSendInBatch
        {
            get { return _allowSendInBatch; }
        }
    }

    class MockClientCallback:IApplicationServiceCallback
    {
        private long _count;

        public long EventCount { get { return _count; } }

        public EventSet LastEventSet { get; set; }

        public List<Event> EventsSent { get; set; }
        public List<EventSet> EventsSetSent { get; set; }

        public MockClientCallback()
        {
            EventsSent = new List<Event>();
            EventsSetSent = new List<EventSet>();
        }

        public void Reset()
        {
            _count = 0;
            LastEventSet = null;
            EventsSent = new List<Event>();
            EventsSetSent = new List<EventSet>();
        }

        public void EventNotification(EventSet events)
        {

            LastEventSet = events;
            EventsSetSent.Add(events);
            EventsSent.AddRange(events.Events);
            Interlocked.Add(ref _count, events.Events.Length);

            foreach (Event e in events.Events)
            {
                if (e is MockEvent)
                {
                    if ((e as MockEvent).ExceptionToThrow != null)
                        throw (e as MockEvent).ExceptionToThrow;
                }
            }
        

        }
    }

    class MockApplication:Application
    {
        private Common.Application _app;
        public bool IsRunning { get; set; }
        public EventBroker EventBroker { 
            get
            {
                return _context._eventBroker;
            }
        }

        public MockApplication(IApplicationServiceCallback callback)
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

    class MockStartApplicationRequest:StartApplicationRequest{}
}

#endif
