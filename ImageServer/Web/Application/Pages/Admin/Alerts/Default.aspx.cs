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
using Macro.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Alerts
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Alert.View)]
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.AlertsPageTitle);
        }
    }
}
