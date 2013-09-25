#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;

namespace Macro.ImageServer.Web.Common
{
    public class Cache
    {
        private readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(15);

        private static readonly Cache _instance = new Cache();

        public static Cache Current
        {
            get { return _instance; }
        }

        private Cache(){}

        public object this[string key]
        {
            get
            {
                return System.Web.HttpContext.Current.Cache[key];
            }
            set
            {
                System.Web.HttpContext.Current.Cache.Remove(key); // returns null if it's not in the cache
                System.Web.HttpContext.Current.Cache.Add(key, value, null,
                                                     System.Web.Caching.Cache.NoAbsoluteExpiration,
                                                     CacheDuration, System.Web.Caching.CacheItemPriority.Normal, null);
                
                
            }
        }

        public bool Contains(string key)
        {
            return this[key] != null;
        }
    }
}