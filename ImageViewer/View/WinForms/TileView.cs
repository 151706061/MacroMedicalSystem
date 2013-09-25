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

using System.Drawing;
using Macro.Common;
using Macro.Desktop;
using Macro.Desktop.View.WinForms;

namespace Macro.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TileComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TileViewExtensionPoint))]
    public class TileView : WinFormsView, IView
    {
        private Tile _tile;
        private TileControl _tileControl;
		private Rectangle _parentRectangle;
		private int _parentImageBoxInsetWidth;

		public Tile Tile
		{
			get { return _tile; }
			set { _tile = value; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set { _parentRectangle = value; }
		}

		public int ParentImageBoxInsetWidth
		{
			get { return _parentImageBoxInsetWidth; }
			set { _parentImageBoxInsetWidth = value; }
		}

		public override object GuiElement
        {
            get
            {
                if (_tileControl == null)
                {
                    _tileControl = new TileControl(this.Tile, this.ParentRectangle, this.ParentImageBoxInsetWidth);
                }
                return _tileControl;
            }
        }
    }
}
