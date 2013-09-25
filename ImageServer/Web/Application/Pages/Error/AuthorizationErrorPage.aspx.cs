#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Security;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Error
{
    public partial class AuthorizationErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.Items[ImageServerConstants.ContextKeys.ErrorMessage] != null) {
                ErrorMessageLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorMessage].ToString();
            } 
            if (Context.Items[ImageServerConstants.ContextKeys.StackTrace] != null)
            {
                StackTraceTextBox.Text = Context.Items[ImageServerConstants.ContextKeys.StackTrace].ToString();
                StackTraceTextBox.Visible = true;
                StackTraceMessage.Visible = true;
            }
            if (Context.Items[ImageServerConstants.ContextKeys.ErrorDescription] != null)
            {
                DescriptionLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorDescription].ToString();
            }

            #region UnitTest
            if (false==String.IsNullOrEmpty(Page.Request.QueryString["test"]))
            {
                StackTraceMessage.Visible = true;
                StackTraceTextBox.Visible = true;
                StackTraceTextBox.Text = "Dummy stack trace";
            }
            #endregion

            SetPageTitle(Titles.AuthorizationErrorPageTitle);
        }

        protected void Logout_Click(Object sender, EventArgs e)
        {
            SessionManager.SignOut();
            Response.Redirect(ImageServerConstants.PageURLs.LoginPage);
        }

        protected void DefaultPage_Click(Object sender, EventArgs e)
        {
            String defaultPageUrl = UserProfile.GetDefaultUrl();
			if (defaultPageUrl == null)
				Response.End();
            Response.Redirect(defaultPageUrl);
        }
    }
}
