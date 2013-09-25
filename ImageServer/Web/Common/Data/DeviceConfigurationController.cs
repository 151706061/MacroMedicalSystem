#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.Common;

namespace Macro.ImageServer.Web.Common.Data
{

    /// <summary>
    /// Device configuration screen controller.
    /// </summary>
    public class DeviceConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set devices from device table
        /// </summary>
        private DeviceDataAdapter _adapter = new DeviceDataAdapter();

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a device in the database.
        /// </summary>
        /// <param name="device"></param>
        public Device AddDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Adding new device : AETitle = {0}", device.AeTitle);

            Device dev = _adapter.AddDevice(device);

            if (dev!=null)
                Platform.Log(LogLevel.Info, "New device added :AE={0}, Key={1}",dev.AeTitle, dev.Key);
            else
                Platform.Log(LogLevel.Info, "Failed to add new device : AETitle={0}", dev.AeTitle);

            return dev;
        }

        /// <summary>
        /// Delete a device from the database.
        /// </summary>
        /// <param name="device"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Deleting {0}, Key={1}", device.AeTitle, device.Key);

            bool ok = _adapter.DeleteDevice(device);

            Platform.Log(LogLevel.Info, "Delete of {0} {1}", device.AeTitle, ok ? "Successful" : "Failed");

            return ok;
        }

        /// <summary>
        /// Update a device in the database.
        /// </summary>
        /// <param name="device"></param>
        /// <returns><b>true</b> if the record is updated successfully. <b>false</b> otherwise.</returns>
        public bool UpdateDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Updating device Key={0} : AETitle={1}", device.Key, device.AeTitle);
            bool ok = _adapter.Update(device);
            Platform.Log(LogLevel.Info, "Device Key={0} {1}", device.Key, ok ? "updated" : " failed to update");

            return ok;
        }

        /// <summary>
        /// Retrieve list of devices.
        /// </summary>
        /// <param name="criteria"/>
        /// <returns>List of <see cref="Device"/> matches <paramref name="criteria"/></returns>
        public IList<Device> GetDevices(DeviceSelectCriteria criteria)
        {
            return _adapter.GetDevices(criteria);
        }

        /// <summary>
        /// Retrieve a list of server partitions.
        /// </summary>
        /// <returns>List of all <see cref="ServerPartition"/>.</returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return _serverAdapter.GetServerPartitions();
        }

        public int GetRelatedWorkQueueCount(Device device)
        {
            WorkQueueAdaptor workQueueAdaptor = new WorkQueueAdaptor();
            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
            criteria.DeviceKey.EqualTo(device.Key);
            return workQueueAdaptor.GetCount(criteria);
        }

        #endregion public methods
    }
}
