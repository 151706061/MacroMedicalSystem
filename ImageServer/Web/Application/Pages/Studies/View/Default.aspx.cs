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
using System.Security.Permissions;
using System.Web;
using Macro.Common;
using Macro.ImageServer.Web.Common.Security;
using System.Text;
using System.Threading;
using Resources;
using Macro.ImageServer.Enterprise.Authentication;

namespace Macro.ImageServer.Web.Application.Pages.Studies.View
{

    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.ViewImages)]
    public partial class Default : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetPageTitle(Titles.ViewImagesPageTitle);
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
                return String.Format("{0}={1},{2}={3},{4}=true", 
                    ImageServerConstants.WebViewerStartupParameters.Username, SessionManager.Current.Credentials.UserName,
                    ImageServerConstants.WebViewerStartupParameters.Session, SessionManager.Current.Credentials.SessionToken.Id,
					ImageServerConstants.WebViewerStartupParameters.IsSessionShared);
            }
        }

        public string ApplicationSettings
        {
            get
            {
                StringBuilder parameters = new StringBuilder();
                parameters.AppendFormat("{0}={1}",ImageServerConstants.WebViewerStartupParameters.TimeoutUrl, Page.ResolveUrl(ImageServerConstants.PageURLs.DefaultTimeoutPage));
                parameters.AppendFormat(",{0}={1}",ImageServerConstants.WebViewerStartupParameters.Language, Thread.CurrentThread.CurrentUICulture.Name);

                return parameters.ToString();
            }
        }

        public string OtherParameters
        {
            get
            {
                return String.Format("{0}={1}", ImageServerConstants.WebViewerStartupParameters.LocalIPAddress, Request.UserHostAddress);
            }
        }

        protected void SetPageTitle(string title)
        {
            if (title.Contains("{0}"))
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]) ? String.Format(title, ProductInformation.GetNameAndVersion(false, true)) : String.Format(title, ProductInformation.GetNameAndVersion(false, true)) + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
            else
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]) ? title : title + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
        }
    }
}