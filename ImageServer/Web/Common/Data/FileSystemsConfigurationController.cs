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
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Defines the interface of a file system configuration controller.
    /// </summary>
    public interface IFileSystemsConfigurationController
    {
        bool AddFileSystem(Filesystem filesystem);
        bool UpdateFileSystem(Filesystem filesystem);
        IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria);
        IList<Filesystem> GetAllFileSystems();

        IList<FilesystemTierEnum> GetFileSystemTiers();
    }

    public class FileSystemsConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set filesystems from Filesystem table
        /// </summary>
        private FileSystemDataAdapter _adapter = new FileSystemDataAdapter();

        #endregion

        #region public methods

        public bool AddFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Adding new filesystem : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            bool ok = _adapter.AddFileSystem(filesystem);

            Platform.Log(LogLevel.Info, "New filesystem added : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            return ok;
        }

        public bool UpdateFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Updating filesystem : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            bool ok = _adapter.Update(filesystem);

            if (ok)
                Platform.Log(LogLevel.Info, "Filesystem updated: description = {0}, path={1}", filesystem.Description,
                             filesystem.FilesystemPath);
            else
                Platform.Log(LogLevel.Info, "Unable to update Filesystem: description = {0}, path={1}",
                             filesystem.Description, filesystem.FilesystemPath);

            return ok;
        }

        public Filesystem LoadFileSystem(ServerEntityKey key)
        {
            return Filesystem.Load(key);
        }

        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            return _adapter.GetFileSystems(criteria);
        }

        public IList<Filesystem> GetFileSystems(IList<ServerEntityKey> keys)
        {
            List<Filesystem> fileSystems = new List<Filesystem>();

            foreach(ServerEntityKey key in keys)
            {
                fileSystems.Add(LoadFileSystem(key));
            }

            return fileSystems;
        }

        public IList<Filesystem> GetAllFileSystems()
        {
            return _adapter.GetAllFileSystems();
        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            return _adapter.GetFileSystemTiers();
        }

        #endregion public methods
    }
}
