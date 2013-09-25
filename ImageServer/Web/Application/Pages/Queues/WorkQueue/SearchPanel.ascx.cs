#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
using AjaxControlToolkit;
using Macro.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue
{

    /// <summary>
    /// Work Queue Search Panel
    /// </summary>

    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private Members

        private ServerPartition _serverPartition;

        #endregion Private Members

        #region Events

        /// <summary>
        /// Occurs when the queue is refreshed because user clicked on the Search button.
        /// </summary>
        public event EventHandler<EventArgs> Search;

        #endregion

        #region Public Properties

        public string PatientNameFromUrl
        {
            get; set;
        }

        public string PatientIDFromUrl
        {
            get;
            set;
        }

        public string ProcessingServerFromUrl
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        public Default EnclosingPage { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewItemDetailsUrl")]
        public string ViewItemDetailsURL
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.WorkQueueItemDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return workQueueItemList.WorkQueueItemGridView.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewItemDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("RescheduleButtonClientID")]
        public string RescheduleButtonClientID
        {
            get { return RescheduleItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetButtonClientID")]
        public string ResetButtonClientID
        {
            get { return ResetItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ReprocessButtonClientID")]
        public string ReprocessButtonClientID
        {
            get { return ReprocessItemButton.ClientID; }
        }

        #endregion Public Properties

        #region Protected Methods

        internal void Reset()
        {
            Clear();
            workQueueItemList.Reset();
        }

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleCalendarExtender.ClientID);

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerWorkQueueSingleItem, SR.GridPagerWorkQueueMultipleItems, workQueueItemList.WorkQueueItemGridView,
                                             () => workQueueItemList.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            workQueueItemList.Pager = GridPagerTop;

            workQueueItemList.ServerPartition = _serverPartition;

            workQueueItemList.DataSourceCreated += delegate(WorkQueueDataSource source)
                                                            {
                                                                if (!String.IsNullOrEmpty(PatientName.Text))
                                                                    source.PatientsName = SearchHelper.NameWildCard(PatientName.Text);
                                                                
                                                                source.Partition = ServerPartition;

                                                                if (!String.IsNullOrEmpty(PatientId.Text))
                                                                    source.PatientId = SearchHelper.TrailingWildCard(PatientId.Text);

                                                                if (!String.IsNullOrEmpty(ProcessingServer.Text))
                                                                    source.ProcessingServer = SearchHelper.TrailingWildCard(ProcessingServer.Text);

                                                                source.ScheduledDate = !string.IsNullOrEmpty(ScheduleDate.Text) ? ScheduleDate.Text : string.Empty;                                   

                                                                source.DateFormats = ScheduleCalendarExtender.Format;

                                                                if (TypeListBox.SelectedIndex > -1)
                                                                {
                                                                    var types = new List<WorkQueueTypeEnum>();
                                                                    foreach (ListItem item in TypeListBox.Items)
                                                                    {
                                                                        if (item.Selected)
                                                                        {
                                                                            types.Add(WorkQueueTypeEnum.GetEnum(item.Value));
                                                                        }
                                                                    }
                                                                    source.TypeEnums = types.ToArray();
                                                                }

                                                                if (StatusListBox.SelectedIndex > -1)
                                                                {
                                                                    var statuses = new List<WorkQueueStatusEnum>();
                                                                    foreach (ListItem item in StatusListBox.Items)
                                                                    {
                                                                        if (item.Selected)
                                                                        {
                                                                            statuses.Add(WorkQueueStatusEnum.GetEnum(item.Value));
                                                                        }
                                                                    }
                                                                    source.StatusEnums = statuses.ToArray();
                                                                }

                                                                if (PriorityDropDownList.SelectedValue != string.Empty)
                                                                    source.PriorityEnum = WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);
                                                            };

            MessageBox.Confirmed += delegate
                                        {
                                            workQueueItemList.RefreshCurrentPage();
                                        };

            if(!string.IsNullOrEmpty(PatientNameFromUrl) || !string.IsNullOrEmpty(PatientIDFromUrl)  || !string.IsNullOrEmpty(ProcessingServerFromUrl))
            {
                PatientName.Text = PatientNameFromUrl;
                PatientId.Text = PatientIDFromUrl;
                ProcessingServer.Text = ProcessingServerFromUrl;

                workQueueItemList.SetDataSource();
                workQueueItemList.Refresh();
            }
        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            workQueueItemList.Refresh();

            EventsHelper.Fire(Search, this, EventArgs.Empty);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // re-populate the drop down lists and restore their states
            PopulateDropdownLists();

            ViewItemDetailsButton.Roles = AuthorityTokens.WorkQueue.View;
            DeleteItemButton.Roles = AuthorityTokens.WorkQueue.Delete;
            ReprocessItemButton.Roles = AuthorityTokens.WorkQueue.Reprocess;
            ResetItemButton.Roles = AuthorityTokens.WorkQueue.Reset; 
            RescheduleItemButton.Roles =AuthorityTokens.WorkQueue.Reschedule;
        }

        private void PopulateDropdownLists()
        {
            var workQueueTypes = WorkQueueTypeEnum.GetAll();
            var workQueueStatuses = WorkQueueStatusEnum.GetAll();
            var workQueuePriorities = WorkQueuePriorityEnum.GetAll();

            if (TypeListBox.Items.Count == 0)
            {
                foreach (WorkQueueTypeEnum t in workQueueTypes)
                {
                    TypeListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                }
            }
            if (StatusListBox.Items.Count == 0)
            {
                foreach (WorkQueueStatusEnum s in workQueueStatuses)
                {
                    StatusListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(s), s.Lookup));
                }
            }

            if (PriorityDropDownList.Items.Count==0)
            {
                PriorityDropDownList.Items.Clear();
                PriorityDropDownList.Items.Add(new ListItem(SR.Any, string.Empty));
                foreach (WorkQueuePriorityEnum p in workQueuePriorities)
                    PriorityDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(p), p.Lookup));
            }
        }

        public void Refresh()
        {
            workQueueItemList.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        protected void ViewItemButton_Click(object sender, ImageClickEventArgs e)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.ViewWorkQueueItem(workQueueItemList.SelectedDataKey);
        }

        protected void ResetItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.ResetWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.DeleteWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void ReprocessItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.ReprocessWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void RescheduleItemButton_Click(object sender, ImageClickEventArgs e)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.RescheduleWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        #endregion Protected Methods

        private bool SelectedItemExists()
        {
            if (!workQueueItemList.SelectedItemExists())
            {
                MessageBox.BackgroundCSS = string.Empty;
                MessageBox.Message = SR.SelectedWorkQueueNoLongerOnTheList;
                MessageBox.MessageStyle = "color: red; font-weight: bold;";
                MessageBox.MessageType =
                    Web.Application.Controls.MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();

                return false;
            }

            return true;
        }

        private void Clear()
        {
            PatientName.Text = string.Empty;
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;

            foreach (ListItem item in TypeListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }

            foreach (ListItem item in StatusListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }

            PriorityDropDownList.SelectedIndex = 0;
            ProcessingServer.Text = string.Empty;
        }
    }
}