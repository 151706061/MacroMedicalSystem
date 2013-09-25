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
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.PartitionArchive)]
    public partial class Default : BasePage
    {
        #region Private Members
        // used for database interaction
        private PartitionArchiveConfigController _controller = new PartitionArchiveConfigController();

        #endregion

        #region Protected Methods

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog.OKClicked += AddEditPartitionDialog_OKClicked;
            DeleteConfirmDialog.Confirmed += DeleteConfirmDialog_Confirmed;
        }


        public void UpdateUI()
        {
			SearchPanel.Refresh();
            PageContent.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupEventHandlers();

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;

            UpdateUI();

            SetPageTitle(Titles.PartitionArchivesPageTitle);
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEditPartitionDialog_OKClicked(Model.PartitionArchive partition)
        {
            if (AddEditPartitionDialog.EditMode)
            {
                // Add partition into db and refresh the list
                if (_controller.UpdatePartition(partition))
                {
                    UpdateUI();
                }
            }
            else
            {
                // Add partition into db and refresh the list
                if (_controller.AddPartition(partition))
                {
                    UpdateUI();
                }
            }
        }

        private void DeleteConfirmDialog_Confirmed(object data)
        {
            var key = data as ServerEntityKey;

            Model.PartitionArchive pa = Model.PartitionArchive.Load(key);

            _controller.Delete(pa);

            SearchPanel.Refresh();
        }

        #endregion

        #region Public Methods

        public void AddPartition(ServerPartition partition)
        {
            // display the add dialog
            AddEditPartitionDialog.PartitionArchive = null;
            AddEditPartitionDialog.EditMode = false;
            AddEditPartitionDialog.Show(true);
			AddEditPartitionDialog.Partition = partition;
		}

        public void EditPartition(Model.PartitionArchive partitionArchive)
        {
            AddEditPartitionDialog.PartitionArchive = partitionArchive;
            AddEditPartitionDialog.EditMode = true;
            AddEditPartitionDialog.Show(true);
        	AddEditPartitionDialog.Partition = ServerPartition.Load(partitionArchive.ServerPartitionKey);
        }

        public void DeletePartition(Model.PartitionArchive partitionArchive)
        {
            DeleteConfirmDialog.Message = String.Format(SR.AdminPartitionArchive_DeleteDialog_AreYouSure, partitionArchive.Description);
            DeleteConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmDialog.Data = partitionArchive.GetKey();
            DeleteConfirmDialog.Show();
        }

        #endregion
 
    }
}