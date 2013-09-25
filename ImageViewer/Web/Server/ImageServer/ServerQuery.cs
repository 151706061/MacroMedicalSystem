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
using Macro.Dicom;
using Macro.Enterprise.Core;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;


namespace Macro.ImageViewer.Web.Server.ImageServer
{
    abstract class ServerQuery : IDisposable
    {
        public delegate void ServerQueryResultDelegate(DicomAttributeCollection row);

        #region Private members
        private bool _cancelReceived;
        private readonly object _syncLock = new object();
        #endregion

        #region Public Properties
        public ServerPartition Partition
        {
            get;
            set;
        }
        
        public bool CancelReceived
        {
            get
            {
                lock (_syncLock)
                    return _cancelReceived;
            }
            set
            {
                lock (_syncLock)
                    _cancelReceived = value;
            }
        }
        #endregion

        #region Constructors
        protected ServerQuery(ServerPartition partition)
        {
            Partition = partition;
        }
        #endregion

        public abstract void Query(DicomAttributeCollection query, ServerQueryResultDelegate del);

        /// <summary>
        /// Find the <see cref="ServerEntityKey"/> reference for a given Study Instance UID and Server Partition.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="studyInstanceUid">The list of Study Instance Uids for which to retrieve the table keys.</param>
        /// <returns>A list of <see cref="ServerEntityKey"/>s.</returns>
        protected List<ServerEntityKey> LoadStudyKey(IPersistenceContext read, string[] studyInstanceUid)
        {
            var find = read.GetBroker<IStudyEntityBroker>();

            var criteria = new StudySelectCriteria();

            if (Partition!=null)
                criteria.ServerPartitionKey.EqualTo(Partition.Key);

            if (studyInstanceUid.Length > 1)
                criteria.StudyInstanceUid.In(studyInstanceUid);
            else
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid[0]);

            IList<Study> list = find.Find(criteria);

            var serverList = new List<ServerEntityKey>();

            foreach (Study row in list)
                serverList.Add(row.GetKey());

            return serverList;
        }

        public void Dispose()
        {
            
        }
    }
}



