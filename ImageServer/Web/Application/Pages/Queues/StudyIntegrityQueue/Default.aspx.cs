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
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.StudyIntegrityQueue.Search)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerPartition activePartition = null;

            if (!IsPostBack && !Page.IsAsync)
            {
                string patientID = Request["PatientID"];
                string patientName = Request["PatientName"];
                string partitionKey = Request["PartitionKey"];
                string reason = Request["Reason"];

                if (!string.IsNullOrEmpty(patientID) && !string.IsNullOrEmpty(patientName) &&
                    !string.IsNullOrEmpty(partitionKey))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
                if (string.IsNullOrEmpty(reason))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
            }

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

            if (activePartition != null)
            {
                ServerPartitionSelector.SelectedPartition = activePartition;
            }

            SetPageTitle(Titles.StudyIntegrityQueuePageTitle);
        }

        public void OnReconcileItem(ReconcileDetails details)
        {
            if (details.StudyIntegrityQueueItem.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate)
            {
                DuplicateSopReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                DuplicateSopReconcileDialog.DuplicateEntryDetails = details as DuplicateEntryDetails;

                DuplicateSopReconcileDialog.DataBind();
                DuplicateSopReconcileDialog.Show();
            }
            else
            {
                ReconcileDialog.ReconcileDetails = details;
                ReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                ReconcileDialog.Show();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
        }
    }
}