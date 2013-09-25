#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Common;
using Macro.Desktop;

namespace Macro.ImageViewer.Web.View
{
    [ExtensionOf(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponentView : WebView, IApplicationComponentView
    {
        private ImageViewerComponent _component;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImageViewerComponent)component;
        }

        #endregion

       
    }
}
