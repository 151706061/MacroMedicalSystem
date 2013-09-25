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
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using GridView=Macro.ImageServer.Web.Common.WebControls.UI.GridView;

namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class UserGroupsGridPanel : GridViewPanel
    {
        #region Delegates
        public delegate void UserGroupDataSourceCreated(UserGroupDataSource theSource);
        public event UserGroupDataSourceCreated DataSourceCreated;

        /// <summary>
        /// Defines the handler for <seealso cref="OnUserGroupSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedUserGroup"></param>
        public delegate void UserGroupSelectedEventHandler(object sender, UserGroupRowData selectedUserGroup);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event UserGroupSelectedEventHandler OnUserGroupSelectionChanged;

        #endregion

        #region Private members
        // list of studies to display
        private UserGroupDataSource _dataSource;
        private IList<UserGroupRowData> _userGroupRows;
        #endregion Private members

        
        
        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView UserGroupGrid
        {
            get { return UserGroupsGridView; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new UserGroupDataSource();

                    _dataSource.UserGroupFoundSet += delegate(IList<UserGroupRowData> newlist)
                                            {
                                                _userGroupRows = newlist;
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
            }
        }

        /// <summary>
        /// Gets/Sets the list of users rendered on the screen.
        /// </summary>
        public IList<UserGroupRowData> UserGroups
        {
            get { return _userGroupRows; }
            set
            {
                _userGroupRows = value;
                UserGroupGrid.DataSource = _userGroupRows; // must manually call DataBind() later
            }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public UserGroupRowData SelectedUserGroup
        {
            get
            {
                if(!UserGroupGrid.IsDataBound) UserGroupGrid.DataBind();

                if (UserGroups.Count == 0 || UserGroupGrid.SelectedIndex < 0)
                    return null;

                int index = UserGroupGrid.SelectedIndex;

                if (index < 0 || index > UserGroups.Count - 1)
                    return null;

                return UserGroups[index];
            }
            set
            {
                UserGroupGrid.SelectedIndex = UserGroups.IndexOf(value);
                if (OnUserGroupSelectionChanged != null)
                    OnUserGroupSelectionChanged(this, value);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = UserGroupGrid;
            UserGroupGrid.DataSource = UserGroupDataSourceObject;
        }

        protected void UserGroupsGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void UserGroupsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UserGroupsGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void UserGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (UserGroupGrid.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {                    
                    CustomizeTokensColumn(e);
                    CustomizeDataGroupColumn(e);
                }
            }
        }

        private static void CustomizeTokensColumn(GridViewRowEventArgs e)
        {
            TextBox textBox = ((TextBox)e.Row.FindControl("TokensTextBox"));
            UserGroupRowData rowData = e.Row.DataItem as UserGroupRowData;

            if (rowData != null)
            {
                string tokenList = string.Empty;
                foreach (TokenSummary token in rowData.Tokens)
                {
                    tokenList += token.Description + "\n";
                }
                textBox.Text = tokenList;
            }
        }

        protected void CustomizeDataGroupColumn(GridViewRowEventArgs e)
        {
            var img = ((Image)e.Row.FindControl("DataGroupImage"));
            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "DataGroup"));
                if (active)
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Checked;
                }
                else
                {
                    img.ImageUrl = ImageServerConstants.ImageURLs.Unchecked;
                }
            }
        }

        protected void DisposeUserGroupsDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetUserGroupDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new UserGroupDataSource();

                _dataSource.UserGroupFoundSet += delegate(IList<UserGroupRowData> newlist)
                                        {
                                            _userGroupRows = newlist;
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }
    }
}
