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

namespace Macro.ImageViewer.Imaging
{
	/// <summary>
	/// A read-only Linear Voi Lut, where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> cannot be set.
	/// </summary>
	/// <seealso cref="IVoiLut"/>
	public interface IVoiLutLinear : IVoiLut
	{
		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		double WindowCenter { get; }
	}
}
