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
using Macro.ImageServer.Common.Exceptions;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core;
using Macro.ImageServer.Core.Edit;
using Macro.ImageServer.Core.Process;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;
using Macro.ImageServer.Web.Common.Data.DataSource;
using UpdateItem=Macro.ImageServer.Core.Edit.UpdateItem;

namespace Macro.ImageServer.Web.Common.Data
{
    public class StudyController : BaseController
    {
        #region Private Members

        private readonly StudyAdaptor _adaptor = new StudyAdaptor();
        private readonly SeriesSearchAdaptor _seriesAdaptor = new SeriesSearchAdaptor();
		private readonly PartitionArchiveAdaptor _partitionArchiveAdaptor = new PartitionArchiveAdaptor();
        #endregion
        #region Public Methods

        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }
		public IList<Study> GetRangeStudies(StudySelectCriteria criteria, int startIndex, int maxRows)
		{
			return _adaptor.GetRange(criteria,startIndex,maxRows);
		}

		public int GetStudyCount(StudySelectCriteria criteria)
		{
			return _adaptor.GetCount(criteria);
		}

        public IList<Series> GetSeries(Study study)
        {
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();

            criteria.StudyKey.EqualTo(study.Key);

            return _seriesAdaptor.Get(criteria);
        }

		public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(ServerEntityKey studyStorageKey)
        {
			Platform.CheckForNullReference(studyStorageKey, "storageKey");


            IStudyIntegrityQueueEntityBroker integrityQueueBroker = HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

			parms.StudyStorageKey.EqualTo(studyStorageKey);

            return integrityQueueBroker.Find(parms);
        
        }

        public int GetStudyIntegrityQueueCount(ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "storageKey");


            IStudyIntegrityQueueEntityBroker integrityQueueBroker = HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

            parms.StudyStorageKey.EqualTo(studyStorageKey);

            return integrityQueueBroker.Count(parms);
        }

		/// <summary>
		/// Delete a Study.
		/// </summary>
		public void DeleteStudy(ServerEntityKey studyKey, string reason)
        {
			StudySummary study = StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, Study.Load(HttpContextData.Current.ReadContext, studyKey));
			if (study.IsReconcileRequired)
			{
				throw new ApplicationException(
					String.Format("Deleting the study is not allowed at this time : there are items to be reconciled."));

				// NOTE: another check will occur when the delete is actually processed
			}
			

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                LockStudyParameters lockParms = new LockStudyParameters
                                                	{
                                                		QueueStudyStateEnum = QueueStudyStateEnum.WebDeleteScheduled,
                                                		StudyStorageKey = study.TheStudyStorage.Key
                                                	};
				ILockStudy broker = ctx.GetBroker<ILockStudy>();
				broker.Execute(lockParms);
				if (!lockParms.Successful)
				{
				    throw new ApplicationException(String.Format("Unable to lock the study : {0}", lockParms.FailureReason));
				}
				

				InsertWorkQueueParameters insertParms = new InsertWorkQueueParameters
				                                        	{
				                                        		WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy,
				                                        		ServerPartitionKey = study.ThePartition.Key,
				                                        		StudyStorageKey = study.TheStudyStorage.Key,
				                                        		ScheduledTime = Platform.Time
				                                        	};

				WebDeleteStudyLevelQueueData extendedData = new WebDeleteStudyLevelQueueData
			                                                	{
			                                                		Level = DeletionLevel.Study,
			                                                		Reason = reason,
			                                                		UserId = ServerHelper.CurrentUserId,
			                                                		UserName = ServerHelper.CurrentUserName
			                                                	};
				insertParms.WorkQueueData = XmlUtils.SerializeAsXmlDoc(extendedData);
				IInsertWorkQueue insertWorkQueue = ctx.GetBroker<IInsertWorkQueue>();
				
                if (insertWorkQueue.FindOne(insertParms)==null)
                {
                    throw new ApplicationException("DeleteStudy failed");
                }


				ctx.Commit();
			}
        }

        //TODO: Consolidate this and DeleteStudy?
        public void DeleteSeries(Study study, IList<Series> series, string reason)
        {
            // Load the Partition
            ServerPartitionConfigController partitionConfigController = new ServerPartitionConfigController();
            ServerPartition partition = partitionConfigController.GetPartition(study.ServerPartitionKey);

            List<string> seriesUids = new List<string>();
            foreach (Series s in series)
            {
                seriesUids.Add(s.SeriesInstanceUid);
            }

            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyEditorHelper.DeleteSeries(ctx, partition, study.StudyInstanceUid, seriesUids, reason);
                ctx.Commit();
            }
        }

		/// <summary>
		/// Restore a nearline study.
		/// </summary>
		/// <param name="study">The <see cref="Study"/> to restore.</param>
		/// <returns>true on success, false on failure.</returns>
		public bool RestoreStudy(Study study)
		{
			return _partitionArchiveAdaptor.RestoreStudy(study);
		}

        public bool MoveStudy(Study study, Device device)
        {
            return MoveStudy(study, device, null);
        }

        public bool MoveStudy(Study study, Device device, IList<Series> seriesList)
        {            
			if (seriesList != null)
			{
                using (
					IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
                    ServerPartition partition = ServerPartition.Load(study.ServerPartitionKey);

				    List<string> seriesUids = new List<string>();
                    foreach (Series series in seriesList)
					{
					    seriesUids.Add(series.SeriesInstanceUid);    
					}

                    IList<WorkQueue> entries = StudyEditorHelper.MoveSeries(context, partition, study.StudyInstanceUid, device.Key, seriesUids);
                        if(entries != null) context.Commit();

				    return true;
				}
			}
        	WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
			DateTime time = Platform.Time;
			WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns
			                                 	{
			                                 		WorkQueueTypeEnum = WorkQueueTypeEnum.WebMoveStudy,
			                                 		WorkQueueStatusEnum = WorkQueueStatusEnum.Pending,
			                                 		ServerPartitionKey = study.ServerPartitionKey,
			                                 		StudyStorageKey = study.StudyStorageKey,
			                                 		FailureCount = 0,
			                                 		DeviceKey = device.Key,
			                                 		ScheduledTime = time,
			                                 		ExpirationTime = time.AddMinutes(4)
			                                 	};

        	workqueueAdaptor.Add(columns);

        	return true;
	    }

        public void EditStudy(Study study, List<UpdateItem> updateItems, string reason)
        {
            Platform.Log(LogLevel.Info, String.Format("Editing study {0}", study.StudyInstanceUid));

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                IList<WorkQueue> entries = StudyEditorHelper.EditStudy(ctx, study.StudyStorageKey, updateItems, reason, ServerHelper.CurrentUserName, EditType.WebEdit);
                if (entries!=null)
			        ctx.Commit();
			}
        }

		public bool UpdateStudy(Study study, StudyUpdateColumns columns)
		{
			return _adaptor.Update(study.Key, columns);
		}

        public bool IsScheduledForEdit(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebEditStudy);
        }

        public bool IsScheduledForDelete(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebDeleteStudy);
        }

        public bool CanManipulateSeries(ServerEntityKey studyStorageKey)
        {
            StudyStorage storage = StudyStorage.Load(studyStorageKey);
            
            return storage.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle);
        }

        /// <summary>
        /// Returns a value indicating whether the specified study has been scheduled for delete.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="workQueueType"></param>
        /// <returns></returns>           
		private static bool IsStudyInWorkQueue(Study study, WorkQueueTypeEnum workQueueType)
        {
        	Platform.CheckForNullReference(study, "Study");

        	WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
        	WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
        	workQueueCriteria.WorkQueueTypeEnum.EqualTo(workQueueType);
        	workQueueCriteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
        	workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);

        	workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Pending);

        	IList<WorkQueue> list = adaptor.Get(workQueueCriteria);
        	if (list != null && list.Count > 0)
        		return true;

        	workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Idle); // not likely but who knows
        	list = adaptor.Get(workQueueCriteria);
        	if (list != null && list.Count > 0)
        		return true;

        	return false;
        }

    	/// <summary>
		/// Returns a value indicating whether the specified study has been scheduled for delete.
		/// </summary>
		/// <param name="study"></param>
		/// <param name="read"></param>
		/// <returns></returns>
		public string GetModalitiesInStudy(IPersistenceContext read, Study study)
		{
			Platform.CheckForNullReference(study, "Study");

            IQueryModalitiesInStudy select = read.GetBroker<IQueryModalitiesInStudy>();
            ModalitiesInStudyQueryParameters parms = new ModalitiesInStudyQueryParameters { StudyKey = study.Key };
            IList<Series> seriesList = select.Find(parms);
			List<string> modalities = new List<string>();
			
			foreach (Series series in seriesList)
			{
				bool found = false;
				foreach (string modality in modalities)
					if (modality.Equals(series.Modality))
					{
						found = true;
						break;
					}
				if (!found)
					modalities.Add(series.Modality);
			}

			string modalitiesInStudy = "";
			foreach (string modality in modalities)
				if (modalitiesInStudy.Length == 0)
					modalitiesInStudy = modality;
				else
					modalitiesInStudy += "\\" + modality;

			return modalitiesInStudy;
		}

        public IList<WorkQueue> GetWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
			workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.ScheduledTime.SortAsc(0);
            return adaptor.Get(workQueueCriteria);
        }

        public int GetCountPendingWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            var adaptor = new WorkQueueAdaptor();
            var workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.WorkQueueStatusEnum.In(new [] {WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending});
            return adaptor.GetCount(workQueueCriteria);
        }

        public int GetCountPendingExternalEditWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            var adaptor = new WorkQueueAdaptor();
            var workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.ExternalEdit);
            return adaptor.GetCount(workQueueCriteria);
        }

        public IList<FilesystemQueue> GetFileSystemQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            FileSystemQueueAdaptor adaptor = new FileSystemQueueAdaptor();
            FilesystemQueueSelectCriteria fileSystemQueueCriteria = new FilesystemQueueSelectCriteria();
			fileSystemQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            fileSystemQueueCriteria.ScheduledTime.SortAsc(0);
            return adaptor.Get(fileSystemQueueCriteria);
        }

        public IList<ArchiveQueue> GetArchiveQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
            ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
            archiveQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
			archiveQueueCriteria.ScheduledTime.SortDesc(0);
            return adaptor.Get(archiveQueueCriteria);
        }

		public int GetArchiveQueueCount(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
			ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
			archiveQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
			archiveQueueCriteria.ScheduledTime.SortDesc(0);
			return adaptor.GetCount(archiveQueueCriteria);
		}

        public IList<ArchiveStudyStorage> GetArchiveStudyStorage(Study study)
        {
        	Platform.CheckForNullReference(study, "Study");

        	ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
        	ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
        	archiveStudyStorageCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

        	return adaptor.Get(archiveStudyStorageCriteria);
        }

		public IList<ArchiveStudyStorage> GetArchiveStudyStorage(IPersistenceContext read, ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.Get(read, archiveStudyStorageCriteria);
        }
        public ArchiveStudyStorage GetFirstArchiveStudyStorage(IPersistenceContext read, ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
            archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.GetFirst(read, archiveStudyStorageCriteria);
        }

        public IList<StudyStorageLocation> GetStudyStorageLocation(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            
            IQueryStudyStorageLocation select = HttpContextData.Current.ReadContext.GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
                                                        	{StudyStorageKey = study.StudyStorageKey};

        	IList<StudyStorageLocation> storage = select.Find(parms);

            if (storage == null)
			{
				storage = new List<StudyStorageLocation>();
			    Platform.Log(LogLevel.Warn, "Unable to find storage location for Study item: {0}",
                             study.GetKey().ToString());
            }

            if (storage.Count > 1)
            {
                Platform.Log(LogLevel.Warn,
                             "StudyController:GetStudyStorageLocation: multiple study storage found for study {0}",
                             study.GetKey().Key);
            }

            return storage;        
        }

		public StudyStorage GetStudyStorage(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			return StudyStorage.Load(study.StudyStorageKey);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="key"></param>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        public void ReprocessStudy(String reason, ServerEntityKey key)
        {
            StudyStorageAdaptor adaptor = new StudyStorageAdaptor();
            StudyStorage storage = adaptor.Get(key);
            StudyStorageLocation storageLocation = StudyStorageLocation.FindStorageLocations(storage)[0];
            StudyReprocessor reprocessor = new StudyReprocessor();
            reprocessor.ReprocessStudy(reason, storageLocation, Platform.Time);
        }

        #endregion      
    }
}
