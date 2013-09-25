#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using Macro.Common.Utilities;
using Macro.Web.Common;
using System.Text;
using System;

namespace Macro.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class ImageBox : Entity
    {
		[DataMember(IsRequired = true)]
		public RectangleF NormalizedRectangle { get; set; }
		
		[DataMember(IsRequired = true)]
        public Tile[] Tiles { get; set; }

		[DataMember(IsRequired = true)]
		public bool Selected { get; set; }

		[DataMember(IsRequired = true)]
        public int ImageCount { get; set; }

		[DataMember(IsRequired = true)]
        public int TopLeftPresentationImageIndex { get; set; }

		public override string ToString()
		{
			return String.Format("{0} [Tiles{{Count={1}}}, NormalizedRectangle={2}]", 
									base.ToString(), Tiles == null ? 0 : Tiles.Length, NormalizedRectangle);
		}
    }
}