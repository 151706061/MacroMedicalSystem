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

using System.Collections.Generic;
using System.Runtime.Serialization;
using Macro.Common.Serialization;


namespace Macro.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	[DataContract]
	public class AuthorityTokenSummary : DataContractBase
	{
		public AuthorityTokenSummary(string name)
		{
			Name = name;
			FormerIdentities = new List<string>();
		}

		public AuthorityTokenSummary(string name, string description)
		{
			Name = name;
			Description = description;
			FormerIdentities = new List<string>();
		}

		public AuthorityTokenSummary(string name, string definingAssembly, string description, IEnumerable<string> formerIdentities)
		{
			Name = name;
			DefiningAssembly = definingAssembly;
			Description = description;
			FormerIdentities = new List<string>(formerIdentities);
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string DefiningAssembly;

		[DataMember]
		public string Description;

		[DataMember]
		public List<string> FormerIdentities;
	}
}
