<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import Namespace="Resources" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResultGridView.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.SearchResultGridView" %>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
		
            <asp:ObjectDataSource ID="DataSource" runat="server" 
                TypeName="Macro.ImageServer.Web.Common.Data.DataSource.DeletedStudyDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.Model.DeletedStudyInfo" 
				EnablePaging="true"
				SelectMethod="Select"
				StartRowIndexParameterName="startRowIndex"
				MaximumRowsParameterName="maxRows"
				SelectCountMethod="SelectCount" />
				
				<ccUI:GridView ID="ListControl" runat="server"
					SelectionMode="Single" DataKeyNames="RowKey">
					<Columns>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PatientName %>" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="<%$Resources: ColumnHeaders,PatientID%>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: ColumnHeaders,AccessionNumber %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center"></asp:BoundField>
						<asp:TemplateField HeaderText="Study Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                            </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: ColumnHeaders,StudyDescription%>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
				        <asp:BoundField DataField="PartitionAE" HeaderText="<%$Resources: ColumnHeaders,Partition %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
					    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,AdminDeletedStudies_DeletedBy %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <%# Eval("UserName")%>
                            </itemtemplate>
						</asp:TemplateField>
					</Columns>
					<EmptyDataTemplate>				    
					<ccAsp:EmptySearchResultsMessage runat="server" ID="NoResultFoundMessage" Message="<%$Resources: SR, AdminDeletedStudies_NoStudiesFound %>">
						<SuggestionTemplate>					
						    <ul style="padding-left: 15px; margin-left: 5px; margin-top: 4px; margin-bottom: 4px;">
	                            <li><%=SR.AdminDeletedStudies_ModifySearchCriteria%></li>
	                            <li><%=SR.AdminDeletedStudies_CheckPartitionConfiguration%></li>
	                        </ul>	    
						</SuggestionTemplate>
					</ccAsp:EmptySearchResultsMessage>
					
					</EmptyDataTemplate>
					<RowStyle CssClass="GlobalGridViewRow" />
					<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
					<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>				
			
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
