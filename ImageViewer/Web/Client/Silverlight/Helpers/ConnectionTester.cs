#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using Macro.Web.Client.Silverlight.Utilities;

namespace Macro.ImageViewer.Web.Client.Silverlight.Helpers
{
    public class SpeedTestResult
    {
        public double SpeedInMbps { get; set; }
        public Exception Error { get; set; }
    }

    public class SpeedTestCompletedEventArgs : EventArgs
    {
        public SpeedTestResult Result { get; set; }
    }

    public class ConnectionTester
    {
        const string TinyFile = "Test/speedtest_tiny.bin";
        const string SmallFile = "Test/speedtest_small.bin";

        private long _speedTestStartTime;

        public event EventHandler<SpeedTestCompletedEventArgs> SpeedTestCompleted;
        public event EventHandler SpeedTestBegan;

        private ConnectionTester()
        {
        }

        public  static void TestConnection(Delegate del)
        {
            ConnectionTester.StartAsync((result) =>
            {
                if (result.Error == null)
                {
                    // TODO: Should we continue if the speed is too low? It won't be useful for the user anyway.
                    if (result.SpeedInMbps < 1)
                    {
                        PopupHelper.PopupMessage(DialogTitles.Warning, SR.SlowConnection, Labels.ButtonContinue, false);
                    }

                    SelectThrottleSettings(result);
                    del.DynamicInvoke(null);                    
                }
                else
                {
                    PopupHelper.PopupMessage(DialogTitles.Warning, SR.SpeedTestFailed, Labels.ButtonContinue, false);
                }

            });
        }

        private static void SelectThrottleSettings(SpeedTestResult result)
        {
            if (result == null || result.Error != null)
            {
                ThrottleSettings.Default.EnableDynamicImageQuality = true;
                ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed = 1;
                return;
            }

            // These speeds are picked based on experiments.
            if (result.SpeedInMbps >= 150)
            {
                // the connection is fast enough
                ThrottleSettings.Default.EnableDynamicImageQuality = false;
                ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed = 4;
            }
            else if (result.SpeedInMbps >= 10)
            {
                // the connection is fast enough
                ThrottleSettings.Default.EnableDynamicImageQuality = false;
                ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed = 3;
            }
            else if (result.SpeedInMbps >= 2)
            {
                ThrottleSettings.Default.EnableDynamicImageQuality = true;
                ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed = 2;
            }
            else
            {
                ThrottleSettings.Default.EnableDynamicImageQuality = true;
                ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed = 1;
            }
        }

        public static void StartAsync(Action<SpeedTestResult> actionWhenCompleted)
        {
           // ChildWindow msgBox = PopupHelper.PopupMessage(DialogTitles.Initializing, SR.CheckingConnectionSpeed);

            ConnectionTester test = new ConnectionTester();
            test.SpeedTestCompleted += (s, ev) =>
            {
                UIThread.Execute(() =>
                {
                  //  msgBox.Close();
                    var result = ev.Result;
                    actionWhenCompleted(result);
                });
            };

            test.RunSpeedTest();
        }

        public void RunSpeedTest()
        {
            QuickTest();
        }

        private void QuickTest()
        {
            string uri = string.Format("../{0}?rand={1}{2}", TinyFile, Guid.NewGuid().ToString(), Environment.TickCount);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(QuickTestCompleted);

            if (SpeedTestBegan != null)
            {
                SpeedTestBegan(this, EventArgs.Empty);
            }

            _speedTestStartTime = Environment.TickCount;
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void SmallTest()
        {
            string uri = string.Format("../{0}?rand={1}{2}", SmallFile, Guid.NewGuid().ToString(), Environment.TickCount);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(SmallTestCompleted);

            if (SpeedTestBegan != null)
            {
                SpeedTestBegan(this, EventArgs.Empty);
            }

            _speedTestStartTime = Environment.TickCount;
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void QuickTestCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                long elapsed = Environment.TickCount - _speedTestStartTime;
                float sizeMb = buffer.Length / 1024f / 1024f * 8;
                double speedMpbs = sizeMb * 1000f / elapsed;

                // Test bigger file is speed seems ok
                if (speedMpbs > 2)
                {
                    SmallTest();
                }
                else
                {
                    // TODO: Take average?

                    PerformanceMonitor.CurrentInstance.SpeedInMbps = speedMpbs;

                    if (SpeedTestCompleted != null)
                    {
                        SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { SpeedInMbps = speedMpbs } });
                    }
                }
            }
            else
            {
                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { Error = e.Error } });
                }
            }
        }

        private void SmallTestCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                long elapsed = Environment.TickCount - _speedTestStartTime;
                float sizeMb = buffer.Length / 1024f / 1024f * 8;
                double speedMpbs = sizeMb * 1000f / elapsed;
            
                // TODO: Take average?
                PerformanceMonitor.CurrentInstance.SpeedInMbps = speedMpbs;

                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { SpeedInMbps = speedMpbs } });
                }
            }
            else
            {
                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { Error = e.Error } });
                }
            }
        }
    }
}
