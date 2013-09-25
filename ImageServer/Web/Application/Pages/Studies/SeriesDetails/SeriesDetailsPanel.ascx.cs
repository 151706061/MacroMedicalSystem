#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;

namespace Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Panel within the <see cref="SeriesDetailsPage"/>
    /// </summary>
    public partial class SeriesDetailsPanel : System.Web.UI.UserControl
    {
        #region Private members
        private Model.Study _study;
        private Model.Series _series;
        #endregion Private members


        #region Public Properties

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Properties

        public override void DataBind()
        {
            if (_study != null)
            {
                PatientSummary.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(_study);

                StudySummary.Study = _study;

                if (Series != null)
                    SeriesDetails.Series = _series;

            }

            base.DataBind();
        }
        #endregion Protected Properties

    }
}