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
using Macro.Desktop.View.WinForms;

namespace Macro.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ClipboardComponent"/>.
    /// </summary>
    public partial class ClipboardComponentControl : ApplicationComponentUserControl
    {
        private readonly ClipboardComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClipboardComponentControl(ClipboardComponent component)
            :base(component)
        {
			_component = component;

			InitializeComponent();

        	_component.DataSourceChanged += delegate { _galleryView.DataSource = _component.DataSource; };
        	_galleryView.DataSource = _component.DataSource;

        	_galleryView.ToolbarModel = _component.ToolbarModel;
        	_galleryView.ContextMenuModel = _component.ContextMenuModel;
        	_galleryView.SelectionChanged += OnSelectionChanged;
        	_galleryView.MultiSelect = true;
        	_galleryView.DragReorder = true;
			//_galleryView.DragOutside = true;
        }

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_galleryView.Selection);
		}
    }
}
