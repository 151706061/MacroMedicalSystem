<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertIndicator.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Controls.AlertIndicator" %>
<%@ Import Namespace="Resources" %>

<div runat="server" id="AlertLinkPanel" >| <asp:LinkButton ID="AlertLink" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" CssClass="UserInformationLink"><%= Labels.CriticalAlerts %> <asp:Label ID="AlertsCount" runat="server" /></asp:LinkButton>
<div id="AlertDetailsPanel" class="AlertDetailsPanel" style="display: none">
    <div>
        <asp:Table runat="server" ID="AlertTable" style="background: white; border: lightsteelblue 1px solid; padding: 2px;">
            <asp:TableRow CssClass="AlertTableHeaderCell">
            <asp:TableCell><%=ColumnHeaders.AlertComponent %></asp:TableCell>
            <asp:TableCell><%=ColumnHeaders.AlertSource%></asp:TableCell>
            <asp:TableCell><%=ColumnHeaders.AlertDescription %></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div style="text-align: right; padding: 0px 2px 0px 0px; margin-top: 2px; font-weight: bold;">
    <table>
    <tr>
    <td nowrap="nowrap"><asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" style="color: #6699CC; text-decoration: none;"><%=Labels.AlertIndicator_ViewAllAlerts %></asp:LinkButton></td>
    <td> | </td>
    <td><a id="CloseButton" href="" style="color: #6699CC; text-decoration: none;"><%= Labels.Close %></a></td></tr></table></div>
</div>
</div>

<% if(alerts.Count > 0) { %>        
<script type="text/javascript">

    $(document).ready(function() {

        $("#<%=AlertLink.ClientID %>").mouseover(function() {
            $(".AlertDetailsPanel:hidden").show();
        });
        $("#CloseButton").click(function(event) {
            event.preventDefault();
            $("#AlertDetailsPanel:visible").slideUp("fast");
        });
    });
</script>

<%} else { %>

<script type="text/javascript">
        $("#<%=AlertLinkPanel.ClientID %>").hide();
</script>

<% } %>