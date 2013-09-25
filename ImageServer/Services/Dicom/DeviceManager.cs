#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Globalization;
using Macro.Common;
using Macro.Dicom.Network;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Services.Dicom
{
    class DeviceManager
    {
    	/// <summary>
    	/// Lookup the device entity in the database corresponding to the remote AE of the association.
    	/// </summary>
    	/// <param name="partition">The partition to look up the devices</param>
    	/// <param name="association">The association</param>
    	/// <param name="isNew">Indicates whether the device returned is created by the call.</param>
    	/// <returns>The device record corresponding to the called AE of the association</returns>
		static public Device LookupDevice(ServerPartition partition, AssociationParameters association, out bool isNew)
    	{
    		isNew = false;

    		Device device = null;

    		using (
    			IUpdateContext updateContext =
    				PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
    		{
    			var queryDevice = updateContext.GetBroker<IDeviceEntityBroker>();

    			// Setup the select parameters.
    			var queryParameters = new DeviceSelectCriteria();
    			queryParameters.AeTitle.EqualTo(association.CallingAE);
    			queryParameters.ServerPartitionKey.EqualTo(partition.GetKey());
                var devices = queryDevice.Find(queryParameters);
                foreach (var d in devices)
                {                    
                    if (string.Compare(d.AeTitle,association.CallingAE,false,CultureInfo.InvariantCulture) == 0)
                    {
                        device = d;
                        break;
                    }
                }

    			if (device == null)
    			{
    				if (!partition.AcceptAnyDevice)
    				{
    					return null;
    				}

    				if (partition.AutoInsertDevice)
    				{
    					// Auto-insert a new entry in the table.
    				    var updateColumns = new DeviceUpdateColumns
    				                            {
    				                                AeTitle = association.CallingAE,
    				                                Enabled = true,
    				                                Description = String.Format("AE: {0}", association.CallingAE),
    				                                Dhcp = false,
    				                                IpAddress = association.RemoteEndPoint.Address.ToString(),
    				                                ServerPartitionKey = partition.GetKey(),
    				                                Port = partition.DefaultRemotePort,
    				                                AllowQuery = true,
    				                                AllowRetrieve = true,
    				                                AllowStorage = true,
    				                                ThrottleMaxConnections = ImageServerCommonConfiguration.Device.MaxConnections,
    				                                DeviceTypeEnum = DeviceTypeEnum.Workstation
    				                            };

    				    var insert = updateContext.GetBroker<IDeviceEntityBroker>();

    					device = insert.Insert(updateColumns);

    					updateContext.Commit();

    					isNew = true;
    				}
    			}

    			if (device != null)
    			{
    				// For DHCP devices, we always update the remote ip address, if its changed from what is in the DB.
    				if (device.Dhcp && !association.RemoteEndPoint.Address.ToString().Equals(device.IpAddress))
    				{
    				    var updateColumns = new DeviceUpdateColumns
    				                            {
    				                                IpAddress = association.RemoteEndPoint.Address.ToString(),
    				                                LastAccessedTime = Platform.Time
    				                            };

    				    var update = updateContext.GetBroker<IDeviceEntityBroker>();

    					if (!update.Update(device.GetKey(), updateColumns))
    						Platform.Log(LogLevel.Error,
    						             "Unable to update IP Address for DHCP device {0} on partition '{1}'",
    						             device.AeTitle, partition.Description);
    					else
    						updateContext.Commit();
    				}
    				else if (!isNew)
    				{
    				    var updateColumns = new DeviceUpdateColumns {LastAccessedTime = Platform.Time};

    				    var update = updateContext.GetBroker<IDeviceEntityBroker>();

    					if (!update.Update(device.GetKey(), updateColumns))
    						Platform.Log(LogLevel.Error,
    						             "Unable to update LastAccessedTime device {0} on partition '{1}'",
    						             device.AeTitle, partition.Description);
    					else
    						updateContext.Commit();
    				}
    			}
    		}

    		return device;
    	}
    }
}
