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
using System.Threading;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Desktop;
using Macro.Desktop.Tools;
using Macro.Desktop.Actions;

namespace Macro.ImageViewer.TestTools
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/Memory Analysis", "Show", KeyStroke = XKeys.Control | XKeys.M)]
	[ButtonAction("show", "global-toolbars/ToolbarUtilities/Memory Analysis", "Show")]
	[IconSet("show", "Icons.MemoryAnalysisToolSmall.png", "Icons.MemoryAnalysisToolMedium.png", "")]

	[ExtensionOf(typeof(Macro.Desktop.DesktopToolExtensionPoint))]
	public class MemoryAnalysisTool : Tool<Macro.Desktop.IDesktopToolContext>
	{
		private static IShelf _shelf;

		public MemoryAnalysisTool()
		{
		}

		public void Show()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
			{
				MemoryAnalysisComponent component = new MemoryAnalysisComponent(this.Context.DesktopWindow);
				_shelf = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, component, "Memory Analysis",
					                                   ShelfDisplayHint.DockFloat);
				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
