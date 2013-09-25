<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionGridView.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Dashboard.ServerPartitionGridView" %>

<%@ Register Src="~/Controls/UsersGuideLink.ascx" TagPrefix="cc" TagName="HelpLink" %>

  
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccAsp:GridPager ID="GridPagerTop" runat="server" />   
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccUI:GridView ID="PartitionGridView" runat="server" 
                OnRowDataBound="PartitionGridView_RowDataBound" 
                PageSize="20" SelectionMode="Disabled" MouseHoverRowHighlightEnabled="false">
                <Columns>
                    <asp:BoundField DataField="AeTitle" HeaderText="<%$Resources: ColumnHeaders,AETitle %>" HeaderStyle-HorizontalAlign="Left"/>
                    <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, PartitionDescription %>" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Port" HeaderText="<%$Resources: ColumnHeaders,Port %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="PartitionStorageConfigColumnContent">
                                <%=ColumnHeaders.PartitionStorageConfiguration %>    
                                <cc:HelpLink ID="HelpLink1" runat="server" TopicID="Partition_Storage_Configuration" Target="_blank" />
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="PartitionStorageConfigColumnContent">
                                <asp:Label ID="PartitionStorageConfigurationLabel" runat="server" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,Enabled %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PartitionAcceptAnyDevice %>">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("AcceptAnyDeviceImage") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="AcceptAnyDeviceImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PartitionDuplicateObjectPolicy %>" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="DuplicateSopDescription" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PartitionStudiesCount %>">
                        <ItemTemplate>
                            <asp:Label ID="Studies" runat="server" Text='<%# Bind("StudyCount") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, AdminPartition_NoPartitionsFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                                
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
