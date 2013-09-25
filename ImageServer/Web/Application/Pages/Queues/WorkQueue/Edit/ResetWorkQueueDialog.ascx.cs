#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to reset a work queue entry and carries out the reset if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entry through the <see cref="WorkQueueItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="WorkQueueItemResetListener"/>
    /// which is fired when users confirmed to reset the entry and it was sucessfully reset.
    /// </remarks>
    public partial class ResetWorkQueueDialog : UserControl
    {
        private Model.WorkQueue _workQueue;

        #region Events

        #region Delegates

        public delegate void OnHideEventHandler();

        public delegate void OnShowEventHandler();

        public delegate void WorkQueueItemResetListener(Model.WorkQueue item);

        #endregion

        public new event EventHandler<WorkQueueItemResetErrorEventArgs> Error;

        public event WorkQueueItemResetListener WorkQueueItemReseted;

        public event OnShowEventHandler OnShow;

        public event OnHideEventHandler OnHide;

        #endregion Events

        #region Public Properties

        /// <summary>
        /// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
        /// </summary>
        public ServerEntityKey WorkQueueItemKey
        {
            get { return ViewState["WorkQueueItemKey"] == null ? null : (ServerEntityKey) ViewState["WorkQueueItemKey"]; }
            set { ViewState["WorkQueueItemKey"] = value; }
        }

        public bool IsShown
        {
            get { return ViewState["IsShown"] != null && (bool) ViewState["IsShown"]; }
            set { ViewState["IsShown"] = value; }
        }

        #endregion Public Properties

        private Model.WorkQueue WorkQueue
        {
            get
            {
                if (_workQueue == null)
                {
                    if (WorkQueueItemKey != null)
                    {
                        var adaptor = new WorkQueueAdaptor();
                        _workQueue = adaptor.Get(WorkQueueItemKey);
                    }
                }

                return _workQueue;
            }
        }

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreResetConfirmDialog.Confirmed += PreResetConfirmDialog_Confirmed;
            PreResetConfirmDialog.Cancel += Hide;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && IsShown)
            {
                DataBind();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void PreResetConfirmDialog_Confirmed(object data)
        {
            Hide();

            var key = data as ServerEntityKey;

            if (key != null)
            {
                var adaptor = new WorkQueueAdaptor();
                Model.WorkQueue item = adaptor.Get(key);
                if (item == null)
                {
                    String errorMessage = SR.WorkQueueNotAvailable;
                    EventsHelper.Fire(Error, this, new WorkQueueItemResetErrorEventArgs(errorMessage, null));
                }
                else
                {
                    var controller = new WorkQueueController();
                    DateTime scheduledTime = item.ScheduledTime;
                    if (scheduledTime < Platform.Time)
                        scheduledTime = Platform.Time.AddSeconds(WorkQueueSettings.Default.WorkQueueProcessDelaySeconds);

                    DateTime expirationTime = item.ExpirationTime;
                    if (expirationTime < scheduledTime)
                        expirationTime = scheduledTime.AddSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds);

                    try
                    {
                        var items = new List<Model.WorkQueue>();
                        items.Add(item);

                        controller.ResetWorkQueueItems(items, scheduledTime, expirationTime);

                        Platform.Log(LogLevel.Info, "{0} Work Queue item reset:  Key={1}.", item.WorkQueueTypeEnum,
                                     item.GetKey());
                        if (WorkQueueItemReseted != null)
                            WorkQueueItemReseted(item);

                        if (OnHide != null) OnHide();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unable to reset {0} work queue item. Key={1}.",
                                     item.WorkQueueTypeEnum, item.GetKey());

                        String errorMessage = String.Format(SR.WorkQueueResetFailed, e.Message);

                        EventsHelper.Fire(Error, this, new WorkQueueItemResetErrorEventArgs(errorMessage, e));
                    }
                }
            }
        }

        #endregion Private Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                if (WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    if (!String.IsNullOrEmpty(_workQueue.ProcessorID)) // somebody has claimed it
                    {
                        PreResetConfirmDialog.MessageType =
                            MessageBox.MessageTypeEnum.INFORMATION;
                        PreResetConfirmDialog.Message = SR.WorkQueueBeingProcessed;
                    }
                }
                else
                {
                    PreResetConfirmDialog.Data = WorkQueueItemKey;
                    PreResetConfirmDialog.MessageType =
                        MessageBox.MessageTypeEnum.YESNO;
                    PreResetConfirmDialog.Message = SR.WorkQueueResetConfirm;
                }
            }

            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for reseting the <see cref="WorkQueue"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="WorkQueueItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
        public void Show()
        {
            IsShown = true;

            DataBind();

            if (OnShow != null) OnShow();
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            IsShown = false;
            if (OnHide != null) OnHide();
        }

        protected override void OnPreRender(EventArgs e)
        {
            PreResetConfirmDialog.Close();
            if (IsShown)
            {
                if (WorkQueue != null)
                {
                    PreResetConfirmDialog.Show();
                }
            }
            base.OnPreRender(e);
        }

        #endregion Public Methods
    }

    public class WorkQueueItemResetErrorEventArgs : EventArgs
    {
        public WorkQueueItemResetErrorEventArgs(string error, Exception exception)
        {
            ErrorMessage = error;
            Exception = exception;
        }

        public string ErrorMessage { get; set; }

        public Exception Exception { get; set; }
    }
}