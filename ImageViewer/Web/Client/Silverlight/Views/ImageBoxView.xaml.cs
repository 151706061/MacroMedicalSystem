#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class ImageBoxView
    {
        private List<TileView> _tileViews;
		private System.Windows.Size _parentSize;
        private ImageBoxScrollbarView _scrollbarView;
        private ServerEventMediator _eventMediator;

        public ImageBoxView(ImageBox serverEntity, ServerEventMediator eventMediator)
        {
			ServerEntity = serverEntity;
            _eventMediator = eventMediator;
			_eventMediator.RegisterEventHandler(serverEntity.Identifier, OnImageBoxEvent);
            InitializeComponent();
			UpdateTileViews();
        }
		
        public ImageBox ServerEntity { get; private set; }

		private void UpdateTileViews()
        {
			DestroyTileViews();

			if (ServerEntity.Tiles == null || ServerEntity.Tiles.Count == 0)
				return;

			_tileViews = new List<TileView>();
			
			// No Need for dispatcher here, this is called from a routine that is already in a dispatcher
            foreach (Tile tile in ServerEntity.Tiles)
            {
				var tileView = new TileView(tile, _eventMediator);
				TileContainer.Children.Add(tileView);
                _tileViews.Add(tileView);
            }

			UpdateBorder();
			CreateScrollbar();
        }

		private void UpdateSize()
		{
			// Must round-up the values to prevent SL from rendering fuzzy images  (sub-pixel rendering)
            Width = Math.Ceiling(_parentSize.Width * (ServerEntity.NormalizedRectangle.Right - ServerEntity.NormalizedRectangle.Left));
			Height = Math.Ceiling(_parentSize.Height * (ServerEntity.NormalizedRectangle.Bottom - ServerEntity.NormalizedRectangle.Top));
			SetValue(Canvas.LeftProperty, Math.Ceiling(_parentSize.Width * ServerEntity.NormalizedRectangle.Left));
            SetValue(Canvas.TopProperty, Math.Ceiling(_parentSize.Height * ServerEntity.NormalizedRectangle.Top));

			if (ServerEntity.Tiles == null || ServerEntity.Tiles.Count == 0)
				return;

            // Note: the scrollbar visibility changes based on the ImageCount update event.
            // When UpdateLayout() is called, the size of the TileContainer will reflect that change.
            UpdateLayout();

            if (_tileViews != null)
            {
                foreach (TileView tileView in _tileViews)
                {
                    tileView.SetParentSize(new System.Windows.Size(TileContainer.ActualWidth, TileContainer.ActualHeight));
                }
            }			
		}
		
		private void DestroyTileViews()
		{
			TileContainer.Children.Clear();

			if (_tileViews != null)
			{
				foreach (TileView tileView in _tileViews)
					tileView.Dispose();

				_tileViews = null;
			}

            if (_scrollbarView != null)
            {
                ImageBoxRightColumn.Children.Remove(_scrollbarView); 
                _scrollbarView.Dispose();
                _scrollbarView = null;
            }
		}

		public void SetParentSize(System.Windows.Size size)
		{
			_parentSize = size;
			UpdateSize();
		}

        private void CreateScrollbar()
        {
            _scrollbarView = new ImageBoxScrollbarView(ServerEntity,_eventMediator);
            ImageBoxRightColumn.Children.Add(_scrollbarView);
        }

        private void UpdateBorder()
		{
			ImageBoxBorder.BorderBrush = ServerEntity.Selected ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Gray);
		}

		private void OnImageBoxEvent(object sender, ServerEventArgs e)
		{
			if (!(e.ServerEvent is PropertyChangedEvent))
				return;

			OnPropertyChanged(e.ServerEvent as PropertyChangedEvent);
		}

		private void OnPropertyChanged(PropertyChangedEvent @event)
		{
            //NOTE: the scrollbar will update automatically whenever ServerEntity is updated.

			if (@event.PropertyName == "Tiles")
			{
                ServerEntity.Tiles = (Collection<Tile>)@event.Value;
                UpdateTileViews();
                UpdateSize();
				return;
			}

			if (@event.PropertyName == "Selected")
			{
				ServerEntity.Selected = (bool)@event.Value;
				UpdateBorder();
			}
            if (@event.PropertyName == "TopLeftPresentationImageIndex")
            {
                ServerEntity.TopLeftPresentationImageIndex = (int)@event.Value;     
            }

            if (@event.PropertyName == "ImageCount")
            {
                var count = (int)@event.Value;
                if (ServerEntity.ImageCount!=count)
                {
                    ServerEntity.ImageCount = count;
                    // Note: Because the scrollbar observes PropertyChanged event on the ServerEntity, its visibility will be updated automatically in ImageBoxScrollbarView
                    // When UpdateLayout() is called in UpdateSize(), the size of the TileContainer will be updated accordingly.
                    UpdateSize();
                }
            }
		}

		public void Destroy()
        {
            if (_eventMediator != null)
            {
                _eventMediator.UnregisterEventHandler(ServerEntity.Identifier);
                _eventMediator = null;
            }

		    DestroyTileViews();

		    ServerEntity = null;
        }
	}
}