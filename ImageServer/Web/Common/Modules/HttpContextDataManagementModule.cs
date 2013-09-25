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

namespace Macro.ImageServer.Web.Common.Modules
{
    class HttpContextDataManagementModule:  IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            // Per MSDN: we don't need to unregister events attached to HttpApplication here
        }

        public void Init(HttpApplication context)
        {
            // Apparently we don't need to unregister events added to HttpApplication.
            // IHttpModule.Dispose() is called when HttpApplication is dispose(). HttpApplication disposes
            // all events itself.
            context.EndRequest += EndRequest;
            
        }

        static void EndRequest(object sender, EventArgs e)
        {
            var contextData = HttpContextData.Current;
            if (contextData != null)
            {
                contextData.Dispose();
            } 
        }

        #endregion
    }
}
