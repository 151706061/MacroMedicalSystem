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

using Macro.Common;
using Macro.Desktop.View.WinForms;
using Macro.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;

namespace Macro.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	[ExtensionOf(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuActionView : WinFormsActionView
	{
		private object _guiElement;
		
		public ListFilterMenuActionView()
		{}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
				{
					_guiElement = new ClickableToolStripControlHost(
						new ListFilterControl((ListFilterMenuAction)base.Context.Action));
				}

				return _guiElement;
			}
		}
	}
}