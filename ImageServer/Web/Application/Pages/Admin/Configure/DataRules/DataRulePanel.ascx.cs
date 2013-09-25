#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Resources;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.js",
        "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules
{
    [ClientScriptResource(
        ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel",
        ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.js")]
    public partial class DataRulePanel : AJAXScriptControl
    {
        private readonly ServerRuleController _controller = new ServerRuleController();

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteDataRuleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditDataRuleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("CopyButtonClientID")]
        public string CopyButtonClientID
        {
            get { return CopyDataRuleButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DataRuleListClientID")]
        public string DataRuleListClientID
        {
            get { return DataRuleGridViewControl.TheGrid.ClientID; }
        }

        public ServerPartition ServerPartition { get; set; }

        public Default EnclosingPage { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void LoadRules()
        {
            var criteria = new ServerRuleSelectCriteria();

            // only query for rule in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());
            criteria.ServerRuleTypeEnum.EqualTo(ServerRuleTypeEnum.DataAccess);

            if (StatusFilter.SelectedIndex != 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

            if (DefaultFilter.SelectedIndex != 0)
            {
                if (DefaultFilter.SelectedIndex == 1)
                    criteria.DefaultRule.EqualTo(true);
                else
                    criteria.DefaultRule.EqualTo(false);
            }

            DataRuleGridViewControl.DataRules = _controller.GetServerRules(criteria);
            DataRuleGridViewControl.DataBind();
        }

        /// <summary>
        /// Updates the rules list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadRules()"/> instead.
        /// </remarks>
        public void Refresh()
        {
            LoadRules();

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            SearchUpdatePanel.Update();
        }

        internal void Reset()
        {
            Clear();
            DataRuleGridViewControl.Reset();
        }

        public override void DataBind()
        {
            LoadRules();
            base.DataBind();
        }

        public void OnRowSelected(int index)
        {
        }

        #endregion Public Methods

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            DataRuleGridViewControl.DataRulePanel = this;

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerServerRulesSingleItem,
                                             SR.GridPagerServerRulesMultipleItems,
                                             DataRuleGridViewControl.TheGrid,
                                             () => DataRuleGridViewControl.DataRules == null
                                                       ? 0
                                                       : DataRuleGridViewControl.DataRules.Count,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            DataRuleGridViewControl.Pager = GridPagerTop;
            GridPagerTop.Reset();

            if (Page.IsPostBack)
                DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

            DefaultFilter.Items.Add(new ListItem(SR.All));
            DefaultFilter.Items.Add(new ListItem(SR.Default));
            DefaultFilter.Items.Add(new ListItem(SR.NotDefault));
        }

        protected override void OnPreRender(EventArgs e)
        {
            ServerRule rule = DataRuleGridViewControl.SelectedRule;
            if (rule == null)
            {
                // no rule being selected

                EditDataRuleButton.Enabled = false;

                DeleteDataRuleButton.Enabled = false;
            }
            else
            {
                EditDataRuleButton.Enabled = true;

                DeleteDataRuleButton.Enabled = !rule.DefaultRule;
            }

            base.OnPreRender(e);
            Refresh();
        }

        protected void AddDataRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddRule(null, ServerPartition);
        }

        protected void EditDataRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            if (DataRuleGridViewControl.SelectedRule != null)
                EnclosingPage.OnEditRule(DataRuleGridViewControl.SelectedRule, ServerPartition);
        }

        protected void CopyDataRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            if (DataRuleGridViewControl.SelectedRule != null)
                EnclosingPage.OnCopyRule(DataRuleGridViewControl.SelectedRule, ServerPartition);
        }

        protected void DeleteDataRuleButton_Click(object sender, ImageClickEventArgs e)
        {
            if (DataRuleGridViewControl.SelectedRule != null)
                EnclosingPage.OnDeleteRule(DataRuleGridViewControl.SelectedRule, ServerPartition);
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            StatusFilter.SelectedIndex = 0;
            DefaultFilter.SelectedIndex = 0;

            LoadRules();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        private void Clear()
        {
            StatusFilter.SelectedIndex = 0;
            DefaultFilter.SelectedIndex = 0;
        }

        #endregion Protected Methods
    }
}