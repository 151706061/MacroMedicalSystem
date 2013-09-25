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
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Exceptions;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    
    public partial class StudyList : BasePage
    {
        public string IsAuthorized
        {
            get { return ViewState["Authorized"] as string; }
            set { ViewState["Authorized"] = value; }
        }

        public string WebViewerInitString
        {
            get { return ViewState["WebViewerInitString"] as string; }
            set { ViewState["WebViewerInitString"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IsAuthorized))
            {
                if (Context.Items["Authorized"] == null || !Context.Items["Authorized"].Equals("true"))
                {
                    Server.Transfer("~/Pages/Error/WebViewerAuthorizationErrorPage.aspx");
                }
                else
                {
                    IsAuthorized = "true";
                }
            }

            if (string.IsNullOrEmpty(WebViewerInitString))
            {
                WebViewerInitString = Request.Params[ImageServerConstants.WebViewerQueryStrings.WebViewerInitParams];
            }

            //Extract the WebViewer Init Parameters for retreiving studies
            var initParams = new WebViewerInitParams();
            string[] vals = HttpUtility.UrlDecode(WebViewerInitString).Split(new[] { '?', ';', '=', ',', '&' });
            for (int i = 0; i < vals.Length - 1; i++)
            {
                if (String.IsNullOrEmpty(vals[i]))
                    continue;

                if (vals[i].Equals(ImageServerConstants.WebViewerStartupParameters.Study))
                {
                    i++;
                    initParams.StudyInstanceUids.Add(vals[i]);
                }
                else if (vals[i].Equals(ImageServerConstants.WebViewerStartupParameters.PatientID))
                {
                    i++;
                    initParams.PatientIds.Add(vals[i]);
                }
                else if (vals[i].Equals(ImageServerConstants.WebViewerStartupParameters.AeTitle))
                {
                    i++;
                    initParams.AeTitle = vals[i];
                }
                else if (vals[i].Equals(ImageServerConstants.WebViewerStartupParameters.AccessionNumber))
                {
                    i++;
                    initParams.AccessionNumbers.Add(vals[i]);
                }
            }

            SearchPanel.InitParams = initParams;
            SearchPanel.Username = Context.Items[ImageServerConstants.WebViewerQueryStrings.Username] as String;
            SearchPanel.SessionId = Context.Items[ImageServerConstants.WebViewerStartupParameters.Session] as String;

        }

        protected void GlobalScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            GlobalScriptManager.AsyncPostBackErrorMessage = ExceptionHandler.ThrowAJAXException(e.Exception);
        }
    }
}
