#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Security;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Error
{
    public partial class TimeoutErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.TimeoutErrorPageTitle);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            SessionManager.SignOut();
        }

        protected void Login_Click(Object sender, EventArgs e)
        {
            Response.Redirect(SessionManager.LoginUrl);
        }

    }
}
