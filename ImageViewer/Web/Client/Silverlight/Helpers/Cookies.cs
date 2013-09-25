#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Text;

namespace Macro.ImageViewer.Web.Client.Silverlight.Helpers
{
    public class Cookies
    {        
        public static Cookies Default
        {
            get
            {
                return new Cookies();
            }
        }

        private Cookies() { }

        public string this[string key]
        {
            get
            {
                string[] cookies = HtmlPage.Document.Cookies.Split(';');
                key += '=';
                foreach (string cookie in cookies)
                {
                    string cookieStr = cookie.Trim();
                    if (cookieStr.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] vals = cookieStr.Split('=');

                        if (vals.Length >= 2)
                        {
                            return vals[1];
                        }

                        return string.Empty;
                    }
                }

                return null;
            }
            set
            {
                //string oldCookie = HtmlPage.Document.GetProperty("cookie") as String;
                //DateTime expiration = DateTime.UtcNow + TimeSpan.FromDays(2000);
                string theCookie = String.Format("{0}={1};expires=0", key, value);

                HtmlPage.Document.SetProperty("cookie", theCookie);


            }
            
        }
    }
}
