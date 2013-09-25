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
    public class PartitionNotFoundException : BaseWebException
    {
        public PartitionNotFoundException(string serverAE, string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.PartitionNotFound, serverAE);
            ErrorDescription = ExceptionMessages.PartitonNotFoundDescription;
            LogMessage = logMessage;
        }

        public PartitionNotFoundException(string serverAE)
        {
            ErrorMessage = string.Format(ExceptionMessages.PartitionNotFound, serverAE);
            ErrorDescription = ExceptionMessages.PartitonNotFoundDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
