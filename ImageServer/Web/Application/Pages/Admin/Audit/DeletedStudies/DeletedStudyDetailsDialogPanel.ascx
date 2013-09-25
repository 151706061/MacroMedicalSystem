<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyDetailsDialogPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialogPanel" %>

<%@ Register Src="DeletedStudyDetailsDialogGeneralPanel.ascx" TagName="GeneralInfoPanel" TagPrefix="localAsp" %>
<%@ Register Src="DeletedStudyArchiveInfoPanel.ascx" TagName="ArchiveInfoPanel" TagPrefix="localAsp" %>
    
<asp:Panel ID="Panel3" runat="server">
    <aspAjax:TabContainer ID="TabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
        <aspAjax:TabPanel ID="StudyInfoTabPanel" runat="server" HeaderText="<%$Resources: Titles, AdminDeleteStudies_DetailsDialog_StudyInfoTabTitle %>" CssClass="DialogTabControl">
            <ContentTemplate>
                <localAsp:GeneralInfoPanel runat="server" ID="GeneralInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
        
        <aspAjax:TabPanel ID="ArchiveInfoTabPanel" runat="server" HeaderText="<%$Resources: Titles, AdminDeleteStudies_DetailsDialog_ArchiveInfoTabTitle %>" CssClass="DialogTabControl">
            <ContentTemplate>
             <localAsp:ArchiveInfoPanel runat="server" ID="ArchiveInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
    
</asp:Panel>