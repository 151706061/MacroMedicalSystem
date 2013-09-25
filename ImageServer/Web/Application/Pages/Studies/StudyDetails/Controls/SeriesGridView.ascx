<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesGridView.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesGridView" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="Macro.ImageServer.Model" %>


    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<div class="GridViewBorder">

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <div class="GridViewBorder">
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">    
                    <ccUI:GridView ID="GridView1" runat="server" 
                        OnPageIndexChanged="GridView1_PageIndexChanged" 
                        OnPageIndexChanging="GridView1_PageIndexChanging" 
                        MouseHoverRowHighlightEnabled = "true"
                        RowHighlightColor = "#EEEEEE"
                        SelectionMode="Multiple"
                        GridLines="Horizontal" BackColor="White">
                        <Columns>
                            <asp:BoundField DataField="SeriesNumber" HeaderText="<%$Resources: ColumnHeaders, SeriesNumber%>">
                                <HeaderStyle Wrap="False" HorizontalAlign="center"/>    
                                <ItemStyle HorizontalAlign="center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Modality" HeaderText="<%$Resources: ColumnHeaders, Modality%>">
                                <HeaderStyle Wrap="False" HorizontalAlign="center"/>   
                                <ItemStyle HorizontalAlign="center" /> 
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesDescription" HeaderText="<%$Resources: ColumnHeaders, SeriesDescription%>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="NumberOfSeriesRelatedInstances" HeaderText="<%$Resources: ColumnHeaders, Instances %>">
                                <HeaderStyle Wrap="False" HorizontalAlign="center" />    
                                <ItemStyle HorizontalAlign="center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesInstanceUid" HeaderText="<%$Resources: Labels, SeriesInstanceUID%>">
                                <HeaderStyle Wrap="False" />  
                            </asp:BoundField>
                            <asp:TemplateField  HeaderText="<%$Resources: Labels, PerformedOn%>">
                                <ItemTemplate>
                                    <ccUI:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedProcedureStepStartDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></ccUI:DALabel>
                                    <ccUI:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedProcedureStepStartTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></ccUI:TMLabel>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>                            
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell><%=ColumnHeaders.SeriesNumber%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.Modality%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.Instances%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.SeriesInstanceUID%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.PerformedOn%></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="5" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText"><%= SR.StudyDetails_NoSeriesForThisStudy%></asp:panel>
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
</div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>