﻿#region License

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
using System.Collections.Generic;
using System.IO;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Dicom;
using Macro.Dicom.Network;
using Macro.Dicom.ServiceModel.Query;
using Macro.ImageViewer.Common;
using Macro.ImageViewer.Common.Auditing;
using Macro.ImageViewer.Common.DicomServer;
using Macro.ImageViewer.Common.StudyManagement;
using Macro.ImageViewer.Common.WorkItem;

namespace Macro.ImageViewer.StudyManagement.Core
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class PublishFilesServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IPublishFiles))
                return null;

            return new DicomFilePublisher();
        }

        #endregion
    }

    public class DicomFilePublishingException : Exception
    {
        internal DicomFilePublishingException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Internal class for publishing files.  Must be accessed through service provider for <see cref="IPublishFiles"/>.
    /// </summary>
    internal class DicomFilePublisher : IPublishFiles
    {
        private static void DeleteEmptyFolders(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
                return;

            foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                try
                {
                    FileInfo[] files = subDirectory.GetFiles();
                    if (files.Length == 0)
                        subDirectory.Delete();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Info, e, "Failed to delete old temp directory ({0}).", subDirectory);
                }
            }
        }

        private static string FixTempDirectoryPrefix(string prefix)
        {
            //replace directory separators with spaces.
            prefix = prefix.Replace('\\', ' ');
            prefix = prefix.Replace('/', ' ');

            //Replace the wildcard characters as well.
            prefix = prefix.Replace('*', ' ');
            prefix = prefix.Replace('?', ' ');

            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char invalidChar in invalidChars)
                prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

            return prefix;
        }

        private static string GetTempDirectory(string path, string prefix)
        {
            prefix = FixTempDirectoryPrefix(prefix);
            DateTime dateTime = DateTime.Now;

            int number = 0;
            string tempDirectory;
            do
            {
                tempDirectory = String.Format("{0}.{1}.{2}", prefix, dateTime.Date.ToString("yyyyMMdd"), ++number);
                tempDirectory = Path.Combine(path, tempDirectory);
            } while (Directory.Exists(tempDirectory));

            return tempDirectory;
        }

        private static void SaveFiles(IEnumerable<DicomFile> files, string tempDirectoryPrefix, out string tempDirectory, out List<string> savedFiles)
        {
            tempDirectory = Path.Combine(Platform.ApplicationDataDirectory, "Publishing");

            DeleteEmptyFolders(tempDirectory);
            tempDirectory = GetTempDirectory(tempDirectory, tempDirectoryPrefix);
            Directory.CreateDirectory(tempDirectory);
            savedFiles = new List<string>();

            foreach (DicomFile file in files)
            {
                string savePath = Path.Combine(tempDirectory, file.DataSet[DicomTags.SopInstanceUid] + ".dcm");
                file.Save(savePath);
                savedFiles.Add(savePath);
            }
        }

        private static DicomServerConfiguration GetServerConfiguration()
        {
            return Common.DicomServer.DicomServer.GetConfiguration();
        }

        public void PublishLocal(ICollection<DicomFile> files)
        {
            if (files == null || files.Count == 0)
                return;

            var configuration = GetServerConfiguration();
            var context = new ImportStudyContext(configuration.AETitle, StudyStore.GetConfiguration(),EventSource.CurrentUser);

            var utility = new ImportFilesUtility(context);

            try
            {
                DicomProcessingResult failureResult = null;

                foreach (var file in files)
                {
                    var importResult = utility.Import(file, BadFileBehaviourEnum.Ignore, FileImportBehaviourEnum.Save);
                    if (importResult.DicomStatus != DicomStatuses.Success)
                    {
                        Platform.Log(LogLevel.Warn, "Unable to import published file: {0}", importResult.ErrorMessage);
                        failureResult = importResult;
                    }
                }

                if (failureResult != null)
                    throw new ApplicationException(failureResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to import files");
                throw new DicomFilePublishingException(message, ex);
            }
              
        }

        public void PublishRemote(ICollection<DicomFile> files, IDicomServiceNode destinationServer)
        {
            if (files == null || files.Count == 0)
                return;

            // cache files to temporary storage
            string tempFileDirectory;
            List<string> savedFiles;
            SaveFiles(files, destinationServer.AETitle, out tempFileDirectory, out savedFiles);

            try
            {
                var client = new DicomSendBridge();
                client.PublishFiles(destinationServer, new StudyRootStudyIdentifier(CollectionUtils.FirstElement(files).DataSet),
                                    DeletionBehaviour.DeleteOnSuccess, savedFiles);
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to connect to the dicom send service to send files.  The files must be published manually (location: {0})", tempFileDirectory);
                throw new DicomFilePublishingException(message, ex);
            }
        }

        public bool CanPublish()
        {
            return WorkItemActivityMonitor.IsRunning;
        }
    }
}
