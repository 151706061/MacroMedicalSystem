#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;

public class DicomConstants
{
    public const string DicomDate = "yyyyMMdd";
    public const string DicomDateTime = "YYYYMMDDHHMMSS.FFFFFF";
    public const string DicomSeparator = "^";
    public const string Male = "M";
    public const string Female = "F";
    public const string Other = "O";
    
    public class DicomTags {
        public const string PatientsName = "00100010";
        public const string PatientID = "00100020";
        public const string PatientsBirthDate = "00100030";
        public const string PatientsSex = "00100040";
        public const string PatientsAge = "00101010";
        public const string ReferringPhysician = "00080090";
        public const string StudyDate = "00080020";
        public const string StudyTime = "00080030";
        public const string AccessionNumber = "00080050";
        public const string StudyDescription = "00081030";
        public const string StudyInstanceUID = "0020000D";
        public const string StudyID = "00200010";
        public const string IssuerOfPatientID = "00100021";
    }
}
