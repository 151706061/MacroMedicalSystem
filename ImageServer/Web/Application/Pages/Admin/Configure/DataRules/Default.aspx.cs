#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using System.Xml;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules
{
    [PrincipalPermission(SecurityAction.Demand, Role = ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.DataAccessRules)]
    public partial class Default : BasePage
    {
        private readonly ServerRuleController _controller = new ServerRuleController();

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
                                                            {
                                                                SearchPanel.ServerPartition = partition;
                                                                SearchPanel.Reset();
                                                            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);
            SearchPanel.EnclosingPage = this;

            ConfirmDialog.Confirmed += delegate(object data)
            {
                // delete the device and reload the affected partition.
                var key = data as ServerEntityKey;

                ServerRule rule = ServerRule.Load(key);

                _controller.DeleteServerRule(rule);

                SearchPanel.Refresh();
            };


            AddEditDataRuleControl.OKClicked += delegate(ServerRule rule)
            {
                if (AddEditDataRuleControl.Mode == AddEditDataRuleDialogMode.Edit)
                {
                    // Commit the change into database
                    if (!_controller.UpdateServerRule(rule))
                    {
                        // TODO: alert user
                    }
                }
                else
                {
                    // Create new device in the database
                    ServerRule newRule = _controller.AddServerRule(rule);
                    if (newRule == null)
                    {
                        //TODO: alert user
                    }
                }

                SearchPanel.Refresh();                
            };


            SetPageTitle(Titles.DataRulesPageTitle);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                SearchPanel.Refresh();
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnEditRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Edit;
            AddEditDataRuleControl.ServerRule = rule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to edit a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnCopyRule(ServerRule rule, ServerPartition partition)
        {
            var copiedRule = new ServerRule(rule.RuleName + " (Copy)",rule.ServerPartitionKey,rule.ServerRuleTypeEnum, rule.ServerRuleApplyTimeEnum, rule.Enabled, rule.DefaultRule, rule.ExemptRule, (XmlDocument)rule.RuleXml.CloneNode(true));

            // Store a dummy entity key
            copiedRule.SetKey(new ServerEntityKey("ServerRule",Guid.NewGuid()));
 
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.Copy;
            AddEditDataRuleControl.ServerRule = copiedRule;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to delete a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format(SR.AdminServerRules_DeleteDialog_AreYouSure, rule.RuleName, partition.AeTitle);
            ConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            ConfirmDialog.Data = rule.GetKey();
            ConfirmDialog.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to add a new rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnAddRule(ServerRule rule, ServerPartition partition)
        {
            AddEditDataRuleControl.Mode = AddEditDataRuleDialogMode.New;
            AddEditDataRuleControl.ServerRule = null;
            AddEditDataRuleControl.Partition = partition;
            AddEditDataRuleControl.Show();
        }

        #endregion Public Methods
    }
}
