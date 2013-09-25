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
using Macro.Common.Serialization;
using Macro.Enterprise.Common;
using System.Runtime.Serialization;
using Macro.Common.Configuration;

namespace Macro.Enterprise.Common.Configuration
{
    [DataContract]
    public class ImportSettingsGroupRequest : DataContractBase
    {
        public ImportSettingsGroupRequest(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            this.Group = group;
            this.Properties = properties;
        }

        /// <summary>
        /// Settings group to update.  Required.
        /// </summary>
        [DataMember]
        public SettingsGroupDescriptor Group;

        /// <summary>
        /// Settings properties of the specified <see cref="Group"/>. Optional.  If null,
        /// the properties will not be updated.  If specified, all properties must be included,
        /// because any that are not will be deleted.
        /// </summary>
        [DataMember]
        public List<SettingsPropertyDescriptor> Properties;
    }
}
