#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;

namespace Macro.ImageViewer.Web.View
{
     /// <summary>
    /// Abstract base class for all Web-based views.  Any class that implements a view using
    /// a Web View as the underlying GUI toolkit should subclass this class.
    /// </summary>
    [GuiToolkit(Macro.Common.GuiToolkitID.Web)]
    public abstract class WebView
    {
         /// <summary>
        /// Gets the toolkit ID, which is always <see cref="Macro.Common.GuiToolkitID.Web"/>.
        /// </summary>
        public string GuiToolkitID
        {
            get { return Macro.Common.GuiToolkitID.Web; }
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object GuiElement
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
