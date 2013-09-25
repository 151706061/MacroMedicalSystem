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
using Macro.Common.Utilities;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Studies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Study.Search)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            DeleteStudyConfirmDialog.StudyDeleted += DeleteStudyConfirmDialogStudyDeleted;

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
                                                            {
                                                                SearchPanel.ServerPartition = partition;
                                                                SearchPanel.Reset();
                                                            };
            SetPageTitle(Titles.StudiesPageTitle);

            ServerPartitionSelector.SetUpdatePanel(PageContent);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
            SearchPanel.DeleteButtonClicked += SearchPanelDeleteButtonClicked;
            SearchPanel.AssignAuthorityGroupsButtonClicked += SearchPanelAssignAuthorityGroupsButtonClicked;
        }

        private void DeleteStudyConfirmDialogStudyDeleted(object sender, DeleteStudyConfirmDialogStudyDeletedEventArgs e)
        {
            SearchPanel.Refresh();
        }

        private void SearchPanelDeleteButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
        {
            var list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowDeletedDialog(list);
        }

        protected void ShowDeletedDialog(IList<StudySummary> studyList)
        {
            DeleteStudyConfirmDialog.Initialize(CollectionUtils.Map(
                studyList,
                delegate(StudySummary study)
                {
                    var info = new DeleteStudyInfo
                                   {
                                       StudyKey = study.Key,
                                       ServerPartitionAE = study.ThePartition.AeTitle,
                                       AccessionNumber = study.AccessionNumber,
                                       Modalities = study.ModalitiesInStudy,
                                       PatientId = study.PatientId,
                                       PatientsName = study.PatientsName,
                                       StudyDate = study.StudyDate,
                                       StudyDescription = study.StudyDescription,
                                       StudyInstanceUid = study.StudyInstanceUid
                                   };
                    return info;
                }
                ));
            DeleteStudyConfirmDialog.Show();
        }

        private void SearchPanelAssignAuthorityGroupsButtonClicked(object sender, SearchPanelButtonClickedEventArgs e)
        {
            var list = new List<StudySummary>();
            list.AddRange(e.SelectedStudies);
            ShowAddAuthorityGroupDialog(list);
        }

        protected void ShowAddAuthorityGroupDialog(IList<StudySummary> studyList)
        {
            AddAuthorityGroupsDialog.Initialize(studyList);
            AddAuthorityGroupsDialog.Show();
        }
    }
}
