#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using Macro.Common;
using Macro.ImageServer.Web.Common.Security;

namespace Macro.ImageServer.Web.Application.Controls
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {

        protected TimeSpan MinCountDownDuration
        {
            get
            {
                double duration = 30; // seconds
                double.TryParse(ConfigurationManager.AppSettings.Get("ClientTimeoutWarningMinDuration"), out duration);
                duration = Math.Min(duration, (SessionManager.Current.Credentials.SessionToken.ExpiryTime - Platform.Time).TotalSeconds);

                return TimeSpan.FromSeconds(duration);
            }
        }
  }

}