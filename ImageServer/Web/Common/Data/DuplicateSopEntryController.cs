#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Common;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Controllers for updating <see cref="StudyIntegrityQueue"/> record that has <see cref="StudyIntegrityReasonEnum"/> equal to <see cref="StudyIntegrityReasonEnum.Duplicate"/>
    /// </summary>
    public class DuplicateSopEntryController
    {
        /// <summary>
        /// Inserts work queue entry to process the duplicates.
        /// </summary>
        /// <param name="entryKey"><see cref="ServerEntityKey"/> of the <see cref="StudyIntegrityQueue"/> entry  that has <see cref="StudyIntegrityReasonEnum"/> equal to <see cref="StudyIntegrityReasonEnum.Duplicate"/> </param>
        /// <param name="action"></param>
        public void Process(ServerEntityKey entryKey, ProcessDuplicateAction action)
        {
            
            DuplicateSopReceivedQueue entry = DuplicateSopReceivedQueue.Load(HttpContextData.Current.ReadContext, entryKey);
            Platform.CheckTrue(entry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate, "Invalid type of entry");

            IList<StudyIntegrityQueueUid> uids = LoadDuplicateSopUid(entry);

            using(IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ProcessDuplicateQueueEntryQueueData data = new ProcessDuplicateQueueEntryQueueData
                {
                    Action = action,
                    DuplicateSopFolder = entry.GetFolderPath(context),
                    UserName = ServerHelper.CurrentUserName,                                                              		
                }; 
                
                LockStudyParameters lockParms = new LockStudyParameters
                {
                    QueueStudyStateEnum = QueueStudyStateEnum.ReconcileScheduled,
                    StudyStorageKey = entry.StudyStorageKey
                };

                ILockStudy lockBbroker = context.GetBroker<ILockStudy>();
                lockBbroker.Execute(lockParms);
                if (!lockParms.Successful)
                {
                    throw new ApplicationException(lockParms.FailureReason);
                }

            	IWorkQueueProcessDuplicateSopBroker broker = context.GetBroker<IWorkQueueProcessDuplicateSopBroker>();
                WorkQueueProcessDuplicateSopUpdateColumns columns = new WorkQueueProcessDuplicateSopUpdateColumns
                                                                    	{
                                                                    		Data = XmlUtils.SerializeAsXmlDoc(data),
                                                                    		GroupID = entry.GroupID,
                                                                    		ScheduledTime = Platform.Time,
                                                                    		ExpirationTime =
                                                                    			Platform.Time.Add(TimeSpan.FromMinutes(15)),
                                                                    		ServerPartitionKey = entry.ServerPartitionKey,
                                                                    		WorkQueuePriorityEnum =
                                                                    			WorkQueuePriorityEnum.Medium,
                                                                    		StudyStorageKey = entry.StudyStorageKey,
                                                                    		WorkQueueStatusEnum = WorkQueueStatusEnum.Pending
                                                                    	};

            	WorkQueueProcessDuplicateSop processDuplicateWorkQueueEntry = broker.Insert(columns);

                IWorkQueueUidEntityBroker workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
                IStudyIntegrityQueueUidEntityBroker duplicateUidBroke = context.GetBroker<IStudyIntegrityQueueUidEntityBroker>();
                foreach (StudyIntegrityQueueUid uid in uids)
                {
                    WorkQueueUidUpdateColumns uidColumns = new WorkQueueUidUpdateColumns
                                                           	{
                                                           		Duplicate = true,
                                                           		Extension = ServerPlatform.DuplicateFileExtension,
                                                           		SeriesInstanceUid = uid.SeriesInstanceUid,
                                                           		SopInstanceUid = uid.SopInstanceUid,
                                                           		RelativePath = uid.RelativePath,
                                                           		WorkQueueKey = processDuplicateWorkQueueEntry.GetKey()
                                                           	};

                	workQueueUidBroker.Insert(uidColumns);

                    duplicateUidBroke.Delete(uid.GetKey());
                }

                IDuplicateSopEntryEntityBroker duplicateEntryBroker =
                    context.GetBroker<IDuplicateSopEntryEntityBroker>();
                duplicateEntryBroker.Delete(entry.GetKey());


                context.Commit();
            }
        }

        private static IList<StudyIntegrityQueueUid> LoadDuplicateSopUid(DuplicateSopReceivedQueue entry)
        {
            IStudyIntegrityQueueUidEntityBroker broker =
                HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueUidEntityBroker>();

            StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
            criteria.StudyIntegrityQueueKey.EqualTo(entry.GetKey());
            return broker.Find(criteria);
        }
    }
}
