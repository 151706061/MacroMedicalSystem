<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Import namespace="System.ComponentModel"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails.SeriesDetailsView" %>

<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="2" 
    GridLines="Horizontal" Width="100%" CssClass="GlobalGridView" OnDataBound="DetailsView1_DataBound">
    <Fields>
        <asp:BoundField DataField="SeriesInstanceUid" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesInstanceUID %>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Modality" HeaderText="<%$Resources: DetailedViewFieldLabels, Modality%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesNumber" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesNumber%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesDescription" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesDescription%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, PerformedOn%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></ccUI:DALabel>
                <ccUI:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="NumberOfSeriesRelatedInstances"  HeaderText="<%$Resources: DetailedViewFieldLabels, Instances%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SourceApplicationEntityTitle" HeaderText="<%$Resources: DetailedViewFieldLabels, SourceAE%>">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        
        
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
</asp:DetailsView>

