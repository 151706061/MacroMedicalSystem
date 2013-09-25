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
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete server partition entries in the database.
    /// </summary>
    public class PartitionArchiveDataAdapter :
        BaseAdaptor
            <PartitionArchive, IPartitionArchiveEntityBroker, PartitionArchiveSelectCriteria, PartitionArchiveUpdateColumns>
    {
        #region Public methods

        /// <summary>
        /// Gets a list of all server partitions.
        /// </summary>
        /// <returns></returns>
        public IList<PartitionArchive> GetPartitionArchives()
        {
            return Get();
        }

        public IList<PartitionArchive> GetPartitionArchives(PartitionArchiveSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Creats a new server parition.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddPartitionArchive(PartitionArchive partition)
        {
            PartitionArchiveAdaptor adaptor = new PartitionArchiveAdaptor();
            PartitionArchiveUpdateColumns columns = new PartitionArchiveUpdateColumns();
                
            columns.Description = partition.Description;
            columns.ArchiveDelayHours = partition.ArchiveDelayHours;
            columns.ArchiveTypeEnum = partition.ArchiveTypeEnum;
            columns.ConfigurationXml = partition.ConfigurationXml;
            columns.Enabled = partition.Enabled;
            columns.ReadOnly = partition.ReadOnly;
            columns.ServerPartitionKey = partition.ServerPartitionKey;

            adaptor.Add(columns);

            return true;
        }

        public bool Update(PartitionArchive partition)
        {
            PartitionArchiveUpdateColumns parms = new PartitionArchiveUpdateColumns();
            parms.Description = partition.Description;
            parms.ServerPartitionKey = partition.ServerPartitionKey;
            parms.ArchiveTypeEnum = partition.ArchiveTypeEnum;
            parms.ConfigurationXml = partition.ConfigurationXml;
            parms.Enabled = partition.Enabled;
            parms.ReadOnly = partition.ReadOnly;
            parms.ArchiveDelayHours = partition.ArchiveDelayHours;
            
            
            return Update(partition.Key, parms);
        }

        #endregion Public methods
    }
}

