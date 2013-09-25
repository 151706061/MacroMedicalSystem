#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using Macro.Common;
using Macro.ImageServer.Web.Application.Pages.Common;
using Resources;
using Macro.ImageServer.Web.Common.Extensions;

namespace Macro.ImageServer.Web.Application.Pages.Help
{
    [ExtensibleAttribute(ExtensionPoint = typeof(AboutPageExtensionPoint))]
    public partial class About : BasePage, IAboutPage
    {
        public string LicenseKey { get; set; }

        public About()
        {
            try
            {
                if (Thread.CurrentPrincipal.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.ServerPartitions))
                {
                    LicenseInformation.Reset();
                    LicenseKey = LicenseInformation.LicenseKey;
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ForeachExtension<IAboutPageExtension>(ext => ext.OnAboutPageInit(this));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.AboutPageTitle);            
        }

        #region IAboutPage Members

        public System.Web.UI.Control ExtensionContentParent
        {
            get
            {
                return ExtensionContentPlaceHolder;
            }
        }

        #endregion
    }
}
