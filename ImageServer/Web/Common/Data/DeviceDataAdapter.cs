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
    /// Used to create/update/delete device entries in the database.
    /// </summary>
    /// 
    public class DeviceDataAdapter : BaseAdaptor<Device, IDeviceEntityBroker, DeviceSelectCriteria, DeviceUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of devices.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices()
        {
            return Get();
        }

        /// <summary>
        /// Delete a device in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteDevice(Device dev)
        {
            return Delete(dev.Key);
        }

        /// <summary>
        /// Update a device entry in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool Update(Device dev)
        {
            bool ok = true;

            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = dev.ServerPartitionKey;
            param.Enabled = dev.Enabled;
            param.AeTitle = dev.AeTitle;
            param.Description = dev.Description;
            param.Dhcp = dev.Dhcp;
            param.IpAddress = dev.IpAddress;
            param.Port = dev.Port;
            param.AllowQuery = dev.AllowQuery;
            param.AcceptKOPR = dev.AcceptKOPR;
            param.AllowRetrieve = dev.AllowRetrieve;
            param.AllowStorage = dev.AllowStorage;
            param.AllowAutoRoute = dev.AllowAutoRoute;
            param.ThrottleMaxConnections = dev.ThrottleMaxConnections;
            param.DeviceTypeEnum = dev.DeviceTypeEnum;
            Update(dev.Key, param);

            return ok;
        }

        public IList<Device> DummyList
        {
            get
            {
                // return dummy list
                List<Device> list = new List<Device>();
                Device dev = new Device();
                dev.AeTitle = "Checking";

                dev.ServerPartitionKey = new ServerEntityKey("Testing", "Checking");
                list.Add(dev);

                return list;
            }
        }

        /// <summary>
        /// Retrieve a list of devices with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices(DeviceSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Create a new device.
        /// </summary>
        /// <param name="newDev"></param>
        /// <returns></returns>
        public Device AddDevice(Device newDev)
        {
            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = newDev.ServerPartitionKey;
            param.AeTitle = newDev.AeTitle;
            param.Description = newDev.Description;
            param.IpAddress = newDev.IpAddress;
            param.Port = newDev.Port;
            param.Enabled = newDev.Enabled;
            param.Dhcp = newDev.Dhcp;
            param.AllowQuery = newDev.AllowQuery;
            param.AcceptKOPR = newDev.AcceptKOPR;
            param.AllowRetrieve = newDev.AllowRetrieve;
            param.AllowStorage = newDev.AllowStorage;
            param.AllowAutoRoute = newDev.AllowAutoRoute;
            param.ThrottleMaxConnections = newDev.ThrottleMaxConnections;
            param.DeviceTypeEnum = newDev.DeviceTypeEnum;
            return Add(param);
        }
    }
}

