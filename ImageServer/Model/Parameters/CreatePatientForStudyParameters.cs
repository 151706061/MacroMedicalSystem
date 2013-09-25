#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Macro.Dicom;
using Macro.ImageServer.Enterprise;

namespace Macro.ImageServer.Model.Parameters
{
    public class CreatePatientForStudyParameters : ProcedureParameters
    {
        public CreatePatientForStudyParameters()
            : base("CreatePatientForStudy")
        {
        }

        public ServerEntityKey StudyKey
        {
            set { SubCriteria["StudyKey"] = new ProcedureParameter<ServerEntityKey>("StudyKey", value); }
        }

        [DicomField(DicomTags.PatientsName)]
        public string PatientsName
        {
            set { SubCriteria["PatientsName"] = new ProcedureParameter<string>("PatientsName", value); }
        }

        [DicomField(DicomTags.PatientId)]
        public string PatientId
        {
            set { SubCriteria["PatientId"] = new ProcedureParameter<string>("PatientId", value); }
        }

        [DicomField(DicomTags.IssuerOfPatientId)]
        public string IssuerOfPatientId
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<string>("IssuerOfPatientId", value); }
        }

        [DicomField(DicomTags.SpecificCharacterSet)]
        public string SpecificCharacterSet
        {
            set { SubCriteria["SpecificCharacterSet"] = new ProcedureParameter<string>("SpecificCharacterSet", value); }
        }

    }
}
