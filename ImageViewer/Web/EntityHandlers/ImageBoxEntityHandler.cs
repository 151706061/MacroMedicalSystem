#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Common.Utilities;
using Macro.Web.Common;
using Macro.Web.Common.Messages;
using Macro.Web.Services;
using ImageBoxEntity = Macro.ImageViewer.Web.Common.Entities.ImageBox;
using TileEntity = Macro.ImageViewer.Web.Common.Entities.Tile;

namespace Macro.ImageViewer.Web.EntityHandlers
{
	internal class ImageBoxEntityHandler : EntityHandler<ImageBoxEntity>
	{
		private ImageBox _imageBox;
		private readonly List<TileEntityHandler> _tileHandlers = new List<TileEntityHandler>();

		public ImageBoxEntityHandler()
		{
		}

		private int ImageCount
		{
			get { return _imageBox.DisplaySet != null ? _imageBox.DisplaySet.PresentationImages.Count : 0; }	
		}

		private Common.Entities.Tile[] GetTileEntities()
		{
			return CollectionUtils.Map(_tileHandlers,
				(TileEntityHandler handler) => handler.GetEntity()).ToArray();
		}

		public override void SetModelObject(object modelObject)
		{
			_imageBox = (ImageBox)modelObject;
			_imageBox.Drawing += OnImageBoxDrawing;
			_imageBox.SelectionChanged += OnSelectionChanged;
            _imageBox.LayoutCompleted += OnLayoutCompleted;

			RefreshTileHandlers(false);
		}

		protected override void UpdateEntity(ImageBoxEntity entity)
		{
			entity.ImageCount = ImageCount;
			entity.NormalizedRectangle = _imageBox.NormalizedRectangle;
			entity.Selected = _imageBox.Selected;
			entity.TopLeftPresentationImageIndex = _imageBox.TopLeftPresentationImageIndex;
			entity.Tiles = GetTileEntities();
		}

		public void Draw()
		{
			foreach (TileEntityHandler handler in _tileHandlers)
				handler.Draw(false);

			NotifyEntityPropertyChanged("ImageCount", ImageCount);
			NotifyEntityPropertyChanged("TopLeftPresentationImageIndex", _imageBox.TopLeftPresentationImageIndex);
		}

		private void OnSelectionChanged(object sender, ItemEventArgs<IImageBox> e)
		{
			NotifyEntityPropertyChanged("Selected", e.Item.Selected);
		}

		private void OnImageBoxDrawing(object sender, System.EventArgs e)
		{
			Draw();
		}

		private void OnLayoutCompleted(object sender, System.EventArgs e)
		{
			RefreshTileHandlers(true);
		}

		private void RefreshTileHandlers(bool notify)
		{
			DisposeTileHandlers();

			foreach (ITile tile in _imageBox.Tiles)
			{
				TileEntityHandler newHandler = Create<TileEntityHandler>();
				((IEntityHandler)newHandler).SetModelObject(tile);
				_tileHandlers.Add(newHandler);
			}

			if (!notify)
				return;

			NotifyEntityPropertyChanged("Tiles", GetTileEntities());
			NotifyEntityPropertyChanged("ImageCount", ImageCount);
			NotifyEntityPropertyChanged("TopLeftPresentationImageIndex", _imageBox.TopLeftPresentationImageIndex);
		}

		public override void ProcessMessage(Message message)
		{
            if (message is UpdatePropertyMessage)
            {
                ProcessUpdatePropertyMessage(message as UpdatePropertyMessage);
            }
		}

	    private void ProcessUpdatePropertyMessage(UpdatePropertyMessage message)
	    {
            if (message.PropertyName == "TopLeftPresentationImageIndex")
	        {
                int newIndex = (int)message.Value;
                if (newIndex!=_imageBox.TopLeftPresentationImageIndex)
                {
                    _imageBox.TopLeftPresentationImageIndex = newIndex;

					// Assume the image index is only updated by the client
					// when the scroll bar is used. If this is the case, the
					// image box should be selected.
                    if (!_imageBox.Selected)
                        _imageBox.SelectDefaultTile();
                    
                    Draw();
                }
	        }
	    }

	    protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _imageBox != null)
			{
				_imageBox.SelectionChanged -= OnSelectionChanged;
				_imageBox.Drawing -= OnImageBoxDrawing;
				_imageBox.LayoutCompleted -= OnLayoutCompleted;
				DisposeTileHandlers();
				_imageBox = null;
			}
		}

		private void DisposeTileHandlers()
		{
			foreach (TileEntityHandler handler in _tileHandlers)
				handler.Dispose();

			_tileHandlers.Clear();
		}
	}
}
