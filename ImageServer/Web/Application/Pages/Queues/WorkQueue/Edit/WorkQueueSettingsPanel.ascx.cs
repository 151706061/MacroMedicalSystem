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
using Macro.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Represents a panel containing the different settings users can specify for a work queue item.
    /// </summary>
    public partial class WorkQueueSettingsPanel : UserControl
    {
        #region Constants

        private const int DEFAULT_TIME_GAP_MINS = 15; // minutes

        #endregion Constants

        #region Private members

        private ListItemCollection _defaultTimeList;

        #endregion

      
        #region Public Properties

        /// <summary>
        /// Gets/Sets the current selected work queue priority  displayed on the UI
        /// </summary>
        public WorkQueuePriorityEnum SelectedPriority
        {
            get { return WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue); }
            set { PriorityDropDownList.SelectedValue = value.Lookup; }
        }

        /// <summary>
        /// Gets/Sets the new scheduled date/time displayed on the UI
        /// </summary>
        public DateTime? NewScheduledDateTime
        {
            get { return (DateTime?) ViewState["NewScheduledDateTime"]; }
            set
            {
                ViewState["NewScheduledDateTime"] = value;
                CalendarExtender.SelectedDate = value;
                NewScheduleDate.Text = value == null ? string.Empty : value.Value.ToString(CalendarExtender.Format);
                if (value != null && ScheduleNowCheckBox.Checked == false)
                {
                    AddCustomTime(value.Value);
                }
            }
        }

        public bool ScheduleNow
        {
            get { return (Boolean) ViewState["_ScheduleNow"]; }
            set
            {
                ViewState["_ScheduleNow"] = value;
                ScheduleNowCheckBox.Checked = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the default list of schedule times available for user selection
        /// </summary>
        public ListItemCollection DefaultTimeList
        {
            get
            {
                if (_defaultTimeList != null)
                    return _defaultTimeList;

                _defaultTimeList = new ListItemCollection();
                DateTime dt = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                double scheduleTimeWindow = DEFAULT_TIME_GAP_MINS;
                while (dt < tomorrow)
                {
                    _defaultTimeList.Add(dt.ToString("HH:mm"));
                    dt = dt.AddMinutes(scheduleTimeWindow);
                }

                return _defaultTimeList;
            }
        }

        /// <summary>
        /// Appends a specified time to the default list of schedule time.
        /// </summary>
        /// <param name="time"></param>
        public void AddCustomTime(DateTime time)
        {
            string timeValue = time.ToString("HH:mm");

            NewScheduleTime.Text = timeValue;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CalendarExtender.Format = DateTimeFormatter.DefaultDateFormat;

            PriorityDropDownList.Items.Clear();
            IList<WorkQueuePriorityEnum> priorities = WorkQueuePriorityEnum.GetAll();
            foreach (WorkQueuePriorityEnum priority in priorities)
            {
                PriorityDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(priority), priority.Lookup));
            }

            ScheduleNowCheckBox.Checked = false;
            NewScheduleDate.Enabled = true;
            NewScheduleTime.Enabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected void ScheduleNow_CheckChanged(object sender, EventArgs arg)
        {
            ScheduleNow = ScheduleNowCheckBox.Checked;

            if (ScheduleNow)
            {
                NewScheduleDate.Enabled = false;
                NewScheduleTime.Enabled = false;
            }
            else
            {
                NewScheduleDate.Enabled = true;
                NewScheduleTime.Enabled = true;
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public override void DataBind()
        {
            if (Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(NewScheduleDate.Text))
                {
                    DateTime dt = DateTime.ParseExact(NewScheduleDate.Text, CalendarExtender.Format, null);

                    DateTime time = Platform.Time;

                    if (NewScheduleTime.Text.Contains("_") == false)
                    {
                        try
                        {
                            time = DateTime.ParseExact(NewScheduleTime.Text, "HH:mm", null);
                        }
                        catch (Exception)
                        {
                            //Ignore this exception since the time is not fully typed in or in an incorrect format,
                            //that will be validated when the user presses apply.
                        }
                    }

                    NewScheduledDateTime = dt.Add(time.TimeOfDay);

                    CalendarExtender.SelectedDate = NewScheduledDateTime;
                }
                else
                {
                    NewScheduledDateTime = null;
                }
            }

            NewScheduleDate.Enabled = true;
            NewScheduleTime.Enabled = true;

            base.DataBind();
        }

        public void UpdateScheduleDateTime()
        {
            DateTime newDate = DateTime.Parse(NewScheduleDate.Text + " " + NewScheduleTime.Text);
            NewScheduledDateTime = newDate;
        }

        internal void ResetSelections()
        {
            ScheduleNowCheckBox.Checked = false;
            NewScheduleDate.Enabled = true;
            NewScheduleTime.Enabled = true;
        }

        #endregion Public Methods

    }
}