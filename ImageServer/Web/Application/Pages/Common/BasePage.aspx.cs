#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Macro.Common;
using Macro.ImageServer.Common;
using Macro.ImageServer.Web.Common;
using Resources;
using Macro.ImageServer.Web.Common.Extensions;
using Macro.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Common
{
    /// <summary>
    /// Base class for all the pages.
    /// </summary>
    /// <remarks>
    /// Derive new page from this class to ensure consistent look across all pages.
    /// </remarks>
    public partial class BasePage : System.Web.UI.Page
    {
        private bool _extensionLoaded;
        protected List<object> Extensions = new List<object>();

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            LoadExtensions();

            ThemeManager.ApplyTheme(this);

            // This is necessary because Safari and Chrome browsers don't display the Menu control correctly.
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        
        }

        private void LoadExtensions()
        {
            lock (Extensions)
            {
                if (!_extensionLoaded)
                {
                    try
                    {
                        Extensions.Clear();

                        var attrs = Macro.Common.Utilities.AttributeUtils.GetAttributes<ExtensibleAttribute>(this.GetType(), true);
                        foreach (var attr in attrs)
                        {
                            var xp = Activator.CreateInstance(attr.ExtensionPoint);
                            Extensions.AddRange((xp as ExtensionPoint).CreateExtensions());
                        }
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Unable to load page extension");
                    }
                }
                
            }
            
        }
        
        protected void SetPageTitle(string title)
        {
            SetPageTitle(title, true);
        }

        private static string GetProductInformation()
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(ProductInformation.Release))
                tags.Add(Titles.LabelNotForDiagnosticUse);
            if (!ServerPlatform.IsManifestVerified)
                tags.Add(ConstantResourceManager.ModifiedInstallation);

            var name = ProductInformation.GetName(false, true);
            if (tags.Count == 0)
                return name;

            var tagString = string.Join(" | ", tags.ToArray());
            return string.IsNullOrEmpty(name) ? tagString : string.Format("{0} - {1}", name, tagString);
        }

        protected void SetPageTitle(string title, bool includeProductInfo)
        {
            if (includeProductInfo)
            {
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                                 ? String.Format(title, GetProductInformation())
                                 : String.Format(title, GetProductInformation()) + " [" +
                                   ConfigurationManager.AppSettings["ServerName"] + "]";
            }
            else
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                    ? title
                    : title + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
		}

        protected void ForeachExtension<T>(Action<T> action)
        {
            CollectionUtils.ForEach<T>(Extensions.OfType<T>(), action);
        }
    }
}