<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemList.ascx.cs"
	Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.WorkQueueItemList" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="Macro.ImageServer.Model"%>
<%@ Import Namespace="Macro.ImageServer.Common"%>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Data.DataSource"%>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Data"%>


<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Table runat="server" ID="ListContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
			Width="100%">
			<asp:TableRow VerticalAlign="top">
				<asp:TableCell ID="ListContainerCell" CssClass="GlobalGridViewPanelContent" VerticalAlign="top">
					<asp:ObjectDataSource ID="WorkQueueDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.WorkQueueDataSource"
						DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.WorkQueueSummary" EnablePaging="true"
						SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetWorkQueueDataSource"
						OnObjectDisposing="DisposeDataSource"></asp:ObjectDataSource>
					<ccUI:GridView ID="WorkQueueGridView" runat="server" 
						OnRowDataBound="WorkQueueListView_RowDataBound"
						OnDataBound="WorkQueueListView_DataBound"
						DataKeyNames="Key" SelectUsingDataKeys="true">
						<Columns>
						<asp:BoundField HeaderText="<%$Resources: ColumnHeaders,PatientID %>" DataField="PatientId" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"/>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,PatientName %>" HeaderStyle-HorizontalAlign="Left">
								<headerstyle wrap="false" horizontalalign="Left" />
								<itemstyle wrap="false" />
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="<%$Resources: ColumnHeaders,WorkQueueType %>" DataField="TypeString" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false"/>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,WorkQueueSchedule %>">
							<headerstyle wrap="false" horizontalalign="Center" />
							<itemstyle wrap="false" horizontalalign="Center"/>
							<itemtemplate>
								<asp:Label ID="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledDateTime")) %>'></asp:Label>
							</itemtemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="<%$Resources: ColumnHeaders,WorkQueuePriority %>" DataField="PriorityString" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,WorkQueueStatus %>">
							<headerstyle wrap="false" horizontalalign="Center" />
							<itemstyle wrap="false" horizontalalign="Center" />
							<itemtemplate>
								<asp:Label ID="StatusLabel" runat="server" Text='<%# Eval("StatusString") %>'></asp:Label>
								
								<asp:Image runat="server" Visible='<%# !(Container.DataItem as WorkQueueSummary).TheWorkQueueItem.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) && !ServerPlatform.IsActiveWorkQueue((Container.DataItem as WorkQueueSummary).TheWorkQueueItem) %>'  ImageAlign="AbsBottom" ID="StuckIcon" SkinID="WarningSmall"
								    ToolTip="<%$Resources: Tooltips,WorkQueueIsStuck %>"/>
							</itemtemplate>
						</asp:TemplateField>
						
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,WorkQueueProcessingServer %>">
							<headerstyle wrap="false" horizontalalign="Center" />
							<itemstyle wrap="false" horizontalalign="Center" />
							<itemtemplate>
								<asp:Label ID="ServerInfoLabel" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
							</itemtemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,WorkQueueNotes %>">
							<headerstyle wrap="false" horizontalalign="Left" />
							<itemstyle wrap="false" horizontalalign="Left" />
							<itemtemplate>
								<asp:Label ID="NotesLabel" runat="server" Text='<%# Eval("Notes") %>' ToolTip='<%# Eval("FullDescription") %>'></asp:Label>
							</itemtemplate>
						</asp:TemplateField>
						</Columns>
						<EmptyDataTemplate>
                            <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No items were found using the provided criteria." />
						</EmptyDataTemplate>
						<RowStyle CssClass="GlobalGridViewRow" />
						<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
						<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
						<HeaderStyle CssClass="GlobalGridViewHeader" />
					</ccUI:GridView>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox runat="server" ID="MessageBox" />

