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

using System.Windows.Forms;
using Macro.Desktop.View.WinForms;
using System;

namespace Macro.ImageViewer.Tools.Measurement.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CalibrationComponent"/>.
    /// </summary>
    public partial class CalibrationComponentControl : ApplicationComponentUserControl
    {
        private readonly CalibrationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CalibrationComponentControl(CalibrationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	this.AcceptButton = _ok;
        	this.CancelButton = _cancel;

        	_length.DecimalPlaces = _component.DecimalPlaces;
        	_length.Increment = (decimal) _component.Increment;
        	_length.Minimum = (decimal) _component.Minimum;

            BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

        	_length.DataBindings.Add("Value", bindingSource, "LengthInCm", true, DataSourceUpdateMode.OnPropertyChanged);
        	_ok.Click += delegate
        	             	{
        	             		_component.Accept();
        	             	};
        	_cancel.Click += delegate
        	                 	{
        	                 		_component.Cancel();
        	                 	};
        }
    }
}
