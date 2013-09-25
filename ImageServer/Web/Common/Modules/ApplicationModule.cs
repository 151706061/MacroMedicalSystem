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

namespace Macro.ImageServer.Web.Common.Modules
{
    public class ApplicationModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(PluginManager_PluginLoaded);
        }

        #endregion

        void PluginManager_PluginLoaded(object sender, PluginLoadedEventArgs e)
        {
            Platform.Log(LogLevel.Info, e.Message);
        }
    }
}
