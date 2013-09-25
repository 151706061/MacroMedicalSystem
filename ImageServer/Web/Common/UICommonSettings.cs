#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Macro.ImageServer.Common;

namespace Macro.ImageServer.Web.Common
{
    public static class UICommonSettings
    {
        public static class  Admin
        {
            public static class Device
            {
                public static short MaxConnections
                {
                    get
                    {
                        return ImageServerCommonConfiguration.Device.MaxConnections;
                    }
                }
            }
        }
    }
}
