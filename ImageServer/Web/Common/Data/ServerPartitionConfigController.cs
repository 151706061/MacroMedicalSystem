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
    public class ServerPartitionConfigController
    {
        #region Private members

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private readonly ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a partition in the database.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddPartition(ServerPartition partition, List<string> groupsWithAccess)
        {
            Platform.Log(LogLevel.Info, "Adding new server partition : AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.AddServerPartition(partition, groupsWithAccess);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition added : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to add Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        /// <summary>
        /// Update the partition whose GUID and new information are specified in <paramref name="partition"/>.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool UpdatePartition(ServerPartition partition, List<string> groupsWithAccess)
        {
            Platform.Log(LogLevel.Info, "Updating server partition: AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.Update(partition, groupsWithAccess);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition updated : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to update Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        

        /// <summary>
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public IList<ServerPartition> GetPartitions(ServerPartitionSelectCriteria criteria)
        {
            return _serverAdapter.GetServerPartitions(criteria);
        }

        /// <summary>
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public ServerPartition GetPartition(ServerEntityKey key)
        {
            return _serverAdapter.GetServerPartition(key);
        }

        /// <summary>
        /// Retrieves all server paritions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetAllPartitions()
        {
        	ServerPartitionSelectCriteria searchCriteria = new ServerPartitionSelectCriteria();
        	searchCriteria.AeTitle.SortAsc(0);
			return GetPartitions(searchCriteria);
        }

        /// <summary>
        /// Checks if a specified partition can be deleted
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool CanDelete(ServerPartition partition)
        {
            return partition.StudyCount <= 0;
        }


        /// <summary>
        /// Delete the specified partition
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool Delete(ServerPartition partition)
        {
            Platform.Log(LogLevel.Info, "Deleting server partition: AETitle = {0}", partition.AeTitle);

            try
            {
                using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IDeleteServerPartition broker = ctx.GetBroker<IDeleteServerPartition>();
                    ServerPartitionDeleteParameters parms = new ServerPartitionDeleteParameters();
                    parms.ServerPartitionKey = partition.Key;
                    if (!broker.Execute(parms))
                        throw new Exception("Unable to delete server partition from database");
                    ctx.Commit();
                }

                Platform.Log(LogLevel.Info, "Server Partition deleted : AETitle = {0}", partition.AeTitle);
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e, "Failed to delete Server Partition: AETitle = {0}", partition.AeTitle);
                return false;
            }


        }

        #endregion // public methods

    }
}
