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
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
	public class RestoreQueueController
	{
        private readonly RestoreQueueAdaptor _adaptor = new RestoreQueueAdaptor();


		/// <summary>
		/// Gets a list of <see cref="RestoreQueue"/> items with specified criteria
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList<RestoreQueue> FindRestoreQueue(WebQueryRestoreQueueParameters parameters)
		{
			try
			{
				IWebQueryRestoreQueue broker = HttpContextData.Current.ReadContext.GetBroker<IWebQueryRestoreQueue>();
				IList<RestoreQueue> list = broker.Find(parameters);

				return list;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "FindRestoreQueue failed", e);
				return new List<RestoreQueue>();
			}
		}

        public bool DeleteRestoreQueueItem(RestoreQueue item)
        {
        	using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
				LockStudyParameters parms = new LockStudyParameters
				                            	{
				                            		StudyStorageKey = item.StudyStorageKey,
				                            		QueueStudyStateEnum = QueueStudyStateEnum.Idle
				                            	};
				if (!lockStudyBroker.Execute(parms))
					return false;
				if (!parms.Successful)
					return false;

				bool retValue = _adaptor.Delete(updateContext, item.Key);

				updateContext.Commit();

				return retValue;
			}
        }

        
	}
}
