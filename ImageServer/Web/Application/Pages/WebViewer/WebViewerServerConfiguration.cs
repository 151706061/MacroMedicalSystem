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
using System.Configuration;
using System.Web;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    public class WebServiceConfiguration
    {

        public static string InitParamString
        {
            get
            {
#if SILVERLIGHT4
                return String.Format("{0}={1},{2}={3},{4}={5}",
                                     ImageServerConstants.WebViewerStartupParameters.LANMode, true,
                                     ImageServerConstants.WebViewerStartupParameters.Port, ConfigurationManager.AppSettings["Port"],
                                     ImageServerConstants.WebViewerStartupParameters.InactivityTimeout, ConfigurationManager.AppSettings["InactivityTimeout"]);
#else
                return String.Format("LANMode={0},Port={1},InactivityTimeout={2}",
                                     false, ConfigurationManager.AppSettings["Port"],
                                     ConfigurationManager.AppSettings["InactivityTimeout"]);
#endif

            }
        }
    }
}