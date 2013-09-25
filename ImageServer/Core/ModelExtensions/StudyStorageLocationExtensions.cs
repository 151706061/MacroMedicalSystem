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
using System.Linq;
using System.Text;
using System.IO;
using Macro.ImageServer.Model;
using Macro.ImageServer.Common;

namespace Macro.ImageServer.Core.ModelExtensions
{

    public static class StudyStorageLocationExtensions
    {
        /// <summary>
        /// Returns the path to the Reconcile folder for the Partition in the same filesystem <see cref="location"/> where study is stored.
        /// This is usually \\filesystemPath\PartitionFolder\RECONCILE
        /// </summary>
        /// <returns></returns>
        public static string GetReconcileRootPath(this StudyStorageLocation location)
        {
            string path = Path.Combine(location.FilesystemPath, location.PartitionFolder);
            path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            return path;
        }
    }
}
