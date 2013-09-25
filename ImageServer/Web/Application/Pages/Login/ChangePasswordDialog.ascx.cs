#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using Macro.Common;
using Macro.Enterprise.Common;
using Macro.Web.Enterprise.Authentication;
using Resources;
using Macro.ImageServer.Web.Common.Security;

namespace Macro.ImageServer.Web.Application.Pages.Login
{
    public partial class ChangePasswordDialog : UserControl
    {
        public void Show(bool updateUI)
        {
            ChangePasswordUsername.Text = ((System.Web.UI.WebControls.TextBox)Page.FindControl("UserName")).Text;
            LoginPasswordChange.Checked = true;
            ErrorMessage.Text = String.Empty;
            ErrorMessagePanel.Visible = false;
            Panel1.DefaultButton = "OKButton";
            ChangePasswordUsername.Focus();
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
                    SessionInfo session = service.Login(ChangePasswordUsername.Text, OriginalPassword.Text, ImageServerConstants.DefaultApplicationName);

                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text = ErrorMessages.PasswordsDoNotMatch;
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(ChangePasswordUsername.Text, OriginalPassword.Text,NewPassword.Text);

                        session = service.Login(ChangePasswordUsername.Text, NewPassword.Text, ImageServerConstants.DefaultApplicationName);
                        SessionManager.InitializeSession(session);

                        if (LoginPasswordChange.Checked)
                        {
                            Response.Redirect(
                                FormsAuthentication.GetRedirectUrl(ChangePasswordUsername.Text, false), false);
                        }
                        else
                        {
                            ModalDialog1.Hide();
                        }
                    }
                }
                catch(ArgumentException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    string error = String.Format(ErrorMessages.ChangePasswordError, ex.Message);
                    ShowError(error);
                }
                catch (PasswordExpiredException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    ShowError(ErrorMessages.PasswordExpired);
                }
                catch (UserAccessDeniedException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    ShowError(ErrorMessages.UserAccessDenied);
                }
                catch (RequestValidationException ex)
                {
                    // NOTE: The server is throwing FaultException<RequestValidationException> when username or password doesn't match the configured policy
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    
                    string error = String.Format(ErrorMessages.PasswordPolicyNotMet);
                    ShowError(error);
                }
                catch (CommunicationException ex)
                {
                    Platform.Log(LogLevel.Error, ex, ErrorMessages.CannotContactEnterpriseServer);
                    ShowError(ErrorMessages.CannotContactEnterpriseServer);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    // May want to elimiate this.
                    Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
                }
            }
        }

        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
            ChangePasswordUsername.Focus();
        }
    }
}