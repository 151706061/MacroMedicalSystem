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
using Macro.ImageServer.Web.Common.Security;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {
        protected TimeSpan MinCountDownDuration
        {
            get
            {
                int duration = 30; // seconds
                Int32.TryParse(ConfigurationManager.AppSettings.Get("ClientTimeoutWarningMinDuration"), out duration);
                return TimeSpan.FromSeconds(duration);
            }
        }

        protected TimeSpan TimeoutLength
        {
            get
            {
                int duration = 1; // minutes
                Int32.TryParse(ConfigurationManager.AppSettings.Get("MultipleStudiesWebViewerTimeout"), out duration);
                return TimeSpan.FromMinutes(duration);
            }
        }
  }

}