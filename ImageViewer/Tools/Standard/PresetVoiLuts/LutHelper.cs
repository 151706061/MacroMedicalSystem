#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.Imaging;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.Tools.Standard.PresetVoiLuts
{
    internal static class LutHelper
    {
        public static bool IsModalityLutProvider(IPresentationImage presentationImage)
        {
            return presentationImage is IModalityLutProvider;
        }

        public static bool IsVoiLutProvider(IPresentationImage presentationImage)
        {
            return presentationImage is IVoiLutProvider;
        }

        public static bool IsVoiLutEnabled(IPresentationImage presentationImage)
        {
            var provider = presentationImage as IVoiLutProvider;
            return provider != null && provider.VoiLutManager.Enabled;
        }

        public static bool IsImageSopProvider(IPresentationImage presentationImage)
        {
            return presentationImage is IImageSopProvider;
        }

        public static bool IsDicomVoiLutsProvider(IPresentationImage presentationImage)
        {
            return presentationImage is IDicomVoiLutsProvider;
        }

        public static bool IsGrayScaleImage(IPresentationImage presentationImage)
        {
            var graphicProvider = presentationImage as IImageGraphicProvider;
            return graphicProvider != null && graphicProvider.ImageGraphic.PixelData is GrayscalePixelData;
        }

        public static bool IsColorImage(IPresentationImage presentationImage)
        {
            var graphicProvider = presentationImage as IImageGraphicProvider;
            return graphicProvider != null && graphicProvider.ImageGraphic.PixelData is ColorPixelData;
        }
    }
}
