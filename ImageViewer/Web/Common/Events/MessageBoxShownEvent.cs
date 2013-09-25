#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.Web.Common;

namespace Macro.ImageViewer.Web.Common.Events
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MessageBoxShownEvent : Event
    {
        [DataMember(IsRequired = true)]
        public MessageBox MessageBox { get; set; }

        public override string ToString()
        {
            return string.Format("MessageBoxShownEvent [{0}]", MessageBox);
        }
    }
}
