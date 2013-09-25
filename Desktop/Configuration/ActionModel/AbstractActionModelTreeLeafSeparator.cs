#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using Macro.Common.Utilities;
using Macro.Desktop.Trees;

namespace Macro.Desktop.Configuration.ActionModel
{
	public sealed class AbstractActionModelTreeLeafSeparator : AbstractActionModelTreeLeaf
	{
		public AbstractActionModelTreeLeafSeparator() : base(new PathSegment("Separator", SR.LabelSeparator)) 
		{
			base.IconSet = new IconSet("Icons.ActionModelSeparatorSmall.png", "Icons.ActionModelSeparatorMedium.png", "Icons.ActionModelSeparatorLarge.png");
			base.ResourceResolver = new ApplicationThemeResourceResolver(this.GetType().Assembly);
			base.CheckState = CheckState.Checked;
		}

		internal Path GetSeparatorPath()
		{
			Stack<PathSegment> stack = new Stack<PathSegment>();
			stack.Push(new PathSegment(Guid.NewGuid().ToString()));

			AbstractActionModelTreeNode current = this.Parent;
			while (current != null)
			{
				stack.Push(current.PathSegment);
				current = current.Parent;
			}

			Path path = new Path(stack.Pop());
			while (stack.Count > 0)
			{
				path = path.Append(stack.Pop());
			}
			return path;
		}
	}
}