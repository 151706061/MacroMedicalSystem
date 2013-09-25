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

namespace Macro.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface for providers of <see cref="Sop"/> objects that are not necessarily <see cref="ImageSop"/>s.
	/// </summary>
	/// <remarks>
	/// See <see cref="ImageSop"/> for a more detailed explanation of 'providers'.
	/// </remarks>
	public interface ISopProvider
	{
		/// <summary>
		/// Gets a <see cref="Sop"/>.
		/// </summary>
		Sop Sop { get; }
	}
}
