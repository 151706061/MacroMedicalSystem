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
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Model;
using Resources;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js",
        "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [ClientScriptResource(
        ComponentType = "Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel",
        ResourcePath = "Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("ReconcileButtonClientID")]
        public string ReconcileButtonClientID
        {
            get { return ReconcileButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.ClientID; }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        public string PatientNameFromUrl { get; set; }

        public string PatientIdFromUrl { get; set; }

        public string ReasonFromUrl { get; set; }

        public bool DataBindFromUrl { get; set; }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientName.Text = string.Empty;
            PatientId.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            FromDate.Text = string.Empty;
            ToDate.Text = string.Empty;
        }

        public void Refresh()
        {
            SearchUpdatePanel.Update();
            StudyIntegrityQueueItemList.RefreshCurrentPage();
        }

        internal void Reset()
        {
            Clear();
            StudyIntegrityQueueItemList.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearFromDateButton.OnClientClick = ScriptHelper.ClearDate(FromDate.ClientID,
                                                                       FromDateCalendarExtender.ClientID);
            ClearToDateButton.OnClientClick = ScriptHelper.ClearDate(ToDate.ClientID,
                                                                     ToDateCalendarExtender.ClientID);
            ToDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDate.ClientID, ToDate.ClientID,
                                                                        ToDate.ClientID, ToDateCalendarExtender.ClientID,
                                                                        "To Date must be greater than From Date");
            FromDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDate.ClientID, ToDate.ClientID,
                                                                          FromDate.ClientID,
                                                                          FromDateCalendarExtender.ClientID,
                                                                          "From Date must be less than To Date");

            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem,
                                             Labels.GridPagerQueueMultipleItems,
                                             StudyIntegrityQueueItemList.StudyIntegrityQueueGrid,
                                             () => StudyIntegrityQueueItemList.ResultCount,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            StudyIntegrityQueueItemList.Pager = GridPagerTop;

            StudyIntegrityQueueItemList.DataSourceCreated += delegate(StudyIntegrityQueueDataSource source)
                                                                 {
                                                                     source.Partition = ServerPartition;

                                                                     if (!String.IsNullOrEmpty(PatientName.Text))
                                                                         source.PatientName = SearchHelper.NameWildCard(PatientName.Text);
                                                                     if (!String.IsNullOrEmpty(PatientId.Text))
                                                                         source.PatientId = SearchHelper.TrailingWildCard(PatientId.Text);
                                                                     if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                                         source.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.Text);
                                                                     if (!String.IsNullOrEmpty(FromDate.Text))
                                                                         source.FromInsertTime = FromDate.Text;

                                                                     if (!String.IsNullOrEmpty(ToDate.Text))
                                                                         source.ToInsertTime = ToDate.Text;

                                                                     if (ReasonListBox.SelectedIndex > -1)
                                                                     {
                                                                         var reasonEnums =
                                                                             new List<StudyIntegrityReasonEnum>();
                                                                         foreach (ListItem item in ReasonListBox.Items)
                                                                         {
                                                                             if (item.Selected)
                                                                             {
                                                                                 reasonEnums.Add(
                                                                                     StudyIntegrityReasonEnum.GetEnum(
                                                                                         item.Value));
                                                                             }
                                                                         }

                                                                         source.ReasonEnum = reasonEnums;
                                                                     }
                                                                 };

            ReconcileButton.Roles =
                AuthorityTokens.StudyIntegrityQueue.Reconcile;

            List<StudyIntegrityReasonEnum> reasons = StudyIntegrityReasonEnum.GetAll();
            foreach (StudyIntegrityReasonEnum reason in reasons)
            {
                ReasonListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription( reason), reason.Lookup));
            }

            if (!string.IsNullOrEmpty(ReasonFromUrl))
                ReasonListBox.Items.FindByValue(ReasonFromUrl).Selected = true;

            if (!string.IsNullOrEmpty(PatientNameFromUrl) || !string.IsNullOrEmpty(PatientIdFromUrl))
            {
                PatientName.Text = PatientNameFromUrl;
                PatientId.Text = PatientIdFromUrl;
                
                StudyIntegrityQueueItemList.SetDataSource();
                StudyIntegrityQueueItemList.Refresh();
            }
            else if (DataBindFromUrl)
            {
                StudyIntegrityQueueItemList.SetDataSource();
                StudyIntegrityQueueItemList.Refresh();
            }
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyIntegrityQueueItemList.Refresh();
        }

        protected void ReconcileButton_Click(object sender, EventArgs e)
        {
            ReconcileDetails details =
                ReconcileDetailsAssembler.CreateReconcileDetails(StudyIntegrityQueueItemList.SelectedItems[0]);

            ((Default) Page).OnReconcileItem(details);
        }

        #endregion Protected Methods
    }
}