<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Import namespace="System.Xml"%>
<%@ Import namespace="Macro.ImageServer.Common.Utilities"%>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.HistoryPanel" %>
<%@ Import Namespace="Resources"%>
<%@ Register Src="StudyHistoryChangeDescPanel.ascx" TagName="StudyHistoryChangeDescPanel" TagPrefix="localAsp" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
	
<div class="GridViewBorder">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top"> 
        <ccUI:GridView ID="StudyHistoryGridView" runat="server" 
                       OnRowDataBound="StudyHistoryGridView_RowDataBound"
                       OnPageIndexChanged="StudyHistoryGridView_PageIndexChanged" 
                       OnPageIndexChanging="StudyHistoryGridView_PageIndexChanging" 
                       SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                       <Columns>
                          <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, HistoryTimestamp %>">
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="InsertTime" runat="server" Value='<%# Eval("InsertTime") %>' ></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, HistoryDescription %>">
                            <ItemTemplate>
                                <asp:Label ID="Description" runat="server" Text='<%# Eval("StudyHistoryTypeEnum") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, HistoryDetails %>">
                            <ItemTemplate>
                                <localAsp:StudyHistoryChangeDescPanel runat="server" id="StudyHistoryChangeDescPanel"></localAsp:StudyHistoryChangeDescPanel>
                            </ItemTemplate>
                        </asp:TemplateField>
                                                  
                       </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell><%= ColumnHeaders.HistoryTimestamp%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.HistoryDescription%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%= ColumnHeaders.HistoryDetails%></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText"><%= SR.StudyDetails_NoHistoryItemForThisStudy %></asp:panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>          
                    		</asp:TableCell>
	</asp:TableRow>
</asp:Table>                              
    </ContentTemplate>
</asp:UpdatePanel>
</div>