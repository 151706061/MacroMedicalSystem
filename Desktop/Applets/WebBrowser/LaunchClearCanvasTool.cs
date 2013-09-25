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

using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Tools;
using Macro.Desktop.Actions;


namespace Macro.Desktop.Applets.WebBrowser
{
	[ButtonAction("activate1", "webbrowser-toolbar/Macro", "LaunchMacro")]
	[Tooltip("activate1", "Launch Macro")]
	[IconSet("activate1", IconScheme.Colour, "Icons.MacroToolSmall.png", "Icons.MacroToolSmall.png", "Icons.MacroToolSmall.png")]

	[ButtonAction("activate2", "webbrowser-toolbar/Discussion Forum", "LaunchDiscussionForum")]
	[Tooltip("activate2", "Launch Macro Discussion Forum")]
	[IconSet("activate2", IconScheme.Colour, "Icons.MacroToolSmall.png", "Icons.MacroToolSmall.png", "Icons.MacroToolSmall.png")]

	[ExtensionOf(typeof(WebBrowserToolExtensionPoint))]
	public class LaunchMacroTool : Tool<IWebBrowserToolContext>
	{
		public LaunchMacroTool()
		{

		}

		private void LaunchMacro()
		{
			this.Context.Url = "http://www.ClearCanvas.ca";
			this.Context.Go();
		}

		private void LaunchDiscussionForum()
		{
			this.Context.Url = "http://www.ClearCanvas.ca/dnn/Community/Forums/tabid/69/Default.aspx";
			this.Context.Go();
		}
	}
}
