<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Import Namespace="Microsoft.JScript" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Data.DataSource" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="System.Xml" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertsGridView.ascx.cs"
	Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Dashboard.AlertsGridView" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
     <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccAsp:GridPager ID="GridPagerTop" runat="server" />   
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="AlertDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.AlertDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.AlertSummary"
				EnablePaging="true" SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetAlertDataSource"
				OnObjectDisposing="DisposeAlertDataSource" />
			<ccUI:GridView ID="AlertGridView" runat="server" OnRowDataBound="AlertGridView_RowDataBound"
				SelectionMode="Single" DataKeyNames="Key" PageSize="10">
				<Columns>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertContent %>">
						<ItemTemplate>
							<%# Eval("Message") %>
							<asp:LinkButton runat="server" ID="AppLogLink" Text="<%$Resources: Labels,AlertGridPanel_LinkToLogs%>"
								CssClass="LogInfo" />
							<asp:PlaceHolder runat="server" ID="DetailsHoverPlaceHolder"></asp:PlaceHolder>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Component" HeaderText="<%$Resources:ColumnHeaders,AlertComponent %>"
						HeaderStyle-HorizontalAlign="Left" />
					<asp:BoundField DataField="Source" HeaderText="<%$Resources:ColumnHeaders,AlertSource %>"
						HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertInsertDate%>" HeaderStyle-HorizontalAlign="Center"
						ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<ccUI:DateTimeLabel ID="InserTime" runat="server" Value='<%# Eval("InsertTime") %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertLevel %>" HeaderStyle-HorizontalAlign="Center"
						ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:Label ID="Level" Text="" runat="server" />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="Category" HeaderText="<%$Resources:ColumnHeaders,AlertCategory %>"
						HeaderStyle-HorizontalAlign="Left" />
				</Columns>
				<EmptyDataTemplate>
					<ccAsp:EmptySearchResultsMessage runat="server" Message="<%$Resources: SR,NoAlertsFound %>" />
				</EmptyDataTemplate>
				<RowStyle CssClass="GlobalGridViewRow" />
				<AlternatingRowStyle CssClass="GlobalGridViewRow" />
				<HeaderStyle CssClass="GlobalGridViewHeader" />
				<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
			</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
