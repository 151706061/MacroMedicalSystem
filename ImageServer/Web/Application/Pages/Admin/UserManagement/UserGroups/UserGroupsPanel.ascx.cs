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
using AjaxControlToolkit;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{

    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js")]
    public partial class UserGroupsPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserGroupsListClientID")]
        public string UserGroupsListClientID
        {
            get { return UserGroupsGridPanel.UserGroupGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerUserGroupsSingleItemFound,
                                             SR.GridPagerUserGroupsMultipleItemsFound,
                                             UserGroupsGridPanel.UserGroupGrid, 
                                             delegate
                                                 {
                                                     return UserGroupsGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.Top);
            UserGroupsGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            UserGroupsGridPanel.DataSourceCreated += delegate(UserGroupDataSource source)
                            {
                                source.GroupName = GroupName.Text;
                            };

        }

        public void UpdateUI()
        {
            UserGroupRowData userGroupRow = UserGroupsGridPanel.SelectedUserGroup;

            if (userGroupRow == null)
            {
                // no device being selected
                EditUserGroupButton.Enabled = false;
                DeleteUserGroupButton.Enabled = false;
            }
            else
            {
                EditUserGroupButton.Enabled = true;
                DeleteUserGroupButton.Enabled = true;
            }
            
            UserGroupsGridPanel.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        protected void AddUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            ((Default)Page).OnAddUserGroup();
        }

        protected void EditUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnEditUserGroup(userGroup);
        }

        protected void DeleteUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnDeleteUserGroup(userGroup);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGroupsGridPanel.Refresh();
    	}
    }
}