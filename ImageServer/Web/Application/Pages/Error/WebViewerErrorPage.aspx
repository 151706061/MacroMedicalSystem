<%-- License
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebViewerErrorPage.aspx.cs" MasterPageFile="WebViewerErrorPageMaster.Master" Inherits="Macro.ImageServer.Web.Application.Pages.Error.WebViewerErrorPage" %>
<%@ Import Namespace="Resources"%>
<%@ Import namespace="System.Threading"%>


<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" runat="server">
	        <%= ErrorMessages.UnexpectedError %>
	    </asp:label>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        <%= Macro.ImageServer.Web.Common.Utilities.HtmlUtility.Encode(ErrorMessages.GeneralErrorMessage)%>
        <div ID="StackTraceMessage" runat="server" Visible="false" onclick="javascript:toggleLayer('StackTrace');" style="margin-top:20px">
                <%= Macro.ImageServer.Web.Common.Utilities.HtmlUtility.Encode(ErrorMessages.ErrorShowStackTraceMessage)%>
        </div>
    </asp:Label>
    <div id="StackTrace" style="margin-top: 15px" visible="false">
        <asp:TextBox runat="server" ID="StackTraceTextBox" Visible="false" Rows="5" Width="90%" TextMode="MultiLine" ReadOnly="true"></asp:TextBox>
    </div>
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td style="height: 30px; text-align: center;"><a href="javascript:window.close()" class="UserEscapeLink"><%= Labels.Close %></a></td></tr></table>
</asp:Content>