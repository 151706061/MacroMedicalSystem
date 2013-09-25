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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    public partial class WorkQueueAlertPanel : System.Web.UI.UserControl
    {
        public string Text
        {
            get { return Message.Text; }
            set { Message.Text = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Visible = !String.IsNullOrEmpty(Message.Text);
            base.OnPreRender(e);
        }
    }
}