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
	[DataContract(Namespace=ViewerNamespace.Value)]
    public struct Rectangle
    {
		public Rectangle(System.Drawing.Rectangle rectangle)
		{
			Top = rectangle.Top;
			Left = rectangle.Left;
			Width = rectangle.Width;
			Height = rectangle.Height;
		}

        [DataMember(IsRequired = true)]
        public int Top;

		[DataMember(IsRequired = true)]
        public int Left;

		[DataMember(IsRequired = true)]
        public int Width;

		[DataMember(IsRequired = true)]
        public int Height;

		public override string ToString()
		{
			return String.Format("L,T={0},{1}, W,H={2},{3}", Left, Top, Width, Height);
		}

		public static implicit operator System.Drawing.Rectangle(Rectangle rectangle)
		{
			return new System.Drawing.Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}

		public static implicit operator Rectangle(System.Drawing.Rectangle rectangle)
		{
			return new Rectangle(rectangle);
		}
	}

	[DataContract(Namespace = ViewerNamespace.Value)]
    public struct RectangleF
    {
		public RectangleF(System.Drawing.RectangleF rectangle)
		{
			Top = rectangle.Top;
			Left = rectangle.Left;
			Right = rectangle.Right;
			Bottom = rectangle.Bottom;
		}

		[DataMember(IsRequired = true)]
        public float Top;

		[DataMember(IsRequired = true)]
		public float Left;

		[DataMember(IsRequired = true)]
		public float Bottom;

		[DataMember(IsRequired = true)]
		public float Right;

		public override string ToString()
		{
			return String.Format("L,T={0},{1}, R,B={2},{3}", Left, Top, Right, Bottom);
		}

		public static implicit operator System.Drawing.RectangleF(RectangleF rectangle)
		{
			return System.Drawing.RectangleF.FromLTRB(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}

		public static implicit operator RectangleF(System.Drawing.RectangleF rectangle)
		{
			return new RectangleF(rectangle);
		}
	}	
}