<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudyList.aspx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.WebViewer.StudyList"%>

<%@ Import Namespace="Resources" %>

<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<%@ Register Src="SearchPanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>
<%@ Register Src="SessionTimeout.ascx" TagName="SessionTimeout" TagPrefix="ccAsp" %>
<%@ Register Src="JQuery.ascx" TagName="JQuery" TagPrefix="localAsp" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <localAsp:JQuery ID="MainJQuery" MultiSelect="true" Effects="true" runat="server" />
</head>
<body style="background: #333333; margin: 0px;">
    <form id="form1" runat="server">
    
        <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true" OnAsyncPostBackError="GlobalScriptManager_AsyncPostBackError" >
        </asp:ScriptManager>   
        
        <ccAsp:SessionTimeout runat="server" ID="SessionTimeout" />
   
        <div>
            <div><table width="100%" cellpadding="0" cellspacing="0"><tr><td style="padding-bottom: 2px">
            <asp:Image ID="Image1" ImageUrl="~/Pages/WebViewer/Images/StudiesPageLogo.png" runat="server" /></td>
            <td valign="bottom" align="right" style="padding-bottom: 2px; padding-right: 5px;">
            <span style="color: #dddddd; font-family: Sans-Serif; font-weight: bold">
                <%= SR.WebViewerMutipleMatchingStudiesFound%>
            </span></td></tr></table></div>
            <asp:Panel ID="Panel2" runat="server" style="border-top: solid 2px #999999;">
                <localAsp:SearchPanel ID="SearchPanel" runat="server"></localAsp:SearchPanel>
            </asp:Panel>
    </div>
    </form>
</body>
</html>
