#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Macro.Common;

namespace Macro.ImageServer.Web.Common.Modules
{
    /// <summary>
    /// This class is intented for fixing the localization issue when browser only sends the language (and not the culture) in the header
    /// .NET 4 support neutral culture but we are using .NET 3.5
    /// </summary>
    class CultureOverrideModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
        }

        void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (CultureInfo.CurrentUICulture.IsNeutralCulture)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = GetDefaultCulture(CultureInfo.CurrentUICulture.Name);
            }
        }

        public void Dispose()
        {
            
        }

        static CultureInfo GetDefaultCulture(string language)
        {
            try
            {
                // Use CreateSpecificCulture to create culture-specific CultureInfo
                // CultureInfo.CreateSpecificCulture("en") will return an "en-US" CultureInfo
                //
                // CreateSpecificCulture does have a few caveats. The first is that it will return an ARBITRARY culture that has the requested language.  For en it will return en-US even if your user's in Australia or the Canada.
                // 
                // The second issue is that Chinese (zh) will throw an exception instead of returning a specific culture because its not possible to pick a geopolitically correct culture for Chinese
                //
                // Note: .NET 4 does not have problem with neutral culture
                // It picks the culture which has majority population for the language (eg, Spain for Spanish "es")
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch(Exception ex)
            {
                // fallback
                Platform.Log(LogLevel.Debug, ex, "Unable to determine the culture for '{0}' language. Fallback to English.", language);
                return new CultureInfo("en-US");
            }
        }
    }
}
