#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Macro.Common.Utilities;
using Macro.Dicom.Utilities;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Query;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using Macro.ImageServer.Web.Common.Data.Model;

namespace Macro.ImageServer.Web.Common.Data.DataSource
{
	public class DeletedStudyDataSource 
	{
		private IList<DeletedStudyInfo> _studies;

		public string AccessionNumber { get; set; }

		public DeletedStudyInfo  Find(object key)
		{
			return CollectionUtils.SelectFirst(_studies,
			                                   info => info.RowKey.Equals(key));
		}

		public string PatientId { get; set; }

		public DateTime? StudyDate { get; set; }

		public string DeletedBy { get; set; }

		public string PatientsName { get; set; }

		public string StudyDescription { get; set; }

		private StudyDeleteRecordSelectCriteria GetSelectCriteria()
		{
			var criteria = new StudyDeleteRecordSelectCriteria();

            QueryHelper.SetGuiStringCondition(criteria.AccessionNumber, AccessionNumber);
            QueryHelper.SetGuiStringCondition(criteria.PatientId, PatientId);
            QueryHelper.SetGuiStringCondition(criteria.PatientsName, PatientsName);
            QueryHelper.SetGuiStringCondition(criteria.StudyDescription, StudyDescription);
            
			if (StudyDate != null)
				criteria.StudyDate.Like("%" + DateParser.ToDicomString(StudyDate.Value) + "%");

			return criteria;
		}

		public IEnumerable Select(int startRowIndex, int maxRows)
		{
			
			IStudyDeleteRecordEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDeleteRecordEntityBroker>();
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();
			criteria.Timestamp.SortDesc(0);
			IList<StudyDeleteRecord> list = broker.Find(criteria, startRowIndex, maxRows);

			_studies = CollectionUtils.Map(
				list, (StudyDeleteRecord record) => DeletedStudyInfoAssembler.CreateDeletedStudyInfo(record));

			// Additional filter: DeletedBy
            if (String.IsNullOrEmpty(DeletedBy)==false)
            {
                _studies = CollectionUtils.Select(_studies, delegate(DeletedStudyInfo record)
                                       {
                                           if (String.IsNullOrEmpty(record.UserId) || String.IsNullOrEmpty(record.UserName))
                                               return false;

                                           // either the id or user matches
                                           return record.UserId.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0 ||
                                                  record.UserName.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0;
                                       });
            }

			return _studies;
		
		}

		public int SelectCount()
		{
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();

            IStudyDeleteRecordEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDeleteRecordEntityBroker>();
		    return broker.Count(criteria);
		}
	}

	internal static class DeletedStudyInfoAssembler
	{
		public static DeletedStudyInfo CreateDeletedStudyInfo(StudyDeleteRecord record)
		{
			Filesystem fs = Filesystem.Load(record.FilesystemKey);

		    StudyDeleteExtendedInfo extendedInfo = XmlUtils.Deserialize<StudyDeleteExtendedInfo>(record.ExtendedInfo);
			DeletedStudyInfo info = new DeletedStudyInfo
			                        	{
			                        		DeleteStudyRecord = record.GetKey(),
			                        		RowKey = record.GetKey().Key,
			                        		StudyInstanceUid = record.StudyInstanceUid,
			                        		PatientsName = record.PatientsName,
			                        		AccessionNumber = record.AccessionNumber,
			                        		PatientId = record.PatientId,
			                        		StudyDate = record.StudyDate,
			                        		PartitionAE = record.ServerPartitionAE,
			                        		StudyDescription = record.StudyDescription,
			                        		BackupFolderPath = fs.GetAbsolutePath(record.BackupPath),
			                        		ReasonForDeletion = record.Reason,
			                        		DeleteTime = record.Timestamp,
			                        		UserName = extendedInfo.UserName,
			                        		UserId = extendedInfo.UserId
			                        	};
			if (record.ArchiveInfo!=null)
				info.Archives = XmlUtils.Deserialize<DeletedStudyArchiveInfoCollection>(record.ArchiveInfo);

            
			return info;
		}
	}
}