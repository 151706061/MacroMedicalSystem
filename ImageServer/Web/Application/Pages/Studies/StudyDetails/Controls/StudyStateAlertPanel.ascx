<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyStateAlertPanel.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyStateAlertPanel" %>
<asp:Panel CssClass="StudyDetailsMessage" runat="server" ID="MessagePanel" Visible="true">
<asp:Label ID="Message" runat="Server" Text="" />
</asp:Panel>