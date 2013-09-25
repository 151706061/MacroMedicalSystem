#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Desktop;

namespace Macro.ImageViewer.Web
{
    internal class WebDesktopWindow : DesktopWindow
    {
        internal WebDesktopWindow(DesktopWindowCreationArgs args, Application application) 
            : base(args, application)
        {
        }

        internal new void Open()
        {
            base.Open();
        }
    }
}
