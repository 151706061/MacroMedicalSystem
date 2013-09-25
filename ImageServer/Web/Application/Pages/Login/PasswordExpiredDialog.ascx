<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>



<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PasswordExpiredDialog.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Login.PasswordExpiredDialog" %>
<%@ Import Namespace="Resources"%>



<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="500px" Title="<%$Resources: Titles, PasswordExpiredDialogTitle %>">
    <ContentTemplate>
    
    <asp:Panel runat="server" Visible="false" ID="ErrorMessagePanel" CssClass="ErrorMessage" style="margin-bottom: 10px;">
        <asp:Label runat="server" ID="ErrorMessage" ></asp:Label>
    </asp:Panel>
    
    <asp:Panel ID="Panel1" runat="server" width="100%" CssClass="DialogPanelContent">

        <asp:Panel runat="server" CssClass="PasswordExpiredMessage">
            <asp:Label runat="server" ID="Label1">
                <%= SR.YourPasswordHasExpired %>
            </asp:Label>
    
        <table style="margin-top: 10px; margin-bottom: 10px;">
        <tr><td class="ChangePasswordLabel"><%= Labels.UserID %>:</td><td><asp:TextBox runat="server" Width="150px" ID="Username"/></td></tr>
        <tr><td class="ChangePasswordLabel"><%= Labels.NewPassword %>:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="NewPassword"/></td></tr>
        <tr><td class="ChangePasswordLabel"><%= Labels.RetypeNewPassword %>:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="ConfirmNewPassword"/></td></tr>
        </table>
        
        <input type="hidden" runat="server" id="OriginalPassword" />
           
        </asp:Panel>
    
        <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="ChangePassword_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </asp:Panel>
    </ContentTemplate>
</ccAsp:ModalDialog>


