#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Macro.Web.Client.Silverlight
{
    public interface IBlackoutPopup : IPopup
    {
        event EventHandler ClickOutsideChild;
    }

    public class BlackoutPopup : FrameworkPopupProxy, IBlackoutPopup
    {
        private Popup _backgroundPopup;
        private Canvas _backgroundCanvas;

        private event EventHandler ClickOutsideContent;

        public BlackoutPopup()
        {
            _backgroundCanvas = new Canvas
                                    {
                                        Background = new SolidColorBrush(Colors.Transparent)
                                    };
            _backgroundCanvas.MouseLeftButtonDown += OnCanvasButtonDown;
            _backgroundCanvas.MouseRightButtonDown += OnCanvasButtonDown;
            _backgroundCanvas.MouseLeftButtonUp += OnCanvasButtonUp;
            _backgroundCanvas.MouseRightButtonUp += OnCanvasButtonUp;

            _backgroundPopup = new Popup();
            _backgroundPopup.Opened += OnBackgroundPopupOpened;
            _backgroundPopup.Closed += OnBackgroundPopupClosed;
            _backgroundPopup.LayoutUpdated += OnBackgroundPopupLayoutUpdated;
            _backgroundPopup.Child = _backgroundCanvas;
        }

        private void OnBackgroundPopupLayoutUpdated(object sender, EventArgs e)
        {
            ResizeBackgroundCanvas();
        }

        public Brush Background
        {
            get { return _backgroundCanvas.Background; }
            set { _backgroundCanvas.Background = value; }
        }

        public override bool IsOpen
        {
            get { return base.IsOpen; }
            set
            {
                if (_backgroundPopup != null)
                    _backgroundPopup.IsOpen = value;
                else
                    return;

                base.IsOpen = value;
            }
        }

        public event EventHandler ClickOutsideChild
        {
            add { ClickOutsideContent += value; }
            remove { ClickOutsideContent -= value; }
        }

        private void ResizeBackgroundCanvas()
        {
            _backgroundCanvas.Width = Application.Current.Host.Content.ActualWidth;
            _backgroundCanvas.Height = Application.Current.Host.Content.ActualHeight;
        }

        private void OnRootVisualSizeChanged(object sender, EventArgs e)
        {
            ResizeBackgroundCanvas();
        }

        private void OnBackgroundPopupOpened(object sender, EventArgs e)
        {
            Application.Current.Host.Content.Resized += OnRootVisualSizeChanged;
        }

        private void OnBackgroundPopupClosed(object sender, EventArgs e)
        {
            Application.Current.Host.Content.Resized -= OnRootVisualSizeChanged;
        }

        private void OnCanvasButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnCanvasButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ClickOutsideContent != null)
                ClickOutsideContent(sender, EventArgs.Empty);
        }
    }
}
