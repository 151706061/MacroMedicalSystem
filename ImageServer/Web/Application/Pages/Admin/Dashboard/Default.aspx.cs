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
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Web.Application.Pages.Common;
using Resources;
using Macro.ImageServer.Common;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Dashboard.View)]
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.DashboardTitle);

            
        }
    }
}