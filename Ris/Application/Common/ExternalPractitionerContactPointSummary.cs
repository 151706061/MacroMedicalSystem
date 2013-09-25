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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerContactPointSummary : DataContractBase, ICloneable
	{
		public ExternalPractitionerContactPointSummary(
			EntityRef contactPointRef, 
			string name, 
			string description, 
			bool isDefaultContactPoint,
			bool isMerged,
			bool deactivated)
		{
			this.ContactPointRef = contactPointRef;
			this.Name = name;
			this.Description = description;
			this.IsDefaultContactPoint = isDefaultContactPoint;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerContactPointSummary()
		{
		}

		[DataMember]
		public EntityRef ContactPointRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public bool IsDefaultContactPoint;

		[DataMember]
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerContactPointSummary(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				this.IsMerged,
				this.Deactivated);
		}

		#endregion
	}
}
