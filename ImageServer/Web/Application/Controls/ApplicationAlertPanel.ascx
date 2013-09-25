<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

--%>


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ApplicationAlertPanel.ascx.cs"
    Inherits="Macro.ImageServer.Web.Application.Controls.ApplicationAlertPanel" %>
<%@ Import Namespace="Resources" %>


   <!--[if gte IE 5.5]>
    <![if lt IE 7]>
    <style type="text/css">
        .FixedPos {
            /* used by  IE6 */
	        left: expression( ( ( ignoreMe2 = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ) ) + 'px' );
            top: expression( ( ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ) ) + 'px' );
            position: absolute;
//            z-index: 10000000;
        }
        
    </style>
    <![endif]>
    <![endif]-->

    <!--[if IE]>
    <style type="text/css">
        .FixedPos {
            /* used by  IE6 */
	        position: fixed;
//            z-index: 10000000;
        }
        
        .ScreenBlocker {
            /* used by  IE */
            left: expression( ( ( ignoreMe2 = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ) ) + 'px' );
            top: expression( ( ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ) ) + 'px' );
            width:expression( ( ( ignoreMe = document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body.clientWidth ) ) + 'px' );
            height:expression( ( ( ignoreMe = document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight ) ) + 'px' );
            position:absolute;
            background-color:white;
	        filter:alpha(opacity=0);
	
        }        

        html,body {height:100%}    
    </style>
    <![endif]-->
    
    <!--[if !IE]>
    <style type="text/css">
        .ScreenBlocker {
            /* used by  IE */
            left: 0;
            top:0;
            width:100%;
            height:100%;
            position:absolute;
            background-color:white;
	        opacity: 0; /* Safari, Opera */
	        -moz-opacity: 0; /* FireFox */
        }   
    </style>
    <![endif]-->

<script type="text/javascript">

function RaiseAppAlert(html, stay)
        {
            $("#<%= AppAlertMessagePanel.ClientID %>").html(html).slideDown("slow",
                function()
                {
                    setTimeout(function(){ $("#<%= AppAlertMessagePanel.ClientID %>").hide();}, stay);
                }
            );
        }

</script>

<div class="FixedPos">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:Panel runat="server" ID="AppAlertMessagePanel" CssClass="AppAlertMessagePanel" />
            </td>
        </tr>
    </table>
</div>
