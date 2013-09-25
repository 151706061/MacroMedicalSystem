<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>



<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails.SeriesDetailsPanel" %>

<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
        <ContentTemplate>
              
  <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="MainContentTitle"><%= Titles.SeriesDetails %></td>
  </tr>
    
  <tr>
  <td class="PatientInfo"><localAsp:PatientSummaryPanel ID="PatientSummary" runat="server" /></td>
  </tr>
  
  <tr><td><asp:Image runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
      <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle"><%= Labels.SeriesDetails_StudySummary%></td></tr>
        <tr><td>
        <localAsp:StudySummaryPanel ID="StudySummary" runat="server" />
        </td></tr>
    </table>
  </td>
  </tr>
  
  <tr><td><asp:Image ID="Image1" runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle"><%= Labels.SeriesDetails_SeriesSummary %></td></tr>
        <tr><td>
        <localAsp:SeriesDetailsView ID="SeriesDetails" runat="server" />
        </td></tr>
    </table>
  </tr>
  
  
  </table>
  
        </ContentTemplate>
    </asp:UpdatePanel>

