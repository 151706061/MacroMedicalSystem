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
using System.Runtime.Serialization;
using System.Collections.Generic;
using Macro.Common.Serialization;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace Macro.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class UserDetail : DataContractBase
    {
        public UserDetail(string userId, string displayName, string emailAddress, DateTime creationTime, DateTime? validFrom, DateTime? validUntil, 
            DateTime? lastLoginTime, bool enabled, DateTime? expiryTime, List<AuthorityGroupSummary> authorityGroups)
        {
            UserName = userId;
            DisplayName = displayName;
            AuthorityGroups = authorityGroups;
            CreationTime = creationTime;
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            LastLoginTime = lastLoginTime;
            Enabled = enabled;
            PasswordExpiryTime = expiryTime;
            EmailAddress = emailAddress;
        }

        public UserDetail()
        {
            AuthorityGroups = new List<AuthorityGroupSummary>();
        }

        [DataMember]
        public string UserName;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public DateTime CreationTime;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;

        [DataMember]
        public DateTime? LastLoginTime;

        [DataMember]
        public bool Enabled;

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;

        /// <summary>
        /// Used by client to request password reset.
        /// </summary>
        [DataMember]
        public bool ResetPassword;

        [DataMember]
        public DateTime? PasswordExpiryTime;

        [DataMember]
        public string EmailAddress;
    }
}
