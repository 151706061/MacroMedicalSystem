<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataRulePanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel" %>
<%@ Register Src="DataRuleGridView.ascx" TagName="DataRuleGridView" TagPrefix="localAsp" %>

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
                                    <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleStatus %>"
                                        CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList"/>                                    
                                </td>
                                <td align="left" valign="bottom">
                                    <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, ServerRuleDefault %>"
                                        CssClass="SearchTextBoxLabel"></asp:Label><br />
                                    <asp:DropDownList ID="DefaultFilter" runat="server" CssClass="SearchDropDownList"/>                                    
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
                                            <ccUI:ToolbarButton ID="AddDataRuleButton" runat="server" SkinID="<%$Image:AddButton%>"
                                                OnClick="AddDataRuleButton_Click" />
                                            <ccUI:ToolbarButton ID="EditDataRuleButton" runat="server" SkinID="<%$Image:EditButton%>"
                                                OnClick="EditDataRuleButton_Click" />
                                            <ccUI:ToolbarButton ID="CopyDataRuleButton" runat="server" SkinID="<%$Image:CopyButton%>"
                                                OnClick="CopyDataRuleButton_Click" />
                                            <ccUI:ToolbarButton ID="DeleteDataRuleButton" runat="server" SkinID="<%$Image:DeleteButton%>"
                                                OnClick="DeleteDataRuleButton_Click" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel1" runat="server"  CssClass="SearchPanelResultContainer">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <ccAsp:GridPager ID="GridPagerTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: white;">
                                                <localAsp:DataRuleGridView ID="DataRuleGridViewControl" runat="server" Height="500px" />
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
