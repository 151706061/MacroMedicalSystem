<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.WebViewer.GridPager" %>
<%@ Import Namespace="Resources"%>


<table width="100%" cellpadding="0" cellspacing="0" class="WebViewerGridPager">
    <tr>
        <td align="left" style="padding-left: 6px;">
                                            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.Top)
                                   { %>
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td>
                        <asp:Image runat="server" ImageUrl="~/Pages/WebViewer/Images/WebViewerPagerTotalStudiesLeft.png" ImageAlign="AbsBottom"/>
                    </td>
                    <td nowrap="nowrap">
                        <%
                            if (Request.UserAgent.Contains("Chrome"))
                            {%>
                        <div id="WebViewerItemCountContainer_Chrome">
                        <%} else if (Request.UserAgent.Contains("MSIE")) {%>
                        <div id="WebViewerItemCountContainer">
                        <%}%>       
                        <% else {%>
                        <div id="WebViewerItemCountContainer_FF">
                        <%}%>                    
                            <asp:Label ID="ItemCountLabel" runat="server" Text="Label" CssClass="WebViewerGridPagerLabel" />
                        </div>
                    </td>
                    <td>
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Pages/WebViewer/Images/WebViewerPagerTotalStudiesRight.png" ImageAlign="AbsBottom"/>
                    </td>
                </tr>
            </table>
                        <%} %>            
        </td>
        <td align="center">
            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.Top)
               { %>
               
            <asp:UpdateProgress ID="SearchUpdateProgress" runat="server" DisplayAfter="50">
                <ProgressTemplate>
                    <asp:Image ID="Image10" runat="server" ImageUrl="~/Pages/WebViewer/Images/Searching.gif" ImageAlign="Bottom"/>
                </ProgressTemplate>
            </asp:UpdateProgress>
            
            <%} %>
        </td>
        <td align="right" style="padding-right: 6px; padding-bottom: 2px; padding-top: 0px;">
            <table cellspacing="0" cellpadding="0">
                <tr>

                    <td valign="bottom" style="padding-top: 1px;">
                        <asp:ImageButton ID="FirstPageButton" runat="server" CommandArgument="First" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="WebViewerGridPagerLink" />
                    </td>                
                    <td valign="bottom" style="padding-top: 1px;">
                        <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="WebViewerGridPagerLink" />
                    </td>
                    <td nowrap="nowrap" valign="bottom">
                        <asp:panel ID="CurrentPageContainer" runat="server">
                            <asp:Label ID="Label3" runat="server" CssClass="WebViewerGridPagerLabel"><%= GridPager.Page %></asp:Label>
                            <asp:TextBox ID="CurrentPage" runat="server" Width="85px" CssClass="WebViewerGridViewTextBox"
                                Style="font-size: 12px;" />
                            <asp:Label ID="PageCountLabel" runat="server" Text="<%$Resources: GridPager, Page %>" CssClass="WebViewerGridPagerLabel" />
                            <aspAjax:FilteredTextBoxExtender runat="server" ID="CurrentPageFilter" FilterType="Numbers" TargetControlID="CurrentPage"  />
                        </asp:panel>
                    </td>
                    <td valign="bottom" style="padding-top: 1px;">
                        <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="WebViewerGridPagerLink" />
                    </td>
                    <td valign="bottom" style="padding-right: 1px; padding-top: 1px;">
                        <asp:ImageButton ID="LastPageButton" runat="server" CommandArgument="Last" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="WebViewerGridPagerLink" />
                    </td>     
                    <td valign="bottom">
                        <%-- This Link Button is used to submit the Page from the TextBox when the user clicks enter on the text box. --%>
                        <asp:LinkButton ID="ChangePageButton" runat="server" CommandArgument="ChangePage"
                            CommandName="Page" OnCommand="PageButtonClick" Text="" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
