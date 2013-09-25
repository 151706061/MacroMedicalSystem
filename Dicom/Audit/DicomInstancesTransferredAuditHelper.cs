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

using Macro.Common;
using Macro.Dicom.Network;
using Macro.Dicom.Network.Scu;

namespace Macro.Dicom.Audit
{
	/// <summary>
	/// DICOM Instances Transferred
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of the completion of transferring DICOM SOP Instances between two
	/// Application Entities. This message may only include information about a single patient.
	/// </para>
	/// <para>
	/// Note: This message may have been preceded by a Begin Transferring Instances message. The Begin
	/// Transferring Instances message conveys the intent to store SOP Instances, while the Instances
	/// Transferred message records the completion of the transfer. Any disagreement between the two
	/// messages might indicate a potential security breach.
	/// </para>
	/// </remarks>
	public class DicomInstancesTransferredAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
        public DicomInstancesTransferredAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			EventIdentificationContentsEventActionCode action,
			AssociationParameters parms)
			: base("DicomInstancesTransferred")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.DICOMInstancesTransferred;
			AuditMessage.EventIdentification.EventActionCode = action;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddActiveDicomParticipant(parms);

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
        public DicomInstancesTransferredAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			EventIdentificationContentsEventActionCode action,
			string sourceAE, string sourceHost, string destinationAE, string destinationHost)
			: base("DicomInstancesTransferred")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.DICOMInstancesTransferred;
			AuditMessage.EventIdentification.EventActionCode = action;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddActiveDicomParticipant(sourceAE, sourceHost, destinationAE, destinationHost);

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// (Optional) The identity of any other participants that might be involved andknown, especially third parties that are the requestor
		/// </summary>
		/// <param name="participant">The participant</param>
		public void AddOtherParticipants(AuditActiveParticipant participant)
		{
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="study"></param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study"></param>
		public void AddStudyParticipantObject(AuditStudyParticipantObject study)
		{
			InternalAddParticipantObject(study.StudyInstanceUid, study);
		}

		/// <summary>
		/// Add details of images within a study.  SOP Class information is automatically updated.
		/// </summary>
		/// <param name="instance">Descriptive object being audited</param>
		public void AddStorageInstance(StorageInstance instance)
		{
			InternalAddStorageInstance(instance);
		}
	}
}
