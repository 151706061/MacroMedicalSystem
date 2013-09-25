#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

namespace Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Model object behind the <see cref="PatientSummaryPanel"/>
    /// </summary>
    public class PatientSummary
    {
        #region Private members
        private string _patientId;
        private string _issuerOfPatientId;
        private string _patientName;
        private string _birthdate;
        private string _sex;

        #endregion Private members

        #region Public Properties
        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public string Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        #endregion Public Properties
    }
}
