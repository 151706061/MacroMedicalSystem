#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Common;
using Macro.Web.Enterprise.Authentication;
using Macro.Web.Services;

namespace Macro.ImageServer.Web.Common.Security
{
    [ExtensionOf(typeof(UserAuthenticationExtensionPoint))]
    public class WebViewerUserAuthentication:IUserAuthentication
    {
        public UserSessionInfo QuerySession(string sessionId)
        {
            using(LoginService service = new LoginService())
            {
                var sessionInfo = service.ValidateSession(sessionId);
                if (sessionInfo == null)
                {
                    throw new SessionDoesNotExistException();
                }
                return new UserSessionInfo(sessionInfo.User, sessionInfo.Credentials.SessionToken);
            }
        }

        public UserSessionInfo RenewSession(UserSessionInfo session)
        {
            using (LoginService service = new LoginService())
            {
                return new UserSessionInfo(session.Principal, session.SessionToken);
            }
        }

        public void Logout(UserSessionInfo session)
        {
            using (LoginService service = new LoginService())
            {
                service.Logout(session.SessionToken.Id);
            }
        }
    }
}