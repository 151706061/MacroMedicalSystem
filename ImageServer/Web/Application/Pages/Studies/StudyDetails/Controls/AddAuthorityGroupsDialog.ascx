<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="Macro.ImageServer.Web.Application.Helpers" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddAuthorityGroupsDialog.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.AddAuthorityGroupsDialog" %>
<%@ Import Namespace="Resources" %>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="<%$ Resources:Titles, AddAuthorityGroupsDialogTitle %>"
    Width="800px">
    <ContentTemplate>
        <div class="ContentPanel">
            <div class="DialogPanelContent">
                <table border="0" cellspacing="5" width="100%">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label runat="server" CssClass="DialogTextBoxLabel" Text="<%$ Resources:Labels, AddAuthorityGroupsDialog_StudyListingLabel %>"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="DeleteStudiesTableContainer" style="background: white">
                                            <asp:Repeater runat="server" ID="StudyListing">
                                                <HeaderTemplate>
                                                    <table cellspacing="0" width="100%" class="DeleteStudiesConfirmTable">
                                                        <tr>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.PatientName %>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.PatientID%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.StudyDate%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.StudyDescription%>
                                                            </th>
                                                            <th style="white-space: nowrap" class="GlobalGridViewHeader">
                                                                <%= ColumnHeaders.AccessionNumber%>
                                                            </th>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr class="GlobalGridViewRow">
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientsName", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "PatientId", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDate", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "StudyDescription", "&nbsp;")%>
                                                        </td>
                                                        <td>
                                                            <%# HtmlUtility.GetEvalValue(Container.DataItem, "AccessionNumber", "&nbsp;")%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label runat="server" CssClass="DialogTextBoxLabel" Text="<%$ Resources:Labels, AddAuthorityGroupsDialog_AuthorityGroupsLabel %>"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="DeleteStudiesTableContainer">
                                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                                Width="100%" RepeatColumns="1">
                                            </asp:CheckBoxList>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="AddButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="AddButton_Clicked" />
                            <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                                OnClick="CancelButton_Clicked" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</ccAsp:ModalDialog>
