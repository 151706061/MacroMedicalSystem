#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace Macro.ImageViewer.Web.Utiltities
{
    class ImageComparisonResult
    {
        public ChannelComparisonResult[] Channels { get; set; }
    }

    static class BitmapComparison
    {
        public static ImageComparisonResult Compare(ref Bitmap bmp1, ref Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                throw new ArgumentException("Image sizes do not match");
            }

            ImageComparisonResult result = new ImageComparisonResult();
            result.Channels = new[]
                                  {
                                      new ChannelComparisonResult(){ Name = "R"}, 
                                      new ChannelComparisonResult(){ Name = "G"},
                                      new ChannelComparisonResult(){ Name = "B"}
                                  };

            for(int y=0; y<bmp1.Height; y++)
            {
                for(int x=0; x<bmp1.Width; x++)
                {
                    Color c1 = bmp1.GetPixel(x, y);
                    Color c2 = bmp2.GetPixel(x, y);

                    result.Channels[0].AddSample(c1.R - c2.R);
                    result.Channels[1].AddSample(c1.G - c2.G);
                    result.Channels[2].AddSample(c1.B - c2.B);
                }
            }

            return result;
        }
    }
}