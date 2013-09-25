<%-- License
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebViewerAuthorizationErrorPage.aspx.cs" MasterPageFile="WebViewerErrorPageMaster.Master" Inherits="Macro.ImageServer.Web.Application.Pages.Error.WebViewerAuthorizationErrorPage" %>
<%@ Import Namespace="Resources"%>
<%@ Import namespace="System.Threading"%>


<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel"  runat="server">
	        <%= ErrorMessages.AuthorizationError %>
	    </asp:label>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        <%= Macro.ImageServer.Web.Common.Utilities.HtmlUtility.Encode(String.Format(ErrorMessages.WebAuthorizationErrorDescription, UserID))%>
    </asp:Label>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td style="height: 30px; text-align: center;"><a href="javascript:window.close()" class="UserEscapeLink"><%= Labels.Close  %></a></td></tr></table>
</asp:Content>