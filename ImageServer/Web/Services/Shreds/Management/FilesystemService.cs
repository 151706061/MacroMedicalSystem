#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Web.Services.Shreds.Management
{
    [ServiceContract]
    public interface IFilesystemService
    {
        [OperationContract]
        FilesystemInfo GetFilesystemInfo(string path);
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class FilesystemService : WcfShred, IFilesystemService
    {

        #region IFilesystemService Members

        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return FilesystemUtils.GetDirectoryInfo(path);
        }

        #endregion

        #region WcfShred override
        public override void Start()
        {
            try
            {
                ServiceEndpointDescription sed = StartHttpHost<FilesystemService, IFilesystemService>("FilesystemService", SR.FilesystemServiceDisplayDescription);

            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, "Failed to start {0} : {1}", SR.FilesystemServiceDisplayName, e.StackTrace);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, SR.FilesystemServiceDisplayName,
                                     AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, SR.AlertFilesystemUnableToStart, e.Message);
            }
        }

        public override void Stop()
        {
            StopHost("FilesystemService");
        }

        public override string GetDisplayName()
        {
            return SR.FilesystemServiceDisplayName;
        }

        public override string GetDescription()
        {
            return SR.FilesystemServiceDisplayDescription;
        }

        #endregion WcfShred override
    }

}

