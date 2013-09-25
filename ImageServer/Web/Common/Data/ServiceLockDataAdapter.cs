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
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete service  entries in the database.
    /// </summary>
    /// 
    public class ServiceLockDataAdapter : BaseAdaptor<ServiceLock, IServiceLockEntityBroker, ServiceLockSelectCriteria, ServiceLockUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of service s.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks()
        {
            return Get();
        }

        /// <summary>
        /// Delete a service  in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteServiceLock(ServiceLock dev)
        {
            return base.Delete(dev.Key);
        }

        /// <summary>
        /// Update a service  entry in the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool Update(ServiceLock service)
        {
            bool ok = true;

            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = service.Enabled;
            param.FilesystemKey = service.FilesystemKey;
            param.Lock = service.Lock;
            param.ProcessorId = service.ProcessorId;
            param.ScheduledTime = service.ScheduledTime;
            param.ServiceLockTypeEnum = service.ServiceLockTypeEnum;

            ok = base.Update(service.Key,param);

            return ok;
        }

        

        /// <summary>
        /// Retrieve a list of service s with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return base.Get(criteria);
        }

        /// <summary>
        /// Create a new service .
        /// </summary>
        /// <param name="newService"></param>
        /// <returns></returns>
        public ServiceLock AddServiceLock(ServiceLock newService)
        {
            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = newService.Enabled;
            param.FilesystemKey = newService.FilesystemKey;
            param.Lock = newService.Lock;
            param.ProcessorId = newService.ProcessorId;
            param.ScheduledTime = newService.ScheduledTime;
            param.ServiceLockTypeEnum = newService.ServiceLockTypeEnum;

            return base.Add(param);
        }
    }
}

