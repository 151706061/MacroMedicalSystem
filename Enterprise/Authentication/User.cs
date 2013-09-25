#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using ClearCanvas.Enterprise.Common;
using Iesi.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Enterprise.Authentication {


    /// <summary>
    /// User entity
    /// </summary>
	public partial class User
	{
        #region Public methods

        /// <summary>
        /// Creates a new user with the specified initial password.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="initialPassword"></param>
        /// <param name="authorityGroups"></param>
        /// <returns></returns>
        public static User CreateNewUser(UserInfo userInfo, Password initialPassword, Iesi.Collections.Generic.ISet<AuthorityGroup> authorityGroups)
        {
			Platform.CheckForNullReference(userInfo, "userInfo");
			Platform.CheckForNullReference(initialPassword, "initialPassword");
			Platform.CheckForEmptyString(userInfo.UserName, "UserName");

            return new User(
                userInfo.UserName,
                initialPassword,
                userInfo.DisplayName,
                userInfo.ValidFrom,
                userInfo.ValidUntil,
                true, // initially enabled
                Platform.Time, // creation time
                null, // last login time
                userInfo.EmailAddress,
                authorityGroups,
                new HashedSet<UserSession>()  // empty session collection
                );
        }

        /// <summary>
		/// Creates a new user with the specified temporary password.
		/// </summary>
        /// <param name="userInfo"></param>
		/// <param name="temporaryPassword"></param>
		/// <returns></returns>
		public static User CreateNewUser(UserInfo userInfo, string temporaryPassword)
        {
			return CreateNewUser(userInfo, Authentication.Password.CreateTemporaryPassword(temporaryPassword), new HashedSet<AuthorityGroup>());
        }

        /// <summary>
        /// Changes the user's password, setting a new expiry date.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="expiryTime"></param>
        public virtual void ChangePassword(string newPassword, DateTime expiryTime)
        {
            _password = Authentication.Password.CreatePassword(newPassword, expiryTime);
        }

        /// <summary>
        /// Resets the user's password to the specified temporary password,
        /// set to expire immediately.
        /// </summary>
        public virtual void ResetPassword(string temporaryPassword)
        {
            _password = Authentication.Password.CreateTemporaryPassword(temporaryPassword);
        }

        /// <summary>
        /// Gets a value indicating whether this account is currently active.
        /// </summary>
        public virtual bool IsActive(DateTime currentTime)
        {
            return _enabled
                   && (_validFrom == null || _validFrom < currentTime)
                   && (_validUntil == null || _validUntil > currentTime);
        }

		/// <summary>
		/// Obtains the set of sessions that are currently active (not expired).
		/// </summary>
    	public IEnumerable<UserSession> ActiveSessions
    	{
			get { return _sessions.Where(s => !s.IsExpired).ToList(); }
    	}

		/// <summary>
		/// Initiates a new session for this user, updating the <see cref="Sessions"/> collection and returning
		/// the new session.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="hostName"></param>
		/// <param name="password"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public virtual UserSession InitiateSession(string application, string hostName, string password, TimeSpan timeout)
		{
			Platform.CheckForEmptyString(application, "application");
			Platform.CheckForEmptyString(hostName, "hostName");
			Platform.CheckForNullReference(password, "password");
			Platform.CheckPositive(timeout.TotalMilliseconds, "timeout");

			DateTime startTime = Platform.Time;

			// check account is active and password correct
			if (!IsActive(startTime) || !_password.Verify(password))
			{
				// account not active, or invalid password
				// the error message is deliberately vague
				throw new UserAccessDeniedException();
			}

			// check if password expired
			if (_password.IsExpired(startTime))
				throw new PasswordExpiredException();

			// create new session
			UserSession session = new UserSession(
				this,
				hostName,
				application,
				Guid.NewGuid().ToString("N"),
				startTime,
				startTime + timeout);

			_sessions.Add(session);

			// update last login time
			_lastLoginTime = startTime;

			return session;
		}

		/// <summary>
		/// Terminates any expired sessions and returns a list of them.
		/// </summary>
		/// <returns></returns>
		public virtual List<UserSession> TerminateExpiredSessions()
		{
			// find any expired sessions
			List<UserSession> expiredSessions = CollectionUtils.Select(_sessions,
				delegate(UserSession session)
				{
					return session.IsExpired;
				});

			// terminate them
			foreach (UserSession session in expiredSessions)
			{
				session.Terminate();
			}

			// return the list
			return expiredSessions;
		}

        #endregion

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}