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
using Macro.Desktop.Tools;

namespace Macro.ImageViewer.Configuration
{

    [MenuAction("show", "global-menus/MenuTools/MenuSharedConfiguration", "Show")]
    [Tooltip("show", "MenuOptions")]
    [GroupHint("show", "Application.Configuration")]
    [ExtensionOf(typeof(Macro.Desktop.DesktopToolExtensionPoint))]
    public class SharedConfigurationTool : Tool<IDesktopToolContext>
    {
        public override IActionSet Actions
        {
            get
            {
                //If there's no pages, don't show the button.
                if (!SharedConfigurationDialog.CanShow)
                    return new ActionSet();

                return base.Actions;
            }
        }
        public void Show()
        {
            try
            {
                SharedConfigurationDialog.Show(this.Context.DesktopWindow);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }
    }
}
