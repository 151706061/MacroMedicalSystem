<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSummary.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Dashboard.WorkQueueSummary" %>
<%@ Import Namespace="Resources"%>


<asp:DataList ID="WorkQueueDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;"><%= ColumnHeaders.Dashboard_WorkQueueSummary_Server%></td>
        <td align="center" nowrap="nowrap"><%= ColumnHeaders.Dashboard_WorkQueueSummary_NumberOfItems %></td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="WorkQueueLink"><%#Eval("Server") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("ItemCount") %></td></tr>
    </ItemTemplate>
</asp:DataList>