<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Import namespace="Macro.ImageServer.Web.Common.Data.DataSource"%>
<%@ Import namespace="Macro.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="System.Xml"%>
<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AlertsGridPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsGridPanel" %>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">   
            <asp:ObjectDataSource ID="AlertDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.AlertDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.AlertSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetAlertDataSource"
				OnObjectDisposing="DisposeAlertDataSource"/>
            <ccUI:GridView ID="AlertGridView" runat="server" OnRowDataBound="AlertGridView_RowDataBound" SelectionMode="Multiple"
                DataKeyNames="Key">
                <Columns>
                    <asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertContent %>">
					    <itemtemplate>
					        <%# Eval("Message") %>
					        <asp:LinkButton runat="server" ID="AppLogLink" Text="<%$Resources: Labels,AlertGridPanel_LinkToLogs%>" CssClass="LogInfo"/>
					        <asp:PlaceHolder runat="server" ID="DetailsHoverPlaceHolder"></asp:PlaceHolder>
					    </itemtemplate>
				    </asp:TemplateField>
                    <asp:BoundField DataField="Component" HeaderText="<%$Resources:ColumnHeaders,AlertComponent %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Source" HeaderText="<%$Resources:ColumnHeaders,AlertSource %>" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertInsertDate%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <ccUI:DateTimeLabel ID="InserTime" runat="server" Value='<%# Eval("InsertTime") %>' />                        
                        </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,AlertLevel %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
					    <itemtemplate>
                            <asp:Label ID="Level" Text="" runat="server" />
                        </itemtemplate>
				    </asp:TemplateField>
                    <asp:BoundField DataField="Category" HeaderText="<%$Resources:ColumnHeaders,AlertCategory %>" HeaderStyle-HorizontalAlign="Left" />                                                                               
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
