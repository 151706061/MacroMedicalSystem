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
using System.Linq;
using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Actions;
using Macro.Dicom.Iod;
using Macro.ImageViewer.Explorer.Dicom;
using Macro.ImageViewer.StudyManagement;
using Macro.ImageViewer.StudyManagement.Core;
using Macro.ImageViewer.StudyManagement.Core.Storage;

namespace Macro.ImageViewer.Utilities.Media
{
    [MenuAction("Open", "dicomstudybrowser-contextmenu/MenuWriteMedia", "Open")]
    [EnabledStateObserver("Open", "Enabled", "EnabledChanged")]
    [Tooltip("Open", "TooltipWriteMedia")]
    [ButtonAction("Open", "dicomstudybrowser-toolbar/ToolbarWriteMedia", "Open")]
    [IconSet("Open", "Icon.MediaWriterToolSmall.png", "Icon.MediaWriterToolMedium.png", "Icon.MediaWriterToolLarge.png")]
    [VisibleStateObserver("Open", "Visible", "VisibleChanged")]

    [ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    internal sealed class LaunchMediaWriterDicomExplorerTool : StudyBrowserTool
    {
        private IShelf mediaIShelf;
        private MediaWriterComponent component;

        private void ShelfClose(object sender, EventArgs e)
        {
            this.mediaIShelf.Closed -= this.ShelfClose;
            this.mediaIShelf = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            Visible = GetAtLeastOneServerSupportsLoading();
            Enabled = Context.SelectedStudies.Count > 0 && GetAtLeastOneServerSupportsLoading();
        }

        private bool GetAtLeastOneServerSupportsLoading()
        {
            return Context.SelectedServers.AnySupport<IStudyLoader>();
        }

        private int GetNumberOfLoadableStudies()
        {
            return base.Context.SelectedStudies.Count(s => s.Server.IsSupported<IStudyLoader>());
        }

        private IEnumerable<IPatientData> GetSelectedPatients()
        {
            return Context.SelectedStudies.Cast<IPatientData>();
        }

        public void Open()
        {
            try
            {
                if (!Enabled)
                {
                    return;
                }

                int numberOfSelectedStudies = Context.SelectedStudies.Count;
                if (Context.SelectedStudies.Count == 0)
                    return;

                int numberOfLoadableStudies = GetNumberOfLoadableStudies();
                if (numberOfLoadableStudies != numberOfSelectedStudies)
                {
                    int numberOfNonLoadableStudies = numberOfSelectedStudies - numberOfLoadableStudies;
                    string message;
                    if (numberOfSelectedStudies == 1)
                    {
                        message = SR.MessageCannotOpenNonStreamingStudy;
                    }
                    else
                    {
                        if (numberOfNonLoadableStudies == 1)
                            message = SR.MessageOneNonStreamingStudyCannotBeOpened;
                        else
                            message = String.Format(SR.MessageFormatXNonStreamingStudiesCannotBeOpened, numberOfNonLoadableStudies);
                    }

                    Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
                    return;
                }

                UIStudyTree tree = new UIStudyTree();

                foreach (var item in Context.SelectedStudies)
                {
                    if (item.Server.IsLocal)
                    {
                        using (var context = new DataAccessContext())
                        {
                            IStudy study = context.GetStudyBroker().GetStudy(item.StudyInstanceUid);
                            if (study != null)
                            {
                                tree.AddStudy(study);
                            }
                        }
                    }
                }

                if (this.mediaIShelf != null)
                {
                    this.mediaIShelf.Activate();
                    component.Tree = tree.Tree;
                }
                else
                {
                    if (base.Context.DesktopWindow == null)
                    {
                        return;
                    }

                    component = new MediaWriterComponent();
                    this.mediaIShelf = ApplicationComponent.LaunchAsShelf(base.Context.DesktopWindow, component, "Media Writer", ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft);
                    this.mediaIShelf.Closed += new EventHandler<ClosedEventArgs>(this.ShelfClose);
                    this.mediaIShelf.Activate();
                    component.Tree = tree.Tree;
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Report(exception, base.Context.DesktopWindow);
            }
        }
    }
}

