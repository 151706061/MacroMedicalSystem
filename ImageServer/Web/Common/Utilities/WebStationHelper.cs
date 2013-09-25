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

namespace Macro.ImageServer.Web.Common.Utilities
{
    public interface IWebStationHelper
    {
        string GetSplashScreenXamlAbsolutePath();
    }

    [ExtensionPoint]
    public sealed class WebStationHelperExtensionPoint:ExtensionPoint<IWebStationHelper>{}

    public static class EmbeddedWebStationHelper
    {
        private static readonly IWebStationHelper _helper;

        static EmbeddedWebStationHelper()
        {
            try
            {
                _helper = new WebStationHelperExtensionPoint().CreateExtension() as IWebStationHelper;
            }
            catch(Exception){
                // ignore
            }
        }

        public static string GetSplashScreenXamlAbsolutePath()
        {
            if (_helper==null)
            {
                return GetDefaultSplashScreenXamlAbsolutePath();
            }
            else
            {
                return _helper.GetSplashScreenXamlAbsolutePath();
            }
        }

        public static string GetDefaultSplashScreenXamlAbsolutePath()
        {
            string path = string.Format("~/App_Themes/{0}/WebStation/SplashScreen.xaml", ThemeManager.CurrentTheme);
            return VirtualPathUtility.ToAbsolute(path);
        }
    }
}