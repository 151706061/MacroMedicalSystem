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
using Macro.Common;
using Macro.Desktop;
using Macro.Dicom.ServiceModel.Query;

namespace Macro.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class SearchPanelComponentExtensionPoint : ExtensionPoint<ISearchPanelComponent>
	{
	}

	public class SearchRequestedEventArgs : EventArgs
	{
		public SearchRequestedEventArgs(StudyRootStudyIdentifier queryCriteria)
		{
		    this.QueryCriteria = queryCriteria;
		}

		public StudyRootStudyIdentifier QueryCriteria { get; private set; }
	}

	public interface ISearchPanelComponent : IApplicationComponent
	{
		void Search();
		void Clear();
		bool SearchInProgress { get; set; }

		event EventHandler<SearchRequestedEventArgs> SearchRequested;
		event EventHandler SearchCancelled;
	}
}
