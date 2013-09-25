<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>


<%@ Import namespace="Macro.ImageServer.Core.Validation"%>
<%@ Import namespace="Macro.ImageServer.Services.WorkQueue"%>
<%@ Import namespace="Macro.ImageServer.Web.Common.Utilities"%>
<%@ Import Namespace="Resources" %>


<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueAlertContextDataView.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Admin.Alerts.WorkQueueAlertContextDataView" %>

<%  WorkQueueAlertContextData data = this.Alert.ContextData as WorkQueueAlertContextData;
    String viewWorkQueueUrl = HtmlUtility.ResolveWorkQueueDetailsUrl(Page, data.WorkQueueItemKey);
    String viewStudyUrl = data.ValidationStudyInfo != null? HtmlUtility.ResolveStudyDetailsUrl(Page, data.ValidationStudyInfo.ServerAE, data.ValidationStudyInfo.StudyInstaneUid):null;
    
%>

<div >
<table class="WorkQueueAlertStudyTable" cellspacing="0" cellpadding="0">
<% if (data.ValidationStudyInfo!=null) { %>
<tr><td style="font-weight: bold; color: #336699"><%=SR.Partition %>:</td><td><%= data.ValidationStudyInfo.ServerAE %>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.PatientName %>:</td><td><pre><%= data.ValidationStudyInfo.PatientsName%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.PatientID %>:</td><td><pre><%= data.ValidationStudyInfo.PatientsId %>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.StudyInstanceUID %>:</td><td><%= data.ValidationStudyInfo.StudyInstaneUid%>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.AccessionNumber %>:</td><td><pre><%= data.ValidationStudyInfo.AccessionNumber%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699"><%=SR.StudyDate %>:</td><td><pre><%= data.ValidationStudyInfo.StudyDate%>&nbsp;</pre></td></tr>
<%} else {%>
<tr><td>
    <%=SR.AlertNoStudyInformationForThisItem %>
</td></tr>
<%} %>

</table>

<table cellpadding="0" cellspacing="0" style="margin-top: 3px;">
    <tr >
        <% if (data.ValidationStudyInfo!=null){%>
        <td><a href='<%=viewStudyUrl%>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;"><%=Labels.WorkQueueAlertContextDataView_ViewStudy%></a></td>
        <td style="font-weight: bold; color: #336699;">|</td>
        <%}%>
        <td><a href='<%= viewWorkQueueUrl %>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;"><%=Labels.WorkQueueAlertContextDataView_ViewWorkQueue%></a></td>
    </tr>
</table>

</div>
