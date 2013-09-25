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

namespace Macro.ImageViewer.Web.Common.Events
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class TileUpdatedEvent : Event
    {
        [DataMember(IsRequired = true)]
        public Entities.Tile Tile { get; set; } //Fired on the server whenever Tile.Draw is called (through the callback) - Image data included.

        [DataMember(IsRequired = true)]
        public string MimeType { get; set; }

        [DataMember(IsRequired = false)]
        public long Quality { get; set; }

        public override bool AllowSendInBatch
        {
            get
            {
                // It may be counter-intuitive to disallow multiple tile update 
                // events to be sent in a single batch. It actually gives better
                // user experience if images are sent in separate messages. In case of
                // stacking, the delay between each message makes users feel 
                // they are actually stacking
                return false;
            }
        }
    }

	//TODO: delete ?
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class SpecialImageEvent : Event
    {
        [DataMember(IsRequired = false)]
        public byte[] Image { get; set; }

    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MouseMoveProcessedEvent : Event
    {
    }
}