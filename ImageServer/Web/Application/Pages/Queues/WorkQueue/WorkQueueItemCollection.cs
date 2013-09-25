#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
	/// <summary>
	/// Encapsulates a collection of <see cref="WorkQueue"/> which can be accessed based on the <see cref="ServerEntityKey"/>
	/// </summary>
	public class WorkQueueItemCollection : KeyedCollectionBase<WorkQueueSummary, ServerEntityKey>
	{

		public WorkQueueItemCollection(IList<WorkQueueSummary> list)
			: base(list)
		{
		}

		protected override ServerEntityKey GetKey(WorkQueueSummary item)
		{
			return item.Key;
		}
	}
}
