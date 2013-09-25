<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="Macro.ImageServer.Web.Application.Pages.WebViewer.StudyListGridView"
	Codebehind="StudyListGridView.ascx.cs" %>




<style>

.RowStyle
{
    border-bottom: solid 2px #666666;
    padding: 0px 0px 0px 8px;
    height: 20px;
    background: #222222;
    font-family: Sans-Serif;
    font-size: 12px;
    color: #eeeeee;
}

.RowStyle td
{
    border-bottom: solid 2px #666666;
    padding: 0px 0px 0px 8px;
    height: 20px;
}

.AlternatingRowStyle
{
    border-bottom: solid 2px #666666;
    padding: 0px 0px 0px 8px;
    height: 20px;
    background: #222222;
    font-family: Sans-Serif;
    font-size: 12px;
    color: #eeeeee;
}

.AlternatingRowStyle td
{
    border-bottom: solid 2px #666666;
    padding: 0px 0px 0px 8px;
    height: 20px;
}


.SelectedRowStyle
{
    background: #999999;
    border-bottom: solid 2px #666666;
    padding: 0px 0px 0px 8px;
    height: 20px;    
}

.SelectedRowStyle td
{
    background: #999999;
    border-bottom: solid 2px #666666;
    color: #eeeeee;
    height: 20px;
    padding: 0px 0px 0px 8px;
}

</style>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
				<ccUI:GridView ID="StudyListControl" runat="server" 
					OnRowDataBound="GridView_RowDataBound"
					SelectionMode="Single" SkinID="WebViewerMultipleStudiesGridView">
					<Columns>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, PatientName %>" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="<%$Resources: ColumnHeaders, PatientID %>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: ColumnHeaders, AccessionNumber %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center"></asp:BoundField>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, StudyDate %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: ColumnHeaders, StudyDescription %>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="<%$Resources: ColumnHeaders, SeriesCount %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="<%$Resources: ColumnHeaders, Instances %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
		                <asp:BoundField DataField="ModalitiesInStudy" HeaderText="<%$Resources: ColumnHeaders, Modality %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="ReferringPhysiciansName" HeaderText="<%$Resources: ColumnHeaders, ReferringPhysician %>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, StudyStatus %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                            <asp:Label ID="StudyStatusEnum" runat="server" Text='<%# Eval("StudyStatusEnum") %>' ></asp:Label>
                        </itemtemplate>
						</asp:TemplateField>														
					</Columns>
					<EmptyDataTemplate>				    
                        <ccAsp:EmptySearchResultsMessage runat="server" ID="EmptySearchResultsMessage" />
					</EmptyDataTemplate>
					<RowStyle CssClass="RowStyle" />
					<AlternatingRowStyle CssClass="AlternatingRowStyle" />
					<SelectedRowStyle CssClass="SelectedRowStyle" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
