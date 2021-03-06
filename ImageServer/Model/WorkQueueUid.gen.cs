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

// This file is auto-generated by the Macro.Model.SqlServer.CodeGenerator project.

namespace Macro.ImageServer.Model
{
    using System;
    using System.Xml;
    using Macro.Dicom;
    using Macro.Enterprise.Core;
    using Macro.ImageServer.Enterprise;
    using Macro.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class WorkQueueUid: ServerEntity
    {
        #region Constructors
        public WorkQueueUid():base("WorkQueueUid")
        {}
        public WorkQueueUid(
             ServerEntityKey _workQueueKey_
            ,Boolean _failed_
            ,Boolean _duplicate_
            ,Int16 _failureCount_
            ,String _groupID_
            ,String _relativePath_
            ,String _extension_
            ,String _seriesInstanceUid_
            ,String _sopInstanceUid_
            ):base("WorkQueueUid")
        {
            WorkQueueKey = _workQueueKey_;
            Failed = _failed_;
            Duplicate = _duplicate_;
            FailureCount = _failureCount_;
            GroupID = _groupID_;
            RelativePath = _relativePath_;
            Extension = _extension_;
            SeriesInstanceUid = _seriesInstanceUid_;
            SopInstanceUid = _sopInstanceUid_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="WorkQueueGUID")]
        public ServerEntityKey WorkQueueKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Failed")]
        public Boolean Failed
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Duplicate")]
        public Boolean Duplicate
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="FailureCount")]
        public Int16 FailureCount
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="GroupID")]
        public String GroupID
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="RelativePath")]
        public String RelativePath
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Extension")]
        public String Extension
        { get; set; }
        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        { get; set; }
        [DicomField(DicomTags.SopInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SopInstanceUid")]
        public String SopInstanceUid
        { get; set; }
        #endregion

        #region Static Methods
        static public WorkQueueUid Load(ServerEntityKey key)
        {
            using (var read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public WorkQueueUid Load(IPersistenceContext read, ServerEntityKey key)
        {
            var broker = read.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUid theObject = broker.Load(key);
            return theObject;
        }
        static public WorkQueueUid Insert(WorkQueueUid entity)
        {
            using (var update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                WorkQueueUid newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public WorkQueueUid Insert(IUpdateContext update, WorkQueueUid entity)
        {
            var broker = update.GetBroker<IWorkQueueUidEntityBroker>();
            var updateColumns = new WorkQueueUidUpdateColumns();
            updateColumns.WorkQueueKey = entity.WorkQueueKey;
            updateColumns.Failed = entity.Failed;
            updateColumns.Duplicate = entity.Duplicate;
            updateColumns.FailureCount = entity.FailureCount;
            updateColumns.GroupID = entity.GroupID;
            updateColumns.RelativePath = entity.RelativePath;
            updateColumns.Extension = entity.Extension;
            updateColumns.SeriesInstanceUid = entity.SeriesInstanceUid;
            updateColumns.SopInstanceUid = entity.SopInstanceUid;
            WorkQueueUid newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
