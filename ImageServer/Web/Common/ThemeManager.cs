#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web;
using Macro.Common;
using System.Web.UI;

namespace Macro.ImageServer.Web.Common
{
    public interface IThemeManager
    {
        string GetTheme(Page page);
        string GetDefaultTheme();
    }

    [ExtensionPoint]
    public class ThemeManagerExtensionPoint : ExtensionPoint<IThemeManager>
    {
    }

    public static class ThemeManager
    {
        private static readonly IThemeManager _manager;
        static ThemeManager()
        {
            try
            {
                var xp = new ThemeManagerExtensionPoint();
                _manager = xp.CreateExtension() as IThemeManager;
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Debug, "Unable to find theme manager. {0}", ex.Message);
            }
        }

        public static string CurrentTheme
        {
            get
            {
                var existingTheme = HttpContext.Current.Items["Theme"] as string;
                if (!string.IsNullOrEmpty(existingTheme))
                {
                    return existingTheme;
                }
                else
                {
                    var theme = _manager != null ? _manager.GetTheme(HttpContext.Current.Handler as Page) : ImageServerConstants.Default;
                    HttpContext.Current.Items["Theme"] = theme;
                    return theme;
                }

            }
        }

        public static void ApplyTheme(Page page)
        {
            if (page!=null)
            {
                var existingTheme = HttpContext.Current.Items["Theme"] as string;
                if (!string.IsNullOrEmpty(existingTheme))
                {
                    page.Theme = existingTheme;
                }
                else
                {
                    page.Theme = _manager != null ? _manager.GetTheme(page) : ImageServerConstants.Default;
                    HttpContext.Current.Items["Theme"] = page.Theme;
                }
                
            }
        }

        public static string GetDefaultTheme()
        {
            return _manager != null ? _manager.GetDefaultTheme() : ImageServerConstants.Default;
        }
    }
}