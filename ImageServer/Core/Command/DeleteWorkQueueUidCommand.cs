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
using Macro.Dicom.Utilities.Command;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.Command;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Core.Command
{
    /// <summary>
    /// Command to delete a work queue uid.
    /// </summary>
    public class DeleteWorkQueueUidCommand : ServerDatabaseCommand
    {
        #region Private Fields
        private readonly WorkQueueUid _uid;
        #endregion

        public DeleteWorkQueueUidCommand(WorkQueueUid uid)
            : base("Delete WorkQueue Uid Entry")
        {
            Platform.CheckForNullReference(uid, "uid");
            _uid = uid;
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
            delete.Delete(_uid.GetKey());
        }
    }
}
