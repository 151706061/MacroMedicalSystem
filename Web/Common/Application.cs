#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace Macro.Web.Common
{
	[DataContract(Namespace = Namespace.Value)]
	public abstract class Application
	{
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }
	}
}
