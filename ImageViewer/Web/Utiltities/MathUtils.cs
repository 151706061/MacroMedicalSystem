#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace Macro.ImageViewer.Web.Utiltities
{
    static class MathUtils
    {
       public static double Mean(IList<double> samples)
        {
            double sum = 0;
            foreach (double v in samples) sum += v;
            return sum/ samples.Count;
        }

        public static Statistics CalculateStatistics(List<double> samples)
        {
            Statistics result = new Statistics();
            result.Mean = Mean(samples);
            double sumX2 = 0;
            double min = 0;
            double max = 0;
            
            //subtract the mean from each of the numbers
            foreach (double sample in samples)
            {
                double delta = result.Mean - sample;
                if (Math.Abs(sample) < min)
                    min = Math.Abs(sample);
                else if (Math.Abs(sample) > max)
                    max = Math.Abs(sample);

                sumX2 += delta * delta;
            }

            result.StdDeviation = Math.Sqrt(sumX2 / samples.Count);
            result.Min = min;
            result.Max = max;
            return result;
        }
    }
}