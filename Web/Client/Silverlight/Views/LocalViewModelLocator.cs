#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using Macro.Web.Client.Silverlight.ViewModel;

namespace Macro.Web.Client.Silverlight.Views
{
    public class LocalViewModelLocator
    {
        private static readonly object SyncLock = new Object();

        private static LogPanelViewModel _search;
      

        public LogPanelViewModel LogPanel
        {
            get
            {
                lock (SyncLock)
                {
                    return _search ?? (_search = new LogPanelViewModel());
                }
            }
        }
    }
}
