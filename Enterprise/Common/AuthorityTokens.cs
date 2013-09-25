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

using Macro.Common.Authorization;

namespace Macro.Enterprise.Common
{
    public class AuthorityTokens
    {
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Admin
		{
			public static class System
			{
				[AuthorityToken(Description = "Allow modification of enterprise configuration store data.")]
				public const string Configuration = "Enterprise/Admin/System/Configuration";
			}

			public static class Security
			{
				[AuthorityToken(Description = "Allow administration of User Accounts.")]
                public const string User = "Enterprise/Admin/Security/User";

				[AuthorityToken(Description = "Allow administration of Authority Groups.")]
                public const string AuthorityGroup = "Enterprise/Admin/Security/Authority Group";
			}
		}

        /// <summary>
        /// Tokens that specify data access
        /// </summary>
        /// <remarks>
        /// The tokens are intended to be system wide and used across products.
        /// </remarks>
        public static class DataAccess
        {
            [AuthorityToken(Description = "Allow the user access to all studies.", Formerly = "Web Portal/Data Access/Access to all Studies")]
            public const string AllStudies = "Data Access/Access to all Studies";

            [AuthorityToken(Description = "Allow the user access to all Server Partitions on the ImageServer.")]
            public const string AllPartitions = "Data Access/Access to all Server Partitions";
        }
	}
}
