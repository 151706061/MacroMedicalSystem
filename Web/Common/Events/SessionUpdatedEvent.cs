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
    public class SessionUpdatedEvent : Event
    {
        //TODO (CR May 2010): IsRequired?
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public DateTime ExpiryTimeUtc { get; set; }
    }
}