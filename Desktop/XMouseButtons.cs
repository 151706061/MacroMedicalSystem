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
using System.ComponentModel;

namespace Macro.Desktop
{
	/// <summary>
	/// Enumeration of the (potentially) available mouse buttons.
	/// </summary>
	[Flags]
	[TypeConverter(typeof(XMouseButtonsConverter))]
	public enum XMouseButtons
	{
		/// <summary>
		/// Represents no mouse buttons (the empty value).
		/// </summary>
		None = 0x00000000,

		/// <summary>
		/// The left mouse button (mouse button 1).
		/// </summary>
		Left = 0x00100000,

		/// <summary>
		/// The right mouse button (mouse button 2).
		/// </summary>
		Right = 0x00200000,

		/// <summary>
		/// The middle mouse button (mouse button 3).
		/// </summary>
		Middle = 0x00400000,

		/// <summary>
		/// The first X mouse button (mouse button 4).
		/// </summary>
		XButton1 = 0x00800000,

		/// <summary>
		/// The second X mouse button (mouse button 5).
		/// </summary>
		XButton2 = 0x01000000
	}
}