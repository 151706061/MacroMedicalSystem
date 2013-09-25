<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="Macro.Common.Utilities" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Data.DataSource" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.StudyIntegrityQueueItemList"
    CodeBehind="StudyIntegrityQueueItemList.ascx.cs" %>

<%@ Register Src="~/Pages/Queues/StudyIntegrityQueue/SIQEntryTooltip.ascx" TagPrefix="ccAsp" TagName="SIQItemTooltip" %>

<style type="text/css">
    .SIQRowTooltip
    {
    	display:none;
    	background-color:#fff7e2;
    	border: 1px solid #bdcad2;
    	padding:10px; 
    }
</style>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <asp:ObjectDataSource ID="StudyIntegrityQueueDataSourceObject" runat="server" TypeName="Macro.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueDataSource"
                DataObjectTypeName="Macro.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueSummary"
                EnablePaging="true" SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetStudyIntegrityQueueDataSource"
                OnObjectDisposing="DisposeDataSource" />
            <ccUI:GridView ID="StudyIntegrityQueueGridView" runat="server" OnSelectedIndexChanged="StudyIntegrityQueueGridView_SelectedIndexChanged"
                OnPageIndexChanging="StudyIntegrityQueueGridView_PageIndexChanging" SelectionMode="Single"
                OnRowDataBound="StudyIntegrityQueueItemList_RowDataBound"
                TooltipContainerControlID="RowTooltip"
                >
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Panel runat="server" ID="RowTooltip"  CssClass="SIQRowTooltip">
                                <ccAsp:SIQItemTooltip runat="server"  ID="TooltipContent"  />
                            </asp:Panel>
                            <aspAjax:DropShadowExtender runat="server" TrackPosition="true" TargetControlID="RowTooltip" Opacity="0.5" Radius="5" />

                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Reason" HeaderText="" HeaderStyle-HorizontalAlign="Left"
                        ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, StudyInstanceUID %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="ExistingStudyInstanceUidLabel" Text='<%# (bool)Eval("StudyExists")? Eval("StudyInstanceUid"): "N/A" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, SIQExistingStudy%>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <div id="ExistingStudyTable" runat="server" class="SIQItemSummary">
                                <asp:Label runat="server" ID="ExistingPatientId" CssClass="StudyField" Text='<%# Eval("ExistingPatientId")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ExistingPatientName" CssClass="StudyField" Text='<%# NameFormatter.Format(Eval("ExistingPatientName") as string)%>'></asp:Label>
                                <br /><div class="SIQSubItemSummary">
                                <asp:Label runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# Eval("ExistingAccessionNumber")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ExistingStudyDescription" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDescription")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDate")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ExistingModalitiesInStudy" CssClass="StudyField" Text='<%# StringUtilities.EmptyIfNull(Eval("StudySummary.ModalitiesInStudy") as String).Replace("\\", ",") %>'></asp:Label>
                                <asp:Label runat="server" ID="StudyNotAvailableLabel" Text="N/A"></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, SIQConflictingImageInfo %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <div id="ConflictingStudyTable" runat="server" class="SIQItemSummary">
                                <asp:Label runat="server" ID="ConflictingPatientId" CssClass="StudyField" Text='<%# Eval("ConflictingPatientId")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ConflictingPatientName" CssClass="StudyField" Text='<%# NameFormatter.Format(Eval("ConflictingPatientName") as string)%>'></asp:Label>
                                <br /><div class="SIQSubItemSummary">
                                <asp:Label runat="server" ID="ConflictingAccessionNumber" CssClass="StudyField" Text='<%# Eval("ConflictingAccessionNumber")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ConflictingStudyDescription" CssClass="StudyField"
                                    Text='<%# Eval("ConflictingStudyDescription")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ConflictingStudyDate" CssClass="StudyField" Text='<%# Eval("ConflictingStudyDate")%>'></asp:Label>
                                /
                                <asp:Label runat="server" ID="ConflictingModalities" CssClass="StudyField" Text='<%# StringUtilities.Combine(Eval("ConflictingModalities") as string[], ",") %>'></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders, SIQTimeReceived%>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <ccUI:DateTimeLabel ID="TimeReceived" runat="server" Value='<%# Eval("ReceivedTime") %>'></ccUI:DateTimeLabel>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="<%$Resources: SR, NoSIQItemsFound %>" />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow StudyIntegrityQueueRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow StudyIntegrityQueueRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow StudyIntegrityQueueRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
