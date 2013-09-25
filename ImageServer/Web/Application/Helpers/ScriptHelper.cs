#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Macro.ImageServer.Web.Application.Helpers
{
    public class ScriptHelper
    {
        public static string ClearDate(string textBoxID, string calendarExtenderID)
        {
            return "document.getElementById('" + textBoxID + "').value='';" +
                         "$find('" + calendarExtenderID + "').set_selectedDate(null);" +
                         "return false;";
        }

        public static string CheckDateRange(string fromDateTextBoxID, string toDateTextBoxID, string textBoxID, string calendarExtenderID, string message)
        {
            return
                "CheckDateRange(document.getElementById('" + fromDateTextBoxID + "').value, document.getElementById('" +
                toDateTextBoxID + "').value, '" + textBoxID + "' , '" + calendarExtenderID + "' , '" + message + "');";
        }

        public static string PopulateDefaultFromTime(string fromTimeTextBoxID)
        {
            return "if(document.getElementById('" + fromTimeTextBoxID + "').value == '') { document.getElementById('" + fromTimeTextBoxID + "').value = '00:00:00.000'; }";
        }

        public static string PopulateDefaultToTime(string toTimeTextBoxID)
        {
            return "if(document.getElementById('" + toTimeTextBoxID + "').value == '') { document.getElementById('" + toTimeTextBoxID + "').value = '23:59:59.999'; }";
        }

    }
}
