#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Represents a datetime label control, which displays date/time on a Web page.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="DateTimeLabel"/> to display date time on a web page. The date/time to be displayed is set through 
    /// the <see cref="Value"/> property. The format of the date/time can be set through the <see cref="Format"/> property. If <see cref="Format"/> 
    /// is not set, the date/time format will set to one of the followings, in the listed order:
    /// 
    /// - The date and time formats specified in the web configuration. 
    /// - The default date/time formats for the UI culture specified in <globalization>
    /// - The default date/time format for the region setting of the system. For eg, for English (Canada) region, 
    /// the default Long date format is "MMMM dd, yyyy". For US, the default date format is "DDD, MMMM dd, yyyy"
    /// 
    /// </remarks>
    /// <example>
    /// The following example illustrate how to use <see cref="DateTimeLabel"/> to display a date in MMM/dd/yyyy format:
    /// 
    /// <code>
    /// 
    /// <%@ Register Assembly="Macro.ImageServer.Web.Common" Namespace="Macro.ImageServer.Web.Common.WebControls.UI" TagPrefix="Macro" %>
    /// ...
    /// <Macro:DateTimeLabel ID="Today" runat="server" ForeColor="white" Format="MMM/dd/yyyy" EmptyValueText="Unknown"></Macro:DateTimeLabel>
    /// 
    /// 
    /// </code>
    /// </example>
    [DefaultProperty("Value")]
    [ToolboxData("<{0}:DateTimeLabel runat=server></{0}:DateTimeLabel>")]
    public class DateTimeLabel : Label
    {

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public DateTime? Value
        {
            get
            {
                return ViewState["Value"] as DateTime?;
            }
            set
            {
                ViewState["Value"] = value;
            }
        }


        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Format
        {
            get {
                return ViewState["Format"] as string;
                
            }
            set {
                ViewState["Format"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            string text = GetRenderText();
            if (!String.IsNullOrEmpty(text))
                writer.Write(text);
        }
        
        protected string GetRenderText()
        {
            if (String.IsNullOrEmpty(Text))
                return GetRenderedDateTimeText();
            else
                return String.Format(Text, GetRenderedDateTimeText());
        }

        protected virtual string GetRenderedDateTimeText()
        {
            DateTime? datetime= Value;

            if (datetime != null)
            {
                if (!String.IsNullOrEmpty(Format))
                    return DateTimeFormatter.Format(datetime.Value, Format);
                else
                    return DateTimeFormatter.Format(datetime.Value);
            }
            else
                return null;
            
            
        }
    }
}
