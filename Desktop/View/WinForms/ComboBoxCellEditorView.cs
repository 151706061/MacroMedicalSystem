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
using Macro.Desktop.Tables;

namespace Macro.Desktop.View.WinForms
{
	[ExtensionOf(typeof(ComboBoxCellEditorViewExtensionPoint))]
	public class ComboBoxCellEditorView : WinFormsView, ITableCellEditorView
	{
		private readonly ComboBoxCellEditorControl _control;

		public ComboBoxCellEditorView()
		{
			_control = new ComboBoxCellEditorControl();
		}

		public void SetEditor(ITableCellEditor editor)
		{
			_control.SetEditor((ComboBoxCellEditor)editor);
		}

		public override object GuiElement
		{
			get { return _control; }
		}
	}}
