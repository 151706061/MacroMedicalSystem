#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Desktop;

namespace Macro.ImageViewer.Web.View
{
    /// <summary>
    /// Silverlight implementation of <see cref="IWorkspaceView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateWorkspaceView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding <see cref="SetTitle"/> to customize the display of the workspace title.
    /// </para>
    /// </remarks>
    public class WorkspaceView : DesktopObjectView, IWorkspaceView
    {
        private readonly DesktopWindowView _desktopView;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="desktopView"></param>
        protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
        {
            _desktopView = desktopView;
        }


        #region DesktopObjectView overrides

        /// <summary>
        /// Sets the title of the workspace.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
        }

        /// <summary>
        /// Opens the workspace, adding the tab to the tab group.
        /// </summary>
        public override void Open()
        {
            _desktopView.AddWorkspaceView(this);
        }

        /// <summary>
        /// Activates the workspace, making the tab the selected tab.
        /// </summary>
        public override void Activate()
        {
        }

        public IWorkspaceDialogBoxView CreateDialogBoxView(WorkspaceDialogBox dialogBox)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Show()
        {
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Hide()
        {
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _desktopView.RemoveWorkspaceView(this);
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}
