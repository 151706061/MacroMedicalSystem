<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerRulePanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel" %>
<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <script>
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
        </script>
        <asp:Table ID="Table" runat="server" Width="100%" CellPadding="2" Style="border-color: #6699CC"
            BorderWidth="2px">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Bottom" Wrap="false">
                    <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleType %>"
                                        CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                    <asp:DropDownList ID="RuleTypeDropDownList" runat="server" CssClass="SearchDropDownList"
                                        ToolTip="<%$Resources: Tooltips, SearchByType %>" />
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleApplyType %>"
                                        CssClass="SearchTextBoxLabel" EnableViewState="False"></asp:Label><br />
                                    <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" CssClass="SearchDropDownList"
                                        ToolTip="<%$Resources: Tooltips, SearchByApplyTime %>" />
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleStatus %>"
                                        CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                    </asp:DropDownList>
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleDefault %>"
                                        CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="DefaultFilter" runat="server" CssClass="SearchDropDownList">
                                    </asp:DropDownList>
                                </td>
                                <td align="right" valign="bottom">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="SearchButtonPanel">
                                        <ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>"
                                            OnClick="SearchButton_Click" /></asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="ToolbarUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                            <ccUI:ToolbarButton ID="AddServerRuleButton" runat="server" SkinID="<%$Image:AddButton%>"
                                                OnClick="AddServerRuleButton_Click" />
                                            <ccUI:ToolbarButton ID="EditServerRuleButton" runat="server" SkinID="<%$Image:EditButton%>"
                                                OnClick="EditServerRuleButton_Click" />
                                            <ccUI:ToolbarButton ID="DeleteServerRuleButton" runat="server" SkinID="<%$Image:DeleteButton%>"
                                                OnClick="DeleteServerRuleButton_Click" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchPanelResultContainer">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <ccAsp:GridPager ID="GridPagerTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: white;">
                                                <localAsp:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server" Height="500px" />
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
    </ContentTemplate>
</asp:UpdatePanel>
