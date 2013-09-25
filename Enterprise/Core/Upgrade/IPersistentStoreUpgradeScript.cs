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

namespace Macro.Enterprise.Core.Upgrade
{
	/// <summary>
	/// Interface representing an upgrade script for a PersistentStore.
	/// </summary>
	public interface IPersistentStoreUpgradeScript
	{
		/// <summary>
		/// The PersistentStore version for which the script upgrades from.
		/// </summary>
		Version SourceVersion { get; }
		/// <summary>
		/// The resultant PersistentStore version after the script has been run.
		/// </summary>
		Version DestinationVersion { get; }
		/// <summary>
		/// Execute the upgrade script.
		/// </summary>
		void Execute();
	}
}
