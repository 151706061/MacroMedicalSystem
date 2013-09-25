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

namespace Macro.ImageViewer.Web.Common.Messages
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MouseMoveMessage : MouseMessage
    {
        public override string ToString()
        {
            return String.Format("Mouse Move Message {0} {1} {2} X={3} Y={4}", 
                                 Identifier, Button, MouseButtonState, MousePosition.X, MousePosition.Y);
        }
    }
}