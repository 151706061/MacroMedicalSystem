#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Edit;
using Macro.ImageServer.Core.Reconcile;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Provides methods to decode the information in a <see cref="StudyHistory"/> record.
    /// </summary>
    static class StudyHistoryRecordDecoder
    {
        public static ReconcileHistoryRecord ReadReconcileRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled,
                               "History record has invalid history record type");

            ReconcileHistoryRecord record = new ReconcileHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
            record.UpdateDescription = parser.Parse(historyRecord.ChangeDescription);
            return record;
        }

        public static WebEditStudyHistoryRecord ReadEditRecord(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited
                               || historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.ExternalEdit,
                               "History record has invalid history record type");

            WebEditStudyHistoryRecord record = new WebEditStudyHistoryRecord
                                                   {
                                                       InsertTime = historyRecord.InsertTime,
                                                       StudyStorageLocation =
                                                           StudyStorageLocation.FindStorageLocations(
                                                               StudyStorage.Load(historyRecord.StudyStorageKey))[0],
                                                       UpdateDescription =
                                                           XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(
                                                               historyRecord.ChangeDescription)
                                                   };
            return record;
        }
    }
    
    internal class StudyHistoryRecordBase
    {
        public DateTime InsertTime;
        public StudyStorageLocation StudyStorageLocation;
    }
}
