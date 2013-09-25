<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ScheduleWorkQueueDialog.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.ScheduleWorkQueueDialog" %>

<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="localAsp" %>
<%@ Register Src="../WorkQueueItemList.ascx" TagName="WorkQueueItemList" TagPrefix="localAsp" %>

<ccAsp:ModalDialog id="ModalDialog" runat="server" title="<%$Resources: Titles,ScheduleWorkQueueDialogTitle %>" Width="900px">
<ContentTemplate>
   
    <asp:Panel ID="Panel1" runat="server" CssClass="DialogPanelContent" width="100%">
        
        <asp:UpdatePanel ID="TestUpdatePanel" runat="server">
            <ContentTemplate>
            <localAsp:WorkQueueItemList ID="SelectedWorkQueueItemList" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Panel runat="server" style="border-top: solid 1px #CCCCCC; padding-top: 3px; text-align: center; padding-top: 5px; padding-bottom: 5px;">
            <localAsp:WorkQueueSettingsPanel  ID="WorkQueueSettingsPanel" runat="server" />           
        </asp:Panel>
       
    </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel5" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OnApplyButtonClicked" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="OnCancelButtonClicked" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
          
    
</ContentTemplate>
</ccAsp:ModalDialog>
<ccAsp:MessageBox id="PreOpenConfirmDialog" runat="server" />
<ccAsp:MessageBox id="PreApplyChangeConfirmDialog" runat="server" />
<ccAsp:MessageBox id="MessageDialog" runat="server" />
