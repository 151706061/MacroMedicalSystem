#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class WorkQueueAlertContextDataView : System.Web.UI.UserControl
    {
        private AlertSummary _alert;

        public AlertSummary Alert
        {
            get { return _alert; }
            set { _alert = value; }
        }
    }
}