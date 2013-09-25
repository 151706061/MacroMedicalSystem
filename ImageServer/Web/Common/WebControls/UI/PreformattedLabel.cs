#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:PreformattedLabel runat=server></{0}:PreformattedLabel>")]
    public class PreformattedLabel : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            HtmlGenericControl ctrl = new HtmlGenericControl("PRE");
            ctrl.Attributes["class"] = CssClass;
            ctrl.InnerText = Text;
            ctrl.RenderControl(output);
        }
    }
}
