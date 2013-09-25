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
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
	public class AlertController
	{
        private readonly AlertAdaptor _adaptor = new AlertAdaptor();

        public IList<Alert> GetAllAlerts()
        {
            AlertSelectCriteria searchCriteria = new AlertSelectCriteria();
            searchCriteria.InsertTime.SortAsc(0);
            return GetAlerts(searchCriteria);
        }

        public IList<Alert> GetAlerts(AlertSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<Alert> GetRangeAlerts(AlertSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetAlertsCount(AlertSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteAlert(Alert item)
        {
            return _adaptor.Delete(item.Key);
        }

        public bool DeleteAlertItems(IList<Alert> items)
        {
            foreach (Alert item in items)
            {
                if (_adaptor.Delete(item.Key) == false)
                    return false;
            }

            return true;
        }

        public bool DeleteAlertItem(ServerEntityKey key)
        {
            return _adaptor.Delete(key);   
        }

        public bool DeleteAllAlerts()
        {
            return _adaptor.Delete(new AlertSelectCriteria());
        }
	}
}
