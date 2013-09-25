<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSettingsPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueSettingsPanel" %>

<asp:UpdatePanel ID="WorkQueueSettingsUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="100%">
        <tr>
            <td valign="bottom" align="left" width="75%">
                <asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels, ScheduleWorkQueueDialog_Priority %>" CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="DialogDropDownList"></asp:DropDownList>
            </td>
            <td valign="bottom">
                            <asp:Label ID="Label1" runat="server" Text="<%$Resources: Labels, ScheduleWorkQueueDialog_Schedule %>" CssClass="DialogTextBoxLabel"></asp:Label>
            </td>
            <td valign="bottom" align="left">
                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SR,Date %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
                <asp:TextBox ID="NewScheduleDate" runat="server" Width="80px" CssClass="DialogTextBox"></asp:TextBox>
                <ccUI:CalendarExtender ID="CalendarExtender" runat="server" TargetControlID="NewScheduleDate" CssClass="Calendar">
                </ccUI:CalendarExtender>
            </td>
            <td valign="bottom" align="left">
                <asp:Label ID="Label5" runat="server" Text="<%$Resources: SR,Time %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
                <asp:TextBox ID="NewScheduleTime" runat="server" CssClass="DialogTextBox"  ValidationGroup="WorkQueueSettingsValidationGroup"/>
                <aspAjax:MaskedEditExtender runat="server" ID="NewScheduleTimeMaskedEditExtender" MaskType="Time" AcceptAMPM="false" TargetControlID="NewScheduleTime" Mask="99:99" MessageValidatorTip="false" OnInvalidCssClass="InvalidTextEntered" />
                <aspAjax:MaskedEditValidator runat="server" ID="NewScheduleTimeMaskedEditValidator" ControlExtender="NewScheduleTimeMaskedEditExtender" ControlToValidate="NewScheduleTime" ValidationExpression="(0[0-9]*|1[0-9]*|2[0-3]):[0-5][0-9]" ValidationGroup="WorkQueueSettingsValidationGroup"  />
            </td>
            <td valign="bottom" nowrap="nowrap" style="padding-left: 10px; padding-right: 10px;">
                <asp:Label ID="Label3" runat="server" Text="<%$Resources: Labels, ScheduleWorkQueueDialog_ScheduleNow %>" CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:CheckBox runat="server" ID="ScheduleNowCheckBox" OnCheckedChanged="ScheduleNow_CheckChanged" Checked="false" AutoPostBack="true"/>
            </td>            
        </tr>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox ID="ErrorMessageBox" runat="server" />


