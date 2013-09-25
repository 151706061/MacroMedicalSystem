#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Macro.ImageViewer.Web.Common
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public struct Position
    {
		public Position(Point point)
		{
			X = point.X;
			Y = point.Y;
		}

        [DataMember(IsRequired = true)]
        public int X;

        [DataMember(IsRequired = true)]
        public int Y;

		public override string ToString()
		{
			return String.Format("X={0}, Y={1}", X, Y);
		}

		public static implicit operator Point(Position position)
		{
			return new Point(position.X, position.Y);
		}

		public static implicit operator Position(Point point)
		{
			return new Position(point);
		}
	}
}