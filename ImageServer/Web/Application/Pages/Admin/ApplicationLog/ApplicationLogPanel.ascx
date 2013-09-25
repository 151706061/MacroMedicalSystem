<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Resources" %>
<%@ Import namespace="Macro.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ApplicationLogPanel.ascx.cs"
	Inherits="Macro.ImageServer.Web.Application.Pages.Admin.ApplicationLog.ApplicationLogSearchPanel" %>
<%@ Register Src="ApplicationLogGridView.ascx" TagName="ApplicationLogGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Table ID="Table1" runat="server" Width="100%">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
					    <ccAsp:JQuery runat="server" ID="JQuery1" MultiSelect="false" Effects="false" MaskedInput="true" />
		 
						<script type="text/javascript">
						    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(ApplyMask);
						
						    function ApplyMask() {
							    jQuery(function($) {
								    $.mask.definitions[';'] = '[01]';
								    $("#<%=FromTimeFilter.ClientID %>").mask("99:99:99.999");
								    $("#<%=ToTimeFilter.ClientID %>").mask("99:99:99.999");
							    });
							}
							
							</script>

						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td>
									<table cellpadding="0" cellspacing="0" border="0">
										<tr>
											<td align="left" valign="bottom">
												<asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogHost%>" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="HostFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByHostname %>"></asp:TextBox></td>

                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogFromDate%>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearFromDateFilterButton" runat="server" Text="X" CssClass="SmallLink"/><br />
                                                <ccUI:TextBox ID="FromDateFilter" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByLogDate %>" />

                                            </td>
                                            <td align="left" valign="bottom">
                                                <table cellspacing="0" cellpadding="0" border="0"><tr><td valign="bottom"><asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogFromTime%>" CssClass="SearchTextBoxLabel" EnableViewState="false"/></td><td style="padding-left: 5px;"><ccAsp:InvalidInputIndicator ID="FromTimeHelp" runat="server" SkinID="InvalidInputIndicator" /></td></tr></table>
												<asp:TextBox ID="FromTimeFilter" runat="server" CssClass="SearchTextBox" ToolTip="From Time (HH:MM:SS.FFF)" ValidationGroup="AppLogValidationGroup"></asp:TextBox>
                                                <ccValidator:RegularExpressionFieldValidator
                                                        ID="FromTimeValidator" runat="server" ControlToValidate="FromTimeFilter" InvalidInputIndicatorID="FromTimeHelp"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AppLogValidationGroup" 
                                                        ValidationExpression="(20|21|22|23|[01]\d):[0-5][0-9]:[0-5][0-9].[0-9][0-9][0-9]" Text="<%$Resources:InputValidation,InvalidTime %>" Display="None" IgnoreEmptyValue="true">
                                                </ccValidator:RegularExpressionFieldValidator>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label8" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogToDate%>" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearToDateFilterButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 35px;"/><br />
                                                <ccUI:TextBox ID="ToDateFilter" runat="server" CssClass="SearchDateBox" ReadOnly="true" ToolTip="<%$Resources: Tooltips,SearchByLogDate %>" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <table cellspacing="0" cellpadding="0" border="0"><tr><td valign="bottom"><asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogToTime%>" CssClass="SearchTextBoxLabel" EnableViewState="false"/></td><td style="padding-left: 5px;"><ccAsp:InvalidInputIndicator ID="ToTimeHelp" runat="server" SkinID="InvalidInputIndicator" /></td></tr></table>
												<asp:TextBox ID="ToTimeFilter" runat="server" CssClass="SearchTextBox" ToolTip="To Time (HH:MM:SS.FFF)" ValidationGroup="AppLogValidationGroup"></asp:TextBox>
                                                <ccValidator:RegularExpressionFieldValidator
                                                        ID="ToTimeValidator" runat="server" ControlToValidate="ToTimeFilter" IgnoreEmptyValue="true"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AppLogValidationGroup" InvalidInputIndicatorID="ToTimeHelp"
                                                        ValidationExpression="(20|21|22|23|[01]\d):[0-5][0-9]:[0-5][0-9].[0-9][0-9][0-9]" Text="<%$Resources:InputValidation,InvalidTime %>" Display="None">
                                                </ccValidator:RegularExpressionFieldValidator>
                                            </td>                                            
											<td align="left" valign="bottom">
												<asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogThread%>" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="ThreadFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByThread %>"></asp:TextBox></td>
											<td align="left" valign="bottom">
												<asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels,AppLogLogLevel%>" CssClass="SearchTextBoxLabel"
													EnableViewState="False" /><br />
												<asp:DropDownList runat="server" ID="LogLevelListBox" CssClass="SearchDropDownList">
													<asp:ListItem Value="ANY" Text="<%$Resources: SR, LogLevelAny%>"></asp:ListItem>
													<asp:ListItem Value="INFO" Text="<%$Resources: SR,LogLevelInfo%>"></asp:ListItem>
													<asp:ListItem Value="ERROR" Text="<%$Resources: SR,LogLevelError%>"></asp:ListItem>
													<asp:ListItem Value="WARN" Text="<%$Resources: SR,LogLevelWarning%>"></asp:ListItem>
												</asp:DropDownList>
											</td>
											<td align="left" valign="bottom">
												<asp:Label ID="Label1" runat="server" Text="<%$Resources:SearchFieldLabels,AppLogLogMessages %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
												<asp:TextBox ID="MessageFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByLogContent %>"></asp:TextBox></td>
											<td valign="bottom">
												<asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
													<ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click" CausesValidation="true" ValidationGroup="AppLogValidationGroup" /></asp:Panel>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow Height="100%">
				<asp:TableCell>
					<table width="100%" cellpadding="0" cellspacing="0" >
<!--
						<tr>
							<td>
								<asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
									<ContentTemplate>
										<asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
										</asp:Panel>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>
-->						
						<tr>
							<td>
								<asp:Panel ID="Panel2" runat="server" >
									<table width="100%" cellpadding="0" cellspacing="0">
										<tr>
											<td style="border-bottom: solid 1px #66aa65">
												<ccAsp:GridPager ID="GridPagerTop" runat="server" />
											</td>
										</tr>
										<tr>
											<td style="background-color: white;">
												<localAsp:ApplicationLogGridView ID="ApplicationLogGridView" runat="server"/>
											</td>
										</tr>
									</table>
								</asp:Panel>
							</td>
						</tr>
					</table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		
        <ccUI:CalendarExtender ID="FromDateCalendarExtender" runat="server" TargetControlID="FromDateFilter"
            CssClass="Calendar">
        </ccUI:CalendarExtender>
        <ccUI:CalendarExtender ID="ToDateCalendarExtender" runat="server" TargetControlID="ToDateFilter"
            CssClass="Calendar">
        </ccUI:CalendarExtender>
		
	</ContentTemplate>
</asp:UpdatePanel>
