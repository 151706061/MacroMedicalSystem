#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class StudyAlertContextInfoView : System.Web.UI.UserControl, IAlertPopupView
    {
        protected AlertSummary Alert { get; set; }

        public void SetAlert(AlertSummary alert)
        {
            Alert = alert;
        }
    }
}