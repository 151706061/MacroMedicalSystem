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
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;
using MessageBox=Macro.ImageServer.Web.Application.Controls.MessageBox;
using ModalDialog=Macro.ImageServer.Web.Application.Controls.ModalDialog;
using Resources;


namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks
{
    //
    // Dialog for editting an existing service lock.
    //
    public partial class EditServiceLockDialog : UserControl
    {
        #region Constants
        private const string TIME_FORMAT = "HH:mm";
        private const int DEFAULT_TIME_GAP_MINS = 15; // minutes
        private ServiceLock _serviceLock;

        #endregion Constants

        #region Private Members
        private List<ListItem> _defaultTimeList;

        #endregion Private Members

        #region Events

        public delegate void ServiceLockUpdatedListener(ServiceLock serviceLock);

        public event ServiceLockUpdatedListener ServiceLockUpdated;

        #endregion Events

        #region public members


        /// <summary>
        /// Gets the default list of schedule times available for user selection
        /// </summary>
        public List<ListItem> DefaultTimeListItems
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new List<ListItem>();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = DEFAULT_TIME_GAP_MINS;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(new ListItem(dt.ToString(TIME_FORMAT)));
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }


        /// <summary>
        /// Sets/Gets the current editing service.
        /// </summary>
        public ServiceLock ServiceLock
        {
            set
            {
                _serviceLock = value;
                // put into viewstate to retrieve later
                if(_serviceLock != null)
                    ViewState[ "_ServiceLock"] = _serviceLock.GetKey();
            }
            get
            {
                return _serviceLock;
            }
        }

        #endregion // public members

        #region Protected methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ViewState[ "_ServiceLock"] != null)
            {
                ServerEntityKey serviceLockKey = ViewState[ "_ServiceLock"] as ServerEntityKey;
                _serviceLock = ServiceLock.Load(serviceLockKey);
            }

            ScheduleDate.Text = Request[ScheduleDate.UniqueID] ;
        }

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();
                Close();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected methods

        #region Private Methods

        private void AddCustomTime(DateTime customTime)
        {
            string customTimeValue = customTime.ToString(TIME_FORMAT);
            ScheduleTime.Text = customTimeValue;
        }

        private void SaveData()
        {
            if (ServiceLock != null)
            {
                ServiceLock.Enabled = Enabled.Checked;

                ServiceLockConfigurationController controller = new ServiceLockConfigurationController();
                DateTime scheduledDate = DateTime.ParseExact(ScheduleDate.Text, CalendarExtender.Format, null);

                if (ScheduleTime.Text.Contains("_") == false)
                {
                    try
                    {
                        DateTime scheduleTime = DateTime.ParseExact(ScheduleTime.Text, TIME_FORMAT, null);
                        scheduledDate = scheduledDate.Add(scheduleTime.TimeOfDay);
                    }
                    catch (Exception)
                    {
                        //Ignore this exception since the time is not fully typed in or in an incorrect format,
                        //that will be validated when the user presses apply.
                    }
                }               
                
                if (controller.UpdateServiceLock(ServiceLock.GetKey(), Enabled.Checked, scheduledDate))
                {
                    if (ServiceLockUpdated != null)
                        ServiceLockUpdated(ServiceLock);
                }
                else
                {
                    ErrorMessageBox.Message = SR.ServiceLockUpdateFailed_ContactAdmin;
                    ErrorMessageBox.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    ErrorMessageBox.Show();
                }
            }

        }

        #endregion Private Methods

        #region Public methods



        public override void DataBind()
        {
            ServiceLock service = ServiceLock;

            if (service != null)
            {
                // only rebind the data if the dialog has been closed
                if (ModalDialog.State == ModalDialog.ShowState.Hide)
                {
                    Type.Text = ServerEnumDescription.GetLocalizedDescription(service.ServiceLockTypeEnum);
                    Description.Text = ServerEnumDescription.GetLocalizedLongDescription(service.ServiceLockTypeEnum);
                    Enabled.Checked = service.Enabled;

                    if (service.FilesystemKey != null)
                    {
                        FileSystemDataAdapter adaptor = new FileSystemDataAdapter();
                        Model.Filesystem fs = adaptor.Get(service.FilesystemKey);
                        FileSystem.Text = fs.Description;
                    }
                    else
                        FileSystem.Text = string.Empty;

                    CalendarExtender.SelectedDate = service.ScheduledTime;
                    
                    AddCustomTime(service.ScheduledTime);

                }

            }

            base.DataBind();
        }


        /// <summary>
        /// Displays the add/edit service dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();

            ModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();
        }

        #endregion Public methods
    }
}
