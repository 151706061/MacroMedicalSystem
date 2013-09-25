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
using System.Runtime.InteropServices;

namespace Macro.ImageViewer.Utilities.Media.View.WinForms
{
    public class WndProMsgConst
    {

        #region WndProc常量
        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        public const int WM_DEVICECHANGE = 0x219;
        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        public const int DBT_DEVICEARRIVAL = 0x8000;
        /// <summary>
        /// A request to change the current configuration (dock or undock) has been canceled.
        /// </summary>
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        /// <summary>
        /// The current configuration has changed, due to a dock or undock.
        /// </summary>
        public const int DBT_CONFIGCHANGED = 0x0018;
        /// <summary>
        /// A custom event has occurred.
        /// </summary>
        public const int DBT_CUSTOMEVENT = 0x8006;
        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        /// <summary>
        /// A device or piece of media is about to be removed. Cannot be denied.
        /// </summary>
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        /// <summary>
        /// A device-specific event has occurred.
        /// </summary>
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        /// <summary>
        /// A device has been added to or removed from the system.
        /// </summary>
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        /// <summary>
        /// Permission is requested to change the current configuration (dock or undock).
        /// </summary>
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        /// <summary>
        /// The meaning of this message is user-defined.
        /// </summary>
        public const int DBT_USERDEFINED = 0xFFFF;
        #endregion

        #region 设备类型
        /// <summary>
        /// Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
        /// </summary>
        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        /// <summary>
        /// File system handle. This structure is a DEV_BROADCAST_HANDLE structure.
        /// </summary>
        public const int DBT_DEVTYP_HANDLE = 0x00000006;
        /// <summary>
        /// OEM- or IHV-defined device type. This structure is a DEV_BROADCAST_OEM structure.
        /// </summary>
        public const int DBT_DEVTYP_OEM = 0x00000000;
        /// <summary>
        /// Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
        /// </summary>
        public const int DBT_DEVTYP_PORT = 0x00000003;
        /// <summary>
        /// Logical volume. This structure is a DEV_BROADCAST_VOLUME structure.
        /// </summary>
        public const int DBT_DEVTYP_VOLUME = 0x00000002;
        #endregion

        #region 逻辑卷类型区分
        /// <summary>
        /// Change affects media in drive. If not set, change affects physical device or drive.
        /// </summary>
        public const int DBTF_MEDIA = 0x0001;
        /// <summary>
        /// ndicated logical volume is a network volume.
        /// </summary>
        public const int DBTF_NET = 0x0002;
        #endregion


    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_VOLUME
    {
        public int dbcv_size;
        public int dbcv_devicetype;
        public int dbcv_reserved;
        public int dbcv_unitmask;
        public int dbcv_flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_HDR
    {
        public int dbch_size;
        public int dbch_devicetype;
        public int dbch_reserved;
    }

}
