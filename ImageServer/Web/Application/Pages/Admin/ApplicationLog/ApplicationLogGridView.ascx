<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationLogGridView.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.ApplicationLog.ApplicationLogGridView" %>
<%@ Import Namespace="Resources" %>
<%@ Import namespace="Macro.ImageServer.Web.Common.Utilities"%>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        <asp:ObjectDataSource ID="ApplicationLogDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.ApplicationLogDataSource"
				DataObjectTypeName="Macro.ImageServer.Model.ApplicationLog" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetApplicationLogDataSource"
				OnObjectDisposing="DisposeApplicationLogDataSource"/>
			
            <ccUI:GridView ID="ApplicationLogListControl" runat="server"
				OnPageIndexChanging="ApplicationLogListControl_PageIndexChanging" 
				OnRowDataBound="GridView_RowDataBound"
				EmptyDataText="No logs found (Please check the filters.)">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Host" HeaderText="<%$Resources:ColumnHeaders, AppLogHost%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
					<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders, AppLogTimestamp%>">
						<HeaderStyle Wrap="false" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" />
						<ItemTemplate>
							<asp:Label ID="Timestamp" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("Timestamp"),DateTimeFormatter.Style.Timestamp) %>'/>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:BoundField DataField="Thread" HeaderText="<%$Resources:ColumnHeaders, AppLogThread%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    	<ItemStyle Wrap="false" HorizontalAlign="Center" />
					</asp:BoundField>
                    <asp:BoundField DataField="LogLevel" HeaderText="<%$Resources:ColumnHeaders, AppLogLogLevel%>" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField DataField="MessageException" HeaderText="<%$Resources:ColumnHeaders, AppLogMessage%>" HeaderStyle-HorizontalAlign="Left" HtmlEncode="false"  />
                </Columns>
                <EmptyDataTemplate>
                   <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR,AppLogNotFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
