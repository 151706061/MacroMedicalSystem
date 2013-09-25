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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion


using System;
using System.Drawing;
using Crownwood.DotNetMagic.Controls;

namespace Macro.Desktop.View.WinForms
{
	/// <summary>
	/// Helper class for initialization of controls to the Macro visual style, now deprecated in favour of extensible application GUI themes.
	/// </summary>
	[Obsolete("The use of this class has been deprecated in favour of extensible application GUI themes")]
	public static class MacroStyle
	{
		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color MacroDarkBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorDark; }
		}

		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color MacroBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorBase; }
		}

		/// <summary>
		/// Deprecated. Use <see cref="Application.CurrentUITheme"/> to retrieve the current colour scheme instead.
		/// </summary>
		[Obsolete("The use of the hard coded color scheme has been deprecated in favour of extensible application GUI themes")]
		public static Color MacroLightBlue
		{
			get { return Application.CurrentUITheme.Colors.StandardColorLight; }
		}

		/// <summary>
		/// Deprecated. Use the <see cref="TitleBar"/> control instead, which will automatically take into account the value of <see cref="Application.CurrentUITheme"/>.
		/// </summary>
		[Obsolete("The use of the toolkit title bar control has been deprecated in favour of using Macro.Desktop.View.WinForms.TitleBar")]
		public static void SetTitleBarStyle(Crownwood.DotNetMagic.Controls.TitleBar titleBar)
		{
			titleBar.BackColor = MacroDarkBlue;
			titleBar.ForeColor = Color.White;
			titleBar.GradientActiveColor = MacroDarkBlue;
			titleBar.GradientColoring = GradientColoring.LightBackToGradientColor;
			titleBar.GradientDirection = GradientDirection.TopToBottom;
		}
	}
}