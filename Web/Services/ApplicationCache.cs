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
using System.Threading;
using Macro.Common;

namespace Macro.Web.Services
{
	internal class Cache
		{
		    private const int CheckIntervalInSeconds = 15;
            private const int ApplicationShutdownDelayInSeconds = 5;

			public static readonly Cache Instance;

			static Cache()
			{
				Instance = new Cache();
			}

			private readonly object _syncLock = new object();
			private readonly Dictionary<Guid, Application> _applications = new Dictionary<Guid, Application>();
            private Dictionary<Guid, DateTime> _appsToBeRemoved = new Dictionary<Guid, DateTime>();
		    
            private Timer _cleanupTimer;

		    internal object SyncLock
		    {
                get { return _syncLock; }
		    }

		    internal int Count
		    {
		        get{
                    lock (_syncLock)
                    {
                        return _applications.Count;
                    }
		        }
		    }

            private Cache()
            {
                // TODO: Cleanup ther timer?
                _cleanupTimer = new Timer(OnCleanupTimerCallback, null, TimeSpan.FromSeconds(CheckIntervalInSeconds),
                                          TimeSpan.FromSeconds(CheckIntervalInSeconds));
            }

            private void OnCleanupTimerCallback(object ignore)
            {
                try
                {
                    lock (_syncLock)
                    {
                        if (_appsToBeRemoved.Count > 0)
                        {

                            List<Guid> removalList = new List<Guid>();
                            foreach (Guid appId in _appsToBeRemoved.Keys)
                            {
                                if (DateTime.Now - _appsToBeRemoved[appId] >
                                    TimeSpan.FromSeconds(ApplicationShutdownDelayInSeconds))
                                {
                                    removalList.Add(appId);
                                }
                            }

                            if (removalList.Count > 0)
                            {
                                string appInstanceName = null;
                                foreach (Guid appId in removalList)
                                {
                                    Application app = null;
                                    try
                                    {

                                        app = _applications[appId];
                                        appInstanceName = app.InstanceName;
                                        _applications.Remove(appId);
                                        _appsToBeRemoved.Remove(appId);
                                    }
                                    finally
                                    {
                                        if (app != null)
                                        {
                                            app.DisposeMembers();
                                        }
                                        Platform.Log(LogLevel.Info, "{0} removed from cache.", appInstanceName);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

		    public void Add(Application application)
			{
				lock (_syncLock)
				{
					_applications.Add(application.Identifier, application);
				    var identifier = application.Identifier;
                    application.Stopped += delegate { Remove(identifier); };
				}
			}

			public Application Find(Guid applicationId)
			{
				lock (_syncLock)
				{
					Application application;
					return _applications.TryGetValue(applicationId, out application) ? application : null;
				}
			}

		    private void Remove(Guid applicationId)
			{
				lock (_syncLock)
				{
					if (!_applications.ContainsKey(applicationId))
						return;

					// App shutdown must be delayed to give the client some time to poll the remaining events.
                    if (!_appsToBeRemoved.ContainsKey(applicationId))
                        _appsToBeRemoved.Add(applicationId, DateTime.Now);            
				}
			}

            // CR 3-22-2011, This is no longer used now that we're hosted in IIS.
			public void StopAndClearAll(string message)
			{
				lock (_syncLock)
				{
					foreach(Application app in _applications.Values)
					{
                        app.Stop(message);
					    app.DisposeMembers();
					}

					_applications.Clear();
				}
			}
		}	
}
