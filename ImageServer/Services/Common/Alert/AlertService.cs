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
using Macro.Common.Utilities;
using Macro.Enterprise.Common;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.ServiceModel;
using Macro.ImageServer.Enterprise;

namespace Macro.ImageServer.Services.Common.Alert
{
    /// <summary>
    /// Alert record service
    /// </summary>
    [ServiceImplementsContract(typeof(IAlertService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class AlertService : IApplicationServiceLayer, IAlertService
    {
        #region Private Members
        private IAlertServiceExtension[] _extensions;
        #endregion

        #region Private Methods
        
        private IAlertServiceExtension[] GetExtensions()
        {
            if (_extensions == null)
            {
                _extensions =
                    CollectionUtils.ToArray<IAlertServiceExtension>(new AlertServiceExtensionPoint().CreateExtensions());
            }

            return _extensions;
        }

        #endregion

        #region IAlertService Members

        public void GenerateAlert(ImageServer.Common.Alert alert)
        {
            IAlertServiceExtension[] extensions = GetExtensions();
            foreach(IAlertServiceExtension ext in extensions)
            {
                try
                {
                    ext.OnAlert(alert);    
                }
                catch(Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error occurred when calling {0} OnAlert()", ext.GetType());
                }
            }

        }
       

        #endregion
    }
}