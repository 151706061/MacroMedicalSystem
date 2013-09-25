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
    public partial class WebViewerAuthorizationErrorPage : BasePage
    {
        public string UserID
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UserID = Request.Params[ImageServerConstants.WebViewerQueryStrings.Username];
            
            if(Context.Items[ImageServerConstants.ContextKeys.ErrorMessage] != null) {
                ErrorMessageLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorMessage].ToString();
            } 

            if (Context.Items[ImageServerConstants.ContextKeys.ErrorDescription] != null)
            {
                DescriptionLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorDescription].ToString();
            }

            SetPageTitle(Titles.AuthorizationErrorPageTitle);
        }
    }
}
