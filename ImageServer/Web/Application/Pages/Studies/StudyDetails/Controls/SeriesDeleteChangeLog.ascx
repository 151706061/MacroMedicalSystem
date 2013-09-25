<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDeleteChangeLog.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesDeleteChangeLog" %>
<%@ Import Namespace="Resources"%>

<%@ Import Namespace="Macro.ImageServer.Core.Edit" %>
<%@ Import Namespace="Macro.ImageServer.Core.Data" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="Macro.Dicom.Utilities.Command" %>

<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=HistoryDetailsPanel.ClientID%>").hide();
        $("#<%=ShowHideDetails.ClientID%>").click(function() {
        if ($("#<%=ShowHideDetails.ClientID%>").text() == "[<%= Labels.StudyDetails_History_ShowDetails %>]") {
                $("#<%=HistoryDetailsPanel.ClientID%>").show();
                $("#<%=ShowHideDetails.ClientID%>").text("[<%= Labels.StudyDetails_History_HideDetails %>]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "bold");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "5px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            } else {
                $("#<%=HistoryDetailsPanel.ClientID%>").hide();
                $("#<%=ShowHideDetails.ClientID%>").text("[<%= Labels.StudyDetails_History_ShowDetails %>]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "normal");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "0px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            }
            return false;
        });
    });

</script>

<div runat="server" id="SummaryPanel">
    <%= ChangeSummaryText %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>
<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.SeriesDeleteDialog_Reason %>
            </td>
            <td align="left">
                <%= GetReason(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.SeriesDeleteDialog_Comment%>
            </td>
            <td align="left">
                <%= GetComment(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" colspan="2" nowrap="nowrap" style="padding-top: 8px;">
                <%= Labels.StudyDetails_History_SeriesDeleted %>
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>
    <div style="padding: 2px;">
    <table width="100%" cellspacing="0">
        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
            <td>
                <b><%= ColumnHeaders.SeriesDescription %></b>
            </td>
            <td>
                <b><%= ColumnHeaders.Modality%></b>
            </td>
            <td>
                <b><%= ColumnHeaders.Instances%></b>
            </td>
        </tr>
        <% foreach (SeriesInformation series in ChangeLog.Series)
           {%>
        <tr style="background: #fefefe">
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.SeriesDescription %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.Modality %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.NumberOfInstances %>
            </td>
        </tr>
        <% }%>
    </table>
    </div>
</div>