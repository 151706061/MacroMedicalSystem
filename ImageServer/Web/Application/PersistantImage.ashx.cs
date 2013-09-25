#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Macro.ImageServer.Web.Common;

namespace Macro.ImageServer.Web.Application
{
    public class PersistantImage : IHttpHandler
    {
        private HttpContext _currentContext = null;

        protected string Key
        {
            get
            {
                return ((_currentContext != null) && (_currentContext.Request != null) && (_currentContext.Request.QueryString["key"] != null)) ? _currentContext.Request.QueryString["key"] : "";
            }
        }

        protected string Url
        {
            get
            {
                return ((Key.Length > 0) && (WebConfigurationManager.AppSettings.Get(Key) != null)) ? WebConfigurationManager.AppSettings.Get(Key) : "";
            }
        }

        protected string Path
        {
            get
            {
                return (Url.Length > 0) ? HttpContext.Current.Server.MapPath("~/App_Themes/" + ThemeManager.CurrentTheme + "/" + Url) : "";
            }
        }

        protected string Extension
        {
            get
            {
                return ((Path.Length > 0) && (Path.LastIndexOf('.') > -1) && (Path.LastIndexOf('.') < (Path.Length - 1))) ? Path.Substring(Path.LastIndexOf('.') + 1).ToLower() : "";
            }
        }

        protected bool IsSafe()
        {
            bool bSafeExtension = (Extension.Equals("jpg") || Extension.Equals("jpeg") || Extension.Equals("gif") || Extension.Equals("png"));

            Regex regEx = new Regex("[^a-zA-Z0-9._-~/]");
            bool bSafeChars = (!regEx.IsMatch(Url)) && (!Url.Contains(".."));

            return (bSafeExtension && bSafeChars);
        }

        public void ProcessRequest(HttpContext context)
        {
            _currentContext = context;

            if ((Path.Length > 0) && IsSafe())
            {
                context.Response.ContentType = "image/" + Extension;
                context.Response.Cache.AppendCacheExtension("post-check=900,pre-check=3600");
                context.Response.WriteFile(Path);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("");
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }

}