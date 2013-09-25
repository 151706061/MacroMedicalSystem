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
using Macro.Desktop.View.WinForms;
using Macro.ImageViewer.Configuration.ServerTree;

namespace Macro.ImageViewer.Configuration.View.WinForms
{
	[ExtensionOf(typeof(ServerTreeConfigurationComponentViewExtensionPoint))]
	public class ServerTreeConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ServerTreeConfigurationComponent _component;
		private ServerTreeConfigurationComponentControl _control;

		#region IApplicationComponentView Members

		/// <summary>
		/// Called by the host to assign this view to a component.
		/// </summary>
		public void SetComponent(IApplicationComponent component)
		{
			_component = (ServerTreeConfigurationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ServerTreeConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}