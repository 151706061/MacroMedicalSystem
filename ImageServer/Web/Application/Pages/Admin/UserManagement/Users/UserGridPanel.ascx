<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGridPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.UserDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.UserRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserDataSource"
				OnObjectDisposing="DisposeUserDataSource"/>
            <ccUI:GridView ID="UserGridView" runat="server" OnRowDataBound="UserGridView_RowDataBound"
                OnSelectedIndexChanged="UserGridView_SelectedIndexChanged" SelectionMode="Single">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="<%$Resources: ColumnHeaders, UserID %>" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayName" HeaderText="<%$Resources: ColumnHeaders, UserDisplayName %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="EmailAddress" HeaderText="<%$Resources: ColumnHeaders, UserEmailAddress %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, UserGroups %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="UserGroupTextBox" runat="server" TextMode="multiline" rows="2" columns="35" CssClass="UserGroupTextArea" ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Enabled" HeaderText="<%$Resources: ColumnHeaders, Enabled %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="LastLoginTime" HeaderText="<%$Resources: ColumnHeaders, LastLogin %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                        <itemstyle width="175px" />
                    </asp:BoundField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoUsersFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
