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
using System.Text;
using Macro.Common.Utilities;

namespace Macro.Enterprise.Common
{
    /// <summary>
    /// When applied to a service contract interface, specifies whether that service requires
    /// authentication or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AuthenticationAttribute : Attribute
    {
        /// <summary>
        /// Tests a service contract to see if it requires authentication.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        public static bool IsAuthenticationRequired(Type serviceContract)
        {
            AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceContract);
            return authAttr == null ? true : authAttr.AuthenticationRequired;
        }

        private readonly bool _required;

        public AuthenticationAttribute(bool required)
        {
            _required = required;
        }

        /// <summary>
        /// Gets a value indicating whether authentication is required.
        /// </summary>
        public bool AuthenticationRequired
        {
            get { return _required; }
        }
    }
}