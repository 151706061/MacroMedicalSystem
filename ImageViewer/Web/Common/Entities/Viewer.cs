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
using Macro.Web.Common;

namespace Macro.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class Viewer : Entity
    {
		[DataMember(IsRequired = false)]
		public WebActionNode[] ToolbarActions { get; set; }

		[DataMember(IsRequired = true)]
		public ImageBox[] ImageBoxes { get; set; }

        [DataMember(IsRequired = false)]
        public WebIconSize ToolStripIconSize { get; set; }
	}
}