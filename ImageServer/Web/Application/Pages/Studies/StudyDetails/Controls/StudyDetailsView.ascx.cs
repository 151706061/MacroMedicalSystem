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
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class StudyDetailsView : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;

        private IList<StudySummary> _studies = new List<StudySummary>();

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the list of studies whose information are displayed
        /// </summary>
        public IList<StudySummary> Studies
        {
            get { return _studies; }
            set { _studies = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value;

                StudyDetailView.Width = value;
            }
        }

        public bool DisplayVetTags()
        {
            return Thread.CurrentPrincipal.IsInRole(Enterprise.Authentication.AuthorityTokens.Study.VetTags);            
        }


        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DisplayVetTags())
            {
                foreach (DataControlField o in StudyDetailView.Fields)
                {
                    // TODO: This is a bit of a Hack, need something better for this in the future.
                    var t = o as TemplateField;
                    if (t!=null)
                    {
                        if (t.Visible == false)
                            t.Visible = true;
                        continue;
                    }

                    var f = o as BoundField;
                    if (f == null) continue;

                    if (f.DataField.Equals("ResponsiblePerson"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsiblePersonRole"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsibleOrganization"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Species"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Breed"))
                        f.Visible = true;
                }
            }

            StudyDetailView.DataSource = Studies;
            StudyDetailView.DataBind();            
        }

        #endregion Protected Methods
    }
}