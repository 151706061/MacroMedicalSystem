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
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.Common.Utilities;
using Macro.Enterprise.Common;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Services.Common;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.Web.Enterprise.Admin;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditUserGroupsDialog : UserControl
    {
        #region private variables

        private bool _editMode;
        private bool _saveDataGroup;

        // user being editted/added
        private UserGroupRowData _userGroup;
     
        #endregion

        #region public members
        public bool SaveDataGroup
        {
            get { return _saveDataGroup; }
            set
            {
                _saveDataGroup = value;
                ViewState["SaveDataGroup"] = value;
            }
        }
        
        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing user group.
        /// </summary>
        public UserGroupRowData UserGroup
        {
            set
            {
                _userGroup = value;
                SaveDataGroup = value.DataGroup;
                // put into viewstate to retrieve later
                ViewState[ "EditedUserGroup"] = _userGroup;
            }
            get { return _userGroup; }
        }

        #endregion // public members

        #region Events

        public delegate bool OnOKClickedEventHandler(UserGroupRowData user);
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Private Methods

        private void SaveData()
        {
            if (UserGroup == null)
            {
                UserGroup = new UserGroupRowData();
            }

            UserGroup.Name = GroupName.Text;
            UserGroup.Description = GroupDescription.Text;
            UserGroup.DataGroup = DataGroupCheckBox.Checked;
            UserGroup.Tokens.Clear();
            foreach (ListItem item in TokenCheckBoxList.Items)
            {
                if (item.Selected)
                {
                    UserGroup.Tokens.Add(new TokenSummary(item.Value, item.Text));
                }
            }
        }

        #endregion

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PasswordConfirmDialog.OKClicked += PasswordConfirmDialog_OKClicked;

            if (Page.IsPostBack == false)
            {

                using (AuthorityManagement service = new AuthorityManagement())
                {
                    IList<AuthorityTokenSummary> tokens = service.ListAuthorityTokens();
                    IList<ListItem> items = CollectionUtils.Map<AuthorityTokenSummary, ListItem>(
                                            tokens,
                                            delegate(AuthorityTokenSummary token)
                                            {
                                                return new ListItem(token.Description, token.Name);
                                            });

                    TokenCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
                };
            }
            else
            {
                if (ViewState["SaveDataGroup"] != null)
                    _saveDataGroup = (bool)ViewState["SaveDataGroup"];

                if (ViewState["EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedUserGroup"] != null)
                    _userGroup = ViewState[ "EditedUserGroup"] as UserGroupRowData;
            }
        }

        private void PasswordConfirmDialog_OKClicked()
        {
            bool success = false;

            UserGroup.Password = PasswordConfirmDialog.PasswordString;
            if (OKClicked != null)
            {
                try
                {
                    success = OKClicked(UserGroup);
                }
                catch (FaultException<UserAccessDeniedException>)
                {
                    PasswordFailErrorMessage.Message = SR.AddEditUserGroupsDialog_InvalidPassword;
                    PasswordFailErrorMessage.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    PasswordFailErrorMessage.Show();
                }
            }
            if (!success)
            {
                Show(false);
            }
            else
            {
                Close();
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {            
            if (Page.IsValid)
            {                 
                SaveData();

                // Only ask for the password if updating 
                if (EditMode && SaveDataGroup && !UserGroup.DataGroup)
                {
                    PasswordConfirmDialog.Show();
                }
                else
                {                    
                    bool success = false;

                    if (OKClicked != null)
                        success = OKClicked(UserGroup);

                    if (!success)
                    {
                        Show(false);
                    }
                    else
                    {
                        Close();
                    }
                }
            }
            else
            {
                Show(false);
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            if (EditMode)
            {
                ModalDialog1.Title = SR.DialogEditUserGroupTitle;
                UpdateButton.Visible = true;
                OKButton.Visible = false;
                GroupName.Text = UserGroup.Name;
                GroupDescription.Text = UserGroup.Description;
                DataGroupCheckBox.Checked = UserGroup.DataGroup;
                OriginalGroupName.Value = UserGroup.Name;
                TokenCheckBoxList.ClearSelection();
                foreach (TokenSummary token in UserGroup.Tokens)
                {
                    TokenCheckBoxList.Items.FindByValue(token.Name).Selected = true;
                }
            }
            else
            {
                TokenCheckBoxList.ClearSelection();
                ModalDialog1.Title = SR.DialogAddUserGroupTitle;
                DataGroupCheckBox.Checked = false;
                UpdateButton.Visible = false;
                OKButton.Visible = true;
            }

            // Update the rest of the fields
            if (UserGroup == null || EditMode == false) 
            {
                GroupName.Text = string.Empty;
                GroupDescription.Text = string.Empty;
                TokenCheckBoxList.SelectedIndex = -1;
            }
        }

        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            ModalDialog1.Show();
        }

        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
