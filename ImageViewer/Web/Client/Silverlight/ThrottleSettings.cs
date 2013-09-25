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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Macro.ImageViewer.Web.Client.Silverlight
{
    public enum ThrottleStrategy
    {
        WhenMouseMoveRspReceived
    }

    public enum LagDetectionStrategy
    {        
        /// <summary>
        /// Detect lag based on the mouse move message processed events.
        /// </summary>
        // Note: When MM messages are received at the wrong order, they are queued on the server and processed later.
        WhenMouseMoveIsProcessed
    }

    public class ThrottleSettings : INotifyPropertyChanged
    {
        static ThrottleSettings _instance = new ThrottleSettings();
        
        bool _enableFPSCap;
        bool _simulateNetworkTrafficOrder;
        int _maxPendingMouseMoveMsgAllowed = 0;
        int _mouseMoveOffset;
        int _constantRate;
        ThrottleStrategy _strategy;
        LagDetectionStrategy _lagStrategy;
        

        public static ThrottleSettings Default
        {
            get { return _instance; }
        }

        private ThrottleSettings()
        {
            Strategy = ThrottleStrategy.WhenMouseMoveRspReceived;
            MaxPendingMouseMoveMsgAllowed = 1;
            ConstantRate = 50;
            SimulateNetworkTrafficOrder = false;
            EnableFPSCap = true;
            //LagDetectionStrategy=Silverlight.LagDetectionStrategy.WhenTileUpdateReturn;
            LagDetectionStrategy = Silverlight.LagDetectionStrategy.WhenMouseMoveIsProcessed;
            EnableDynamicImageQuality = true;
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

        private bool _enableDynamicImageQuality;
        public bool EnableDynamicImageQuality
        {
            get { return _enableDynamicImageQuality; }
            set
            {
                if (_enableDynamicImageQuality != value)
                {
                    _enableDynamicImageQuality = value;
                    OnPropertyChanged("EnableDynamicImageQuality");
                }
            }
        }

        public LagDetectionStrategy LagDetectionStrategy
        {
            get { return _lagStrategy; }
            set
            {
                if (_lagStrategy != value)
                {
                    _lagStrategy = value;
                    OnPropertyChanged("LagDetectionStrategy");
                }
            }
        }

        public ThrottleStrategy Strategy
        {
            get { return _strategy; }
            set
            {
                if (_strategy != value)
                {
                    _strategy = value;
                    OnPropertyChanged("Strategy");
                }
            }
        }

        public int ConstantRate
        {
            get { return _constantRate; }
            set
            {
                if (_constantRate != value)
                {
                    _constantRate = value;
                    OnPropertyChanged("ConstantRate");
                }
            }
        }


        public int MouseMoveOffset
        {
            get { return _mouseMoveOffset; }
            set
            {
                if (_mouseMoveOffset != value)
                {
                    _mouseMoveOffset = value;
                    OnPropertyChanged("MouseMoveOffset");
                }
            }
        }

        public int MaxPendingMouseMoveMsgAllowed
        {
            get { return _maxPendingMouseMoveMsgAllowed; }
            set
            {
                if (_maxPendingMouseMoveMsgAllowed != value)
                {
                    _maxPendingMouseMoveMsgAllowed = value;
                    OnPropertyChanged("MaxPendingMouseMoveMsgAllowed");
                }
            }
        }


        public bool SimulateNetworkTrafficOrder
        {
            get { return _simulateNetworkTrafficOrder; }
            set
            {
                if (_simulateNetworkTrafficOrder != value)
                {
                    _simulateNetworkTrafficOrder = value;
                    OnPropertyChanged("SimulateNetworkTrafficOrder");
                }
            }
        }

        public bool EnableFPSCap {
            get { return _enableFPSCap; }
            set
            {
                if (_enableFPSCap != value)
                {
                    _enableFPSCap = value;
                    OnPropertyChanged("EnableFPSCap");
                }
            }
        }
    }
}
