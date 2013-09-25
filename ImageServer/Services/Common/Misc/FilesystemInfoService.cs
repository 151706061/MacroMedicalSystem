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

using Macro.Common;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.ServiceModel;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Enterprise;

namespace Macro.ImageServer.Services.Common.Misc
{
    [ServiceImplementsContract(typeof(IFilesystemService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class FilesystemInfoService : IApplicationServiceLayer, IFilesystemService
    {
        #region IFilesystemService Members

        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return FilesystemUtils.GetDirectoryInfo(path);
        }

        #endregion
    }
}