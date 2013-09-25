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
using System.Runtime.Serialization;
using Macro.Dicom.Iod;

namespace Macro.Dicom.ServiceModel.Query
{
    public interface IStudyRootStudyIdentifier : IStudyRootData, IStudyIdentifier
    {
    }

    /// <summary>
    /// Study Root Query identifier for a study.
    /// </summary>
    [DataContract(Namespace = QueryNamespace.Value)]
    public class StudyRootStudyIdentifier : StudyIdentifier, IStudyRootStudyIdentifier
    {
        #region Private Fields

        #endregion

        #region Public Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StudyRootStudyIdentifier()
        {
        }

        public StudyRootStudyIdentifier(IStudyRootStudyIdentifier other)
            : base(other)
        {
            CopyFrom(other);
        }

        public StudyRootStudyIdentifier(IStudyRootData other, IIdentifier identifier)
            : base(other, identifier)
        {
            CopyFrom(other);
        }

        public StudyRootStudyIdentifier(IStudyRootData other)
            : base(other)
        {
            CopyFrom(other);
        }

        public StudyRootStudyIdentifier(IPatientData patientData, IStudyIdentifier identifier)
            : base(identifier)
        {
            CopyFrom(patientData);
        }

        public StudyRootStudyIdentifier(IPatientData patientData, IStudyData studyData, IIdentifier identifier)
            : base(studyData, identifier)
        {
            CopyFrom(patientData);
        }

        /// <summary>
        /// Creates an instance of <see cref="StudyRootStudyIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        public StudyRootStudyIdentifier(DicomAttributeCollection attributes)
            : base(attributes)
        {
        }

        #endregion

        private void CopyFrom(IPatientData other)
        {
            if (other == null)
                return;

            PatientId = other.PatientId;
            PatientsName = other.PatientsName;
            PatientsBirthDate = other.PatientsBirthDate;
            PatientsBirthTime = other.PatientsBirthTime;
            PatientsSex = other.PatientsSex;

            PatientSpeciesDescription = other.PatientSpeciesDescription;
            PatientSpeciesCodeSequenceCodingSchemeDesignator = other.PatientSpeciesCodeSequenceCodingSchemeDesignator;
            PatientSpeciesCodeSequenceCodeValue = other.PatientSpeciesCodeSequenceCodeValue;
            PatientSpeciesCodeSequenceCodeMeaning = other.PatientSpeciesCodeSequenceCodeMeaning;
            PatientBreedDescription = other.PatientBreedDescription;
            PatientBreedCodeSequenceCodingSchemeDesignator = other.PatientBreedCodeSequenceCodingSchemeDesignator;
            PatientBreedCodeSequenceCodeValue = other.PatientBreedCodeSequenceCodeValue;
            PatientBreedCodeSequenceCodeMeaning = other.PatientBreedCodeSequenceCodeMeaning;
            ResponsiblePerson = other.ResponsiblePerson;
            ResponsiblePersonRole = other.ResponsiblePersonRole;
            ResponsibleOrganization = other.ResponsibleOrganization;
        }

        public override string ToString()
        {
            return String.Format("{0} | {1} | {2}", PatientsName, PatientId, base.ToString());
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the patient id of the identified study.
        /// </summary>
        [DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = false)]
        public string PatientId { get; set; }

        /// <summary>
        /// Gets or sets the patient's name for the identified study.
        /// </summary>
        [DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = false)]
        public string PatientsName { get; set; }

        /// <summary>
        /// Gets or sets the patient's birth date for the identified study.
        /// </summary>
        [DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = false)]
        public string PatientsBirthDate { get; set; }

        /// <summary>
        /// Gets or sets the patient's birth time for the identified study.
        /// </summary>
        [DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = false)]
        public string PatientsBirthTime { get; set; }

        /// <summary>
        /// Gets or sets the patient's sex for the identified study.
        /// </summary>
        [DicomField(DicomTags.PatientsSex, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = false)]
        public string PatientsSex { get; set; }

        #region Species

        [DicomField(DicomTags.PatientSpeciesDescription)]
        [DataMember(IsRequired = false)]
        public string PatientSpeciesDescription { get; set; }

        [DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientSpeciesCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; set; }

        [DicomField(DicomTags.CodeValue, DicomTags.PatientSpeciesCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientSpeciesCodeSequenceCodeValue { get; set; }

        [DicomField(DicomTags.CodeMeaning, DicomTags.PatientSpeciesCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientSpeciesCodeSequenceCodeMeaning { get; set; }

        #endregion

        #region Breed

        [DicomField(DicomTags.PatientBreedDescription)]
        [DataMember(IsRequired = false)]
        public string PatientBreedDescription { get; set; }

        [DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientBreedCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientBreedCodeSequenceCodingSchemeDesignator { get; set; }

        [DicomField(DicomTags.CodeValue, DicomTags.PatientBreedCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientBreedCodeSequenceCodeValue { get; set; }

        [DicomField(DicomTags.CodeMeaning, DicomTags.PatientBreedCodeSequence)]
        [DataMember(IsRequired = false)]
        public string PatientBreedCodeSequenceCodeMeaning { get; set; }

        #endregion

        #region Responsible Person/Organization

        [DicomField(DicomTags.ResponsiblePerson)]
        [DataMember(IsRequired = false)]
        public string ResponsiblePerson { get; set; }

        [DicomField(DicomTags.ResponsiblePersonRole)]
        [DataMember(IsRequired = false)]
        public string ResponsiblePersonRole { get; set; }

        [DicomField(DicomTags.ResponsibleOrganization)]
        [DataMember(IsRequired = false)]
        public string ResponsibleOrganization { get; set; }

        #endregion

        #endregion
    }
}