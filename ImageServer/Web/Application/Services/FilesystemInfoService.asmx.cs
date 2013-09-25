#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.Script.Services;
using System.Web.Services;
using System.ComponentModel;
using Macro.Common;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Web.Common.Utilities;
using System.Globalization;
using System.Threading;

namespace Macro.ImageServer.Web.Application.Services
{
    //// <summary>
    /// Summary description for FilesystemInfoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FilesystemInfoService : WebService
    {

        [WebMethod]
        public FilesystemInfo GetFilesystemInfo(string path)
        {
            Platform.CheckForEmptyString(path, "requested path is empty or null");

            if (Context.Request.UserLanguages != null && Context.Request.UserLanguages.Length>0)
            {
                CultureInfo culture = new CultureInfo(Context.Request.UserLanguages[0]);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            return ServerUtility.GetFilesystemInfo(path.Trim());
        }
    }
}
