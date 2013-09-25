<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.ClearCanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
--%>


<%@ Page Language="C#" AutoEventWireup="True" Codebehind="LoginPage.aspx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Login.LoginPage" %>
<%@ Import Namespace="Macro.Common"%>
<%@ Import Namespace="System.Threading"%>
<%@ Import namespace="Macro.ImageServer.Common"%> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="ChangePasswordDialog.ascx" TagName="ChangePasswordDialog" TagPrefix="localAsp" %>
<%@ Register Src="PasswordExpiredDialog.ascx" TagName="PasswordExpiredDialog" TagPrefix="localAsp" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

</head>
<body class="LoginBody" runat="server">
    
    <form runat="server">

    <ccAsp:CheckJavascript runat="server" ID="CheckJavascript" />          

    <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true">
    </asp:ScriptManager>
    
    <script type="text/javascript">        
        function focusOnUserName() 
        {
            $get('<%= UserName.ClientID %>').focus();
        }

        Sys.Application.add_load(focusOnUserName);
    </script>
                
    <asp:Panel ID="LoginSplash" DefaultButton="LoginButton" runat="server" SkinID="<%$ Image : LoginSplash%>">
        
        <div id="VersionInfoPanel">
            <table cellpadding="1">
            <tr><td align="right">
                        <asp:Label runat="server" Text="<%$Resources: Labels,Version %>"></asp:Label>:
                        <asp:Label runat="server" ID="VersionLabel" />
                        <%--<%= String.IsNullOrEmpty(ServerPlatform.VersionString) ? Resources.SR.Unknown : ServerPlatform.VersionString%>--%>
                        </td>
                        </tr>
             <tr><td align="right">
                    <asp:Label runat="server" ID="LanguageLabel" />
                        <%--<%= Thread.CurrentThread.CurrentUICulture.NativeName %>--%></td></tr>
            </table>
        </div>
        
        <div id="WarningLabel">
            <asp:Label runat="server" ID="ManifestWarningTextLabel" CssClass="ManifestWarningTextLabel"></asp:Label>
        </div>
        
        <div id="Copyright">
           <asp:Label runat="server" ID="CopyrightLabel">
            <%--<%= ProductInformation.Copyright %>--%>
           </asp:Label>        
        </div>
    
        <div id="LoginCredentials">
        <table>
            <tr>
            <td align="right"><asp:Label runat="server" Text="<%$Resources: Labels,UserID %>"></asp:Label></td>
            <td align="right"><asp:TextBox runat="server" ID="UserName" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr>
            <tr>
            <td align="right"><asp:Label ID="Label1" runat="server" Text="<%$Resources: Labels,Password %>"></asp:Label></td>
            <td align="right"><asp:TextBox runat="server" ID="Password" TextMode="Password" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr> 
            <tr>
                <td colspan="2" align="right"><asp:Button runat="server" ID="LoginButton" OnClick="LoginClicked"  Text="<%$Resources: Labels,ButtonLogin %>" CssClass="LoginButton"/></td>
            </tr>               
            <tr>
                <td colspan="2" align="right" ><asp:LinkButton ID="LinkButton1" runat="server" CssClass="LoginLink" OnClick="ChangePassword"><asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels,ChangePassword%>"></asp:Label></asp:LinkButton></td>            
            </tr>
        </table>
          
        </div>  
        
        <asp:Panel CssClass="LoginErrorMessagePanel" runat="server" ID="ErrorMessagePanel" 
            Visible='<%# !String.IsNullOrEmpty(Page.Request.QueryString["error"]) %>'>
            <asp:Label runat="server" ID="ErrorMessage" ForeColor="red" Text='<%# Page.Request.QueryString["error"] %>' />
        </asp:Panel>  
                        
            
    </asp:Panel>      
    
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <localAsp:ChangePasswordDialog runat="server" id="ChangePasswordDialog" />
            <localAsp:PasswordExpiredDialog runat="server" id="PasswordExpiredDialog" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    </form>
</body>
</html>
