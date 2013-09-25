#region License

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
using Macro.Common;

namespace Macro.ImageViewer.Common.DicomServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    internal class DicomServerConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IDicomServerConfiguration))
                return null;

            return new DicomServerConfigurationProxy();
        }

        #endregion
    }

    internal class DicomServerConfigurationProxy : IDicomServerConfiguration
    {
        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            var settings = new DicomServerSettings();
            return new GetDicomServerConfigurationResult { Configuration = settings.GetBasicConfiguration() };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            new DicomServerSettings().UpdateBasicConfiguration(request.Configuration);
            return new UpdateDicomServerConfigurationResult();
        }

        public GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request)
        {
            return new GetDicomServerExtendedConfigurationResult {ExtendedConfiguration = new DicomServerSettings().GetExtendedConfiguration()};
        }

        public UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            new DicomServerSettings().UpdateExtendedConfiguration(request.ExtendedConfiguration);
            return new UpdateDicomServerExtendedConfigurationResult();
        }

        #endregion
    }
}