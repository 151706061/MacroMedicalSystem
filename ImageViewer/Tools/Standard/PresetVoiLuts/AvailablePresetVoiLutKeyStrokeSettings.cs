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

using System.Configuration;
using System.Collections.Generic;
using Macro.Common.Configuration;
using Macro.Desktop;
using System.ComponentModel;

namespace Macro.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[SettingsGroupDescription("Stores the available keystrokes for LUT presets.")]
	[SettingsProvider(typeof(Macro.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class AvailablePresetVoiLutKeyStrokeSettings
	{
		public AvailablePresetVoiLutKeyStrokeSettings()
		{
		}

		public IEnumerable<XKeys> GetAvailableKeyStrokes()
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof (XKeys));
			foreach (string keyStroke in AvailableKeyStrokes)
				yield return (XKeys)converter.ConvertFromString(keyStroke);
		}
	}
}
