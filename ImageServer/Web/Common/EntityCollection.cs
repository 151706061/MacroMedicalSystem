#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Web.Common;

namespace Macro.ImageServer.Web.Common
{
    /// <summary>
    /// Encapsulates a generic collection of object which can be indexed based on the <see cref="ServerEntityKey"/>
    /// </summary>
    public class EntityCollection<T> : KeyedCollectionBase<T, ServerEntityKey>
        where T:ServerEntity
    {
        protected override ServerEntityKey GetKey(T item)
        {
            return item.Key;
        }
    }
}