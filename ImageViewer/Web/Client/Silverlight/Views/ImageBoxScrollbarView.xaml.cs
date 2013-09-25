#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class ImageBoxScrollbarView : IDisposable
    {
      	public const string ImageCountPropertyName = "ImageCount";
        public const string TilesPropertyName = "Tiles";
        public const string TopLeftPresentationImageIndexPropertyName = "TopLeftPresentationImageIndex";

        private readonly ServerEventMediator _eventMediator;
        private ImageBox ServerEntity { get; set; }
        private DelayedEventPublisher<ScrollBarUpdateEventArgs> _scrollbarEventPublisher;

        public ImageBoxScrollbarView(ImageBox imageBox, ServerEventMediator eventMediator)
        {
            _eventMediator = eventMediator;
            IsTabStop = true; // allow focus
            ServerEntity = imageBox;

            DataContext = imageBox; 
            
            InitializeComponent();

            LayoutRoot.IsHitTestVisible = !imageBox.Tiles.Any(t => t.HasCapture);

            _eventMediator.TileHasCaptureChanged += EventBrokerTileHasCaptureChanged;

            ImageScrollBar.SetBinding(RangeBase.ValueProperty,
                    new Binding(TopLeftPresentationImageIndexPropertyName) { 
                        Mode = BindingMode.OneTime 
            });

            ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            ImageScrollBar.Visibility = ImageScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;

            ServerEntity.PropertyChanged += OnPropertyChanged;

            _scrollbarEventPublisher =
                new DelayedEventPublisher<ScrollBarUpdateEventArgs>(
                    (s, ev) => _eventMediator.DispatchMessage(new UpdatePropertyMessage
                                                                  {
                                                                      Identifier = Guid.NewGuid(),
                                                                      PropertyName =
                                                                          TopLeftPresentationImageIndexPropertyName,
                                                                      TargetId = ServerEntity.Identifier,
                                                                      Value = ev.ScrollbarPosition
                                                                  }), 100);
        }

        void EventBrokerTileHasCaptureChanged(object sender, EventArgs e)
        {
            LayoutRoot.IsHitTestVisible = (MouseHelper.ActiveElement == null || !MouseHelper.ActiveElement.HasCapture);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs ev)
        {
            if (ev.PropertyName == TopLeftPresentationImageIndexPropertyName)
            {
                ImageScrollBar.Value = ServerEntity.TopLeftPresentationImageIndex;
            }
            else if (ev.PropertyName == ImageCountPropertyName)
            {
                ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            }
            else if (ev.PropertyName == TilesPropertyName)
            {
                ImageScrollBar.Maximum = ServerEntity.ImageCount - ServerEntity.Tiles.Count;
            }

            ImageScrollBar.Visibility = ImageScrollBar.Maximum > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ImageScrollBarScroll(object sender, ScrollEventArgs e)
        {
            PopupManager.CloseActivePopup();
            Focus();
            switch (e.ScrollEventType)
            {
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.SmallDecrement:
                case ScrollEventType.SmallIncrement:
                case ScrollEventType.Last:
                case ScrollEventType.LargeIncrement:
                case ScrollEventType.LargeDecrement:
                case ScrollEventType.First:
                    _scrollbarEventPublisher.Publish(sender, new ScrollBarUpdateEventArgs { ScrollbarPosition = (int)e.NewValue });
                    
                    // don't move the thumb, keep in sync with the server side.
                    // Update will be done via PropertyChange event
                    ImageScrollBar.Value = ServerEntity.TopLeftPresentationImageIndex;
                    break;

                case ScrollEventType.EndScroll: 
                    // ignore it
                    break;
            }
        }

        public void Dispose()
        {
            if (_scrollbarEventPublisher != null)
            {
                _scrollbarEventPublisher.Dispose();
                _scrollbarEventPublisher = null;
            }

            _eventMediator.TileHasCaptureChanged -= EventBrokerTileHasCaptureChanged;
        }
    }

    class ScrollBarUpdateEventArgs : EventArgs
    {
        public int ScrollbarPosition { get; set; }
    }

}
