<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsView.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsView" %>

<div class="GridViewBorder">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
<asp:DetailsView ID="StudyDetailView" runat="server" AutoGenerateRows="False" GridLines="Horizontal" CellPadding="4" 
     CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:BoundField DataField="StudyDescription" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDescription%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="AccessionNumber" HeaderText="<%$Resources: DetailedViewFieldLabels, AccessionNumber%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
            
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, ReferringPhysician%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("ReferringPhysiciansName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="StudyInstanceUid" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyInstanceUID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="StudyStatusEnumString" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyStatus%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$Resources: DetailedViewFieldLabels, StudyDateTime%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="StudyDate" runat="server"  Value='<%# Eval("StudyDate") %>' ></ccUI:DALabel>
                <ccUI:TMLabel ID="StudyTime" runat="server"  Value='<%# Eval("StudyTime") %>'  ></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>  
        <asp:BoundField DataField="StudyID" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField Visible="False" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsiblePerson%>" >
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ResponsiblePersonName" runat="server" PersonName='<%# Eval("ResponsiblePerson") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ResponsiblePersonRole" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsiblePersonRole%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="ResponsibleOrganization" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, ResponsibleOrganization%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Species" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, Species%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Breed" Visible="false" HeaderText="<%$Resources: DetailedViewFieldLabels, Breed%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="StudyID" HeaderText="<%$Resources: DetailedViewFieldLabels, StudyID%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="<%$Resources: DetailedViewFieldLabels, SeriesCount%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="<%$Resources: DetailedViewFieldLabels, Instances%>">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
    
</asp:DetailsView>
                    		</asp:TableCell>
	</asp:TableRow>
</div>