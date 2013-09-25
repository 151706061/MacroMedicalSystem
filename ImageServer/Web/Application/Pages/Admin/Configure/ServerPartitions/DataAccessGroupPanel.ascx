<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataAccessGroupPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.DataAccessGroupPanel" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="Macro.ImageServer.Model" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" CssClass="ServerPartitionDialogTabDescription">
            <%= SR.AdminPartition_DataAccess_PleaseSelect %>
        </asp:Panel>
        <asp:Panel runat="server" CssClass="DataAccessGroupContainer">
            <asp:CheckBoxList ID="DataAccessGroupCheckBoxList" runat="server" CssClass="DataAccessGroupCheckBoxList" RepeatColumns="1" />
        </asp:Panel>
        <asp:Panel ID="Legends" runat="server" CssClass="ServerPartitionDialogTabDescription" style="text-align:center">
            <span class="GlocalSeeNotesMarker"/>*</span> <%= SR.AdminPartition_DataAccess_WarningAllStudiesAccess%>
        </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>