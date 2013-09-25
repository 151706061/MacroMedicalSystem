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

namespace Macro.Common.Configuration
{
	public enum MigrationScope
	{
		User,
		Shared
	}
	
	public class UserSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SharedSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SettingsPropertyMigrationValues
    {
		public SettingsPropertyMigrationValues(string propertyName, MigrationScope migrationScope, object currentValue, object previousValue)
        {
			PropertyName = propertyName;
			MigrationScope = migrationScope;
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

		public MigrationScope MigrationScope { get; private set; }
		public string PropertyName { get; private set; }
        public object PreviousValue { get; private set; }
        public object CurrentValue { get; set; }
    }

	public interface IMigrateSettings
    {
        void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues);
    }
}