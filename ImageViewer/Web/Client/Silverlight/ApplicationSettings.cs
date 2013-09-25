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
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using System.Threading;

namespace Macro.ImageViewer.Web.Client.Silverlight
{
    public enum ApplicationServiceMode
    {
        BasicHttp        
    }

    public class ApplicationStartupParameters
    {
        static private ApplicationStartupParameters _current = new ApplicationStartupParameters(Application.Current.Host.InitParams);

        public string TimeoutUrl { get; set; }
        public string Username { get; set; }
        public string SessionToken { get; set; }
		public bool IsSessionShared { get; private set; }
        public bool LogPerformance { get; private set; }
        public string LocalIPAddress { get; private set; }
        public ApplicationServiceMode Mode { get; set; }
        public string Language { get; private set; }

        private ApplicationStartupParameters(System.Collections.Generic.IDictionary<string, string> initParams)
        {
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.TimeoutUrl))
                TimeoutUrl = initParams[Constants.SilverlightInitParameters.TimeoutUrl];

            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Username))
                Username = initParams[Constants.SilverlightInitParameters.Username];       

            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Session))
                SessionToken = initParams[Constants.SilverlightInitParameters.Session];

			if (initParams.ContainsKey(Constants.SilverlightInitParameters.IsSessionShared))
				IsSessionShared = initParams[Constants.SilverlightInitParameters.IsSessionShared] == "true";

            LocalIPAddress = initParams.ContainsKey(Constants.SilverlightInitParameters.LocalIPAddress) 
                ? initParams[Constants.SilverlightInitParameters.LocalIPAddress] 
                : SR.Unknown;

            bool logPerformance;
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.LogPerformance) && 
                bool.TryParse(initParams[Constants.SilverlightInitParameters.LogPerformance], out logPerformance))
            {
                LogPerformance = logPerformance;
            }
            else
            {
                LogPerformance = false;
            }

            Mode = ApplicationServiceMode.BasicHttp; 
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Mode))
            {
                ApplicationServiceMode mode;
                if (Enum.TryParse(initParams[Constants.SilverlightInitParameters.Mode], out mode))
                    Mode = mode;
            }

            Language = initParams.ContainsKey(Constants.SilverlightInitParameters.Language) 
                ? initParams[Constants.SilverlightInitParameters.Language] 
                : Thread.CurrentThread.CurrentUICulture.Name;
        }

        static public ApplicationStartupParameters Current
        {
            get
            {
                return _current;
            }
        }
    }
}
