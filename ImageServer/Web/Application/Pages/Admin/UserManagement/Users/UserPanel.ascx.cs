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

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel", ResourcePath = "Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js")]
    public partial class UserPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetPasswordButtonClientID")]
        public string ResetPasswordButtonClientID
        {
            get { return ResetPasswordButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserListClientID")]
        public string UserListClientID
        {
            get { return UserGridPanel.UserGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerUserSingleItemFound,
                                             SR.GridPagerUserMultipleItemsFound,
                                             UserGridPanel.UserGrid, 
                                             delegate
                                                 {
                                                     return UserGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.Top);
            UserGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            UserGridPanel.DataSourceCreated += delegate(UserDataSource source)
                            {
                                source.UserName = UserNameTextBox.Text;
                                source.DisplayName = DisplayNameTextBox.Text;
                            };

            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        public void UpdateUI()
        {
            UserRowData userRow = UserGridPanel.SelectedUser;

            if (userRow == null)
            {
                // no device being selected
                EditUserButton.Enabled = false;
                DeleteUserButton.Enabled = false;
                ResetPasswordButton.Enabled = false;
            }
            else
            {
                EditUserButton.Enabled = true;
                DeleteUserButton.Enabled = true;
                ResetPasswordButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            UserGridPanel.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        protected void AddUserButton_Click(object sender, ImageClickEventArgs e)
        {             
            ((Default)Page).OnAddUser();
        }

        protected void EditUserButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnEditUser(user);
        }

        protected void DeleteUserButton_Click(object sender, ImageClickEventArgs e)
        {            
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnDeleteUser(user);
        }

        protected void ResetPasswordButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnResetPassword(user);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGridPanel.Refresh();
    	}
    }
}