#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.Threading;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
   	internal class ImageServerPrefetchingStrategy : PrefetchingStrategy
	{
		private ViewerFrameEnumerator _imageBoxEnumerator;
		private BlockingThreadPool<Frame> _retrieveThreadPool;

		private volatile bool _stopAllActivity;
		private int _activeRetrieveThreads;

		public ImageServerPrefetchingStrategy()
			: base("CC_ImageServer", "Prefetching Strategy")
		{
		}

		protected override void Start()
		{
			InternalStart();
		}

		private void InternalStart()
		{
            int retrieveConcurrency = Prefetch.Default.RetrieveConcurrency;
			if (retrieveConcurrency == 0)
				return;

			_imageBoxEnumerator = new ViewerFrameEnumerator(ImageViewer,
                Math.Max(Prefetch.Default.SelectedWeighting, 1),
                Math.Max(Prefetch.Default.UnselectedWeighting, 0),
                Prefetch.Default.ImageWindow);

			_retrieveThreadPool = new BlockingThreadPool<Frame>(_imageBoxEnumerator, RetrieveFrame);
			_retrieveThreadPool.ThreadPoolName = "Retrieve";
			_retrieveThreadPool.Concurrency = retrieveConcurrency;
			_retrieveThreadPool.ThreadPriority = ThreadPriority.BelowNormal;
			_retrieveThreadPool.Start();
			
		}
		
		protected override void Stop()
		{
			InternalStop();
		}

		private void InternalStop()
		{
			if (_retrieveThreadPool != null)
			{
				_retrieveThreadPool.Stop(false);
				_retrieveThreadPool = null;
			}

			if (_imageBoxEnumerator != null)
			{
				_imageBoxEnumerator.Dispose();
				_imageBoxEnumerator = null;
			}
		}

		private void RetrieveFrame(Frame frame)
		{
            if (_stopAllActivity)
            {
                return;
            }

			try
			{
				//just return if the available memory is getting low - only retrieve and decompress on-demand now.
                if (SystemResources.GetAvailableMemory(SizeUnits.Megabytes) < Prefetch.Default.AvailableMemoryLimitMegabytes)
                {
                    return;
                }

				Interlocked.Increment(ref _activeRetrieveThreads);

				//TODO (CR May 2010): do we need to do this all the time?
				string message = String.Format("Retrieving Frame (active threads: {0})", Thread.VolatileRead(ref _activeRetrieveThreads));
				Trace.WriteLine(message);
			    Console.WriteLine(message);
			    
                //TODO: try to trigger header retrieval for data luts?
                frame.GetNormalizedPixelData();
			}
			catch(OutOfMemoryException)
			{
				_stopAllActivity = true;
				Platform.Log(LogLevel.Error, "Out of memory trying to retrieve pixel data.  Prefetching will not resume unless memory becomes available.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error retrieving frame pixel data.");
			}
			finally
			{
				Interlocked.Decrement(ref _activeRetrieveThreads);
			}
		}		
	}
}
