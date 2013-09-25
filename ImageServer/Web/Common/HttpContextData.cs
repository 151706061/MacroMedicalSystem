#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web;
using Macro.Enterprise.Core;

namespace Macro.ImageServer.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpContextData: IDisposable
    {
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private const string CUSTOM_DATA_ENTRY = "CUSTOM_DATA_ENTRY";
        private IReadContext _readContext;
        private readonly object _syncRoot = new object();

        private HttpContextData()
        {
        }

        static public HttpContextData Current
        {
            get
            {
                lock( HttpContext.Current.Items.SyncRoot)
                {
                    HttpContextData instance = HttpContext.Current.Items[CUSTOM_DATA_ENTRY] as HttpContextData;
                    if (instance == null)
                    {
                        instance = new HttpContextData();
                        HttpContext.Current.Items[CUSTOM_DATA_ENTRY] = instance;
                    }
                    return instance;
                }
                
            }
        }

        public IReadContext ReadContext
        {
            get
            {
                if (_readContext == null)
                {
                    lock (_syncRoot)
                    {
                        if (_readContext == null)
                        {
                            _readContext = _store.OpenReadContext();
                        }
                    }
                }
                return _readContext;
                
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_readContext != null)
            {
                lock (_syncRoot)
                {
                    if (_readContext != null)
                    {
                        _readContext.Dispose();
                        _readContext = null;
                    }
                }
            }
        }

        #endregion
    }
}