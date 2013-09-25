﻿<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataAccessPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DataAccessPanel" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="Macro.ImageServer.Model" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<style>

.GroupListing
{
	margin-top:30px;
}

.LinkToOtherGroupListing
{
	padding-left:20px;
}    


.LayoutRoot
{
	margin:10px;
}

</style>

<asp:Panel runat="server" ID="LayoutRoot" CssClass="LayoutRoot">

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="DataAccessGroupsListing" CssClass="GroupListing">
            <div>
                <asp:Panel runat="server">
                    <table width="100%">
                        <tr>
                            <td><asp:Panel runat="server" CssClass="StudyDetailsSubTitle">
                                <%= SR.StudyDetails_UpdatableDataAccessGroupListingSectionTitle%>
                            </asp:Panel></td>
                            <td align="left">                            
                                <asp:Panel runat="server" ID="LinkToOtherGroupListing" CssClass="LinkToOtherGroupListing">                                
                                    <a href="#Archor_OtherGroups" style="font-style:italic"><%= SR.StudyDetails_JumpToNonUpdatableGroupListingSectionLink %></a>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    
                </asp:Panel>            
            </div>
            <div class="GridViewBorder">
            <asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
                Width="100%">
                <asp:TableRow VerticalAlign="top">
                    <asp:TableCell VerticalAlign="top">
                        <ccUI:GridView ID="UpdatableDataAccessGroupsGridView" runat="server" OnPageIndexChanged="GridView1_PageIndexChanged"
                            OnPageIndexChanging="GridView1_PageIndexChanging" MouseHoverRowHighlightEnabled="true"  AllowPaging="false"
                            RowHighlightColor="#EEEEEE" SelectionMode="Multiple" GridLines="Horizontal" BackColor="White">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderStyle-Width="30%" HeaderText="<%$Resources: ColumnHeaders, StudyDetails_GroupName%>">
                                    <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupDescription%>">
                                    <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Table runat="server" Width="100%" CellPadding="0" CellSpacing="0">
                                    <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                        <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupName%></asp:TableHeaderCell>
                                        <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupDescription%></asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="5" Height="50" HorizontalAlign="Center">
                                            <asp:Panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">
                                                <%= SR.StudyDetails_NoAuthorityGroupsForThisStudy%></asp:Panel>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </EmptyDataTemplate>
                            <RowStyle CssClass="GlobalGridViewRow" />
                            <HeaderStyle CssClass="GlobalGridViewHeader" />
                            <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                            <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                        </ccUI:GridView>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>        
            </div>
            
        </asp:Panel>
        
        <asp:Panel runat="server" ID="OtherGroupsListing" CssClass="GroupListing">
            <a name="Archor_OtherGroups" />
                
            <div>
                <asp:Panel ID="Panel2" runat="server">
                    <table width="100%">
                        <tr>
                            <td><asp:Panel ID="Panel3" runat="server" CssClass="StudyDetailsSubTitle">
                                <%= SR.StudyDetails_NonUpdatableGroupListingSectionTitle%>
                            </asp:Panel></td>
                        </tr>
                    </table>
                    
                </asp:Panel>
                
            </div>
            
            <div class="GridViewBorder">
            <asp:Table runat="server" ID="Table2" Height="100%" CellPadding="0" CellSpacing="0"
                Width="100%">
                <asp:TableRow VerticalAlign="top">
                    <asp:TableCell VerticalAlign="top">
                        <ccUI:GridView ID="OtherGroupsWithAccessGridView" runat="server" OnPageIndexChanged="GridView1_PageIndexChanged"
                            OnPageIndexChanging="GridView1_PageIndexChanging" MouseHoverRowHighlightEnabled="true" AllowPaging="false"
                            RowHighlightColor="#EEEEEE" SelectionMode="Multiple" GridLines="Horizontal" BackColor="White">
                            <Columns>
                                <asp:BoundField DataField="Name"  HeaderStyle-Width="30%" HeaderText="<%$Resources: ColumnHeaders, StudyDetails_GroupName%>">
                                    <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupDescription%>">
                                    <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0">
                                    <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                        <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupName%></asp:TableHeaderCell>
                                        <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupDescription%></asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="5" Height="50" HorizontalAlign="Center">
                                            <asp:Panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">
                                                <%= SR.StudyDetails_NoAuthorityGroupsForThisStudy%></asp:Panel>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </EmptyDataTemplate>
                            <RowStyle CssClass="GlobalGridViewRow" />
                            <HeaderStyle CssClass="GlobalGridViewHeader" />
                            <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                            <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                        </ccUI:GridView>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            
            </div>
        
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

</asp:Panel>