<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateAuthorityGroupDialog.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.AddAuthorityGroupDialog" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="650px" Title="<%$ Resources:Titles, UpdateAuthorityGroupDialog %>">
    <ContentTemplate>
        <div class="DialogPanelContent">
            <asp:Table ID="Table2" runat="server" SkinID="NoSkin" CellSpacing="3" CellPadding="3">
                <asp:TableRow ID="TableRow5" runat="server">
                    <asp:TableCell ID="TableCell13" runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: InputLabels, AuthorityGroupsDataAccess %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                RepeatColumns="1">
                            </asp:CheckBoxList>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server" />
                </asp:TableRow>               
            </asp:Table>
        </div>
        <table width="100%" cellspacing="0" cellpadding="0">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>"
                            OnClick="UpdateButton_Click"  />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>