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
    [DataContract]
    public class SessionValidationFault
    {
        [DataMember(IsRequired=false)]
        public string ErrorMessage { get; set; }
    }

    [DataContract(Namespace = Namespace.Value)]
    public class OutOfResourceFault
    {
        [DataMember(IsRequired = false)]
        public string ErrorMessage { get; set; }
    }
}