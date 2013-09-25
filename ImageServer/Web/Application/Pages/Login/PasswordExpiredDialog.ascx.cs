#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.Security;
using System.Web.UI;
using Macro.Common;
using Macro.ImageServer.Web.Common.Security;
using Macro.Web.Enterprise.Authentication;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Login
{
    public partial class PasswordExpiredDialog : UserControl
    {
        public void Show(string username, string password)
        {
            Username.Text = username;
            OriginalPassword.Value = password;
            ErrorMessagePanel.Visible = false;
            Panel1.DefaultButton = "OKButton";
            NewPassword.Focus();
            ModalDialog1.Show();
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            Panel1.DefaultButton = "";
            ModalDialog1.Hide();
        }

        public void ChangePassword_Click(object sender, EventArgs e)
        {
            using(LoginService service = new LoginService())
            {
                try
                {
                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text = ErrorMessages.PasswordsDontMatch;
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(Username.Text, OriginalPassword.Value, NewPassword.Text);
                        SessionManager.InitializeSession(Username.Text, NewPassword.Text);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.Text = ex.Message;
                    ErrorMessagePanel.Visible = true;
                    NewPassword.Focus();
					// May want to elimiate this.
					Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
				}
            }
        }
    }
}