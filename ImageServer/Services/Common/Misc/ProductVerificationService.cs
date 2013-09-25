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
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.ServiceModel;
using Macro.ImageServer.Enterprise;
using Macro.Utilities.Manifest;

namespace Macro.ImageServer.Services.Common.Misc
{
    [ServiceImplementsContract(typeof(IProductVerificationService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint),Enabled=true)]
    public class ProductVerificationService : IApplicationServiceLayer, IProductVerificationService
    {
        private readonly TimeSpan ManifestRecheckTimeSpan = TimeSpan.FromMinutes(15);
        private DateTime? _lastCheckTimestamp;

        public ProductVerificationResponse Verify(ProductVerificationRequest request)
        {
            if (_lastCheckTimestamp == null || (DateTime.Now - _lastCheckTimestamp.Value) > ManifestRecheckTimeSpan)
            {
                ManifestVerification.Invalidate();
            }

            _lastCheckTimestamp = DateTime.Now;

            return new ProductVerificationResponse
                       {
                            IsManifestValid = ManifestVerification.Valid,
                            ComponentName = ProductInformation.Component,
                            Edition = ProductInformation.Edition
                       };
        }
    }
}