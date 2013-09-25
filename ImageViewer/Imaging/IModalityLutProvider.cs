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
	/// A provider of a modality lut.
	/// </summary>
	/// <remarks>
	/// Implementors should not return null from the <see cref="ModalityLut"/> property.
	/// </remarks>
	public interface IModalityLutProvider
	{
		/// <summary>
		/// Gets the modality lut owned by the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IModalityLut ModalityLut { get; }
	}
}
