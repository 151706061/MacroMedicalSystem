<%-- License
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Help.About" %>

<%@ Import Namespace="Macro.Common" %>
<%@ Import Namespace="Macro.ImageServer.Common" %>
<%@ Register Src="Contact.ascx" TagName="Contact" TagPrefix="localAsp" %>
<asp:Content ID="MainMenuContent" ContentPlaceHolderID="MainMenuPlaceHolder" runat="server">
    <asp:SiteMapDataSource ID="MainMenuSiteMapDataSource" runat="server" ShowStartingNode="False" />
    <asp:Menu runat="server" ID="MainMenu" SkinID="MainMenu" DataSourceID="MainMenuSiteMapDataSource"
        Style="font-family: Sans-Serif">
    </asp:Menu>
</asp:Content>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder"
    runat="server">
    <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,About%>" /></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
        });

        function showLicense(divName) {
            $("#" + divName).toggle();
        }
   
    </script>
    <style>
        .pre
        {
            font-family: Sans-Serif;
            font-size: 14px;
        }
        .LicenseLink
        {
            font-weight: bold;
            padding-top: 5px;
        }
        
        .LicenseLink a
        {
            color: #205F87;
        }
        
        .License
        {
            padding: 10px 0px 10px 10px;
        }
    </style>
    <div class="AboutBackground">
        <table width="100%">
            <tr>
                <td>
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="padding-top: 1px;">
                                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="700" CssClass="AboutPanel"
                                    Style="padding-top: 5px;">
                                    <pre>
<b>ClearCanvas Inc.</b>
<%= ProductInformation.License %>
</pre>
                                    <p>
                                    </p>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" width="40%">
                    <div style="margin-left: 20px; margin-top: 20px; font-size: 16px; color: #205F87;">
                        <localAsp:Contact runat="server" />
                        <% if (!ProductInformation.Name.Equals(ProductInformation.Component))
     { %>
                        <div class="MarketingName">
                            <%= ProductInformation.Name%></div>
                        <%}    %>
                        <span style="font-weight: bold;">
                            <%=ProductInformation.Component %>
                            v<%=String.IsNullOrEmpty(ServerPlatform.VersionString) ? "Unknown Version" : ServerPlatform.VersionString %></span><br />
                        <span style="font-weight: bold;">
                            <%=ProductInformation.Edition %>
                            <%=ProductInformation.Release %></span><br />
                        <div style="font-weight: bold;">
                            Part of the Macro PACS</div>
                        <div style="font-weight: bold; font-size: small; color: Red">
                            <%=ServerPlatform.IsManifestVerified ? "" : Resources.SR.NonStandardInstallation%></div>
                        <p>
                            <b>ClearCanvas Inc.</b><br />
                        </p>
                        <p>
                            <span style="color: #999999; font-size: 12px; font-weight: bold;">
                                <%=ProductInformation.Copyright%></span><br />
                        </p>
                        <p>
                            <span style="color: #999999; font-size: 12px; font-weight: bold;">Current Regional Settings:
                                <%=System.Globalization.CultureInfo.CurrentCulture%>,
                                <%=System.Globalization.CultureInfo.CurrentUICulture%></span><br />
                        </p>
                        <p>
                            <%
        if (!string.IsNullOrEmpty(LicenseKey)) { %>
                            <span style="color: #999999; font-size: 12px; font-weight: bold;">License Key:
                                <%=LicenseKey%>
                            </span>
                            <%}%>
                        </p>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <asp:PlaceHolder runat="server" ID="ExtensionContentPlaceHolder" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
