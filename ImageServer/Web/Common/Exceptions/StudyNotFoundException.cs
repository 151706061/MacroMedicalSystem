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
    public class StudyNotFoundException : BaseWebException
    {
        public StudyNotFoundException(string studyInstanceUID, string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = logMessage;
        }

        public StudyNotFoundException(string studyInstanceUID)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
