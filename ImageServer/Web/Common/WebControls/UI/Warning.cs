#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    [ToolboxData("<{0}:Warning runat=server></{0}:Warning>")]
    [Themeable(true)]
    public class Warning : System.Web.UI.WebControls.Image
    {
        
        #region Public Properties

        /// <summary>
        /// Sets or gets the warning message
        /// </summary>
        public string Message { get; set; }

        #endregion Public Properties

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.ToolTip = Message;
        }
    }
}