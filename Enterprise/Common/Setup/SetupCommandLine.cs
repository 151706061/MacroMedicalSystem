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

namespace Macro.Enterprise.Common.Setup
{
	class SetupCommandLine : CommandLine
	{
		public SetupCommandLine()
		{
			SysAdminGroup = "Administrators";
			Password = "Macro";
			UserName = "sa";
			ImportSettingsGroups = true;
			ImportDefaultAuthorityGroups = true;
			ImportAuthorityTokens = true;
			MigrateSharedSettings = true;
		}

        /// <summary>
        /// Specifies whether to import authority tokens.
        /// </summary>
        [CommandLineParameter("tokens", "t", "Specifies whether to import authority tokens. This option is enabled by default.")]
		public bool ImportAuthorityTokens { get; set; }

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups. This option is enabled by default.")]
		public bool ImportDefaultAuthorityGroups { get; set; }

        /// <summary>
        /// Specifies whether to import settings groups.
        /// </summary>
        [CommandLineParameter("settings", "s", "Specifies whether to import settings groups. This option is enabled by default.")]
		public bool ImportSettingsGroups { get; set; }

        /// <summary>
		/// Specifies whether to import settings groups.
		/// </summary>
		[CommandLineParameter("migrate", "m", "Specifies whether to migrate shared settings from a previously installed version.")]
		public bool MigrateSharedSettings { get; set; }

		/// <summary>
		/// Specifies the filename of the previous local configuration file.
		/// </summary>
		[CommandLineParameter("previousConfig", "p", "Specifies the filename of the previous local configuration file.")]
		public string PreviousExeConfigFilename { get; set; }

		/// <summary>
		/// Specifies user name to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("suid", "Specifies user name to connect to enterprise server. Default is 'sa'.")]
		public string UserName { get; set; }

		/// <summary>
		/// Specifies password to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("spwd", "Specifies password to connect to enterprise server. Default is 'Macro'.")]
		public string Password { get; set; }

		/// <summary>
		/// Name of the sys-admin group. Imported tokens will be automatically added to this group.
		/// </summary>
		[CommandLineParameter("sgroup", "Specifies the name of the system admin authority group, so that imported tokens can be added to it.")]
		public string SysAdminGroup { get; set; }
		}
}
