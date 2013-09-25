#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Windows;

namespace Macro.Web.Client.Silverlight.Utilities
{
    public static class UIThread
    {
        /// <summary>
        /// Helper class to ensure an action is executed on the UI thread. 
        /// If the current thread is no the UI thread, the action will be queued for execution.
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
                action.Invoke();
            else
                Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
