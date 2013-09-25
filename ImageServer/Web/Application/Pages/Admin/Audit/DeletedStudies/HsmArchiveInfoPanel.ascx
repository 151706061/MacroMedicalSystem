<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Macro.Dicom" %>
<%@ Import Namespace="Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="HsmArchiveInfoPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.HsmArchiveInfoPanel" %>
<asp:Panel ID="Panel3" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:DetailsView ID="ArchiveInfoView" runat="server" AutoGenerateRows="False" GridLines="Horizontal"
                    CellPadding="4" CssClass="GlobalGridView" Width="100%">
                    <Fields>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_Archive %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label runat="server" ID="ArchiveType" Text="Hsm Archive" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_DateTime %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="ArchiveTime" runat="server" Value='<%# Eval("ArchiveTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, TransferSyntax %>">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="TransferSyntaxUid" runat="server" Text='<%# String.Format("{1} ({0})", Eval("TransferSyntaxUid"), TransferSyntax.GetTransferSyntax( (string) Eval("TransferSyntaxUid") ).Name) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, AdminDeletedStudies_Hsm_Location %>Archive Location:">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <asp:Label ID="TransferSyntaxUid" runat="server" Text='<%# Eval("ArchiveFolderPath" ) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <RowStyle CssClass="GlobalGridViewRow" />
                    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                    <EmptyDataTemplate>
                        <asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
                            <asp:Label runat="server" Text="<%$Resources: SR, AdminDeletedStudies_StudyWasNotArchived %>" />
                        </asp:Panel>
                    </EmptyDataTemplate>
                </asp:DetailsView>
            </td>
        </tr>
    </table>
</asp:Panel>
