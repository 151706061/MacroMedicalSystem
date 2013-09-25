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
using Macro.Common.Utilities;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.WebControls.Validators;
using Macro.Web.Enterprise.Admin;
using Resources;


namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditUserDialog : UserControl
    {
        #region private variables

        private bool _editMode;
        // user being editted/added
        private UserRowData _user;

        #endregion

        #region public members

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
        /// Sets/Gets the current editing user.
        /// </summary>
        public UserRowData User
        {
            set
            {
                _user = value;
                // put into viewstate to retrieve later
                ViewState[ "EditedUser"] = _user;
            }
            get { return _user; }
        }

        public ConditionalRequiredFieldValidator UserNameValidator
        {
            get { return UserNameRequiredFieldValidator; }
        }

        public InvalidInputIndicator UsernameIndicator
        {
            get { return UserLoginHelpId;  }
        }

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="user">The user being added.</param>
        public delegate bool OnOKClickedEventHandler(UserRowData user);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                using (var service = new AuthorityManagement())
                 {
                    IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();
                    IList<ListItem> items = CollectionUtils.Map(
                        list,
                        delegate(AuthorityGroupSummary summary)
                        {
                            return new ListItem(summary.Name, summary.AuthorityGroupRef.Serialize());
                        }
                        );
                    UserGroupListBox.Items.AddRange(CollectionUtils.ToArray(items));
                };
            }
            else
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedUser"] != null)
                    _user = ViewState[ "EditedUser"] as UserRowData;
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

                bool success = false;

                if (OKClicked != null)
                    success = OKClicked(User);

                if (!success)
                {
                    Show(false);
                } else
                {
                    Close();
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
        
        private void SaveData()
        {
            if (User == null)
            {
                User = new UserRowData();
            }
            
            if(EditMode)
            {
                User.Enabled = UserEnabledCheckbox.Checked;
            } 
            else
            {
                User.UserName = UserLoginId.Text;
                User.Enabled = true;
            }

            User.DisplayName = DisplayName.Text;
            User.EmailAddress = EmailAddressId.Text;

            User.UserGroups.Clear();
            foreach (ListItem item in UserGroupListBox.Items)
            {
                if (item.Selected)
                {
                    User.UserGroups.Add(new UserGroup(
                        item.Value, item.Text));
                }
            }
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            if (EditMode)
            {
                ModalDialog1.Title = SR.DialogEditUserTitle;
                OKButton.Visible = false;
                UpdateButton.Visible = true;

                UserLoginId.ReadOnly = true;
                UserLoginId.Style.Add("Color", "#999999");
                EnabledRow.Visible = true;
                UserLoginId.Text = User.UserName;
                OriginalUserLoginId.Value = User.UserName;
                DisplayName.Text = User.DisplayName;
                UserEnabledCheckbox.Checked = User.Enabled;
                EmailAddressId.Text = User.EmailAddress;

                List<UserGroup> groups = User.UserGroups;

                UserGroupListBox.ClearSelection();
                foreach (UserGroup group in groups)
                {
                    UserGroupListBox.Items.FindByText(group.Name).Selected = true;
                }
            }
            else
            {
                ModalDialog1.Title = SR.DialogAddUserTitle;
                OKButton.Visible = true;
                UpdateButton.Visible = false;
                
                EnabledRow.Visible = false;
                UserLoginId.ReadOnly = false;
                UserLoginId.Style.Add("Color", "#000000");
                UserGroupListBox.ClearSelection();
            }

            // Update the rest of the fields
            if (User == null || EditMode == false)
            {
                UserLoginId.Text = string.Empty;
                OriginalUserLoginId.Value = string.Empty;
                DisplayName.Text = string.Empty;
                EmailAddressId.Text = string.Empty; 
                UserEnabledCheckbox.Checked = false;                
                UserGroupListBox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            ModalDialog1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
