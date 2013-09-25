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

namespace Macro.ImageServer.Web.Common.Exceptions
{
    public class BaseWebException : Exception
    {
        private string _logMessage;
        private string _errorMessage;
        private string _errorDescription;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }

        public BaseWebException(string errorMessage, string logMessage, string errorDescription)
        {
            _errorMessage = errorMessage;
            _errorDescription = errorDescription;
            _logMessage = logMessage;
        }

        public BaseWebException() {}

    }
}
