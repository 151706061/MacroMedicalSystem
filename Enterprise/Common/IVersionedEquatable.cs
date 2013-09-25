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

namespace Macro.Enterprise.Common
{
	public interface IVersionedEquatable
	{
		bool Equals(object other, bool ignoreVersion);
	}


	/// <summary>
	/// Extends the <see cref="IEquatable{T}"/> interface with an overload of equals that supports
	/// version insensitive equatability.
	/// </summary>
	/// <remarks>
	/// The default implementation of <see cref="IEquatable{T}.Equals(T)"/> should be implemented
	/// such that it is version-sensitive.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public interface IVersionedEquatable<T> : IEquatable<T>, IVersionedEquatable
	{
		/// <summary>
		/// Determine whether two objects are equal, optionally ignoring the version.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="ignoreVersion"></param>
		/// <returns></returns>
		bool Equals(T other, bool ignoreVersion);
	}
}
