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
using Macro.ImageViewer.Web.Common.Entities;
using Macro.Web.Common;

namespace Macro.ImageViewer.Web.Common.Events
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class ContextMenuEvent : Event
    {
		[DataMember(IsRequired = false)]
		public WebActionNode ActionModelRoot { get; set; }
    }
}