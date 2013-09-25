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

using System.Diagnostics;
using System.IO;
using Macro.Common;
using Macro.Desktop.Tools;
using Macro.Desktop.Actions;
using System;

namespace Macro.Desktop.Help
{
	[MenuAction("showAbout", "global-menus/MenuHelp/MenuAbout", "ShowAbout")]
	[GroupHint("showAbout", "Application.Help.About")]

    //[MenuAction("showWebsite", "global-menus/MenuHelp/MenuWebsite", "ShowWebsite")]
    //[GroupHint("showWebsite", "Application.Help.Website")]

    //[MenuAction("showUsersGuide", "global-menus/MenuHelp/MenuUsersGuide", "ShowUsersGuide")]
    //[GroupHint("showUsersGuide", "Application.Help.UsersGuide")]

    //[MenuAction("showLicense", "global-menus/MenuHelp/MenuLicense", "ShowLicense")]
    //[GroupHint("showLicense", "Application.Help.License")]

	[MenuAction("showLogs", "global-menus/MenuHelp/MenuShowLogs", "ShowLogs")]
	[GroupHint("showLogs", "Application.Help.Support")]
	[ActionPermission("showLogs", AuthorityTokens.Desktop.ShowLogs)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HelpTool : Tool<IDesktopToolContext>
	{
		public HelpTool()
		{
		}

        public void ShowAbout()
		{
			var aboutForm = AboutDialogExtensionPoint.CreateDialog();
			aboutForm.ShowDialog();
		}

		public void ShowWebsite()
		{
			Execute("http://www.ClearCanvas.ca", SR.URLNotFound);
		}

		public void ShowUsersGuide()
		{
			Execute(HelpSettings.Default.UserGuidePath, SR.UsersGuideNotFound);
		}

		public void ShowLicense()
		{
			Execute("EULA.rtf", SR.LicenseNotFound);
		}

		public void ShowLogs()
		{
			string logdir = Platform.LogDirectory;
			if (!string.IsNullOrEmpty(logdir) && Directory.Exists(logdir))
				Process.Start(logdir);
		}

		private void Execute(string filename, string errorMessage)
		{
			bool showMessageBox = String.IsNullOrEmpty(filename);
			if (!showMessageBox)
			{
				try
				{
					ProcessStartInfo info = new ProcessStartInfo();
					info.WorkingDirectory = Platform.InstallDirectory;
					info.FileName = filename;

					Process.Start(info);
				}
				catch (Exception e)
				{
					showMessageBox = true;
					Platform.Log(LogLevel.Warn, e, "Failed to launch '{0}'.", filename);
				}
			}

			if (showMessageBox)
				this.Context.DesktopWindow.ShowMessageBox(errorMessage, MessageBoxActions.Ok);
		}
	}
}
