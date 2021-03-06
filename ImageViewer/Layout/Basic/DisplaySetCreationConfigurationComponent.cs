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
using System.ComponentModel;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Desktop;
using Macro.Desktop.Configuration;

namespace Macro.ImageViewer.Layout.Basic
{
	[ExtensionPoint]
	public sealed class DisplaySetCreationConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DisplaySetCreationConfigurationComponentViewExtensionPoint))]
	public class DisplaySetCreationConfigurationComponent : ConfigurationApplicationComponent
	{
		private BindingList<StoredDisplaySetCreationSetting> _settings;

		public DisplaySetCreationConfigurationComponent()
		{
		}

		public override void Start()
		{
			Initialize();
			base.Start();
		}

		public override void Save()
		{
			DisplaySetCreationSettings.DefaultInstance.Save(_settings);
		}

		private void Initialize()
		{
			List<StoredDisplaySetCreationSetting> sortedSettings = DisplaySetCreationSettings.DefaultInstance.GetStoredSettings();
			sortedSettings = CollectionUtils.Sort(sortedSettings,
			                                      (setting1, setting2) => setting1.Modality.CompareTo(setting2.Modality));

			_settings = new BindingList<StoredDisplaySetCreationSetting>(sortedSettings);

			foreach (StoredDisplaySetCreationSetting setting in _settings)
				setting.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.Modified = true;
		}

		public BindingList<StoredDisplaySetCreationSetting> Options
		{
			get { return _settings; }
		}
	}
}
