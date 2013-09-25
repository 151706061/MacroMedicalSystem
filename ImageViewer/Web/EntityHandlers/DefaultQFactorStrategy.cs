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
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.Web.EntityHandlers
{
    internal class DefaultQFactorStrategy : IQFactorStrategy
    {

        private readonly Dictionary<long, long> _8bitImageSizeToQMap = new Dictionary<long, long>();
        private readonly Dictionary<long, long> _12bitImageSizeToQMap = new Dictionary<long, long>();
        private readonly Dictionary<long, long> _16bitImageSizeToQMap = new Dictionary<long, long>();
	    
        public DefaultQFactorStrategy()
        {
            InitOptimalQFactors();
        }

        private void InitOptimalQFactors()
        {
            // TODO: It's probably better to adjust this dynamically
            // based on: connection speed, bitdepth and range, 
            // zoom level (can be more aggressive at 5x than at 1x?), 
            // action: zoom, stack, pan
            _8bitImageSizeToQMap.Add(300 * 300, 70L);
            _8bitImageSizeToQMap.Add(400 * 400, 65L);
            _8bitImageSizeToQMap.Add(600 * 600, 60L);
            _8bitImageSizeToQMap.Add(800 * 800, 55L);
            _8bitImageSizeToQMap.Add(900 * 900, 50L);

            _12bitImageSizeToQMap.Add(300 * 300, 70L);
            _12bitImageSizeToQMap.Add(400 * 400, 60L);
            _12bitImageSizeToQMap.Add(600 * 600, 50L);
            _12bitImageSizeToQMap.Add(800 * 800, 40L);
            _12bitImageSizeToQMap.Add(900 * 900, 30L);

            _16bitImageSizeToQMap.Add(300 * 300, 50L);
            _16bitImageSizeToQMap.Add(400 * 400, 50L);
            _16bitImageSizeToQMap.Add(600 * 600, 45L);
            _16bitImageSizeToQMap.Add(800 * 800, 40L);
            _16bitImageSizeToQMap.Add(900 * 900, 30L);
        }

        public long GetOptimalQFactor(int imageWidth, int imageHeight, IImageSopProvider sop)
        {
            // TODO: It's probably better to adjust this dynamically
            // based on: connection speed, bitdepth and pixel value range, 
            // zoom level (can be more aggressive at 5x than at 1x or otherwise), 
            // action: zoom, stack, pan

            // We don't need to change the quality if the previous image
            // already < 32K
            //if (_prevImageSize >0 && _prevImageSize < 1024*30)
            //{
            //    return _quality;
            //}

            //float zoomLevel = 1.0f;
            //if (sop is ISpatialTransformProvider)
            //    zoomLevel = (sop as ISpatialTransformProvider).SpatialTransform.Scale;

            long lowestQuality = 50L;
            int highBit = sop.Frame.HighBit;

            if (highBit <= 8)
            {
                foreach (long k in _8bitImageSizeToQMap.Keys)
                {
                    if (k > imageWidth * imageHeight)
                        return _8bitImageSizeToQMap[k];

                    lowestQuality = _8bitImageSizeToQMap[k];
                }

                return lowestQuality;
            }
            if (highBit <= 12)
            {
                foreach (long k in _12bitImageSizeToQMap.Keys)
                {
                    if (k > imageWidth * imageHeight)
                        return _12bitImageSizeToQMap[k];
                    lowestQuality = _12bitImageSizeToQMap[k];
                }

                return lowestQuality;
            }
            foreach (long k in _16bitImageSizeToQMap.Keys)
            {
                if (k > imageWidth * imageHeight)
                    return _16bitImageSizeToQMap[k];

                lowestQuality = _16bitImageSizeToQMap[k];
            }

            return lowestQuality;
        }
    }
}