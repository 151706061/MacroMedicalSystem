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

using System.ComponentModel;

namespace Macro.ImageViewer.Graphics
{
	/// <summary>
	/// Represents the method that will handle the <see cref="IGraphic.VisualStateChanged"/> event raised
	/// when a property is changed on a graphic, resulting in a change in the graphic's visual state.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="VisualStateChangedEventArgs"/> that contains the event data. </param>
	public delegate void VisualStateChangedEventHandler(object sender, VisualStateChangedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="IGraphic.VisualStateChanged"/> event. 
	/// </summary>
	public sealed class VisualStateChangedEventArgs : PropertyChangedEventArgs
	{
		/// <summary>
		/// Gets the graphic whose visual state changed.
		/// </summary>
		public readonly IGraphic Graphic;

		/// <summary>
		/// Initializes a new instance of the <see cref="VisualStateChangedEventArgs"/> class. 
		/// </summary>
		/// <param name="graphic">The graphic whose visual state changed.</param>
		/// <param name="propertyName">The name of the property that changed. </param>
		public VisualStateChangedEventArgs(IGraphic graphic, string propertyName)
			: base(propertyName)
		{
			this.Graphic = graphic;
		}
	}
}