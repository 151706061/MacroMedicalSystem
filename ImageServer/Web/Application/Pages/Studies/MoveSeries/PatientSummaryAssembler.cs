#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    static public class PatientSummaryAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="PatientSummary"/> for a <see cref="Study"/>.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public PatientSummary CreatePatientSummary(Study study)
        {
            PatientSummary patient = new PatientSummary();

            patient.PatientId = study.PatientId;
            patient.Birthdate = study.PatientsBirthDate;
            patient.PatientName = study.PatientsName;
            patient.Sex = study.PatientsSex;

            PatientAdaptor adaptor = new PatientAdaptor();
            Patient pat = adaptor.Get(study.PatientKey);
            patient.IssuerOfPatientId = pat.IssuerOfPatientId;
            return patient;
        }
    }
}
