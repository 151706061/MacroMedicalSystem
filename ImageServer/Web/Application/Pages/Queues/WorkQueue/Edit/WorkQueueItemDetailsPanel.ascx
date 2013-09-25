<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemDetailsPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel" %>

<%@ Register Src="~/Pages/Queues/WorkQueue/WorkQueueAlertPanel.ascx" TagName="WorkQueueAlertPanel" TagPrefix="localAsp" %>


<script type="text/javascript">
    Sys.Application.add_load(function(){
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").hide();
        });
        prm.add_endRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").show();
        });
    });
    
</script>
<asp:Panel ID="Panel1" runat="server">
     <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
         
           <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="MainContentTitle">
                        <asp:Label ID="WorkQueueItemTitle" runat="server" Text="<%$Resources: Titles, WorkQueueItemDetails %>"></asp:Label>
                    </td>
                    <td align="right" class="MainContentTitle">                        
                         <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel" DisplayAfter="0">
                            <ProgressTemplate>
                                <asp:Image ID="RefreshingIndicator" runat="server" SkinID="AjaxLoadingBlue" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:Image ID="AutoRefreshIndicator" runat="server" SkinID="RefreshEnabled" />
                            
                    </td>
                </tr>
                <tr runat="server" id="WorkQueueAlertPanelRow"><td colspan="2"><localAsp:WorkQueueAlertPanel runat="server" ID="WorkQueueAlertPanel" /></td>
                </tr>
                <tr><td colspan="2">
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel" >
                            <tr><td style="padding-top: 5px; padding-left: 5px;">
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" SkinID="<%$Image:RescheduleButton%>" OnClick="Reschedule_Click"/>
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" SkinID="<%$Image:ResetButton%>" OnClick="Reset_Click"/>
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="Delete_Click"/>
                                    <ccUI:ToolbarButton ID="ReprocessButton" runat="server" SkinID="<%$Image:ReprocessButton%>" OnClick="Reprocess_Click"/>
                                    <ccUI:ToolbarButton ID="StudyDetailsButton" runat="server" SkinID="<%$Image:ViewStudyButton%>" />      
                            </td></tr>
                            <tr><td><asp:PlaceHolder ID="WorkQueueDetailsViewPlaceHolder" runat="server"></asp:PlaceHolder></td></tr>
                       </table>
                  </td></tr>
              </table>
              
              
            <ccUI:Timer ID="RefreshTimer" runat="server" Interval="10000" OnTick="RefreshTimer_Tick" OnAutoDisabled="OnAutoRefreshDisabled" DisableAfter="3"></ccUI:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
