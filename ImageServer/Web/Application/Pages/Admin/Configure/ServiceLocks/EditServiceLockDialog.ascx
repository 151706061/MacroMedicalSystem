<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>


<%@ Control Language="C#" AutoEventWireup="true" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.EditServiceLockDialog"
    Codebehind="EditServiceLockDialog.ascx.cs" %>
<%@ Import Namespace="Resources"%>    

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$Resources: Titles, AdminServices_EditDialogTitle%>" Width="450px">
    <ContentTemplate>
        <asp:Panel runat="server" CssClass="DialogPanelContent" style="padding: 6px;">
            <asp:Table ID="Table1" runat="server">
                <asp:TableRow>
                    <asp:TableCell Width="30%" CssClass="DialogTextBoxLabel"><%= InputLabels.ServiceDescription %></asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Description" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel"><%= InputLabels.ServiceType %>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Type" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel"><%= InputLabels.ServiceFilesystem %></asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="FileSystem" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel"><%= InputLabels.Enabled %></asp:TableCell>
                    <asp:TableCell>
                        <asp:CheckBox ID="Enabled" runat="server" CssClass="DialogCheckbox" />
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel"><%= InputLabels.ServiceSchedule %></asp:TableCell>
                    <asp:TableCell Wrap="false">
                        <asp:TextBox ID="ScheduleDate" runat="server" Width="80px" ReadOnly="true" CssClass="DialogTextBox"></asp:TextBox>
                        <asp:Button ID="DatePickerButton" runat="server" Text="..."/>
                        <ccUI:CalendarExtender ID="CalendarExtender" runat="server" 
                                    TargetControlID="ScheduleDate" PopupButtonID="DatePickerButton" CssClass="Calendar" >
                        </ccUI:CalendarExtender>&nbsp;
                        <asp:TextBox ID="ScheduleTime" runat="server" CssClass="DialogTextBox"  ValidationGroup="AddEditServiceLockValidationGroup"/>
                        <aspAjax:MaskedEditExtender runat="server" ID="ScheduleTimeMaskedEditExtender" MaskType="Time" AcceptAMPM="false" TargetControlID="ScheduleTime" Mask="99:99" MessageValidatorTip="false" OnInvalidCssClass="InvalidTextEntered"/>
                        <aspAjax:MaskedEditValidator runat="server" ID="ScheduleTimeMaskedEditValidator" ControlExtender="ScheduleTimeMaskedEditExtender" ControlToValidate="ScheduleTime" ValidationExpression="(0[0-9]*|1[0-9]*|2[0-3]):[0-5][0-9]" ValidationGroup="AddEditServiceLockValidationGroup"  />
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow><asp:TableCell><asp:image runat="server" SkinID="Spacer" height="3" /></asp:TableCell></asp:TableRow>
            </asp:Table>
</asp:Panel>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:ApplyButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditServiceLockValidationGroup" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</ccAsp:ModalDialog>

<ccAsp:MessageBox ID="ErrorMessageBox" runat="server" />