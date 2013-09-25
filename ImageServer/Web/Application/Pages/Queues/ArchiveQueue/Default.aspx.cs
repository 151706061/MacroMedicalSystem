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
using System.Security.Permissions;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Pages.Common;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
    public partial class Default : BasePage
    {
        
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.ArchiveQueue.Search)]      
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchPanel.EnclosingPage = this;

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

            SetPageTitle(Titles.ArchiveQueuePageTitle);
        }

		public void ResetArchiveQueueItem(IList<Model.ArchiveQueue> list)
    	{
			if (list != null)
			{
				ResetArchiveQueueDialog.ArchiveQueueItemList = list;
				ResetArchiveQueueDialog.Show();
			}
    	}

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
        }
    }
}
