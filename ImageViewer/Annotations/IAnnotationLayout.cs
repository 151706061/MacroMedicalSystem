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

namespace Macro.ImageViewer.Annotations
{
	/// <summary>
	/// Defines an entire layout of <see cref="AnnotationBox"/>es to be rendered to
	/// the overlay by an <see cref="Macro.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	public interface IAnnotationLayout
	{
		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		IEnumerable<AnnotationBox> AnnotationBoxes { get; }

		/// <summary>
		/// Gets or sets whether the <see cref="IAnnotationLayout"/> is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets a deep clone of the annotation layout.
		/// </summary>
		IAnnotationLayout Clone();
	}
}
