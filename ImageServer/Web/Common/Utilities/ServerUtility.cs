#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Common;
using Macro.ImageServer.Common.ServiceModel;
using Macro.ImageServer.Common.Utilities;

namespace Macro.ImageServer.Web.Common.Utilities
{
    public static class ServerUtility
    {
        /// <summary>
        /// Retrieves the <see cref="FilesystemInfo"/> for a specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FilesystemInfo GetFilesystemInfo(string path)
        {
            FilesystemInfo fsInfo = null;
            Platform.GetService(delegate(IFilesystemService service)
            {
                fsInfo = service.GetFilesystemInfo(path);
            });

            return fsInfo;
        }
    }
}
