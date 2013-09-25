#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Actions;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.ImageViewer.Web.Common.Events;
using Macro.ImageViewer.Web.EntityHandlers;
using Macro.Web.Services;

namespace Macro.ImageViewer.Web.View
{
    /// <summary>
    /// WinForms implementation of <see cref="IDesktopWindowView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="ApplicationView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="ApplicationView.CreateDesktopWindowView"/> method.
    /// </para>
    /// </remarks>
    public class DesktopWindowView : DesktopObjectView, IDesktopWindowView
    {
        private DialogBoxAction _result;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window"></param>
        protected internal DesktopWindowView(DesktopWindow window)
        {
        }

        #region IDesktopWindowView Members

        /// <summary>
        /// Creates a new view for the specified <see cref="Workspace"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IWorkspaceView"/>.
        /// In practice, it is preferable to subclass <see cref="WorkspaceView"/> rather than implement <see cref="IWorkspaceView"/>
        /// directly.
        /// </remarks>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public virtual IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return new WorkspaceView(workspace, this);
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="Shelf"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IShelfView"/>.
        /// </remarks>
        /// <param name="shelf"></param>
        /// <returns></returns>
        public virtual IShelfView CreateShelfView(Shelf shelf)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new view for the specified <see cref="DialogBox"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDialogBoxView"/>.
        /// </remarks>
        /// <param name="dialogBox"></param>
        /// <returns></returns>
        public virtual IDialogBoxView CreateDialogBoxView(DialogBox dialogBox)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the menu model, causing the menu displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetMenuModel(ActionModelNode model)
        {
        }

        /// <summary>
        /// Sets the toolbar model, causing the toolbar displayed on the screen to be updated.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        public virtual void SetToolbarModel(ActionModelNode model)
        {
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the display of message boxes.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
        {
            if (ApplicationContext.Current != null)
            {

                MessageBoxEntityHandler handler = EntityHandler.Create<MessageBoxEntityHandler>();
                handler.SetModelObject(this);

                MessageBox box = handler.GetEntity();
                box.Message = message;
                box.Title = title;
                switch (buttons)
                {
                    case MessageBoxActions.Ok:
                        box.Actions = WebMessageBoxActions.Ok;
                        _result = DialogBoxAction.Ok;
                        break;
                    case MessageBoxActions.OkCancel:
                        box.Actions = WebMessageBoxActions.OkCancel;
                        _result = DialogBoxAction.Ok;
                        break;
                    case MessageBoxActions.YesNo:
                        box.Actions = WebMessageBoxActions.YesNo;
                        _result = DialogBoxAction.Yes;
                        break;
                    case MessageBoxActions.YesNoCancel:
                        box.Actions = WebMessageBoxActions.YesNoCancel;
                        _result = DialogBoxAction.Yes;
                        break;
                }

                MessageBoxShownEvent @event = new MessageBoxShownEvent
                                                  {
                                                      Identifier = box.Identifier,
                                                      MessageBox = box,
                                                      SenderId = ApplicationContext.Current.ApplicationId,
                                                  };

                ApplicationContext.Current.FireEvent(@event);

                IWebSynchronizationContext context = (SynchronizationContext.Current as IWebSynchronizationContext);
                if (context != null) context.RunModal();
                return _result;
            }

            return DialogBoxAction.Ok;
        }

        public void SetAlertContext(IDesktopAlertContext alertContext)
        {
            // TODO (Marmot) - Need to implement for Webstation
            //throw new NotImplementedException();
        }

        public void ShowAlert(AlertNotificationArgs args)
        {
            // TODO (Marmot) - Need to implement for Webstation

            LogLevel level;
            if (args.Level == AlertLevel.Info)
                level = LogLevel.Info;
            else if (args.Level == AlertLevel.Warning)
                level = LogLevel.Warn;
            else
            {
                level = LogLevel.Error;
            }
            Platform.Log(level, args.Message);
        }

        /// <summary>
        /// Called to dismiss the dialog
        /// </summary>
        /// <param name="action"></param>
        public void Dismiss(DialogBoxAction action)
        {
            _result = action;

            IWebSynchronizationContext context = (SynchronizationContext.Current as IWebSynchronizationContext);
            if (context != null) context.BreakModal();
        }

        /// <summary>
    	/// Shows a 'Save file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowSaveFileDialogBox(FileDialogCreationArgs args)
    	{
            throw new NotSupportedException();
    	}

    	/// <summary>
    	/// Shows a 'Open file' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public virtual FileDialogResult ShowOpenFileDialogBox(FileDialogCreationArgs args)
    	{
            throw new NotSupportedException();
		}

    	/// <summary>
    	/// Shows a 'Select folder' dialog in front of this window.
    	/// </summary>
    	/// <param name="args"></param>
    	/// <returns></returns>
    	public FileDialogResult ShowSelectFolderDialogBox(SelectFolderDialogCreationArgs args)
    	{
            throw new NotSupportedException();
    	}

    	#endregion

        #region DesktopObjectView overrides

        public override void SetTitle(string title)
        {
        }

        /// <summary>
        /// Opens this view, showing the form on the screen.
        /// </summary>
        public override void Open()
        {
        }

        public override void Hide()
        {
        }

        /// <summary>
        /// Activates the view, activating the form on the screen.
        /// </summary>
        public override void Activate()
        {
        }

        /// <summary>
        /// Shows the view, making the form visible on the screen.
        /// </summary>
        public override void Show()
        {
        }

        /// <summary>
        /// Disposes of this object, closing the
        ///  form.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
			}
            base.Dispose(disposing);
        }

        #endregion      
  	
        #region Workspace Management


        internal void AddWorkspaceView(WorkspaceView workspaceView)
        {
            workspaceView.SetVisibleStatus(true);
            workspaceView.SetActiveStatus(true);
        }

        internal void RemoveWorkspaceView(WorkspaceView workspaceView)
        {
            // notify that we are no longer visible
            workspaceView.SetActiveStatus(false);
            workspaceView.SetVisibleStatus(false);
        }
        #endregion
    }
}
