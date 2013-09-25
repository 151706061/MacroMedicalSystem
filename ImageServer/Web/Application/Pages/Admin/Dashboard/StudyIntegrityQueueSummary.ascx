<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyIntegrityQueueSummary.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Dashboard.StudyIntegrityQueueSummary" %>
<%@ Import Namespace="Resources"%>


<table cellpadding="2" width="100%">
    <tr><td style="color: #205F87;"><%= ColumnHeaders.Dashboard_SIQSummary_Duplicates%></td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="DuplicateLink"><%=Duplicates %></asp:LinkButton></td></tr>
    <tr><td style="color: #205F87;"><%= ColumnHeaders.Dashboard_SIQSummary_InconsistentData%></td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="InconsistentDataLink"><%=InconsistentData %></asp:LinkButton></td></tr>
    <tr><td align="right" style="padding-right: 15px; color: #205F87;"><b><%= ColumnHeaders.Dashboard_SIQSummary_Total%></b></td><td align="right" style="padding-right: 15px;"><b><asp:LinkButton runat="server" ID="TotalLinkButton"><%= Duplicates + InconsistentData %></asp:LinkButton></b></td></tr>
</table>