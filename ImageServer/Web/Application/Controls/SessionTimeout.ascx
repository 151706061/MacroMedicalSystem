<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>

<%@ Import namespace="Macro.ImageServer.Web.Common.Security"%>
<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Controls.SessionTimeout" %>


<script type="text/javascript">
    var countdownTimer;
    var loginPage = "<%= ResolveClientUrl(FormsAuthentication.LoginUrl) %>";
    var redirectPage = "<%= ResolveClientUrl("~/Pages/Error/TimeoutErrorPage.aspx") %>";
    var loginId = "<%= HttpContext.Current.User.Identity.Name %>";
    var minCountdownLength = <%= MinCountDownDuration.TotalSeconds %>;
    var timeLeft;
    var hideWarning = true;
    var loggedOut = false;
    
    Sys.Application.add_load(initCountdownTimer);
    
    function initCountdownTimer(){ 
        hideWarning = true;
        countdownTimer = setTimeout("Countdown()", 2000);
    };
    
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
        
        if (loggedOut)
        {
            Logout();
            return;
        }
        
        
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
        var expiryTime = GetExpiryTime();
               	    
        if (expiryTime==null) {
            window.status  = " [ Session Does Not Exist ]";
            return 0;
        }

        var utcNow = new Date();
        utcNow.setMinutes(utcNow.getMinutes() + utcNow.getTimezoneOffset());
                                             
        timeLeft = Math.round( (expiryTime.getTime() - utcNow.getTime()) / 1000 ) + 1;// give 1 second to ensure when we redirect, the session is really expired

        var displayTimeLeft = new Date();
        displayTimeLeft.setSeconds(displayTimeLeft.getSeconds() + timeLeft);
        window.status  = " [ Session Expiry Time: " + displayTimeLeft.toLocaleString() + " ]";

        return timeLeft;
    }
    
    function GetExpiryTime() {
       
        var cookieName = "<%= SessionManager.GetExpiryTimeCookieName() %>";
        var ca = document.cookie.split(';');
        
        for(var i=0;i < ca.length;i++) {
		    var c = ca[i];
		    while (c.charAt(0)==' ') c = c.substring(1,c.length); // trim leading space
		    if (c.indexOf(cookieName) == 0) {
		        if (c.indexOf('=')<0)
	            {
	                // Expiry time has been removed. This happens when user logs out from another page.
	                // See SessionManager.ForceOtherPagesToLogout()
	                loggedOut = true;
	                return null;
	            }
		        else 
		        {
		            // cookie format:  ImageServer.userid=yyyy-mm-dd hh:mm:ss
		            var cookieValue = c.split('=')[1];
		            return GetDateFromString(cookieValue);
                }
		    }
	    }   
	    return null;    
    }
    
    function Logout()
    {
        window.location = loginPage;
            
    }
    
    function GetDateFromString(value)
    {
        var dateTime = value.split(' ');
        var date = dateTime[0];
        var time = dateTime[1];
                
        date = date.split('-');
        time = time.split(':');
        
        value = new Date();
        
        value.setDate(date[2]);
        value.setMonth(date[1]-1);  //Months start at 0
        value.setFullYear(date[0]);
        value.setHours(time[0]);
        value.setMinutes(time[1]);
        value.setSeconds(time[2]);
                                             
        return value;
    }
    
    function HideSessionWarning()
    {
        hideWarning = true;
        $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
        $("#<%= MessageBanner.ClientID %>").slideUp();
    }
    
    function RefreshWarning()
    {   
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (!hideWarning && !updating)
        {
            UpdateCountdownPanel();
            $("#<%= CountdownEffectPanel.ClientID %>:hidden").show();//.animate({height:"30px"});
            $("#<%= MessageBanner.ClientID %>:hidden").show();
            
        }
        else
        {
            $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
		    $("#<%= MessageBanner.ClientID %>").slideUp();
        }
        
        if (!updating)
        {
            countdownTimer = setTimeout("Countdown()", 1000 );
        }
    }
    
    function UpdateCountdownPanel()
    {
        var timeLeft = GetSecondsLeft();
        //$("#<%= SessionTimeoutWarningMessage.ClientID %>").html("No activity is detected. For security reasons, this session will end in " + timeLeft + " seconds.");        
        $("#<%= SessionTimeoutWarningMessage.ClientID %>").html("<%= SR.SessionTimeoutCountdownMessage %>".replace("{0}", timeLeft));
    }
    
</script>


        
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="KeepAliveLink" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <div class="FixedPos">
            <asp:Panel runat="server" ID="MessageBanner" CssClass="MessageBanner">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                    <td>
                    <asp:Panel runat="server" ID="CountdownBanner" CssClass="CountdownBanner">
                        <asp:Label runat="server" ID="SessionTimeoutWarningMessage" CssClass="SessionTimeoutWarningMessage"></asp:Label> 
                        <asp:Button runat="server" ID="KeepAliveLink" Text="<%$Resources: Labels,Cancel %>" Font-Size="12px" UseSubmitBehavior="false" OnClientClick="HideSessionWarning()"></asp:Button>           
                    </asp:Panel></td>
                    </tr>
                </table>                
            </asp:Panel>
        </div>
        
        <asp:Panel runat="server" ID="CountdownEffectPanel" Height="40px" CssClass="CountdownEffectPanel"></asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>