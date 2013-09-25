#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.Common;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Common
{

    public interface IServerPartitionTabsExtension
    {
        IEnumerable<ServerPartition> LoadServerPartitions();
    }
    
    
    [ExtensionPoint]
    public class ServerPartitionTabsExtensionPoint : ExtensionPoint<IServerPartitionTabsExtension> { }


    public sealed class DefaultServerPartitionTabsExtension : IServerPartitionTabsExtension
    {
        private readonly ServerPartitionConfigController _controller = new ServerPartitionConfigController();

        public IEnumerable<ServerPartition> LoadServerPartitions()
        {
            return _controller.GetAllPartitions();
        }
    }
}