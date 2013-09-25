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
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.ServiceScheduling)]
    public partial class Default : BasePage
    {

        #region Private members

        #endregion Private members

        #region Protected methods


        void ServiceLockPanel_ServiceLockUpdated(ServiceLock serviceLock)
        {
            DataBind();
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServiceLockPanel.ServiceLockUpdated += ServiceLockPanel_ServiceLockUpdated;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();

            SetPageTitle(Titles.ServiceSchedulingPageTitle);           
        }

        #endregion  Protected methods

        #region Public Methods


        #endregion
    }
}
