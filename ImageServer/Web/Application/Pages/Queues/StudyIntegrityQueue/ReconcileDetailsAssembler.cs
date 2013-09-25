#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.Common.Utilities;
using Macro.Dicom;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public static class ReconcileDetailsAssembler
    {
        public static ReconcileDetails CreateReconcileDetails(StudyIntegrityQueueSummary item)
        {
            ReconcileDetails details = item.TheStudyIntegrityQueueItem.StudyIntegrityReasonEnum.Equals(
                                           StudyIntegrityReasonEnum.InconsistentData)
                                           ? new ReconcileDetails(item.TheStudyIntegrityQueueItem)
                                           : new DuplicateEntryDetails(item.TheStudyIntegrityQueueItem);

            Study study = item.StudySummary.TheStudy;
            details.StudyInstanceUid = study.StudyInstanceUid;

            //Set the demographic details of the Existing Patient
            details.ExistingStudy = new ReconcileDetails.StudyInfo();
            details.ExistingStudy.StudyInstanceUid = item.StudySummary.StudyInstanceUid;
            details.ExistingStudy.AccessionNumber = item.StudySummary.AccessionNumber;
            details.ExistingStudy.StudyDate = item.StudySummary.StudyDate;
            details.ExistingStudy.Patient.PatientID = item.StudySummary.PatientId;
            details.ExistingStudy.Patient.Name = item.StudySummary.PatientsName;
            details.ExistingStudy.Patient.Sex = study.PatientsSex;
            details.ExistingStudy.Patient.IssuerOfPatientID = study.IssuerOfPatientId;
            details.ExistingStudy.Patient.BirthDate = study.PatientsBirthDate;
            details.ExistingStudy.Series = CollectionUtils.Map(
                study.Series.Values,
                delegate(Series theSeries)
                    {
                        var seriesDetails = new ReconcileDetails.SeriesDetails
                                                {
                                                    Description = theSeries.SeriesDescription,
                                                    SeriesInstanceUid = theSeries.SeriesInstanceUid,
                                                    Modality = theSeries.Modality,
                                                    NumberOfInstances = theSeries.NumberOfSeriesRelatedInstances,
                                                    SeriesNumber = theSeries.SeriesNumber
                                                };
                        return seriesDetails;
                    });


            details.ConflictingImageSet = item.QueueData.Details;


            details.ConflictingStudyInfo = new ReconcileDetails.StudyInfo();

            if (item.QueueData.Details != null)
            {
                // extract the conflicting study info from Details
                details.ConflictingStudyInfo.AccessionNumber = item.QueueData.Details.StudyInfo.AccessionNumber;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;
                details.ConflictingStudyInfo.StudyInstanceUid = item.QueueData.Details.StudyInfo.StudyInstanceUid;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo
                                                           {
                                                               BirthDate =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   PatientsBirthdate,
                                                               IssuerOfPatientID =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   IssuerOfPatientId,
                                                               Name = item.QueueData.Details.StudyInfo.PatientInfo.Name,
                                                               PatientID =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   PatientId,
                                                               Sex = item.QueueData.Details.StudyInfo.PatientInfo.Sex
                                                           };

                details.ConflictingStudyInfo.Series =
                    CollectionUtils.Map(
                        item.QueueData.Details.StudyInfo.Series,
                        delegate(SeriesInformation input)
                            {
                                var seriesDetails = new ReconcileDetails.SeriesDetails
                                                        {
                                                            Description = input.SeriesDescription,
                                                            Modality = input.Modality,
                                                            SeriesInstanceUid = input.SeriesInstanceUid,
                                                            NumberOfInstances = input.NumberOfInstances
                                                        };
                                return seriesDetails;
                            });
            }
            else
            {
                // Extract the conflicting study info from StudyData
                // Note: Not all fields are available.
                ImageSetDescriptor desc =
                    ImageSetDescriptor.Parse(item.TheStudyIntegrityQueueItem.StudyData.DocumentElement);
                string value;

                if (desc.TryGetValue(DicomTags.AccessionNumber, out value))
                    details.ConflictingStudyInfo.AccessionNumber = value;

                if (desc.TryGetValue(DicomTags.StudyDate, out value))
                    details.ConflictingStudyInfo.StudyDate = value;

                if (desc.TryGetValue(DicomTags.StudyInstanceUid, out value))
                    details.ConflictingStudyInfo.StudyInstanceUid = value;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo();

                if (desc.TryGetValue(DicomTags.PatientsBirthDate, out value))
                    details.ConflictingStudyInfo.Patient.BirthDate = value;

                if (desc.TryGetValue(DicomTags.IssuerOfPatientId, out value))
                    details.ConflictingStudyInfo.Patient.IssuerOfPatientID = value;

                if (desc.TryGetValue(DicomTags.PatientsName, out value))
                    details.ConflictingStudyInfo.Patient.Name = value;

                if (desc.TryGetValue(DicomTags.PatientId, out value))
                    details.ConflictingStudyInfo.Patient.PatientID = value;

                if (desc.TryGetValue(DicomTags.PatientsSex, out value))
                    details.ConflictingStudyInfo.Patient.Sex = value;


                var series = new List<ReconcileDetails.SeriesDetails>();
                details.ConflictingStudyInfo.Series = series;

                var uidBroker =
                    HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
                var criteria = new StudyIntegrityQueueUidSelectCriteria();
                criteria.StudyIntegrityQueueKey.EqualTo(item.TheStudyIntegrityQueueItem.GetKey());

                IList<StudyIntegrityQueueUid> uids = uidBroker.Find(criteria);

                Dictionary<string, List<StudyIntegrityQueueUid>> seriesGroups = CollectionUtils.GroupBy(uids,
                                                                                                        uid =>
                                                                                                        uid.
                                                                                                            SeriesInstanceUid);

                foreach (string seriesUid in seriesGroups.Keys)
                {
                    var seriesDetails = new ReconcileDetails.SeriesDetails
                                            {
                                                SeriesInstanceUid = seriesUid,
                                                Description = seriesGroups[seriesUid][0].SeriesDescription,
                                                NumberOfInstances = seriesGroups[seriesUid].Count
                                            };
                    //seriesDetails.Modality = "N/A";
                    series.Add(seriesDetails);
                }
            }


            return details;
        }
    }
}