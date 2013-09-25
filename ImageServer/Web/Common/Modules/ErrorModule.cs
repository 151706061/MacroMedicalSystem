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
using Macro.ImageServer.Web.Common.Exceptions;

namespace Macro.ImageServer.Web.Common.Modules
{
    /// <summary>
    /// Used to handle web application error. 
    /// </summary>
    /// <remarks>
    /// This module allows capture error and write to the log file. To use it, specify this in the <httpModules> section of web.config:
    /// 
    ///     <add  name="ErrorModule" type="Macro.ImageServer.Web.Common.ErrorModule, Macro.ImageServer.Web.Common" />
    /// 
    /// </remarks>
    public class ErrorModule : IHttpModule
    {
        #region IHttpModule Members

        public void Init(HttpApplication application)
        {
            application.Error += new EventHandler(application_Error);
        }

        public void Dispose()
        {
        }

        #endregion

        public void OnError(object obj, EventArgs args)
        {
            HttpContext context = HttpContext.Current;

            Exception baseException = context.Server.GetLastError();
            Platform.Log(LogLevel.Error, context.Error);

            if (baseException != null)
            {
                baseException = baseException.GetBaseException();

                context.Server.ClearError();

                string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", baseException.Message, baseException.Source, baseException.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);

                ExceptionHandler.ThrowException(baseException);
            }
        }
        
        public void application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            Exception theException;
            Platform.Log(LogLevel.Error, ctx.Error);

            for (theException = ctx.Server.GetLastError();
                 theException != null && theException.InnerException != null;
                 theException = theException.InnerException)
            {
            }

            ctx.Server.ClearError();

            if(theException != null && (theException.Message.Equals("Access is denied.") || theException.Message.Equals("Request for principal permission failed.") || theException.GetType().Name.Equals("SecurityException")))
            {
                ExceptionHandler.ThrowException(new AuthorizationException());
            } 
            else if (theException is HttpException)
            {
                HttpException exception = theException as HttpException;
                Platform.Log(LogLevel.Error, "HTTP Error {0}: {0}", exception.ErrorCode, exception);
            }
            else
            {
                Platform.Log(LogLevel.Error, "Unhandled exception: {0}", theException);
            }

            if (theException != null)
                ExceptionHandler.ThrowException(theException);
        }
    }
}
