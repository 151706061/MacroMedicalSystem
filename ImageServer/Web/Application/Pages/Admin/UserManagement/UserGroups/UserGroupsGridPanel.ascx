<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGroupsGridPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserGroupDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.UserGroupDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.UserGroupRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserGroupDataSource"
				OnObjectDisposing="DisposeUserGroupsDataSource"/>
            <ccUI:GridView ID="UserGroupsGridView" runat="server" OnRowDataBound="UserGroupsGridView_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupName %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupDescription %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="275px" />
                    </asp:BoundField>
                     <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,AdminUserGroups_DataGroup %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DataGroup") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DataGroupImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_Tokens %>" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:TextBox ID="TokensTextBox" runat="server" TextMode="multiline" rows="3" columns="100" CssClass="TokenTextArea" ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>                    
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoUserGroupsFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
