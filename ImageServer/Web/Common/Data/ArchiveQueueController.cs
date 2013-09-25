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
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
	public class ArchiveQueueController
	{
        private readonly ArchiveQueueAdaptor _adaptor = new ArchiveQueueAdaptor();


		/// <summary>
		/// Gets a list of <see cref="ArchiveQueue"/> items with specified criteria
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList<ArchiveQueue> FindArchiveQueue(WebQueryArchiveQueueParameters parameters)
		{
			try
			{
				IList<ArchiveQueue> list;

				IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

                IWebQueryArchiveQueue broker = HttpContextData.Current.ReadContext.GetBroker<IWebQueryArchiveQueue>();
				list = broker.Find(parameters);

				return list;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "FindArchiveQueue failed", e);
				return new List<ArchiveQueue>();
			}
		}

        public bool DeleteArchiveQueueItem(ArchiveQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

		public bool ResetArchiveQueueItem(IList<ArchiveQueue> items, DateTime time)
		{
			if (items == null || items.Count == 0)
				return false;

			ArchiveQueueUpdateColumns columns = new ArchiveQueueUpdateColumns();
			columns.ArchiveQueueStatusEnum = ArchiveQueueStatusEnum.Pending;
			columns.ProcessorId = "";
			columns.ScheduledTime = time;

			bool result = true;
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IArchiveQueueEntityBroker archiveQueueBroker = ctx.GetBroker<IArchiveQueueEntityBroker>();
				
				foreach (ArchiveQueue item in items)
				{
					// Only do an update if its a failed status currently
					ArchiveQueueSelectCriteria criteria = new ArchiveQueueSelectCriteria();
					criteria.ArchiveQueueStatusEnum.EqualTo(ArchiveQueueStatusEnum.Failed);
					criteria.StudyStorageKey.EqualTo(item.StudyStorageKey);

					if (!archiveQueueBroker.Update(criteria, columns))
					{
						result = false;
						break;
					}
				}

				if (result)
					ctx.Commit();
			}

			return result;
		}
	}
}
