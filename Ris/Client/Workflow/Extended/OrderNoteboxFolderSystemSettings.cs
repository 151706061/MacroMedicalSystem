#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
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
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{

	[SettingsGroupDescription("Configures behaviour of the Order Notes folder system.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class OrderNoteboxFolderSystemSettings
	{
		[DataContract]
		internal class GroupFoldersData : DataContractBase
		{
			/// <summary>
			/// Deserialization constructor.
			/// </summary>
			public GroupFoldersData()
			{
				StaffGroupNames = new List<string>();
			}

			public GroupFoldersData(List<string> staffGroupNames)
			{
				StaffGroupNames = staffGroupNames;
			}

			/// <summary>
			/// List of staff groups for which folders are visible.
			/// </summary>
			[DataMember]
			public List<string> StaffGroupNames;
		}


		private OrderNoteboxFolderSystemSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public GroupFoldersData GroupFolders
		{
			get
			{
				return string.IsNullOrEmpty(this.GroupFoldersXml)
						? new GroupFoldersData()
						: JsmlSerializer.Deserialize<GroupFoldersData>(this.GroupFoldersXml);
			}
			set
			{
				this.GroupFoldersXml = JsmlSerializer.Serialize(value, "GroupFoldersData");
			}
		}
	}
}
