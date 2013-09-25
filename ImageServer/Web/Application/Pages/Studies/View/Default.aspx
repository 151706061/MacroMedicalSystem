<%-- License
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Studies.View.Default" %>
<%@ Import Namespace="Macro.ImageServer.Web.Common.Utilities"%>
<%@ Import Namespace="Macro.ImageServer.Web.Application.Pages.Studies.View"%>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Macro.ImageViewer.Web.Client.Silverlight</title>
	<style type="text/css">
		html, body
		{
			height: 100%;
			overflow: auto;
		}
		body
		{
			padding: 0;
			margin: 0;
		}
		#silverlightControlHost
		{
			height: 100%;
			text-align: center;
		}
	</style>	
</head>
<body>
	<form id="form2" runat="server" style="height: 100%">
	    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript" src="SplashScreen/SplashScreen.js"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery/jquery-1.3.2.min.js") %>"></script>	

	<script type="text/javascript">
	    function onSilverlightError(sender, args) {
	        var appSource = "";
	        if (sender != null && sender != 0) {
	            appSource = sender.getHost().Source;
	        }

	        var errorType = args.ErrorType;
	        var iErrorCode = args.ErrorCode;

	        if (errorType == "ImageError" || errorType == "MediaError") {
	            return;
	        }

	        var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

	        errMsg += "Code: " + iErrorCode + "    \n";
	        errMsg += "Category: " + errorType + "       \n";
	        errMsg += "Message: " + args.ErrorMessage + "     \n";

	        if (errorType == "ParserError") {
	            errMsg += "File: " + args.xamlFile + "     \n";
	            errMsg += "Line: " + args.lineNumber + "     \n";
	            errMsg += "Position: " + args.charPosition + "     \n";
	        }
	        else if (errorType == "RuntimeError") {
	            if (args.lineNumber != 0) {
	                errMsg += "Line: " + args.lineNumber + "     \n";
	                errMsg += "Position: " + args.charPosition + "     \n";
	            }
	            errMsg += "MethodName: " + args.methodName + "     \n";
	        }

	        throw new Error(errMsg);
	    }

	    function OnSilverlightAppLoaded(sender, args) {
	        var silverlightControlHost = document.getElementById("SilverlightObject");

	        // Attach event handlers
	        silverlightControlHost.Content.ApplicationBridge.ViewerSessionUpdated = ViewerSessionUpdated;
	    }

	    function ViewerSessionUpdated(sender, args) {
	        updateImageServerSession(args.ExpiryTimeUtc);
	    }

	    function updateImageServerSession(expiryTimtUtc) {
	        // by loading the page, we'll update the session and prevent all other
	        // pages from timing-out
	        var url = '<%= Page.ResolveClientUrl("~/KeepSessionAlive.aspx") %>' + "?guid=" + createUUID();
	        $.post(url, function (data) {
	            // throw it away
	        });
	    }

	    function createUUID() {
	        // http://www.ietf.org/rfc/rfc4122.txt
	        var s = [];
	        var hexDigits = "0123456789ABCDEF";
	        for (var i = 0; i < 32; i++) {
	            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
	        }
	        s[12] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
	        s[16] = hexDigits.substr((s[16] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01

	        var uuid = s.join("");
	        return uuid;
	    }        
	</script>
	<div id="silverlightControlHost">
		<object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
			width="100%" height="100%"  id="SilverlightObject">
			<!-- add random string to the end to force the browser not to cache so that the splash screen always shows up-->
			<param name="source" value="../../../ClientBin/Silverlight.xap?<%= Guid.NewGuid().ToString() %>" />
	        <param name="SplashScreenSource" value="<%= EmbeddedWebStationHelper.GetSplashScreenXamlAbsolutePath() %>?<%= Guid.NewGuid().ToString() %>" />
            <param name="onSourceDownloadProgressChanged" value="DownloadProgress" />
			<param name="onLoad" value="OnSilverlightAppLoaded" />
			<param name="onError" value="onSilverlightError" />
			<param name="background" value="#222222" />
			<!-- mouse wheel does not work if windowless is true -->
			<param name="windowless" value="false" />
			<param name="minRuntimeVersion" value="4.0.41108.0" />
			<param name="autoUpgrade" value="true" />
			<param name="Culture" value="<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>" />
			<param name="UICulture" value="<%=System.Threading.Thread.CurrentThread.CurrentUICulture.Name %>" />
			<param name="initParams" value="<%= WebServiceConfiguration.InitParamString %>,<%= UserCredentialsString %>,<%= ApplicationSettings %>,<%= OtherParameters %>" />
			<a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration: none">
				<img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
					style="border-style: none" />
			</a>
		</object><!-- note: any space or newline after this will cause vertical scroll bar displayed in IE8 when Compatibility View mode is turned off--><iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe></div>
	</form>
</body>
</html>