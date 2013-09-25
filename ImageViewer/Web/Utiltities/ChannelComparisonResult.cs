#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace Macro.ImageViewer.Web.Utiltities
{
    class ChannelComparisonResult
    {
        private readonly List<double> _samples = new List<double>();

        private Statistics _statistics;


        public void AddSample(double value)
        {
            _samples.Add(value);
            _statistics = null; // clear it
        }
        
        public string Name { get; set; }

        public double MeanError
        { 
            get
            {
                if (_statistics==null)
                {
                    _statistics = MathUtils.CalculateStatistics(_samples);    
                }

                return _statistics.Mean;
                
            } 
        }

        public double StdDeviation
        {
            get
            {
                if (_statistics == null)
                {
                    _statistics = MathUtils.CalculateStatistics(_samples);
                }
                return _statistics.StdDeviation;
                
            }
        }

        public double MinError
        {
            get
            {
                if (_statistics == null)
                {
                    _statistics = MathUtils.CalculateStatistics(_samples);
                }
                return _statistics.Min;

            }
        }

        public double MaxError
        {
            get
            {
                if (_statistics == null)
                {
                    _statistics = MathUtils.CalculateStatistics(_samples);
                }
                return _statistics.Max;

            }
        }
        
    }
}