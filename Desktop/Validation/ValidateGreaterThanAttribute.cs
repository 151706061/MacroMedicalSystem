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

namespace Macro.Desktop.Validation
{
	/// <summary>
	/// Validates that a property value is greater than a reference value.
	/// </summary>
	public class ValidateGreaterThanAttribute : ValidateCompareAttribute
	{
		/// <summary>
		/// Constructor that accepts the name of a reference property.
		/// </summary>
		/// <param name="referenceProperty">The name of a property on the component that provides a reference value.</param>
		public ValidateGreaterThanAttribute(string referenceProperty)
			: base(referenceProperty)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateGreaterThanAttribute(int referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateGreaterThanAttribute(float referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateGreaterThanAttribute(double referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Returns 1.
		/// </summary>
		protected override int GetCompareSign()
		{
			return 1;
		}
	}
}
