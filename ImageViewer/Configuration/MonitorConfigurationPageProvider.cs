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

using System.Collections.Generic;
using Macro.Common;
using Macro.Desktop.Configuration;

namespace Macro.ImageViewer.Configuration
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class MonitorConfigurationPageProvider : IConfigurationPageProvider
	{
		public MonitorConfigurationPageProvider()
		{

		}

		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();
			if (PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
				listPages.Add(new ConfigurationPage<MonitorConfigurationApplicationComponent>("MonitorConfiguration"));
			return listPages.AsReadOnly();
		}

		#endregion
	}
}