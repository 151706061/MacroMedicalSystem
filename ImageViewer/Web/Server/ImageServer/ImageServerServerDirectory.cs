#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Macro.Common;
using Macro.Dicom.ServiceModel;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Core.ModelExtensions;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageViewer.Common.ServerDirectory;
using Macro.Web.Enterprise.Authentication;
using System.Text;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServerDirectoryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IServerDirectory))
                return null;

            return new ImageServerServerDirectory();
        }

        #endregion
    }

    internal class ImageServerServerDirectory : IServerDirectory
    {
        public GetServersResult GetServers(GetServersRequest request)
        {
            var result = new GetServersResult
            {
                ServerEntries = new List<ServerDirectoryEntry>()
            };

            if (!string.IsNullOrEmpty(request.AETitle))
            {
                var entry = this.FindLocalPartition(request.AETitle);
                if (entry != null)
                    result.ServerEntries.Add(entry);
                else
                {
                    var device = FindServer(request.AETitle);
                    if (device != null)
                        result.ServerEntries.Add(FromDeviceToServerDirectoryEntry(device));
                }
            }

            return result;
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            return new AddServerResult();
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            return new UpdateServerResult();
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            return new DeleteServerResult();
        }

        #region Helpers 

        
        /// <summary>
        /// Gets the <see cref="ServerDirectoryEntry"/> record corresponding to the <see cref="ServerPartition"/> whose AE Title matches the the specified Retrieve AE Title.
        /// PermissionDeniedException will be thrown if the current user does not have permission to access the partition.
        /// </summary>
        /// <param name="retrieveAeTitle"></param>
        /// <returns></returns>
        /// <exception cref="PermissionDeniedException">If user hasn't logged in (Thread.CurrentPrincipal is not set) or user does not have access to the server partition</exception>
        private ServerDirectoryEntry FindLocalPartition(string retrieveAeTitle)
        {
            var webUser = Thread.CurrentPrincipal as CustomPrincipal;
            if (webUser == null)
            {
                throw new PermissionDeniedException("User is not logged in");
            }

            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(retrieveAeTitle);
            if (partition == null)
            {
                Platform.Log(LogLevel.Error, "ImageServerServerDirectory: FindLocalPartition(): Could not find partition with AE Title '{0}'", retrieveAeTitle);
                return null;
            }

            if (!partition.IsUserAccessAllowed(webUser))
                throw new PermissionDeniedException(string.Format("User '{0}' does not have permission to access to partition {1}. Check both the Server Partition and Study Data Access!", webUser.DisplayName, partition.AeTitle));

            return new ServerDirectoryEntry()
            {
                IsPriorsServer = true, // search this partition for prior studies
                Server = new ApplicationEntity()
                {
                    AETitle = partition.AeTitle,
                    Description = partition.Description,
                    Name = partition.AeTitle,
                    ScpParameters = new ScpParameters(WebViewerServices.Default.ArchiveServerHostname, partition.Port),
                    StreamingParameters = new StreamingParameters(WebViewerServices.Default.ArchiveServerHeaderPort, WebViewerServices.Default.ArchiveServerWADOPort)
                }
            };
        }



        private static Device FindServer(string retrieveAeTitle)
        {
         
            var webUser = Thread.CurrentPrincipal as CustomPrincipal;
            if (webUser != null)
            {
                foreach (var partition in ServerPartitionMonitor.Instance)
                {
                    if (partition.AeTitle.Equals(retrieveAeTitle, StringComparison.InvariantCulture))
                    {
                        if (!partition.IsUserAccessAllowed(webUser))
                            throw new PermissionDeniedException(string.Format("User does not have permission to access partition {0}", partition.AeTitle));
                    }
                }
            }

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var broker = ctx.GetBroker<IDeviceEntityBroker>();
                var criteria = new DeviceSelectCriteria();
                criteria.AeTitle.EqualTo(retrieveAeTitle);
                IList<Device> list = broker.Find(criteria);
                foreach (Device theDevice in list)
                {
                    if (string.Compare(theDevice.AeTitle, retrieveAeTitle, false, CultureInfo.InvariantCulture) == 0)
                        return theDevice;
                }
            }

            return null;
        }

        private static ServerDirectoryEntry FromDeviceToServerDirectoryEntry(Device theDevice)
        {
            var entry = new ServerDirectoryEntry(new ApplicationEntity
                                                     {
                                                         AETitle = theDevice.AeTitle,
                                                         Description = theDevice.Description,
                                                         Name = theDevice.Name,
                                                         ScpParameters =
                                                             new ScpParameters(theDevice.IpAddress, theDevice.Port)
                                                     });

            return entry;
        }

        #endregion
    }
}
