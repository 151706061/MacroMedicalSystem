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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Macro.Desktop.View.WinForms;

namespace Macro.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CodeEditorComponent"/>
    /// </summary>
    public partial class DefaultCodeEditorComponentControl : ApplicationComponentUserControl
    {
        private DefaultCodeEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultCodeEditorComponentControl(DefaultCodeEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component.InsertTextRequested += new EventHandler<DefaultCodeEditorComponent.InsertTextEventArgs>(_component_InsertTextRequested);
            _editor.DataBindings.Add("Text", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _component_InsertTextRequested(object sender, DefaultCodeEditorComponent.InsertTextEventArgs e)
        {
            _editor.SelectedText = e.Text;
        }
    }
}
