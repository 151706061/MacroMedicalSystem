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
using Macro.Desktop;
using Macro.ImageViewer.InputManagement;

namespace Macro.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's modified <see cref="MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="IViewerShortcutManager"/>
	/// <seealso cref="DefaultMouseToolButtonAttribute"/>
	[Obsolete("Additional mouse tool button assignments no longer need to be modified.  Use DefaultMouseToolButtonAttribute instead.")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ModifiedMouseToolButtonAttribute : DefaultMouseToolButtonAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ModifiedMouseToolButtonAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
			: base(mouseButton, modifierFlags)
		{
		}
	}
}