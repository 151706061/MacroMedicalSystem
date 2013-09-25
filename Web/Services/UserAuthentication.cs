#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;
using Macro.Enterprise.Common;
using System.Security.Principal;

namespace Macro.Web.Services
{
	public class UserSessionInfo
	{
		public UserSessionInfo(IPrincipal principal, SessionToken sessionToken)
		{
			Principal = principal;
			SessionToken = sessionToken;
		}

		public IPrincipal Principal { get; private set; }
		public SessionToken SessionToken { get; set; }
	}

    public class SessionExpiredException : Exception
    {

    }

    public class SessionDoesNotExistException : Exception
    {

    }
	public interface IUserAuthentication
	{
        /// <summary>
        /// Query session information of the given session id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        /// <remarks>
        /// The implementation of this interface should not renew the session. The returned <see cref="UserSessionInfo"/> may or may not be valid. 
        /// </remarks>
		UserSessionInfo QuerySession(string sessionId);

        /// <summary>
        /// Renew a specified user session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns>The new <see cref="SessionToken"/></returns>
        /// <remarks>
        /// Calling application should discard the <paramref name="session"/>. It is no longer valid when this method returns.
        /// </remarks>
        UserSessionInfo RenewSession(UserSessionInfo session);

        /// <summary>
        /// Log out 
        /// </summary>
        /// <param name="session"></param>
        void Logout(UserSessionInfo session);
	}

	public class UserAuthenticationExtensionPoint : ExtensionPoint<IUserAuthentication>
	{
	}
	
	static class UserAuthentication
    {
		private static readonly IUserAuthentication _instance;
		private static readonly bool _logStuff;

		static UserAuthentication()
		{
			_instance = (IUserAuthentication)new UserAuthenticationExtensionPoint().CreateExtension();
			_logStuff = true;
		}

        /// <summary>
        /// Verify the session is still valid without renewing it.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
		static public UserSessionInfo ValidateSession(string username, string sessionId)
		{
			if (_logStuff)
			{
				string message = String.Format("Validating user session (username={0}, session id={1}).", username, sessionId);
				Platform.Log(LogLevel.Debug, message);
			}

			var sessionInfo = _instance.QuerySession(sessionId);

            if (sessionId==null)
            {
                throw new SessionDoesNotExistException();
            }

            if (sessionInfo.SessionToken.ExpiryTime < Platform.Time)
            {
                throw new SessionExpiredException();
            }
		    return sessionInfo;
		}

        /// <summary>
        /// Renew the specified <see cref="UserSessionInfo"/>
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        /// <remarks>
        /// <paramref name="session"/> is deemed invalid once this method returns and should be discarded.
        /// </remarks>
        static public UserSessionInfo RenewSession(UserSessionInfo session)
		{
			if (_logStuff)
			{
				string message = String.Format("Renewing user session (username={0}, session={1}).", session.Principal.Identity.Name, session.SessionToken.Id);
				//Console.WriteLine(message);
				Platform.Log(LogLevel.Debug, message);
			}

			var newSession = _instance.RenewSession(session);

            // double check
            if (newSession == null || newSession.SessionToken == null || newSession.SessionToken.ExpiryTime < Platform.Time)
            {
                throw new Exception("Unexpected Error: Session was renewed but invalid");
            }
            return newSession;
		}

        static public void Logout(UserSessionInfo session)
		{
            string username = session.Principal.Identity.Name;
			if (_logStuff)
			{
                string message = String.Format("Attempting to log out (username={0}, session={1}).", username, session.SessionToken.Id);
                Platform.Log(LogLevel.Debug, message);
			}

			try
			{
				_instance.Logout(session);
				if (_logStuff)
                    Platform.Log(LogLevel.Info, "Successfully logged out (username={0}, session={1}).", username, session.SessionToken.Id);
			}
			catch (Exception e)
			{
                Platform.Log(LogLevel.Warn, e, "Failed to log out (user={0}, session={1}).", username, session.SessionToken.Id);
			}
		}
	}
}