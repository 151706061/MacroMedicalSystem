#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using System.Web;
using System.Web.Security;
using Macro.Common;
using Macro.Enterprise.Common;
using Macro.ImageServer.Web.Common.Exceptions;
using Macro.Web.Enterprise.Authentication;

namespace Macro.ImageServer.Web.Common.Security
{
    class CustomFormAuthenticationModule : IHttpModule
    {
        private bool _contextDisposed;

        #region IHttpModule Members

        private HttpApplication _context;
        public void Dispose()
        {
        	AppDomain.CurrentDomain.DomainUnload-=CurrentDomain_DomainUnload;
        	
            if (_context != null && !_contextDisposed)
            {
                _context.AuthorizeRequest -= AuthorizeRequest;
                _context.Disposed -= ContextDisposed;
            }
        }

        public void Init(HttpApplication context)
        {
            _context = context;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            _context.AuthorizeRequest += AuthorizeRequest;
            _context.Disposed += ContextDisposed;
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Platform.Log(LogLevel.Info, "App Domain Is Unloaded");
        }

        void ContextDisposed(object sender, EventArgs e)
        {
            _contextDisposed = true;
        }

        static void AuthorizeRequest(object sender, EventArgs e)
        {
            SessionInfo session=null;
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
                {
                    // Note: If user signed out in another window, the ticket would have been 
                    // removed from the browser and this code shoudn't be executed.
                    
                    // resemble the SessionInfo from the ticket.
                    FormsIdentity loginId = (FormsIdentity) HttpContext.Current.User.Identity ;
                    FormsAuthenticationTicket ticket = loginId.Ticket;

                    String[] fields = ticket.UserData.Split('|');
                    String tokenId = fields[0];
                    String userDisplayName = fields[1];
                    SessionToken token = new SessionToken(tokenId, ticket.Expiration);
                    session = new SessionInfo(loginId.Name, userDisplayName, token);

                    // Initialize the session. This will throw exception if the session is no longer
                    // valid. For eg, time-out.
                    SessionManager.InitializeSession(session);
                }

                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                {
                    String user = SessionManager.Current != null ? SessionManager.Current.User.Identity.Name : "Unknown";

                    Thread.CurrentThread.Name =
                        String.Format(SR.WebGUILogHeader, 
                            HttpContext.Current.Request.UserHostAddress,
                            HttpContext.Current.Request.Browser.Browser,
                            HttpContext.Current.Request.Browser.Version,
                            user);
                }
                
            }
            catch (SessionValidationException)
            {
                // SessionValidationException is thrown when the session id is invalid or the session already expired.
                // If session already expired, 
                if (session != null && session.Credentials.SessionToken.ExpiryTime < Platform.Time)
                {
                    SessionManager.SignOut(session);
                }
                else
                {
                    // redirect to login screen
                    SessionManager.TerminateSession("The current session is no longer valid.", SR.MessageCurrentSessionNoLongerValid);
                }
            }
            catch(Exception ex)
            {
                // log the exception
                ExceptionHandler.ThrowException(ex);
            }
            
           
        }

        #endregion
    }
}
