<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>


<%@ Control Language="C#" AutoEventWireup="True" Codebehind="PersonNameInputPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Controls.PersonNameInputPanel" %>
<%@ Register Src="~/Controls/InvalidInputIndicator.ascx" TagName="InvalidInputIndicator"
    TagPrefix="uc" %>
<%@ Import Namespace="Resources" %>

<asp:Table runat="server" ID="table">
    <asp:TableRow runat="server" ID="SingleByteRow" VerticalAlign="Bottom">
        <asp:TableCell Wrap="false" Visible="false">
            <asp:Label ID="SingleByteNameLabel" runat="server" CssClass="DialogTextBoxLabel"
                Text="Single Byte" />
        </asp:TableCell>
        <asp:TableCell Wrap="false">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="<%$ Resources: Labels, PersonNamePanel_Title %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <uc:InvalidInputIndicator ID="PersonTitleIndicator" runat="server" SkinID="InvalidInputIndicator" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="PersonTitle" runat="server" MaxLength="64" CssClass="DialogTextBox"
                            CausesValidation="true" />
                    </td>
                </tr>
            </table>
        </asp:TableCell>
        <asp:TableCell Wrap="false">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" Text="<%$ Resources: Labels, PersonNamePanel_GivenName %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <uc:InvalidInputIndicator ID="PersonGivenNameIndicator" runat="server" SkinID="InvalidInputIndicator" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="PersonGivenName" runat="server" CausesValidation="true" MaxLength="64" CssClass="DialogTextBox" />
                    </td>
                </tr>
            </table>
        </asp:TableCell>
        <asp:TableCell Wrap="false">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="<%$ Resources: Labels, PersonNamePanel_MiddleName %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <uc:InvalidInputIndicator ID="PersonMiddleNameIndicator" runat="server" SkinID="InvalidInputIndicator" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="PersonMiddleName" runat="server" CausesValidation="true" 
                            MaxLength="64" CssClass="DialogTextBox" />
                    </td>
                </tr>
            </table>
        </asp:TableCell>
        <asp:TableCell Wrap="false">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="<%$ Resources: Labels, PersonNamePanel_LastName %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <uc:InvalidInputIndicator ID="PersonLastNameIndicator" runat="server" SkinID="InvalidInputIndicator" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="PersonLastName" runat="server" CausesValidation="true"
                            MaxLength="64" CssClass="DialogTextBox" />
                    </td>
                </tr>
            </table>
        </asp:TableCell>
        <asp:TableCell Wrap="false">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label15" runat="server" Text="<%$ Resources: Labels, PersonNamePanel_Suffix %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <uc:InvalidInputIndicator ID="PersonSuffixIndicator" runat="server" SkinID="InvalidInputIndicator" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="PersonSuffix" runat="server" CausesValidation="true"
                            MaxLength="64" CssClass="DialogTextBox" />
                    </td>
                </tr>
            </table>
        </asp:TableCell>
        <asp:TableCell>
            <asp:Button ID="ShowOtherNameFormatButton" UseSubmitBehavior="false" 
                    runat="server" Text="..." 
                    ToolTip="<%$ Resources: Tooltips, ShowOtherNameFormats %>"/>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server" ID="PhoneticRow" VerticalAlign="Bottom">
        <asp:TableCell Wrap="false" Visible="false">
            <asp:Label ID="PhoneticGroupLabel" runat="server" CssClass="DialogTextBoxLabel" Text="Phonetic:" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="PhoneticTitle" runat="server" MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="PhoneticGivenName" runat="server" CausesValidation="true" 
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="PhoneticMiddleName" runat="server" CausesValidation="true"
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="PhoneticLastName" runat="server" CausesValidation="true"
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="PhoneticSuffix" runat="server" CausesValidation="true" 
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:Image runat="server" ID="PhoneticNameRowIndicator" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server" ID="IdeographicRow" VerticalAlign="Bottom">
        <asp:TableCell Wrap="false" Visible="false">
            <asp:Label ID="IdeographicGroupLabel" runat="server" CssClass="DialogTextBoxLabel"
                Text="Ideographic:" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="IdeographicTitle" runat="server" MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="IdeographicGivenName" runat="server" CausesValidation="true" 
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="IdeographicMiddleName" runat="server" CausesValidation="true" 
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="IdeographicLastName" runat="server" CausesValidation="true" 
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:TextBox ID="IdeographicSuffix" runat="server" CausesValidation="true"
                MaxLength="64" CssClass="DialogTextBox" />
        </asp:TableCell>
        <asp:TableCell>
            <asp:Image runat="server" ID="IdeographyNameIndicator" /></asp:TableCell>
    </asp:TableRow>
</asp:Table>
<ccValidator:RegularExpressionFieldValidator ID="PersonTitleValidator"
    runat="server" ControlToValidate="PersonTitle" InvalidInputCSS="DialogTextBoxInvalidInput"
    IgnoreEmptyValue="true"
    InvalidInputIndicatorID="PersonTitleIndicator" ValidationExpression="^([^\\]){1,64}$"
    Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:RegularExpressionFieldValidator>
<ccValidator:RegularExpressionFieldValidator ID="PersonGivenNameValidator"
    runat="server" ControlToValidate="PersonGivenName" InvalidInputCSS="DialogTextBoxInvalidInput"
    InvalidInputIndicatorID="PersonGivenNameIndicator"
    ValidationExpression="^([^\\]){1,64}$" Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:RegularExpressionFieldValidator>
<ccValidator:RegularExpressionFieldValidator ID="PersonMiddleNameValidator"
    runat="server" ControlToValidate="PersonMiddleName" InvalidInputCSS="DialogTextBoxInvalidInput"
    IgnoreEmptyValue="true" InvalidInputIndicatorID="PersonMiddleNameIndicator" ValidationExpression="^([^\\]){0,64}$"
    Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:RegularExpressionFieldValidator>
<ccValidator:RegularExpressionFieldValidator ID="PersonLastNameValidator"
    runat="server" ControlToValidate="PersonLastName" InvalidInputCSS="DialogTextBoxInvalidInput"
    InvalidInputIndicatorID="PersonLastNameIndicator"
    ValidationExpression="^([^\\]){0,64}$" Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:RegularExpressionFieldValidator>
<ccValidator:RegularExpressionFieldValidator ID="PersonSuffixValidator"
    runat="server" ControlToValidate="PersonSuffix" InvalidInputCSS="DialogTextBoxInvalidInput"
    IgnoreEmptyValue="true" InvalidInputIndicatorID="PersonSuffixIndicator" ValidationExpression="^([^\\]){0,64}$"
    Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:RegularExpressionFieldValidator>
<ccValidator:ConditionalRequiredFieldValidator ID="PersonGivenNameRequiredValidator"
    runat="server" ControlToValidate="PersonGivenName" InvalidInputCSS="DialogTextBoxInvalidInput"
    IgnoreEmptyValue="true" InvalidInputIndicatorID="PersonSuffixIndicator" ValidationExpression="^([^\\]){0,64}$"
    Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:ConditionalRequiredFieldValidator>
<ccValidator:ConditionalRequiredFieldValidator ID="PersonLastNameRequiredValidator"
    runat="server" ControlToValidate="PersonLastName" InvalidInputCSS="DialogTextBoxInvalidInput"
    IgnoreEmptyValue="true" InvalidInputIndicatorID="PersonSuffixIndicator" ValidationExpression="^([^\\]){0,64}$"
    Text="<%$Resources: InputValidation,PersonNameInputPanel_InvalidName %>"
    Display="None">
</ccValidator:ConditionalRequiredFieldValidator>
