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
using Resources;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.StudyDeleteHistory.Search)]
    public partial class Default : BaseAdminPage
    {
        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            SearchPanel.ViewDetailsClicked += SearchPanel_ViewDetailsClicked;
            SearchPanel.DeleteClicked += SearchPanel_DeleteClicked;
            DeleteConfirmMessageBox.Confirmed += DeleteConfirmMessageBox_Confirmed;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetPageTitle(Titles.DeletedStudiesPageTitle);

            if (Page.IsPostBack)
            {
            	// Reload the data on post-back
            	// Note: databinding also happens on initial rendering because the grid pager 
            	// does so on Page_Load.
                DataBind();
            }
        }

        #endregion

        #region Private Methods

        private void DeleteConfirmMessageBox_Confirmed(object data)
        {
            try
            {
                var record = data as ServerEntityKey;
                var controller = new DeletedStudyController();
                controller.Delete(record);
            }
            finally
            {
                SearchPanel.Refresh();
            }
        }

        private void SearchPanel_DeleteClicked(object sender, DeletedStudyDeleteClickedEventArgs e)
        {
            DeleteConfirmMessageBox.Data = e.SelectedItem.DeleteStudyRecord;
            DeleteConfirmMessageBox.Show();
        }

        private void SearchPanel_ViewDetailsClicked(object sender, DeletedStudyViewDetailsClickedEventArgs e)
        {
            var dialogViewModel = new DeletedStudyDetailsDialogViewModel {DeletedStudyRecord = e.DeletedStudyInfo};
            DetailsDialog.ViewModel = dialogViewModel;
            DetailsDialog.Show();
        }

        #endregion
    }
}