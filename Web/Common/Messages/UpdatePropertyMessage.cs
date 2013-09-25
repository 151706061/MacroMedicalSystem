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

namespace Macro.Web.Common.Messages
{
	[DataContract(Namespace = Namespace.Value)]
	public class UpdatePropertyMessage : Message
	{
		[DataMember(IsRequired = false)]
		public string PropertyName { get; set; }

		[DataMember(IsRequired = false)]
		public object Value { get; set; }
	}
}