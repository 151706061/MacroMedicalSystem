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

namespace Macro.ImageViewer.Web.Common
{
	[DataContract(Namespace = ViewerNamespace.Value)]
	public struct Size
	{
		public Size(System.Drawing.Size size)
		{
			Width = size.Width;
			Height = size.Height;
		}

		[DataMember(IsRequired = true)]
		public int Width;

		[DataMember(IsRequired = true)]
		public int Height;

		public override string ToString()
		{
			return String.Format("W={0}, H={1}", Width, Height);
		}

		public static implicit operator System.Drawing.Size(Size size)
		{
			return new System.Drawing.Size(size.Width, size.Height);
		}

		public static implicit operator Size(System.Drawing.Size size)
		{
			return new Size(size);
		}
	}
}