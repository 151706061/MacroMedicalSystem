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
using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Actions;

namespace Macro.Utilities.DicomEditor.Tools
{
	//[MenuAction("activate", "global-menus/MenuTools/MenuToolsMyTools/SaveTool")]
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarSave", "Save")]
	[Tooltip("activate", "TooltipSave")]
	[IconSet("activate", "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png", "Icons.SaveToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class SaveTool : DicomEditorTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public SaveTool() : base(true) {}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Save()
		{
			var message = this.Context.DumpManagement.LoadedFileCount > 1 
				? SR.MessageConfirmSaveAllFiles
				: this.Context.IsLocalFile
					? SR.MessageConfirmSaveSingleLocalFile
					: SR.MessageConfirmSaveSingleRemoteFile;

			if (this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				this.Context.DumpManagement.SaveAll();
				this.Context.UpdateDisplay();
			}
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}