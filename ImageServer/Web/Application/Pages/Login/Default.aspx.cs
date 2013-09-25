#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Xml;
using Macro.Common;
using Macro.Dicom.Audit;
using Macro.Enterprise.Common;
using Macro.ImageServer.Common;
using Macro.ImageServer.Web.Application.Pages.Common;
using Macro.ImageServer.Web.Common.Security;
using SR = Resources.SR;
using Resources;
using Macro.ImageServer.Web.Common.Extensions;
using System.Web.UI;

namespace Macro.ImageServer.Web.Application.Pages.Login
{
    [ExtensibleAttribute(ExtensionPoint=typeof(LoginPageExtensionPoint))]
    public partial class LoginPage : BasePage, ILoginPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ForeachExtension<ILoginPageExtension>(ext => ext.OnLoginPageInit(this));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionManager.Current != null)
            {
                // already logged in. Maybe from a different page
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(SessionManager.Current.Credentials.UserName, false), true);
            } 
            
            if (!ServerPlatform.IsManifestVerified)
            {
                ManifestWarningTextLabel.Text = SR.NonStandardInstallation;
            }

            VersionLabel.Text = String.IsNullOrEmpty(ServerPlatform.VersionString) ? Resources.SR.Unknown : ServerPlatform.VersionString;
            LanguageLabel.Text = Thread.CurrentThread.CurrentUICulture.NativeName;
            CopyrightLabel.Text = ProductInformation.Copyright;

            DataBind();

            SetPageTitle(Titles.LoginPageTitle);

            UserName.Focus();
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            if (SessionManager.Current != null)
            {
                // already logged in. Maybe from different page
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(SessionManager.Current.Credentials.UserName, false), true);
            } 

            try
            {
                SessionManager.InitializeSession(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, SessionManager.Current.Credentials.DisplayName));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (PasswordExpiredException)
            {
                Platform.Log(LogLevel.Info, "Password for {0} has expired. Requesting new password.",UserName.Text);
                PasswordExpiredDialog.Show(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (UserAccessDeniedException ex)
            {
                Platform.Log(LogLevel.Error, ex, ex.Message);
                ShowError(ErrorMessages.UserAccessDenied);
                UserName.Focus();

                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
                    EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, UserAuthenticationEventType.Login);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
                ServerPlatform.LogAuditMessage(audit);
            }
            catch (CommunicationException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unable to contact A/A server");
                ShowError(ErrorMessages.CannotContactEnterpriseServer);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Login error:");
                ShowError(ex.Message);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
        }

        public void ChangePassword(object sender, EventArgs e)
        {
            ChangePasswordDialog.Show(true);
        }

        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
        }

        public Control SplashScreenControl { get { return this.LoginSplash; } }
    }
}
