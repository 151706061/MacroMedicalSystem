<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyGridView.ascx.cs" 
Inherits="Macro.ImageServer.Web.Application.Pages.Studies.Move.StudyGridView" %>

            <asp:GridView ID="StudyListControl" runat="server" skinid="GlobalGridView">
                <Columns>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PatientName%>">
                        <itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PatientId" HeaderText="<%$Resources: ColumnHeaders, PatientID%>"></asp:BoundField>
                    <asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: ColumnHeaders,AccessionNumber %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, StudyDate%>">
                        <itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>' HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></ccUI:DALabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: ColumnHeaders, StudyDescription%>"></asp:BoundField>
                                    </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR, NoStudiesWereFound %>" />
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

