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
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Alerts
{
	/// <summary>
	/// Encapsulates a collection of <see cref="Alert"/> which can be accessed based on the <see cref="ServerEntityKey"/>
	/// </summary>
	public class AlertItemCollection : KeyedCollectionBase<AlertSummary, ServerEntityKey>
	{

		public AlertItemCollection(IList<AlertSummary> list)
			: base(list)
		{
		}

		protected override ServerEntityKey GetKey(AlertSummary item)
		{
			return item.Key;
		}
	}
}
