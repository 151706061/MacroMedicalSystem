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
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Move)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the UserPanel information
            IMasterProperties master = Master as IMasterProperties;
            master.DisplayUserInformationPanel = false;
        }
    }
}
