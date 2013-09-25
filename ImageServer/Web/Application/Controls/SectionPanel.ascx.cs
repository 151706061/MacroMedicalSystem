#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// Information Section Panel.
    /// </summary>
    /// <remarks>
    /// A <see cref="SectionPanel"/> is a panel with heading on top. The heading text and style can be set via <see cref="HeadingText"/> and <see cref="HeadingCSS"/>. 
    /// The content of the section is specified in <see cref="SectionContentTemplate"/>. The controls in the content area can be accessed
    /// using <see cref="SectionContentContainer"/> FindControls() method.
    /// 
    /// <example>
    /// 
    /// <code>
    /// <%@ Register Src="~/Common/Controls/SectionPanel.ascx" TagName="SectionPanel" TagPrefix="uc4" %>
    /// ...
    /// <uc4:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="Button Section" HeadingCSS="CSSStudyHeading"
    ///    Width="100%" CssClass="CSSSection">
    ///    <SectionContentTemplate>
    ///        <asp:Button ID="Button1" Text="Click here"/>
    ///    </SectionContentTemplate>
    ///</uc4:SectionPanel>
    /// 
    /// ....
    /// Button b = (Button) StudySectionPanel.SectionContentContainer.FindControl("Button1");
    /// 
    /// 
    /// </code>
    /// </example>
    /// </remarks>
    public partial class SectionPanel : UserControl
    {
        /// <summary>
        /// The template container class
        /// </summary>
        [ParseChildren(true)]
        private class SectionContentContainer : Control, INamingContainer
        {

        }

        #region Private Members
        private Unit _width;
        private string _cssClass;
    	private ITemplate _contentTemplate = null;
        private readonly SectionContentContainer _contentContainer = new SectionContentContainer();
        #endregion Private Members

        #region Public Properties
        [TemplateContainer(typeof(SectionContentContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate SectionContentTemplate
        {
            get
            {
                return _contentTemplate;
            }
            set
            {
                _contentTemplate = value;
            }
        }

        public Unit Width
        {
            get { return _width; }
            set { 
                _width = value;
                Container.Width = value;
            }
        }

        public string CssClass
        {
            get { return _cssClass; }
            set
            {
                _cssClass = value;
                Container.CssClass = value;
            }
        }

        public override Control FindControl(string id)
        {
            Control ctrl = base.FindControl(id);
            if (ctrl == null)
                ctrl = _contentContainer.FindControl(id);

            return ctrl;
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _contentContainer.ID = "SectionContentContainer";
            _contentTemplate.InstantiateIn(_contentContainer);
            placeholder.Controls.Add(_contentContainer);
        }

        #endregion Protected Methods

    }
}