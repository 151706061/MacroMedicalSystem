#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using Macro.ImageViewer.Web.Client.Silverlight.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight
{


    public class PerformanceMonitor : INotifyPropertyChanged
    {
        object _sync = new object();
        double _lastKnownClientFps = 10;
        TileView _currentTile;
        readonly List<long> _clientDrawTimes = new List<long>();
        readonly List<long> _imageSizes = new List<long>();

        static readonly PerformanceMonitor _current = new PerformanceMonitor();
        public static PerformanceMonitor CurrentInstance
        {
            get { return _current; }
        }

        private PerformanceMonitor()
        {
        }


        public TileView CurrentTile
        {
            get { return _currentTile; }
            set
            {
                if (_currentTile != value)
                {
                    _currentTile = value;
                    ResetClientStats();
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion


        long _sendLag;
        public long SendLag
        {
            get
            {
                lock (_sync)
                {
                    // TODO: why does this sometimes become negative ?
                    return _sendLag > 0 ? _sendLag : 0;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_sendLag != value)
                    {
                        _sendLag = Math.Max(0, value);
                        OnPropertyChanged("SendLag");
                    }
                }

            }
        }


        long _renderingLag;
        public long RenderingLag
        {
            get
            {
                lock (_sync)
                {
                    return _renderingLag;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_renderingLag != value)
                    {
                        _renderingLag = value;
                        OnPropertyChanged("RenderingLag");
                    }
                }

            }
        }

        public double AverageClientFps
        {
            get
            {
                lock (_sync)
                {
                    return _lastKnownClientFps;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_lastKnownClientFps != value)
                    {
                        _lastKnownClientFps = value;

                        OnPropertyChanged("AverageClientFps");
                    }
                }
            }
        }

        private double _aveImageSize;
        public double AverageImageSize
        {
            get
            {
                lock (_sync)
                {
                    return _aveImageSize;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_aveImageSize != value)
                    {
                        _aveImageSize = value;
                        OnPropertyChanged("AverageImageSize");
                    }
                }
            }
        }


        internal void LogImageDraw(long size)
        {
            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            long now = Environment.TickCount;
            lock (_sync)
            {
                _imageSizes.Insert(0, size);
                _clientDrawTimes.Insert(0, now);

                if (_imageSizes.Count > MAX_SAMPLE_SIZE)
                {
                    _imageSizes.RemoveAt(_imageSizes.Count - 1);
                    _clientDrawTimes.RemoveAt(_clientDrawTimes.Count - 1);
                }


                AverageImageSize = _imageSizes.Average();
                double dt = (_clientDrawTimes.First() - _clientDrawTimes.Last()) / 1000.0f;
                /* doesn't make sense to calc the fps with a few frames*/
                if (dt > 0 && _imageSizes.Count >= 5)
                    AverageClientFps = _clientDrawTimes.Count / dt;
                else
                    AverageClientFps = double.NaN;

                LastImageSize = size;
            }
        }

        long _prevBeginTime;
        long _prevExpectedDuration;

        public void Begin(long expectedDuration)
        {
            lock (_sync)
            {
                // TODO: REVIEW THIS
                // Per MSDN:
                // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                long dt = Environment.TickCount - _prevBeginTime;
                if (_prevExpectedDuration < 0 || dt > _prevExpectedDuration)
                {
                    ResetClientStats();
                }

                _prevExpectedDuration = expectedDuration;
                _prevBeginTime = Environment.TickCount;
            }
        }

        public void ResetClientStats()
        {
            lock (_sync)
            {
                _clientDrawTimes.Clear();
                _imageSizes.Clear();

                SendLag = 0;
                RenderingLag = 0;
                AverageImageSize = 0;

                AverageImageSize = 0;
                AverageMouseMoveMsgRTTWithResponse = 250;
                AverageMouseMoveMsgRTTWithoutResponse = 250;

                _mouseMoveDurationWithRsp.Clear();
                _mouseMoveDurationWithoutRsp.Clear();
            }
        }


        object _mouseMoveMsgDurationWithResponseSync = new object();
        List<long> _mouseMoveDurationWithRsp = new List<long>();
        List<long> _mouseMoveDurationWithoutRsp = new List<long>();
        double _mouseMoveMsgDurationWithResponse;
        double _mouseMoveMsgDurationWithoutResponse;
        public double AverageMouseMoveMsgRTTWithResponse
        {
            get
            {
                lock (_sync)
                {
                    return _mouseMoveMsgDurationWithResponse;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_mouseMoveMsgDurationWithResponse != value)
                    {
                        _mouseMoveMsgDurationWithResponse = value;
                        OnPropertyChanged("AverageMouseMoveMsgRTTWithResponse");
                    }
                }
            }
        }


        public double AverageMouseMoveMsgRTTWithoutResponse
        {
            get
            {
                lock (_sync)
                {
                    return _mouseMoveMsgDurationWithoutResponse;
                }
            }
            set
            {
                lock (_sync)
                {
                    if (_mouseMoveMsgDurationWithoutResponse != value)
                    {
                        _mouseMoveMsgDurationWithoutResponse = value;
                        OnPropertyChanged("AverageMouseMoveMsgRTTWithResponse");
                    }
                }
            }


        }

        internal void LogMouseMoveRTTWithResponse(int msgNumber, long dt)
        {
            lock (_mouseMoveMsgDurationWithResponseSync)
            {
                _mouseMoveDurationWithRsp.Insert(0, dt);
                if (_mouseMoveDurationWithRsp.Count > MAX_SAMPLE_SIZE * 2)
                {
                    _mouseMoveDurationWithRsp.RemoveAt(_mouseMoveDurationWithRsp.Count - 1);
                }
                AverageMouseMoveMsgRTTWithResponse = _mouseMoveDurationWithRsp.Average();

                Platform.Log(LogLevel.Debug, "******* RTT #{0}: {1} ms. Average:{2:0.0} *********", msgNumber, dt, AverageMouseMoveMsgRTTWithResponse);
            }
        }

        internal void LogMouseMoveMsgDurationWithoutResponse(long dt)
        {
            lock (_mouseMoveMsgDurationWithResponseSync)
            {
                _mouseMoveDurationWithoutRsp.Insert(0, dt);

                if (_mouseMoveDurationWithoutRsp.Count > MAX_SAMPLE_SIZE)
                {
                    _mouseMoveDurationWithoutRsp.RemoveAt(_mouseMoveDurationWithoutRsp.Count - 1);
                }

                AverageMouseMoveMsgRTTWithoutResponse = _mouseMoveDurationWithoutRsp.Average();

            }
        }

        public const int MAX_SAMPLE_SIZE = 30;

        internal void IncrementSendLag(int count)
        {
            lock (_sync)
            {
                Interlocked.Add(ref _sendLag, count);
                OnPropertyChanged("SendLag");
            }
        }

        internal void DecrementSendLag(int count)
        {
            lock (_sync)
            {
                Interlocked.Add(ref _sendLag, -count);
                OnPropertyChanged("SendLag");
            }
        }

        long _lastImageSize;
        public long LastImageSize
        {
            get { return _lastImageSize; }
            set
            {
                if (_lastImageSize != value)
                {
                    _lastImageSize = value;
                    OnPropertyChanged("LastImageSize");
                }
            }
        }

        double _speedInMbps;
        public double SpeedInMbps
        {
            get { return _speedInMbps; }
            set
            {
                _speedInMbps = value;
                OnPropertyChanged("SpeedInMbps");
            }
        }

        long _mouseWheelCount;
        public long MouseWheelMsgCount
        {
            get
            {
                lock (_sync)
                {
                    return _mouseWheelCount;
                }

            }
        }

        internal void IncrementMouseWheelMsgCount(int count)
        {
            lock (_sync)
            {
                Interlocked.Add(ref _mouseWheelCount, count);
                OnPropertyChanged("MouseWheelMsgCount");
            }
        }

        internal void DecrementMouseWheelMsgCount(int count)
        {
            lock (_sync)
            {
                Interlocked.Add(ref _mouseWheelCount, -count);
                OnPropertyChanged("MouseWheelMsgCount");
            }
        }
    }
}
