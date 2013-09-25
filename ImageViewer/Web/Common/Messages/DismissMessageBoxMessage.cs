#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using Macro.Web.Common;

namespace Macro.ImageViewer.Web.Common.Messages
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public enum WebDialogBoxAction
    {
        /// <summary>
        /// An Ok button should be shown.
        /// </summary>
        [EnumMember]
        Ok = 0x0001,

        /// <summary>
        /// A Cancel button should be shown.
        /// </summary>
        [EnumMember]
        Cancel = 0x0002,

        /// <summary>
        /// A Yes button should be shown.
        /// </summary>
        [EnumMember]
        Yes = 0x0004,

        /// <summary>
        /// A No button should be shown.
        /// </summary>
        [EnumMember]
        No = 0x0008,
    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class DismissMessageBoxMessage : Message
    {
        [DataMember(IsRequired = true)]
        public WebDialogBoxAction Result { get; set; }
    }
}
