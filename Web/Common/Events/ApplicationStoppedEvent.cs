#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace Macro.Web.Common.Events
{
    [DataContract(Namespace = Namespace.Value)]
    public class ApplicationStoppedEvent : Event
    {
        [DataMember(IsRequired = true)]
        public bool IsTimedOut { get; set; }

        [DataMember(IsRequired = false)]
        public string Message { get; set; }
    }
}
