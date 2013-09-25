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
using Macro.Enterprise.Core;
using Macro.ImageServer.Core;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
	public class PartitionArchiveAdaptor : BaseAdaptor<PartitionArchive, IPartitionArchiveEntityBroker, PartitionArchiveSelectCriteria, PartitionArchiveUpdateColumns>
	{
		#region Private Members

		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

		#endregion Private Members

		public bool RestoreStudy(Study theStudy)
		{
		    RestoreQueue restore = ServerHelper.InsertRestoreRequest(theStudy.LoadStudyStorage(HttpContextData.Current.ReadContext));
            if (restore==null)
                throw new ApplicationException("Unable to restore the study. See the log file for details.");

			return true;
		}
	}
}
