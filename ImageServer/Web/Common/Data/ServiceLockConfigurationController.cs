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
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.Common;

namespace Macro.ImageServer.Web.Common.Data
{

    /// <summary>
    /// ServiceLock configuration screen controller.
    /// </summary>
    public class ServiceLockConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set services from service table
        /// </summary>
        private ServiceLockDataAdapter _adapter = new ServiceLockDataAdapter();

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a service in the database.
        /// </summary>
        /// <param name="service"></param>
        public ServiceLock AddServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User adding new service lock { type={0}, filesystem={1} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            ServiceLock dev = _adapter.AddServiceLock(service);

            if (dev!=null)
                Platform.Log(LogLevel.Info, "New service added by user : {Key={0}, type={1}, filesystem={2}", service.Key, service.ServiceLockTypeEnum, service.FilesystemKey);
            else
                Platform.Log(LogLevel.Info, "Failed to add new service : {  type={1}, filesystem={2} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            return dev;
        }

        /// <summary>
        /// Delete a service from the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User deleting service lock {0}", service.Key);

            bool ok = _adapter.DeleteServiceLock(service);

            Platform.Log(LogLevel.Info, "User delete service lock {0}: {1}", service.Key, ok ? "Successful" : "Failed");

            return ok;
        }

        /// <summary>
        /// Update a service in the database.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><b>true</b> if the record is updated successfully. <b>false</b> otherwise.</returns>
        /// <param name="enabled"></param>
        /// <param name="scheduledDateTime"></param>
        public bool UpdateServiceLock(ServerEntityKey key, bool enabled, DateTime scheduledDateTime)
        {
            Platform.Log(LogLevel.Info, "User updating service Key={0}", key.Key);
            ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
            columns.Enabled = enabled;
            columns.ScheduledTime = scheduledDateTime;

            bool ok = _adapter.Update(key, columns);

            Platform.Log(LogLevel.Info, "ServiceLock Key={0} {1}", key.Key, ok ? "updated by user" : " failed to update");

            return ok;
        
        }

        /// <summary>
        /// Retrieve list of services.
        /// </summary>
        /// <param name="criteria"/>
        /// <returns>List of <see cref="ServiceLock"/> matches <paramref name="criteria"/></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return _adapter.GetServiceLocks(criteria);
        }

        /// <summary>
        /// Retrieve a list of server partitions.
        /// </summary>
        /// <returns>List of all <see cref="ServerPartition"/>.</returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return _serverAdapter.GetServerPartitions();
        }

        #endregion public methods
    }
}
