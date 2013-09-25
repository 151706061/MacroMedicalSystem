<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="Macro.ImageServer.Web.Application.Pages.Queues.RestoreQueue.RestoreQueueItemList"
	Codebehind="RestoreQueueItemList.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="RestoreQueueDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.RestoreQueueDataSource"
				DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.RestoreQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetRestoreQueueDataSource"
				OnObjectDisposing="DisposeDataSource"/>
				<ccUI:GridView ID="RestoreQueueGridView" runat="server"
					OnSelectedIndexChanged="RestoreQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="RestoreQueueGridView_PageIndexChanging"
					OnRowDatabound="GridView_RowDataBound"
					SelectionMode="Multiple">
					<Columns>
						<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,PatientName%>" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="<%$Resources:ColumnHeaders,PatientID%>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,RestoreQueueScheduledTime%>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <ccUI:DateTimeLabel ID="ScheduledDateTime" Value='<%# Eval("ScheduledDateTime") %>' runat="server"></ccUI:DateTimeLabel>
                            </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StatusString" HeaderText="<%$Resources:ColumnHeaders,RestoreQueueStatus%>" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="Notes" HeaderText="<%$Resources:ColumnHeaders,RestoreQueueNotes%>" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
					</Columns>
					<EmptyDataTemplate>
                        <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR,NoRestoreQueueItemFound %>" />
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
