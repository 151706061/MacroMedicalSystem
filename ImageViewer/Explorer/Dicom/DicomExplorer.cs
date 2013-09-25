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

using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.Explorer;
using Macro.Common.Authorization;

namespace Macro.ImageViewer.Explorer.Dicom
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the DICOM explorer.")]
		public const string DicomExplorer = "Viewer/Explorer/DICOM";

		public static class Configuration
		{
			[AuthorityToken(Description = "Allow configuration of 'My Servers'.")]
			public const string MyServers = "Viewer/Configuration/My Servers";
		}
	}

	[ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
	public class DicomExplorer : IHealthcareArtifactExplorer
	{
		private DicomExplorerComponent _component;

		public DicomExplorer()
		{
		}

		#region IHealthcareArtifactExplorer Members

		public string Name
		{
			get { return SR.TitleDicomExplorer; }
		}


		public bool IsAvailable
		{
			get { return PermissionsHelper.IsInRole(AuthorityTokens.DicomExplorer); }
		}

		public IApplicationComponent Component
		{
			get
			{
				if (_component == null && IsAvailable)
					_component = DicomExplorerComponent.Create();

				return _component;
			}
		}

		#endregion
	}
}
