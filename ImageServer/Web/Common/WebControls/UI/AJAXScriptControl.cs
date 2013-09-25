#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using AjaxControlToolkit;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    public class AJAXScriptControl : ScriptUserControl
    {
        public AJAXScriptControl()
            : base(false, HtmlTextWriterTag.Div)
        {
        }
    }
}