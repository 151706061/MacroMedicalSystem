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

using Macro.Common.Utilities;
using System.Collections.Generic;
using Macro.ImageViewer.Comparers;
using Macro.Common;

namespace Macro.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IPresentationImage"/> objects.
	/// </summary>
	public class PresentationImageCollection : ObservableList<IPresentationImage>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImageCollection"/>.
		/// </summary>
		internal PresentationImageCollection()
		{
		}

		internal static IComparer<IPresentationImage> GetDefaultComparer()
		{
			return new InstanceAndFrameNumberComparer();
		}

		/// <summary>
		/// The comparer that was last used to sort the collection, via <see cref="Sort"/>.
		/// </summary>
		/// <remarks>
		/// When an item is added to or replaced, this value is set to null.  When an item is
		/// simply removed, the sort order is maintained, so this value also will not change.
		/// </remarks>
		public IComparer<IPresentationImage> SortComparer { get; internal set; }

		/// <summary>
		/// Sorts the collection using <see cref="SortComparer"/>.
		/// </summary>
		/// <remarks>
		/// If <see cref="SortComparer"/> is null, it is first set to a default one.
		/// </remarks>
		public void Sort()
		{
			if (SortComparer == null)
				SortComparer = GetDefaultComparer();
			Sort(SortComparer);
		}

		/// <summary>
		/// Sorts the collection with the given comparer.
		/// </summary>
		public sealed override void Sort(IComparer<IPresentationImage> comparer)
		{
			Platform.CheckForNullReference(comparer, "comparer");
			SortComparer = comparer;
			base.Sort(SortComparer);
		}

		protected override void OnItemAdded(ListEventArgs<IPresentationImage> e)
		{
			SortComparer = null; //we don't know the sort order anymore.
			base.OnItemAdded(e);
		}

		protected override void  OnItemChanged(ListEventArgs<IPresentationImage> e)
		{
			SortComparer = null;//we don't know the sort order anymore.
 			 base.OnItemChanged(e);
		}
	}
}
