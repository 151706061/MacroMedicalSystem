<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>



<%@ Import Namespace="Macro.ImageServer.Core.Edit" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="Macro.Dicom.Utilities.Command" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistoryDetailsColumn.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditHistoryDetailsColumn" %>
<%@ Import Namespace="Resources"%>

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

<div id="SummaryPanel" runat="server">
    <%= ChangeSummaryText %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[<%= Labels.StudyDetails_History_ShowDetails %>]</a>
</div>

<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.StudyDetails_History_Reason %>
            </td>
            <td align="left">
                <%= GetReason(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                <%= Labels.StudyDetails_History_Comment %>
            </td>
            <td align="left">
                <%= GetComment(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" style="padding-top: 8px;">
                <%= Labels.StudyDetails_History_Changes %>
            </td>
            <td align="left">
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>

                    <% if (EditHistory.UpdateCommands == null || EditHistory.UpdateCommands.Count == 0)
                       {%>
    <table class="ChangeHistorySummary" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td>
                    <pre style="padding-left: 10px"><%= SR.StudyDetails_StudyWasNoChanged %></pre>
            </td>
        </tr>
   </table>
                    <%}
                       else
                       {%>
                    <div style="padding: 2px;">                       
                    <table width="100%" cellspacing="0" >
                        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
                            <td>
                                <b><%= ColumnHeaders.Tag%></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.OldValue%></b>
                            </td>
                            <td>
                                <b><%= ColumnHeaders.NewValue%></b>
                            </td>
                        </tr>
                        <%{
                              foreach (BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
                              {
                                  IUpdateImageTagCommand theCmd = cmd as IUpdateImageTagCommand;
                                  if (theCmd != null)
                                  { %><tr style="background: #fefefe">
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.TagPath.Tag) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.OriginalValue) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.Value!=null? theCmd.UpdateEntry.Value.ToString(): "") %></pre>
                                                      </td>
                                                  </tr>
                                <%} %>
                            <%}%>
                        <%}%>
                    </table>
                    </div>
                    <%}%>
</div>
