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
using Macro.Common;
using Macro.Web.Common;

namespace Macro.Web.Services
{
    class PerformanceMonitor
    {
        private static readonly List<IPerformanceLogger> _loggers = new List<IPerformanceLogger>();
        private static bool _initialized;
        private static readonly object _syncLock = new object();

        public static void Initialize()
        {
            if (!_initialized)
            {
                lock (_syncLock)
                {
                    if (_initialized)
                        return;

                    PerformanceMonitorExtensionPoint xp = new PerformanceMonitorExtensionPoint();
                    object[] extensions = xp.CreateExtensions();
                    foreach (IPerformanceLogger logger in extensions)
                    {
                        try
                        {
                            logger.Initialize();
                            _loggers.Add(logger);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                        }
                    }

                    _initialized = true;
                }
            }
        }

        public static void Report(PerformanceData data)
        {
            foreach (IPerformanceLogger logger in _loggers)
            {
                try
                {
                    logger.Report(data.ClientIp, data);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }
            }
        }
    }

    [ExtensionPoint]
    public class PerformanceMonitorExtensionPoint:ExtensionPoint<IPerformanceLogger>
    {
        
    }

    public interface IPerformanceLogger
    {
        void Initialize();
        void Report(string clientIp, PerformanceData data);
    }
}
