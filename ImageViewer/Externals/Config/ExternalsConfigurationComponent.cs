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
using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Configuration;

namespace Macro.ImageViewer.Externals.Config
{
	[ExtensionPoint]
	public sealed class ExternalsConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ExternalsConfigurationComponentViewExtensionPoint))]
	public class ExternalsConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string PATH = "ExternalApplications";

		private ExternalsConfigurationSettings _settings;
		private ExternalCollection _externals;

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
		}

		public ICollection<IExternal> Externals
		{
			get { return _externals; }
		}

		public void FlagModified()
		{
			this.Modified = true;
			this.NotifyPropertyChanged("Externals");
		}

		public override void Start()
		{
			base.Start();

			_settings = ExternalsConfigurationSettings.Default;

			try
			{
				_externals = _settings.Externals;
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to load external application settings. The XML may be corrupt.");
			}

			if (_externals == null)
				_externals = new ExternalCollection();
		}

		public override void Stop()
		{
			_externals = null;
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			try
			{
				_externals.Sort();
				_settings.Externals = _externals;
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to save external application settings.");
			}

			_settings.Save();

			ExternalCollection.ReloadSavedExternals();
		}
	}
}