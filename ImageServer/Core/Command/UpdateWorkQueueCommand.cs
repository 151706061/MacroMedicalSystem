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
using Macro.Common;
using Macro.Dicom;
using Macro.Dicom.Utilities.Command;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.Command;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Core.Command
{
    public class UpdateWorkQueueCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly DicomMessageBase _message;
        private readonly StudyStorageLocation _storageLocation;
        private WorkQueue _insertedWorkQueue;
        private readonly bool _duplicate;
        private readonly string _extension;
    	private readonly string _uidGroupId;

        #endregion

        public UpdateWorkQueueCommand(DicomMessageBase message,
                        StudyStorageLocation location,
                        bool duplicate)
            : this(message, location, duplicate, null, null)
        {
        }

        public UpdateWorkQueueCommand(DicomMessageBase message, StudyStorageLocation location, bool duplicate, string extension, string uidGroupId)
            : base("Update/Insert a WorkQueue Entry")
        {
            Platform.CheckForNullReference(message, "Dicom Message object");
            Platform.CheckForNullReference(location, "Study Storage Location");
            
            _message = message;
            _storageLocation = location;
            _duplicate = duplicate;
            _extension = extension;
            _uidGroupId = uidGroupId;
        }

        public WorkQueue InsertedWorkQueue
        {
            get { return _insertedWorkQueue; }
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var insert = updateContext.GetBroker<IInsertWorkQueue>();
            var parms = new InsertWorkQueueParameters
                            {
                                WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess,
                                StudyStorageKey = _storageLocation.GetKey(),
                                ServerPartitionKey = _storageLocation.ServerPartitionKey,
                                SeriesInstanceUid = _message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty),
                                SopInstanceUid = _message.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty),
                                ScheduledTime = Platform.Time,
                                WorkQueueGroupID = _uidGroupId
                            };

        	if (_duplicate)
            {
                parms.Duplicate = _duplicate;
                parms.Extension = _extension;
                parms.UidGroupID = _uidGroupId;
            }

            _insertedWorkQueue = insert.FindOne(parms);

            if (_insertedWorkQueue == null)
                throw new ApplicationException("UpdateWorkQueueCommand failed");
        }
    }
}
