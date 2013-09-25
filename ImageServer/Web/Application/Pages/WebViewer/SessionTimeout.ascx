<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.WebViewer.SessionTimeout" %>

<%@ Import namespace="Macro.ImageServer.Web.Common.Security"%>



<script type="text/javascript">
    var countdownTimer;
    var redirectPage = "<%= ResolveClientUrl("~/Pages/Error/WebViewerMultipleStudiesTimeoutErrorPage.aspx") %>";
    var loginId = "<%= HttpContext.Current.User.Identity.Name %>";
    var minCountdownLength = <%= MinCountDownDuration.TotalSeconds %>;
    var timeoutLength = <%= TimeoutLength.TotalSeconds %>;
    var endTime;
    var timeLeft;
    var hideWarning = true;
    Sys.Application.add_load(initCountdownTimer);
       
    function initCountdownTimer(){ 
        hideWarning = true;
        countdownTimer = setTimeout("Countdown()", 2000);
        ResetEndTime();
        document.onmousemove = ResetEndTime;
    }
    
    function ResetEndTime() {
        if(hideWarning) {
            endTime = new Date();
            endTime.setSeconds(endTime.getSeconds() + timeoutLength);
        }
    }
        
    function Countdown()
    {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (updating)
        {
            $("#<%= CountdownEffectPanel.ClientID %>").hide();
		    $("#<%= MessageBanner.ClientID %>").hide();
		    return;
        }
        
        timeLeft = GetSecondsLeft();
                
        if (timeLeft<= 0)
        {
            window.location = redirectPage;
            return;
        }
        else if (timeLeft > minCountdownLength)
        {
            hideWarning = true;
        }
        else
        {
            hideWarning = false;
        }
              
        RefreshWarning();
    }
    
    function GetSecondsLeft()
    {
        return (endTime - new Date())/1000;
    }
        
    function HideSessionWarning()
    {
        hideWarning = true;
        $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
        $("#<%= MessageBanner.ClientID %>").slideUp();
        ResetEndTime();
    }
    
    function RefreshWarning()
    {     
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (!hideWarning && !updating)
        {
            UpdateCountdownPanel();
            $("#<%= CountdownEffectPanel.ClientID %>:hidden").show();
            $("#<%= MessageBanner.ClientID %>:hidden").show();
            
        }
        else
        {
            $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
		    $("#<%= MessageBanner.ClientID %>").slideUp();
        }
        
        if (!updating)
        {
            if (timeLeft > minCountdownLength)
                countdownTimer = setTimeout("Countdown()", (timeLeft-minCountdownLength)*1000 );
            else if (timeLeft>30)
                countdownTimer = setTimeout("Countdown()", 5*1000 /* every 5 seconds */);
            else
                countdownTimer = setTimeout("Countdown()", 1000 /* every second */);
        }
    }
    
    function UpdateCountdownPanel()
    {
        var timeLeft = Math.round(GetSecondsLeft());
        $("#<%= SessionTimeoutWarningMessage.ClientID %>").html("<%= SR.SessionTimeoutCountdownMessage %>".replace("{0}", timeLeft));
    }
    
</script>


<asp:UpdatePanel runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="FixedPos">
            <asp:Panel runat="server" ID="MessageBanner" CssClass="MessageBanner">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                    <td>
                    <asp:Panel runat="server" ID="CountdownBanner" CssClass="CountdownBanner">
                        <asp:Label runat="server" ID="SessionTimeoutWarningMessage" CssClass="SessionTimeoutWarningMessage" style="font-family: Sans-Serif; font-weight: bold"></asp:Label> 
                        <asp:Button runat="server" ID="KeepAliveLink" Text="<%$Resources: Labels,Cancel %>"
                                    Font-Size="12px" UseSubmitBehavior="false" OnClientClick="HideSessionWarning()"></asp:Button>
                    </asp:Panel></td>
                    </tr>
                </table>                
            </asp:Panel>
        </div>
        
        <asp:Panel runat="server" ID="CountdownEffectPanel" Height="40px" CssClass="CountdownEffectPanel"></asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>