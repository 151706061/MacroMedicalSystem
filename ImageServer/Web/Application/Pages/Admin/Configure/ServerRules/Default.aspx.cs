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
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Model;
using Resources;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.ServerRules)]
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


            AddEditServerRuleControl.OKClicked += delegate(ServerRule rule)
                                                      {
                                                          if (AddEditServerRuleControl.EditMode)
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


            SetPageTitle(Titles.ServerRulesPageTitle);

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
            AddEditServerRuleControl.EditMode = true;
            AddEditServerRuleControl.ServerRule = rule;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }

        /// <summary>
        /// Displays a popup dialog box for users to delete a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="partition"></param>
        public void OnDeleteRule(ServerRule rule, ServerPartition partition)
        {
            ConfirmDialog.Message = string.Format(SR.AdminServerRules_DeleteDialog_AreYouSure,rule.RuleName, partition.AeTitle);
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
            AddEditServerRuleControl.EditMode = false;
            AddEditServerRuleControl.ServerRule = null;
            AddEditServerRuleControl.Partition = partition;
            AddEditServerRuleControl.Show();
        }

        #endregion Public Methods
    }
}