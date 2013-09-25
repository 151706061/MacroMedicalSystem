#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
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
using System.IdentityModel.Selectors;
using System.Text;
using Macro.Common;
using Macro.Enterprise.Common;
using Macro.Enterprise.Common.Authentication;

namespace Macro.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implemenation of <see cref="UserNamePasswordValidator"/> that authenticates
    /// a user via the <see cref="IAuthenticationService"/>.
    /// </summary>
    class DefaultUserValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            Platform.Log(LogLevel.Debug, "Validating session for user ", userName);

			// Note: password is actually the session token
            //AuthenticationClient authClient = new AuthenticationClient();
            //authClient.ValidateSession(new ValidateSessionRequest(userName, new SessionToken(password)));

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // this call will throw an exception if the session is invalid or has expired
                    service.ValidateSession(new ValidateSessionRequest(userName, new SessionToken(password)));
                });
		}
	}
}