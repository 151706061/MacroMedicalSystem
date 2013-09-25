#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace Macro.Web.Common
{
    [DataContract(Namespace = Namespace.Value)]
    public class PerformanceData
    {
        [DataMember(IsRequired = true)]
        public string ClientIp { get; set; }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public object Value { get; set; }

    }
}