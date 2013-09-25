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
using Macro.Dicom;
using Macro.Dicom.Utilities;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using SR = Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Patient summary information panel within the <see cref="StudyDetailsPanel"/> 
    /// </summary>
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
            base.DataBind();
            if (_patientSummary != null)
            {

                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;
                
				if (String.IsNullOrEmpty(_patientSummary.PatientsAge))
                    PatientAge.Text = SR.Unknown;
				else
                {
                    string patientAge = _patientSummary.PatientsAge.Substring(0, Math.Min(3,_patientSummary.PatientsAge.Length)).TrimStart('0');
                    if (_patientSummary.PatientsAge.Length > 3)
                    {
                        switch (_patientSummary.PatientsAge.Substring(3))
                        {
                            case "Y":
                                patientAge += " " + SR.Years;
                                break;
                            case "M":
                                patientAge += " " + SR.Months;
                                break;
                            case "W":
                                patientAge += " " + SR.Weeks;
                                break;
                            default:
                                patientAge += " " + SR.Days;
                                break;
                        }

                        if (_patientSummary.PatientsAge.Substring(0, Math.Min(3, _patientSummary.PatientsAge.Length)).
                                Equals("001"))
                            patientAge = patientAge.TrimEnd('s');
                    }
                    PatientAge.Text = patientAge;
				}


            	if (String.IsNullOrEmpty(_patientSummary.Sex))
                    PatientSex.Text = SR.Unknown;
                else
                {
                    if (_patientSummary.Sex.StartsWith("F"))
                        PatientSex.Text = SR.Female;
                    else if (_patientSummary.Sex.StartsWith("M"))
                        PatientSex.Text = SR.Male;
                    else if (_patientSummary.Sex.StartsWith("O"))
                        PatientSex.Text = SR.Other;
                    else
                        PatientSex.Text = SR.Unknown;
                }


                PatientId.Text = _patientSummary.PatientId;

            }

        }

        #endregion Protected methods
    }
}