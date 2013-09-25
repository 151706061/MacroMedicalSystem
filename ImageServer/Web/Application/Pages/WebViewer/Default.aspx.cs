#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using System.Web;
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Web.Common.Security;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    public partial class Default : System.Web.UI.Page
    {
        public string UserName
        {
            get
            {
                string userName = string.Empty;
                string initString = Request.QueryString[ImageServerConstants.WebViewerQueryStrings.WebViewerInitParams];
                int start = initString.IndexOf(ImageServerConstants.WebViewerQueryStrings.Username + "=");

                if (start >= 0)
                {
                        start += (ImageServerConstants.WebViewerQueryStrings.Username + "=").Length;
                        userName = initString.Substring(start);
                        int end = userName.IndexOf(',');
                        if (end < 0) userName = userName.Substring(start);
                        else userName = userName.Substring(0, end);
                }

                return userName;
            }

        }

        public string SessionID
        {
            get
            {
                string sessionId = string.Empty;
                string initString = Request.QueryString[ImageServerConstants.WebViewerQueryStrings.WebViewerInitParams];
                int start = initString.IndexOf(ImageServerConstants.WebViewerQueryStrings.Session + "=");

                if (start >= 0)
                {
                    start += (ImageServerConstants.WebViewerQueryStrings.Session + "=").Length;
                    sessionId = initString.Substring(start);
                    int end = sessionId.IndexOf(',');
                    if (end < 0) sessionId = initString.Substring(start);
                    else sessionId = initString.Substring(0, end);
                }

                return sessionId;                
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
        	// for time-out to work, don't cache this page 
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        public string UserCredentialsString
        {
            get
            {
                return String.Format("{0}={1},{2}={3}", 
                    ImageServerConstants.WebViewerStartupParameters.Username, UserName,
                    ImageServerConstants.WebViewerStartupParameters.Session, SessionID);
            }
        }

        public string ApplicationSettings
        {
            get
            {
                return String.Format("{0}={1}", ImageServerConstants.WebViewerStartupParameters.TimeoutUrl, Page.ResolveUrl(ImageServerConstants.PageURLs.WebViewerTimeoutPage));
            }
        }

        public string OtherParameters
        {
            get
            {
                return String.Format("{0}={1}", ImageServerConstants.WebViewerStartupParameters.LocalIPAddress, Request.UserHostAddress);
            }
        }
    }
}