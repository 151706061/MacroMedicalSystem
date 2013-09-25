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
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.Utilities;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    public partial class PatientSummaryPanel : UserControl
    {
        #region private members
        private PatientSummary _patientSummary;
        #endregion private members

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="PatientSummary"/> object used by the panel.
        /// </summary>
        public PatientSummary PatientSummary
        {
            get { return _patientSummary; }
            set { _patientSummary = value; }
        }

        #endregion Public Properties


        #region Protected methods

        public override void DataBind()
        {
            if (_patientSummary != null)
            {
                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;

                DateTime? bdate = DateParser.Parse(_patientSummary.Birthdate);

                if (bdate!=null)
                {
                    //TODO: Fix this. The patient's age should not changed whether the study is viewed today or next year.
                    TimeSpan age = Platform.Time - bdate.Value;
                    if (age > TimeSpan.FromDays(365))
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays / 365, SR.Years);
                    }
                    else if (age > TimeSpan.FromDays(30))
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays / 30, SR.Months);
                    }
                    else
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays, SR.Days);
                    }
                }
                else
                {
                    PatientAge.Text = SR.Unknown;
                }


                if (String.IsNullOrEmpty(_patientSummary.Sex))
                    PatientSex.Text = SR.Unknown;
                else
                {
                    if (_patientSummary.Sex.StartsWith("F"))
                        PatientSex.Text = SR.Female;
                    else if (_patientSummary.Sex.StartsWith("M"))
                        PatientSex.Text = SR.Male;
                    else
                        PatientSex.Text = SR.Unknown;
                }


                PatientId.Text = _patientSummary.PatientId;

            }

            base.DataBind();
        }

        #endregion Protected methods
    }
}