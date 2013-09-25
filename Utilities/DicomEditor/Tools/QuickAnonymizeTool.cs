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
	[ButtonAction("activate", "dicomeditor-contextmenu/ToolbarQuickAnonymize", "Apply")]
	[MenuAction("activate", "dicomeditor-toolbar/MenuQuickAnonymize", "Apply")]
	[Tooltip("activate", "TooltipQuickAnonymize")]
	[IconSet("activate", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png", "Icons.AnonymizeToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class QuickAnonymizeTool : DicomEditorTool
	{
		private bool _promptForAll;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public QuickAnonymizeTool() : base(true) {}

		public void Apply()
		{
			bool applyToAll = false;

			if (_promptForAll)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmAnonymizeAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
					applyToAll = true;
			}

			this.Anonymize(applyToAll);
			this.Context.UpdateDisplay();
		}

		private void Anonymize(bool applyToAll)
		{
			IDicomEditorDumpManagement dump = this.Context.DumpManagement;

			dump.Anonymize(applyToAll);
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			_promptForAll = !e.IsCurrentTheOnly;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}