<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MessageBox.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Controls.MessageBox" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="localAsp" %>

<localAsp:ModalDialog ID="ModalDialog" runat="server" Title="" >
    <ContentTemplate>
        <asp:ScriptManagerProxy ID="DialogScriptManager" runat="server"/>
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" Visible="false" /></td>
                        <td colspan="2" style="height: 24px; vertical-align: top; text-align: center;">
                            <asp:Panel runat="server" CssClass="ConfirmationContent">
                                <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message" />
                                <p/>
                                <asp:Label ID="WarningMessageLabel" runat="server" Style="text-align: center; color:red;" Text="Warning: blah blah blah" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <asp:Panel ID="ButtonPanel" runat="server" DefaultButton="NoButton" CssClass="ConfirmationButtonPanel">
                                            <ccUI:ToolbarButton ID="YesButton" runat="server" SkinID="<%$Image:YesButton%>" OnClick="YesButton_Click" />
                                            <ccUI:ToolbarButton ID="NoButton" runat="server" SkinID="<%$Image:NoButton%>" OnClick="NoButton_Click"  />
                                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="OKButton_Click"  />
                                            <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                           
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</localAsp:ModalDialog>
