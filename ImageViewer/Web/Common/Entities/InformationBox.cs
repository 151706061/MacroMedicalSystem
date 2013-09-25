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

namespace Macro.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class InformationBox
    {
        [DataMember(IsRequired = true)]
        public Position Location { get; set; }

        [DataMember(IsRequired = true)]
        public bool Visible { get; set; }

        [DataMember(IsRequired = true)]
        public string Data { get; set; }

		public override string ToString()
		{
			return String.Format("Location={0}, Data={1}", Location, Data ?? "");
		}
    }
}
