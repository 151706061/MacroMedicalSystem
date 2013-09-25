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
using System.Drawing;
using System.IO;
using System.Threading;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.ImageViewer.Web.Common.Events;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Messages;
using ClearCanvas.Web.Services;
using NUnit.Framework;
using Rectangle=ClearCanvas.ImageViewer.Web.Common.Rectangle;

namespace ClearCanvas.ImageViewer.Web.EntityHandlers.Tests
{
    [TestFixture]
    public class TileEntityHandlerTest:AbstractTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            
        }
        [Test]
        public void TestProcessMessage()
        {
            
            try
            {

                Assert.IsNotNull(SynchronizationContext.Current, "SynchronizationContext.Current");
                
                DicomFile file = new DicomFile("TileEntityHandlerTest.dcm");
                DicomAttributeCollection dataSet = file.DataSet;
                SetupSecondaryCapture(dataSet);
                file.Save();

                
                ImageViewerComponent viewer = new ImageViewerComponent();
                viewer.Start();

                viewer.LoadImages(new[] { "TileEntityHandlerTest.dcm" });

                ManualResetEvent signal = new ManualResetEvent(false);
                viewer.EventBroker.LayoutManagerCompleted += (s, e) => signal.Set();

                viewer.Layout();
                Console.WriteLine("Waiting for layout to complete");
                if (!signal.WaitOne(20000))
                    Assert.Fail("Abort: something is not working properly.");

                Console.WriteLine("Layout completed");
                Assert.IsNotNull(viewer.PhysicalWorkspace);
                Assert.IsNotNull(viewer.PhysicalWorkspace.ImageBoxes[0]);
                Assert.IsNotNull(viewer.PhysicalWorkspace.ImageBoxes[0].Tiles[0]);

                Tile tile = viewer.PhysicalWorkspace.ImageBoxes[0].Tiles[0] as Tile;

                Assert.IsNotNull(tile.PresentationImage);

                MockApplicationContext context = new MockApplicationContext();

                TileEntityHandler handler = new TileEntityHandler { ApplicationContext = context };
                handler.SetModelObject(tile);
                ChangeClientRectangle(context, handler, 0, 0, 512, 512, "Case: Size is even");
                ChangeClientRectangle(context, handler, 0, 0, 311, 311, "Case: Size is odd");
                ChangeClientRectangle(context, handler, 10, 10, 300, 301, "Case: Left,Top are positive");
                ChangeClientRectangle(context, handler, -10, -10, 512, 512, "Case: Left,Top are negative");
                
            }
            finally
            {
                File.Delete("TileEntityHandlerTest.dcm");
            }
        }

        private void ChangeClientRectangle(MockApplicationContext context, TileEntityHandler handler, int left, int top, int height, int width, string errorMessageIfFailed)
        {
            Trace.WriteLine(String.Format("Simulate changing client size to {0}x{1}", width, height));

            handler.ProcessMessage(new UpdatePropertyMessage()
                                       {
                                           PropertyName = "ClientRectangle",
                                           Value = new Rectangle
                                                       {
                                                           Left=left, Top=top,
                                                           Height = height, Width=width
                                                       }
                                       });


            TileUpdatedEvent tileUpdateEvent = context.EventSent.FindLast(e => e is TileUpdatedEvent) as TileUpdatedEvent;

            Bitmap bmp = new Bitmap(new MemoryStream(tileUpdateEvent.Tile.Image));

            Trace.WriteLine(String.Format("Checking size of the image being sent"));
            Assert.AreEqual(width, bmp.Width, errorMessageIfFailed);
            Assert.AreEqual(height, bmp.Height, errorMessageIfFailed);
        }
    }

    internal class MockApplicationContext:IApplicationContext
    {
        private List<Event> _eventSent = new List<Event>();
        public List<Event> EventSent { get { return _eventSent; } }

        #region IApplicationContext Members

        public EntityHandlerStore EntityHandlers
        {
            get { throw new NotImplementedException(); }
        }

        public System.Security.Principal.IPrincipal Principal
        {
            get { throw new NotImplementedException(); }
        }

        public Guid ApplicationId
        {
            get { throw new NotImplementedException(); }
        }

        public void FireEvent(ClearCanvas.Web.Common.Event e)
        {
            _eventSent.Add(e);
        }

        public void FatalError(Exception e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
