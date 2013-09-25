<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptySearchResultsMessage.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Controls.EmptySearchResultsMessage" %>

<asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
    <asp:Label runat="server" ID="ResultsMessage" Text = "No items found using the provided criteria." />
    <p></p>
    <asp:Panel runat="server"  ID="SuggestionPanel" HorizontalAlign="center">
    <center>
        <table  class="EmptySearchResultsSuggestionPanel">
	        <tr align="left">
	        <td class="EmptySearchResultsSuggestionPanelHeader">
	            <asp:Label runat="server" ID="SuggestionTitle" Text="<%$Resources: SR, EmptySearchResult_Suggestions %>"></asp:Label></td>
	        </tr>
	        <tr align="left" class="EmptySearchResultsSuggestionContent">
	        <td class="EmptySearchResultsSuggestionContent" style="padding:10px;">
	             <asp:PlaceHolder ID="SuggestionPlaceHolder" runat="server"></asp:PlaceHolder>
	        </td>
	        </tr>
	    </table>
        </center>
    </asp:Panel>
    
</asp:Panel>