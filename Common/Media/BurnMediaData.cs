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
using Macro.Common.Media.IMAPI2;
using System.Runtime.InteropServices;

namespace Macro.Common.Media
{
    public class BurnMediaData : IBurnMediaData
    {
        #region IBurnMedia 成员

        /// <summary>
        /// 记录下这个Media对象的路径，如果是文件，则包含文件名
        /// 如果是文件夹，则即使包含了文件名，也会自动忽略最后的文件名
        /// </summary>
        private string m_strThisPath;

        /// <summary>
        /// 文件名称
        /// </summary>
        private string m_strDisplayName;

        /// <summary>
        /// 设置路径
        /// </summary>
        public string Path
        {
            get
            {
                return m_strThisPath;
            }
            set
            {
                string strPath = value;
                string[] splitString = strPath.Split('\\');
                m_strDisplayName = splitString[splitString.Length - 1];
                m_strThisPath = value;
            }
        }

        /// <summary>
        /// 记录下这个Media对象的类型
        /// </summary>
        private MediaType m_mediaType;

        /// <summary>
        /// 设置媒体类型
        /// </summary>
        public MediaType Type
        {
            get
            {
                return m_mediaType;
            }
            set
            {
                m_mediaType = value;
            }
        }

        /// <summary>
        /// 增加到文件系统中
        /// </summary>
        /// <returns></returns>
        public bool AddToFileSystem(IFsiDirectoryItem root)
        {
            //根据类型不同，有不同的加法
            if (m_mediaType == MediaType.Dir)
            {
                return AddDir(ref root);
            }
            else
            {
                return AddFile(ref root);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootItem"></param>
        /// <returns></returns>
        private bool AddDir(ref IFsiDirectoryItem rootItem)
        {
            try
            {
                rootItem.AddTree(m_strThisPath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 加文件，完全看不懂啊
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private bool AddFile(ref IFsiDirectoryItem root)
        {
            System.Runtime.InteropServices.ComTypes.IStream stream = null;
            try
            {
                Win32.SHCreateStreamOnFile(m_strThisPath, Win32.STGM_READ | Win32.STGM_SHARE_DENY_WRITE,
                    ref stream);

                if (stream != null)
                {
                    root.AddFile(m_strDisplayName, stream);
                    return true;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.FinalReleaseComObject(stream);
                }
            }
        }

        #endregion
    }
}
