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
using AjaxControlToolkit;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
    [ClientScriptResource(ComponentType="Macro.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel", ResourcePath="Macro.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        private readonly ArchiveQueueController _controller = new ArchiveQueueController();

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("OpenButtonClientID")]
		public string OpenButtonClientID
		{
			get { return ViewStudyDetailsButton.ClientID; }
		}

		[ExtenderControlProperty]
		[ClientPropertyName("ResetButtonClientID")]
		public string ResetButtonClientID
		{
			get { return ResetItemButton.ClientID; }
		}

		[ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return ArchiveQueueItemList.ArchiveQueueGrid.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("OpenStudyPageUrl")]
		public string OpenStudyPageUrl
		{
			get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
		}

        public Default EnclosingPage { get; set; }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
        }

        public void Refresh()
        {
            
        }

        internal void Reset()
        {
            Clear();
            ArchiveQueueItemList.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleDateCalendarExtender.ClientID);
                           
            // setup child controls
            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem, Labels.GridPagerQueueMultipleItems, ArchiveQueueItemList.ArchiveQueueGrid,
                                             () => ArchiveQueueItemList.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            ArchiveQueueItemList.Pager = GridPagerTop;

            MessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Model.ArchiveQueue>)
                                {
                                    var items = data as IList<Model.ArchiveQueue>;
                                    foreach (Model.ArchiveQueue item in items)
                                    {
                                        _controller.DeleteArchiveQueueItem(item);
                                    }
                                }
                                else if (data is Model.ArchiveQueue)
                                {
                                    var item = data as Model.ArchiveQueue;
                                    _controller.DeleteArchiveQueueItem(item);
                                }

                                ArchiveQueueItemList.RefreshCurrentPage();
                                SearchUpdatePanel.Update(); // force refresh

                            };

			ArchiveQueueItemList.DataSourceCreated += delegate(ArchiveQueueDataSource source)
										{
											source.Partition = ServerPartition;
                                            source.DateFormats = ScheduleDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(StatusFilter.SelectedValue) && StatusFilter.SelectedIndex > 0)
                                                source.StatusEnum = ArchiveQueueStatusEnum.GetEnum(StatusFilter.SelectedValue);
                                            if (!String.IsNullOrEmpty(PatientId.Text))
												source.PatientId = SearchHelper.TrailingWildCard(PatientId.Text);
											if (!String.IsNullOrEmpty(PatientName.Text))
												source.PatientName = SearchHelper.NameWildCard(PatientName.Text);
											if (!String.IsNullOrEmpty(ScheduleDate.Text))
												source.ScheduledDate = ScheduleDate.Text;
										};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IList<ArchiveQueueStatusEnum> statusItems = ArchiveQueueStatusEnum.GetAll();

            int prevSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(SR.All, "All"));
            foreach (ArchiveQueueStatusEnum s in statusItems)
                StatusFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(s), s.Lookup));
            StatusFilter.SelectedIndex = prevSelectedIndex;

            DeleteItemButton.Roles = AuthorityTokens.ArchiveQueue.Delete;
        	ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;

			if (!IsPostBack && !Page.IsAsync)
			{
				string patientID = Server.UrlDecode(Request["PatientID"]);
				string patientName = Server.UrlDecode(Request["PatientName"]);
				string partitionKey = Server.UrlDecode(Request["PartitionKey"]);

				if (patientID != null && patientName != null && partitionKey != null)
				{
					var controller = new ServerPartitionConfigController();
					ServerPartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));

					PatientId.Text = patientID;
					PatientName.Text = patientName;

					ArchiveQueueItemList.SetDataSource();
					ArchiveQueueItemList.Refresh();
				}
			}
        }
       
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            ArchiveQueueItemList.Refresh();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs e)
        {            
            IList<Model.ArchiveQueue> items = ArchiveQueueItemList.SelectedItems;

            if (items != null && items.Count>0)
            {
                if (items.Count > 1) MessageBox.Message = string.Format(SR.MultipleArchiveQueueDelete);
                else MessageBox.Message = string.Format(SR.SingleArchiveQueueDelete);

                MessageBox.Message += "<table style=\"border: solid #CCCCCC 2px; margin-top: 5px;\">";
                foreach (Model.ArchiveQueue item in items)
                {
                    MessageBox.Message += String.Format("<tr><td style=\"font-weight: bold; color: #618FAD\">{0}:</td><td style=\"font-weight: normal; color: black;\">{1}</td></tr>", 
                        SR.StudyInstanceUID,            
                        StudyStorage.Load(item.StudyStorageKey).StudyInstanceUid);
                }
                MessageBox.Message += "</table>";

                MessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                MessageBox.MessageStyle = "color: #FF0000; font-weight: bold;";
                MessageBox.Data = items;
                MessageBox.Show();
            }
        }

        #endregion Protected Methods

    	protected void ResetItemButton_Click(object sender, ImageClickEventArgs e)
    	{
			if (ArchiveQueueItemList.SelectedItems == null)
				DataBind();

			if (ArchiveQueueItemList.SelectedItems.Count > 0)
			{
				EnclosingPage.ResetArchiveQueueItem(ArchiveQueueItemList.SelectedItems);
				ArchiveQueueItemList.RefreshCurrentPage();
			}
    	}
    }
}