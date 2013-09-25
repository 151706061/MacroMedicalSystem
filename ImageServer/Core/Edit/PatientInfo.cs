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
using Macro.Dicom.Iod;

namespace Macro.ImageServer.Core.Edit
{
	public class PatientInfo : IEquatable<PatientInfo>
	{
		public PatientInfo()
		{
		}

		public PatientInfo(PatientInfo other)
		{
			Name = other.Name;
			PatientId = other.PatientId;
			IssuerOfPatientId = other.IssuerOfPatientId;
		}

		public string Name { get; set; }

		public string PatientId { get; set; }

		public string IssuerOfPatientId { get; set; }

		#region IEquatable<PatientInfo> Members

		public bool Equals(PatientInfo other)
		{
			PersonName name = new PersonName(Name);
			PersonName otherName = new PersonName(other.Name);
			return name.Equals(otherName) && String.Equals(PatientId, other.PatientId, StringComparison.InvariantCultureIgnoreCase);
		}

		#endregion
	}
}