<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudiesSummary.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Dashboard.StudiesSummary" %>
<%@ Import Namespace="Resources"%>


<asp:DataList ID="StudiesDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;"><%= ColumnHeaders.Dashboard_StudiesSummary_Partition %></td>
        <td align="center" nowrap="nowrap"><%= ColumnHeaders.Dashboard_StudiesSummary_NumberOfStudies %></td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="PartitionLink"><%#Eval("AETitle") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("StudyCount") %></td></tr>
    </ItemTemplate>
    <AlternatingItemStyle CssClass="OverviewAlernateRow" />
</asp:DataList>
<div class="TotalStudiesSummary"><%=Labels.Dashboard_StudiesSummary_TotalStudies %> <asp:Label ID="TotalStudiesLabel" runat="server" Text="100,000,000" CssClass="TotalStudiesCount"/></div>