<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Import namespace="Microsoft.JScript"%>
<%@ Import Namespace="Macro.ImageServer.Model" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesGridView.ascx.cs" 
Inherits="Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries.SeriesGridView" %>

            <asp:GridView ID="SeriesListControl" runat="server" SkinID="GlobalGridView">
                <Columns>
                            <asp:BoundField DataField="SeriesNumber" HeaderText="<%$Resources: Labels, SeriesNumber %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="Modality" HeaderText="<%$Resources: Labels, Modality %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesDescription" HeaderText="<%$Resources:Labels, Description %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="NumberOfSeriesRelatedInstances" HeaderText="<%$Resources:Labels, Instances %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesInstanceUid" HeaderText="<%$Resources:Labels, SeriesInstanceUID %>">
                                <HeaderStyle Wrap="False" />  
                            </asp:BoundField>
                            <asp:TemplateField  HeaderText="<%$Resources:Labels, PerformedOn %>">
                                <ItemTemplate>

                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>
                                    </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR, StudyDetails_NoSeriesForThisStudy%>" />
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

