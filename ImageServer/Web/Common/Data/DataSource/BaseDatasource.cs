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

namespace Macro.ImageServer.Web.Common.Data.DataSource
{
    public class BaseDataSource
    {
        protected Array resizeArray(Array oldArray, int newSize)
        {
            int oldSize = oldArray.Length;
            Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);
            int preserveLength = System.Math.Min(oldSize, newSize);
            if (preserveLength > 0)
                System.Array.Copy(oldArray, newArray, preserveLength);


            return newArray;
        }

        protected int adjustCopyLength(int startRowIndex, int maximumRows, int arrayLength)
        {
            int copyLength = 0;
            if (startRowIndex > 0)
            {
                copyLength = (startRowIndex + maximumRows) > arrayLength
                                 ? arrayLength - startRowIndex
                                 : maximumRows;
            }
            else
            {
                copyLength = arrayLength < maximumRows ? arrayLength : maximumRows;
            }

            return copyLength;
        }
    }
}
