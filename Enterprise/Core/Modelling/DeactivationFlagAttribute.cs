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

namespace Macro.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to an entity class, specifies the name of a boolean property on the class
	/// that acts as a flag to indicate that the entity instance is "de-activated".
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DeactivationFlagAttribute : Attribute
	{
		private readonly string _propertyName;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		public DeactivationFlagAttribute(string propertyName)
		{
			_propertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of a property on the class that acts as a de-activation flag.
		/// </summary>
		public string PropertyName
		{
			get { return _propertyName; }
		}
	}
}
