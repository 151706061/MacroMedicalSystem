#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.Enterprise.Core;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.Parameters;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete file system entries in the database.
    /// </summary>
    ///
    public class FileSystemDataAdapter : BaseAdaptor<Filesystem, IFilesystemEntityBroker, FilesystemSelectCriteria,FilesystemUpdateColumns>
    {
        #region Public methods

        /// <summary>
        /// Gets a list of all file systems.
        /// </summary>
        /// <returns></returns>
        public IList<Filesystem> GetAllFileSystems()
        {
            return Get();
        }

        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Creats a new file system.
        /// </summary>
        /// <param name="filesystem"></param>
        public bool AddFileSystem(Filesystem filesystem)
        {
            bool ok;

            // This filesystem update must be used, because the stored procedure does some 
            // additional work on insert.
            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertFilesystem insert = ctx.GetBroker<IInsertFilesystem>();
                FilesystemInsertParameters parms = new FilesystemInsertParameters();
                parms.Description = filesystem.Description;
                parms.Enabled = filesystem.Enabled;
                parms.FilesystemPath = filesystem.FilesystemPath;
                parms.ReadOnly = filesystem.ReadOnly;
                parms.TypeEnum = filesystem.FilesystemTierEnum;
                parms.WriteOnly = filesystem.WriteOnly;
                parms.HighWatermark = filesystem.HighWatermark;
                parms.LowWatermark = filesystem.LowWatermark;

                Filesystem newFilesystem = insert.FindOne(parms);

				ok = newFilesystem != null;

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }


        public bool Update(Filesystem filesystem)
        {

            FilesystemUpdateColumns parms = new FilesystemUpdateColumns();
            parms.Description = filesystem.Description;
            parms.Enabled = filesystem.Enabled;
            parms.FilesystemPath = filesystem.FilesystemPath;
            parms.ReadOnly = filesystem.ReadOnly;
            parms.FilesystemTierEnum = filesystem.FilesystemTierEnum;
            parms.WriteOnly = filesystem.WriteOnly;
            parms.HighWatermark = filesystem.HighWatermark;
            parms.LowWatermark = filesystem.LowWatermark;


            return Update(filesystem.Key, parms);
        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            return FilesystemTierEnum.GetAll();
        }

        #endregion Public methods
    }
}
