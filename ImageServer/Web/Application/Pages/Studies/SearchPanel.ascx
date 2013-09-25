<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.SearchPanel" %>
<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <script type="text/Javascript">

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

            function MultiSelect() {

                $("#<%=ModalityListBox.ClientID %>").multiSelect({
                    selectAllText: "<%= SR.All %>",
                    noneSelected: '',
                    oneOrMoreSelected: '*',
                    dropdownStyle: 'width: 100px;' /* The list is long and there will  be a vertial scrollbar. 
                                            It's ok to set the width explicitly here so that the items are not obscured by the scrollbar. 
                                            Note: the text for the each item in this list does not change */
                });

                $("#<%=StatusListBox.ClientID %>").multiSelect({
                    noneSelected: '',
                    oneOrMoreSelected: '*'
                });
            }
        </script>

        <asp:Table runat="server">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom">
                    <asp:Table runat="server" CellPadding="0" CellSpacing="0">
                        <asp:TableRow>
                            <asp:TableCell runat="server" HorizontalAlign="Left">
                                <asp:Panel runat="server" ID="SearchFieldsContainer" CssClass="SearchPanelContent"
                                    DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <%-- dummy table used to "clear" the default width for inner table tags--%>
                                        <tr>
                                            <td>
                                                <asp:Table ID="Table1" runat="server" CellPadding="0" CellSpacing="0" Width="0%">
                                                    <asp:TableRow>
                                                        <asp:TableCell>
                                                            <asp:Table ID="Table2" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0">
                                                                <asp:TableRow>
                                                                    <asp:TableCell runat="server" ID="OrganizationFilter" HorizontalAlign="left" Visible="False"
                                                                        VerticalAlign="bottom">
                                                                        <asp:Label ID="ResponsibleOrganizationLabel" runat="server" Text="<%$Resources: SearchFieldLabels,ResponsibleOrganization%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="ResponsibleOrganization" runat="server" CssClass="SearchTextBox"
                                                                            ToolTip="<%$Resources: Tooltips,SearchByResponsibleOrganization%>" Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="ResponsiblePersonFilter" HorizontalAlign="left"
                                                                        Visible="False" VerticalAlign="bottom">
                                                                        <asp:Label ID="ResponsiblePersonLabel" runat="server" Text="<%$Resources: SearchFieldLabels,ResponsiblePerson%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="ResponsiblePerson" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByResponsiblePerson%>"
                                                                            Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels,PatientName %>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientName %>"
                                                                            Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, PatientID%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByPatientID%>"
                                                                            Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, AccessionNumber%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="AccessionNumber" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByAccessionNumber%>"
                                                                            Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels,FromDate %>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="false" />
                                                                        <asp:LinkButton ID="ClearFromStudyDateButton" runat="server" Text="X" CssClass="SmallLink"
                                                                            Style="margin-left: 0px;" /><br />
                                                                        <ccUI:TextBox id="FromStudyDate" runat="server" cssclass="SearchDateBox" readonly="true"
                                                                            tooltip="<%$Resources: Tooltips,SearchByStudyDate%>" style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label7" runat="server" Text="<%$Resources: SearchFieldLabels,ToDate %>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="false" />
                                                                        <asp:LinkButton ID="ClearToStudyDateButton" runat="server" Text="X" CssClass="SmallLink"
                                                                            Style="margin-left: 0px;" /><br />
                                                                        <ccUI:textbox id="ToStudyDate" runat="server" cssclass="SearchDateBox" readonly="true"
                                                                            tooltip="<%$Resources: Tooltips,SearchByStudyDate%>" style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels,Description%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="StudyDescription" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips,SearchByDescription%>"
                                                                            Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label9" runat="server" Text="<%$Resources: SearchFieldLabels,ReferringPhysician%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:TextBox ID="ReferringPhysiciansName" runat="server" CssClass="SearchTextBox"
                                                                            ToolTip="<%$Resources: Tooltips,SearchByRefPhysician%>" Style="width: 95px" />
                                                                    </asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label6" runat="server" Text="<%$Resources: SearchFieldLabels,Modality%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:ListBox runat="server" ID="ModalityListBox" SelectionMode="Multiple">
                                                                            <asp:ListItem Value="CR">CR</asp:ListItem>
                                                                            <asp:ListItem Value="CT">CT</asp:ListItem>
                                                                            <asp:ListItem Value="DX">DX</asp:ListItem>
                                                                            <asp:ListItem Value="ES">ES</asp:ListItem>
                                                                            <asp:ListItem Value="KO">KO</asp:ListItem>
                                                                            <asp:ListItem Value="MG">MG</asp:ListItem>
                                                                            <asp:ListItem Value="MR">MR</asp:ListItem>
                                                                            <asp:ListItem Value="NM">NM</asp:ListItem>
                                                                            <asp:ListItem Value="OT">OT</asp:ListItem>
                                                                            <asp:ListItem Value="PR">PR</asp:ListItem>
                                                                            <asp:ListItem Value="PT">PT</asp:ListItem>
                                                                            <asp:ListItem Value="RF">RF</asp:ListItem>
                                                                            <asp:ListItem Value="SC">SC</asp:ListItem>
                                                                            <asp:ListItem Value="US">US</asp:ListItem>
                                                                            <asp:ListItem Value="XA">XA</asp:ListItem>
                                                                        </asp:ListBox>
                                                                    </asp:TableCell>
                                                                    <asp:TableCell ID="TableCell1" runat="server" Ho="left" VerticalAlign="bottom">
                                                                        <asp:Label ID="Label8" runat="server" Text="<%$Resources: SearchFieldLabels,StudyStatus%>"
                                                                            CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                                        <asp:ListBox runat="server" ID="StatusListBox" SelectionMode="Multiple"></asp:ListBox>
                                                                    </asp:TableCell>
                                                                    <asp:TableCell VerticalAlign="bottom">
                                                                        <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel">
                                                                            <ccUI:toolbarbutton id="SearchButton" runat="server" skinid="<%$Image:SearchIcon%>"
                                                                                onclick="SearchButton_Click" />
                                                                        </asp:Panel>
                                                                    </asp:TableCell>
                                                                </asp:TableRow>
                                                            </asp:Table>
                                                        </asp:TableCell>
                                                    </asp:TableRow>
                                                </asp:Table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <ccUI:calendarextender id="FromStudyDateCalendarExtender" runat="server" targetcontrolid="FromStudyDate"
                        cssclass="Calendar">
                        </ccUI:calendarextender>
                    <ccUI:calendarextender id="ToStudyDateCalendarExtender" runat="server" targetcontrolid="ToStudyDate"
                        cssclass="Calendar">
                        </ccUI:calendarextender>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                            <ccUI:toolbarbutton id="ViewImagesButton" runat="server" skinid="<%$Image:ViewImagesButton%>" />
                                            <ccUI:toolbarbutton id="ViewStudyDetailsButton" runat="server" skinid="<%$Image:ViewDetailsButton%>" />
                                            <ccUI:toolbarbutton id="MoveStudyButton" runat="server" skinid="<%$Image:MoveButton%>" />
                                            <ccUI:toolbarbutton id="DeleteStudyButton" runat="server" skinid="<%$Image:DeleteButton%>"
                                                onclick="DeleteStudyButton_Click" />
                                            <ccUI:toolbarbutton id="RestoreStudyButton" runat="server" skinid="<%$Image:RestoreButton%>"
                                                onclick="RestoreStudyButton_Click" />
                                            <ccUI:toolbarbutton id="AssignAuthorityGroupsButton" runat="server" skinid="<%$Image:AddDataAccessButton%>"
                                                onclick="AssignAuthorityGroupsButton_Click" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel2" runat="server" CssClass="SearchPanelResultContainer">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <ccAsp:GridPager id="GridPagerTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: white;">
                                                <localAsp:StudyListGridView id="StudyListGridView" runat="server" height="500px"></localAsp:StudyListGridView>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
<ccAsp:messagebox id="MessageBox" runat="server" />
<ccAsp:messagebox id="RestoreMessageBox" runat="server" />
<ccAsp:messagebox id="ConfirmStudySearchMessageBox" runat="server" messagetype="YESNO" />
