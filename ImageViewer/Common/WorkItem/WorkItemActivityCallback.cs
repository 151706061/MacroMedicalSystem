﻿#region License

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

using System.Collections.Generic;
using System.ServiceModel;

namespace Macro.ImageViewer.Common.WorkItem
{
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemActivityCallback
    {
        [OperationContract(IsOneWay = true)]
        void StudiesCleared();

        [OperationContract(IsOneWay = true)]
        void WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems);
    }

    public abstract class WorkItemActivityCallback : IWorkItemActivityCallback
    {
        private class NilCallback : WorkItemActivityCallback
        {
			public override void WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
            {
            }

            public override void StudiesCleared()
            {
            }
        }

        public static readonly IWorkItemActivityCallback Nil = new NilCallback();

        #region IWorkItemActivityCallback Members

		public abstract void WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems);

        public abstract void StudiesCleared();

        #endregion
    }
}
