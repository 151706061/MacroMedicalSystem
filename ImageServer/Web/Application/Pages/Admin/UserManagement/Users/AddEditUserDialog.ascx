<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.AddEditUserDialog"
            Codebehind="AddEditUserDialog.ascx.cs" %>

<script type="text/javascript">
function ValidationUsernameParams() {
    params = new Array();
    input = document.getElementById('<%=UserLoginId.ClientID%>');
    params.username = input.value;


    input = document.getElementById('<%= OriginalUserLoginId.ClientID%>');
    params.originalUsername = input.value;
    return params;
}
</script>    

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="450px">
    <ContentTemplate>
    
        <div class="DialogPanelContent">
    
            <asp:Table runat="server" skinID="NoSkin" CellSpacing="3" CellPadding="3">
       
                <asp:TableRow runat="server" ID="UserNameRow">
                    <asp:TableCell runat="server"><asp:Label ID="Label2" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_UserID %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="UserLoginId" CssClass="DialogTextBox" Width="100%"></asp:TextBox><asp:HiddenField ID="OriginalUserLoginId" runat="server" /></asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserLoginHelpId" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ConditionalRequiredFieldValidator ID="UserNameRequiredFieldValidator" runat="server"
                                                                       ControlToValidate="UserLoginId" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                       InvalidInputIndicatorID="UserLoginHelpId" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                                       RequiredWhenChecked="False"/>
                        <ccValidator:DuplicateUsernameValidator ID="DuplicateUserNameValidator" runat="server"
                                                                ControlToValidate="UserLoginId" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                InvalidInputIndicatorID="UserLoginHelpId" Text="<%$Resources: InputValidation, UserIDAlreadyExists %>" Display="None"
                                                                ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateUsername"
                                                                ParamsFunction="ValidationUsernameParams"/>                                                        
                    </asp:TableCell>
                </asp:TableRow>
                   
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel" Wrap="false"><asp:Label ID="Label1" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_Name %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="DisplayName" CssClass="DialogTextBox" Width="100%"></asp:TextBox></asp:TableCell>
                    <asp:TableCell HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserDisplayNameHelp" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1" runat="server"
                                                                       ControlToValidate="DisplayName" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditUserValidationGroup"
                                                                       InvalidInputIndicatorID="UserDisplayNameHelp" Text="<%$Resources: InputValidation, ThisFieldIsRequired %>" Display="None"
                                                                       RequiredWhenChecked="False"/>                                                       
                    </asp:TableCell>
                </asp:TableRow>
                 
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel" Wrap="false"><asp:Label runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_EmailAddress %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell><asp:TextBox runat="server" ID="EmailAddressId" CssClass="DialogTextBox" Width="100%"></asp:TextBox></asp:TableCell>
                    <asp:TableCell HorizontalAlign="left">
                        <ccAsp:InvalidInputIndicator ID="UserEmailAddressHelp" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:RegularExpressionFieldValidator ID="UserEmailAddressValidator"
                                                                     runat="server" ControlToValidate="EmailAddressId" InvalidInputCSS="DialogTextBoxInvalidInput"
                                                                     IgnoreEmptyValue="true" ValidationGroup="AddEditUserValidationGroup"
                                                                     InvalidInputIndicatorID="UserEmailAddressHelp" ValidationExpression="^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$"
                                                                     Text="<%$Resources: InputValidation,EmailAddressInvalid %>"
                                                                     Display="None" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="EnabledRow"><asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label4" runat="server" Text="Enabled" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell ColumnSpan="2"><asp:CheckBox runat="server" ID="UserEnabledCheckbox" /></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, AdminUsers_AddEditDialog_Groups %>" CssClass="DialogTextBoxLabel" /></asp:TableCell>
                    <asp:TableCell ColumnSpan="2" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList runat="server" ID="UserGroupListBox" Width="100%" />
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    
        </div>    
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" ValidationGroup="AddEditUserValidationGroup" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" ValidationGroup="AddEditUserValidationGroup" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
