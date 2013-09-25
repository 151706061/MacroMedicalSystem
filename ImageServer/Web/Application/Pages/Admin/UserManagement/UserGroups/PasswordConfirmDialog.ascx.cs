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

namespace Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class PasswordConfirmDialog : System.Web.UI.UserControl
    {

        public delegate void OnOKClickedEventHandler();
        public event OnOKClickedEventHandler OKClicked;

        public string PasswordString
        {
            get
            {
                return Password.Text;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void OKButton_Click(object sender, ImageClickEventArgs e)
        {
            if (OKClicked != null)
                OKClicked();

            Close();
        }

        protected void CancelButton_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }


        public void Show()
        {
            ModalDialog1.Show();
        }


        public void Close()
        {
            ModalDialog1.Hide();
        }
    }
}