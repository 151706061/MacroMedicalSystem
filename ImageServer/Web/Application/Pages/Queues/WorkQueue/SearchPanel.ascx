<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Macro.ImageServer.Web.Common.WebControls.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel" %>
<%@ Register Src="WorkQueueItemList.ascx" TagName="WorkQueueItemList" TagPrefix="localAsp" %>
    
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <script type="text/Javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

        function MultiSelect(){
            $("#<%=TypeListBox.ClientID %>").multiSelect({
                noneSelected: '',
                oneOrMoreSelected: '*'
            });
            $("#<%=StatusListBox.ClientID %>").multiSelect({
                noneSelected: '',
                oneOrMoreSelected: '*'
            });
        }
        </script>
        <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" CellSpacing="0" BorderWidth="0px">
                <asp:TableRow>
                    <asp:TableCell>                          
                        <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton" >
                            <table cellpadding="0" cellspacing="0">
                                <tr>                         
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, PatientName%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByPatientName %>" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, PatientID%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByPatientID %>" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueScheduledDate%>" CssClass="SearchTextBoxLabel" />&nbsp;&nbsp;
                                        <asp:LinkButton ID="ClearScheduleDateButton" runat="server" Text="X" CssClass="SmallLink"/><br />
                                        <ccUI:TextBox ID="ScheduleDate" runat="server" ReadOnly="true" CssClass="SearchDateBox" ToolTip="<%$Resources: Tooltips,  SearchByScheduledDate %>" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueType%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:ListBox ID="TypeListBox" runat="server" SelectionMode="Multiple" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueStatus%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:ListBox ID="StatusListBox" runat="server" SelectionMode="Multiple" /></td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueuePriority%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="SearchDropDownList">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, WorkQueueProcessingServer%>" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="ProcessingServer" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByProcessingServer %>" />
                                    </td>    
                                    <td valign="bottom">
                                        <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" /></asp:Panel>
                                    </td>  
                                </tr>                                       
                            </table>
                        </asp:Panel>
                        <ccUI:CalendarExtender ID="ScheduleCalendarExtender" runat="server" TargetControlID="ScheduleDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </asp:TableCell> 
                </asp:TableRow>
                     <asp:TableRow>
                        <asp:TableCell>
                        <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons" style="position:relative;" >
                                        <ccUI:ToolbarButton ID="ViewItemDetailsButton" runat="server" SkinID="<%$Image:ViewDetailsButton%>" OnClick="ViewItemButton_Click" Roles='<%= Macro.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.View %>'/>
                                        <ccUI:ToolbarButton ID="RescheduleItemButton" runat="server" SkinID="<%$Image:RescheduleButton%>" OnClick="RescheduleItemButton_Click" Roles='<%= Macro.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reschedule %>'/>
                                        <ccUI:ToolbarButton ID="ResetItemButton" runat="server" SkinID="<%$Image:ResetButton%>" OnClick="ResetItemButton_Click" Roles='<%= Macro.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reset %>'/>
                                        <ccUI:ToolbarButton ID="DeleteItemButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="DeleteItemButton_Click" Roles='<%= Macro.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Delete %>'/>
                                        <ccUI:ToolbarButton ID="ReprocessItemButton" runat="server" SkinID="<%$Image:ReprocessButton%>" OnClick="ReprocessItemButton_Click" Roles='<%= Macro.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reprocess %>'/>
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" CssClass="SearchPanelResultContainer">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:WorkQueueItemList ID="workQueueItemList" Height="500px" runat="server"></localAsp:WorkQueueItemList></td></tr>
                            </table>
                         
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
    </ContentTemplate>
</asp:UpdatePanel>

<ccAsp:MessageBox runat="server" ID="MessageBox" />
