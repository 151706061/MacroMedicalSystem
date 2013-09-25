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
using Macro.ImageServer.Model;
namespace Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    /// <summary>
    /// study summary information panel within the <see cref="SeriesDetailsPanel"/> 
    /// </summary>
    public partial class StudySummaryPanel : UserControl
    {
        #region private members
        private Study _study;
        #endregion private members


        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="PatientSummary"/> object used by the panel.
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        #endregion Public Properties


        #region Protected methods


        public override void DataBind()
        {
            if (_study != null)
            {
                AccessionNumber.Text = _study.AccessionNumber;
                StudyDescription.Text = _study.StudyDescription;

                StudyDate.Value = _study.StudyDate;
                
                ReferringPhysician.PersonName = _study.ReferringPhysiciansName;
            }


            base.DataBind();
        }

        #endregion Protected methods
    }
}