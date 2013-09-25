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

namespace Macro.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class Tile : Entity
    {
		[DataMember(IsRequired = true)]
		public RectangleF NormalizedRectangle { get; set; }

		[DataMember(IsRequired = false)]
		public Rectangle ClientRectangle { get; set; }

		[DataMember(IsRequired = false)]
		public bool Selected { get; set; }

		[DataMember(IsRequired = false)]
        public byte[] Image { get; set; }

		[DataMember(IsRequired = false)]
		public bool HasCapture { get; set; }

		[DataMember(IsRequired = false)]
		public Cursor Cursor { get; set; }

		[DataMember(IsRequired = false)]
		public Position MousePosition { get; set; }

        [DataMember(IsRequired = false)]
        public InformationBox InformationBox { get; set; }

		// TODO: Still need this?
        [DataMember(IsRequired = false)]
        public long DrawTime { get; set; }
    }
}