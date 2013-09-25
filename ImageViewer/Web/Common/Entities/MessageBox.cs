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
using Macro.Web.Common;

namespace Macro.ImageViewer.Web.Common.Entities
{

    [DataContract(Namespace = ViewerNamespace.Value)]
    public enum WebMessageBoxActions
    {
        /// <summary>
        /// An Ok button should be shown.
        /// </summary>
        [EnumMember]
        Ok,

        /// <summary>
        /// Both an Ok and a Cancel button should be shown.
        /// </summary>
        [EnumMember]
        OkCancel,

        /// <summary>
        /// Both a Yes and No button should be shown.
        /// </summary>
        [EnumMember]
        YesNo,

        /// <summary>
        /// Yes, No and Cancel buttons should be shown.
        /// </summary>
        [EnumMember]
        YesNoCancel
    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MessageBox : Entity
    {
        [DataMember(IsRequired = true)]
        public string Title { get; set; }

        [DataMember(IsRequired = true)]
        public string Message { get; set; }

        [DataMember(IsRequired = true)]
        public WebMessageBoxActions Actions { get; set; }

		public override string ToString()
		{
			return String.Format("{0} [Title={1}, Message={2}]", base.ToString(), Title ?? "", Message ?? "");
		}
	}
}
