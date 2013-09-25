#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
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
using System.Text;
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public abstract class StockTool : Tool
    {
        //protected WorkstationView _mainView;
        protected DesktopView _mainView;

        public StockTool()
        {
			// check if the main is *the* view implemented by this plugin
			//IWorkstationView view = WorkstationModel.View;
			IDesktopView view = DesktopApplication.View;
			//if(view is WorkstationView)
			if(view is DesktopView)
			{
				//_mainView = (WorkstationView)view;
				_mainView = (DesktopView)view;
			}
			else
			{
				// this tool is not supported for other main views
				// TODO add a message
				throw new NotSupportedException();
			}
        }
   }
}
