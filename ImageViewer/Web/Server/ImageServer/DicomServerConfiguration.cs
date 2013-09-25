#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;
using Macro.ImageViewer.Common.DicomServer;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
     
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IDicomServerConfiguration))
                return null;

            return new DicomServerConfiguration();
        }

        #endregion
    }

    internal class DicomServerConfiguration : IDicomServerConfiguration
    {
        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            return new GetDicomServerConfigurationResult
                       {
                           Configuration = new Common.DicomServer.DicomServerConfiguration
                                               {
                                                   AETitle = WebViewerServices.Default.AETitle,
                                                   HostName = WebViewerServices.Default.ArchiveServerHostname,
                                                   Port = WebViewerServices.Default.ArchiveServerPort
                                               }
                       };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            return new UpdateDicomServerConfigurationResult();
        }

        public GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request)
        {
            return new GetDicomServerExtendedConfigurationResult
                       {
                           ExtendedConfiguration = new DicomServerExtendedConfiguration()
                                                       {
                                                           AllowUnknownCaller = true
                                                       }
                       };
        }

        public UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request)
        {            
            return new UpdateDicomServerExtendedConfigurationResult();
        }

        #endregion
    }
}
