<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="InvalidInputIndicator.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Controls.InvalidInputIndicator" %>

<asp:Panel ID="ContainerPanel" runat="server">
    <asp:Image ID="Image" runat="server" />
    <asp:Panel ID="HintPanel" runat="server" Wrap="true" CssClass="InvalidIndicatorTooltipPanel">
        <asp:Label ID="HintLabel" runat="server" Text="Label" />
    </asp:Panel>
    <aspAjax:HoverMenuExtender ID="HoverMenuExtender1" runat="server" OffsetX="20" OffsetY="20"
        PopDelay="100" PopupControlID="HintPanel" TargetControlID="Image">
    </aspAjax:HoverMenuExtender>
</asp:Panel>
