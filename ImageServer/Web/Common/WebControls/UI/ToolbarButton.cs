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
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.Common.Utilities;


[assembly: WebResource("Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js", "text/javascript")]

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    public enum NoPermissionVisibility
    {
        Invisible,
        Visible
    }

    [ToolboxData("<{0}:ToolbarButton runat=server></{0}:ToolbarButton>")]
    [Themeable(true)]
    public class ToolbarButton : ImageButton, IScriptControl
    {
        #region Private Members
        private string _roleSeparator = ",";
        private NoPermissionVisibility _noPermissionVisibilityMode;
        #endregion

        #region Public Properties

        /// <summary>
		/// Specifies the roles which users must have to access to this button.
		/// </summary>
        public string Roles
        {
            get
            {
                return ViewState["Roles"] as string;
            }
            set
            {
                ViewState["Roles"] = value;
            }
        }

		/// <summary>
		/// Specifies the visiblity of the button if the user doesn't have the roles specified in <see cref="Roles"/>
		/// </summary>
        public NoPermissionVisibility NoPermissionVisibilityMode
        {
            get { return _noPermissionVisibilityMode; }
            set { _noPermissionVisibilityMode = value; }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is enabled.
        /// </summary>
        public string EnabledImageURL 
        {
            get
            {
                String s = (String)ViewState["EnabledImageURL"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["EnabledImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the button enabled and user hovers the mouse over the button.
        /// </summary>
        public string HoverImageURL
        {
            get
            {
                String s = (String)ViewState["HoverImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["HoverImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the mouse button is clicked.
        /// </summary>
        public string ClickedImageURL
        {
            get
            {
                String s = (String)ViewState["ClickedImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["ClickedImageURL"] = inspectURL(value);
            }
        }   

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is disabled.
        /// </summary>
        public string DisabledImageURL
        {
            get
            {
                String s = (String)ViewState["DisabledImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["DisabledImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to seperate values in the <see cref="Roles"/> property.
        /// </summary>
        public string RoleSeparator
        {
            get { return _roleSeparator; }
            set { _roleSeparator = value; }
        }

        #endregion Public Properties

        #region Private Methods

        private string inspectURL(string value)
        {
            if (!value.StartsWith("~/") && !value.StartsWith("/")) 
                value = value.Insert(0, "~/App_Themes/" + Page.Theme + "/");
            
            return value;
        }       

        #endregion Private Methods


        public override void  RenderControl(HtmlTextWriter writer)
        {
            if (Enabled)
                ImageUrl = EnabledImageURL;
            else
                ImageUrl = DisabledImageURL;

 	        base.RenderControl(writer);
        }


        #region IScriptControl Members
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor("Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton", ClientID);
            desc.AddProperty("EnabledImageUrl", Page.ResolveClientUrl(EnabledImageURL));
            desc.AddProperty("DisabledImageUrl", Page.ResolveClientUrl(DisabledImageURL));
            desc.AddProperty("HoverImageUrl", Page.ResolveClientUrl(HoverImageURL));
            desc.AddProperty("ClickedImageUrl", Page.ResolveClientUrl(ClickedImageURL));

            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(typeof(ToolbarButton), "Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js");
            return new ScriptReference[] { reference };
        }

        #endregion IScriptControl Members

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);
            }

            if (String.IsNullOrEmpty(Roles)==false)
            {
                string[] roles = Roles.Split(new String[]{ RoleSeparator}, StringSplitOptions.RemoveEmptyEntries);
                bool allow = CollectionUtils.Contains(roles,
                                delegate(string role)
                                 {
                                     return Thread.CurrentPrincipal.IsInRole(role.Trim());
                                 });

                if (!allow)
                {
                    Enabled = false;
                    Visible = NoPermissionVisibilityMode!=NoPermissionVisibility.Invisible;
                }
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }
       
    }
}
