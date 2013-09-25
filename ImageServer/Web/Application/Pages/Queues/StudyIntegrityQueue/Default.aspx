<%-- License

Copyright (c) 2012, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="Default.aspx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.Default"
    Title="Study Integrity Queue | Macro ImageServer" %>

<%@ Register Src="ReconcileDialog.ascx" TagName="ReconcileDialog" TagPrefix="localAsp" %>
<%@ Register Src="DuplicateSopDialog.ascx" TagName="DuplicateSopReconcileDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="SearchPanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">
    <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,StudyIntegrityQueue%>" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionSelector runat="server" ID="ServerPartitionSelector" Visible="true" />
            <localAsp:SearchPanel runat="server" id="SearchPanel" visible="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:ReconcileDialog ID="ReconcileDialog" runat="server" />
    <localAsp:DuplicateSopReconcileDialog ID="DuplicateSopReconcileDialog" runat="server" />
</asp:Content>
