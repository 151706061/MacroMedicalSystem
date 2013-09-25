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
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    /// <summary>
    /// Server parition panel  used in <seealso cref="Default"/> web page.
    /// </summary>
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js")]
    public partial class PartitionArchivePanel : AJAXScriptControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<Model.PartitionArchive> _partitionArchives = new List<Model.PartitionArchive>();
        // used for database interaction
        private PartitionArchiveConfigController _theController;

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeletePartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string RestoreButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("PartitionArchiveListClientID")]
        public string PartitionArchiveListClientID
        {
            get { return PartitionArchiveGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<Model.PartitionArchive> PartitionArchives
        {
            get { return _partitionArchives; }
            set
            {
                _partitionArchives = value;
                PartitionArchiveGridPanel.Partitions = _partitionArchives;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public PartitionArchiveConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            int archiveSelectedIndex = ArchiveTypeFilter.SelectedIndex;

            ArchiveTypeFilter.Items.Clear();
            ArchiveTypeFilter.Items.Add(new ListItem(SR.All));
            foreach (ArchiveTypeEnum archiveTypeEnum in ArchiveTypeEnum.GetAll())
            {
                ArchiveTypeFilter.Items.Add(
                    new ListItem(ServerEnumDescription.GetLocalizedDescription(archiveTypeEnum), archiveTypeEnum.Lookup));
            }
            ArchiveTypeFilter.SelectedIndex = archiveSelectedIndex;

            int statusSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(SR.All, SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled, SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled, SR.Disabled));
            StatusFilter.SelectedIndex = statusSelectedIndex;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Refresh();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new PartitionArchiveConfigController();

            GridPagerTop.InitializeGridPager(SR.GridPagerPartitionSingleItem, SR.GridPagerPartitionMultipleItems, PartitionArchiveGridPanel.TheGrid,
                                             () => PartitionArchives.Count, ImageServerConstants.GridViewPagerPosition.Top);
            PartitionArchiveGridPanel.Pager = GridPagerTop;

        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }

        protected void LoadData()
        {
            var criteria = new PartitionArchiveSelectCriteria();

            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                                   SearchHelper.TrailingWildCard(DescriptionFilter.Text));
            }

            if (StatusFilter.SelectedIndex > 0)
            {
                criteria.Enabled.EqualTo(StatusFilter.SelectedIndex == 1);
            }

        	criteria.ServerPartitionKey.EqualTo(ServerPartition.Key);

            PartitionArchives =
                _theController.GetPartitions(criteria);
            PartitionArchiveGridPanel.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
           ((Default)Page).AddPartition(ServerPartition);
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).DeletePartition(selectedPartition);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void Refresh()
        {
            LoadData();
            PartitionArchiveGridPanel.UpdateUI();

            SearchUpdatePanel.Update();
        }

        internal void Reset()
        {
            //Clear();
            PartitionArchiveGridPanel.Reset();
        }
        #endregion Public methods       
    }
}