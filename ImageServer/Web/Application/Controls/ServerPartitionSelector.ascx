<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionSelector.ascx.cs" 
    Inherits="Macro.ImageServer.Web.Application.Controls.ServerPartitionSelector" %>

<%@ Import Namespace="Macro.ImageServer.Web.Common.Security"%>
<%@ Import Namespace="Resources" %>

<asp:Panel runat="server" ID="PartitionPanel" CssClass="PartitionPanel">
    <asp:Label ID="PartitionLabel" runat="server" Text="<%$Resources: Labels,Partitions %>" CssClass="SearchTextBoxLabel" EnableViewState="False" style="padding-left: 5px;"/>
    <asp:Panel ID="PartitionLinkPanel" runat="server" Visible="True" Wrap="True"/> 
    
</asp:Panel>
  
<asp:Panel runat="server" ID="NoPartitionPanel" CssClass="PartitionPanel" Visible="false">
    <% if (SessionManager.Current.User.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.ServerPartitions)){%>
    <asp:Panel ID="Panel1" runat="server" CssClass="AddPartitionMessage">
        <asp:Literal ID="Literal1" runat="server" Text="<%$Resources: SR, NoPartitionAvailable %>"></asp:Literal>
        <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/Pages/Admin/Configure/ServerPartitions/Default.aspx" CssClass="AddPartitionLink" Text="<%$Resources:Labels,AddNewPartition %>"></asp:LinkButton>
    </asp:Panel>
    
    <%} else {%>
    <asp:Panel ID="Panel2" runat="server" CssClass="AddPartitionMessage">
        <asp:Literal ID="Literal2" runat="server" Text="<%$Resources: SR, NoPartitionAvailable %>"></asp:Literal>
        <asp:Literal ID="Literal3" runat="server" Text="<%$Resources: SR, ContactAdmin %>"></asp:Literal>
        
    </asp:Panel>
    
    <%}%>
</asp:Panel>
    
