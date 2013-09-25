<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteWorkQueueDialog.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.DeleteWorkQueueDialog" %>

<ccAsp:MessageBox ID="PreDeleteConfirmDialog" runat="server"  Title="<%$Resources: Titles, DeleteWorkQueueConfirmationDialogTitle %>"/>
<ccAsp:MessageBox ID="MessageBox" runat="server" MessageType="INFORMATION">
</ccAsp:MessageBox>