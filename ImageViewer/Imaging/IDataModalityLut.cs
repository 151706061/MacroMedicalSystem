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
	/// Defines a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values implemented as an array of values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <see cref="IModalityLut"/>
	public interface IDataModalityLut : IModalityLut
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the lookup table data.
		/// </summary>
		double[] Data { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IDataModalityLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.
		/// </remarks>
		new IDataModalityLut Clone();
	}
}