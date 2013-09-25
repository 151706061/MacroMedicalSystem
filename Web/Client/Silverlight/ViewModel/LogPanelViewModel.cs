#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using Macro.Web.Client.Silverlight.Command;

namespace Macro.Web.Client.Silverlight.ViewModel
{
    public class LogPanelViewModel : ViewModelBase, IDisposable
    {
        public LogPanelViewModel()
        {
            ClearLogCommand = new RelayCommand(OnClearLog);
            Platform.Logger += Logger;
        }

        public void Dispose()
        {
            Platform.Logger -= Logger;
        }

        private void Logger(string msg)
        {
            LogContents = msg + "\n" + LogContents;
        }

        private void OnClearLog()
        {
            LogContents = string.Empty;
        }

        public RelayCommand ClearLogCommand
        {
            get;
            private set;
        }

        private string _logContents = string.Empty;
        public string LogContents
        {
            get { return _logContents; }
            set
            {
                if (_logContents == value) return;
                _logContents = value;
                RaisePropertyChanged(() => LogContents);
            }
        }

        public bool DebugLevel
        {
            get { return Platform.IsDebugEnabled; }
            set
            {
                if (Platform.IsDebugEnabled == value) return;
                Platform.IsDebugEnabled = value;
                RaisePropertyChanged(() => DebugLevel);
            }
        }

        public bool InfoLevel
        {
            get { return Platform.IsInfoEnabled; }
            set
            {
                if (Platform.IsInfoEnabled == value) return;
                Platform.IsInfoEnabled = value;
                RaisePropertyChanged(() => InfoLevel);
            }
        }

        public bool WarnLevel
        {
            get { return Platform.IsWarnEnabled; }
            set
            {
                if (Platform.IsWarnEnabled == value) return;
                Platform.IsWarnEnabled = value;
                RaisePropertyChanged(() => WarnLevel);
            }
        }

        public bool ErrorLevel
        {
            get { return Platform.IsErrorEnabled; }
            set
            {
                if (Platform.IsErrorEnabled == value) return;
                Platform.IsErrorEnabled = value;
                RaisePropertyChanged(() => ErrorLevel);
            }
        }

        public bool FatalLevel
        {
            get { return Platform.IsFatalEnabled; }
            set
            {
                if (Platform.IsFatalEnabled == value) return;
                Platform.IsFatalEnabled = value;
                RaisePropertyChanged(() => FatalLevel);
            }
        }      
    }
}
