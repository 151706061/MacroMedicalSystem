#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Helpers
{
    public static class MouseHelper
    {
        private static IMouseElement _activeElement;
        private static UIElement _backgroundElement;
        private static bool _rightMouseDown = false;
        private static bool _leftMouseDown = false;
        private static readonly object _mouseEventLock = new object();

        public static IMouseElement ActiveElement
        {
            get { return _activeElement; }
        }

        public static void SetActiveElement(IMouseElement element)
        {
            lock (_mouseEventLock)
            {
                if (_activeElement == element) return;

                if (_rightMouseDown)
                {
                    if (_activeElement != null) _activeElement.OnMouseRightButtonUp(_activeElement, null);
                }

                if (_leftMouseDown)
                {
                    if (_activeElement != null) _activeElement.OnMouseLeftButtonUp(_activeElement, null);
                }

                _activeElement = element;                
            }
        }

        public static void SetBackgroundElement(UIElement element)
        {
            lock (_mouseEventLock)
            {
                if (element == _backgroundElement) return;

                if (_backgroundElement != null)
                {
                    _backgroundElement.MouseRightButtonDown -= OnBackgroundMouseRightButtonDown;
                    _backgroundElement.MouseRightButtonUp -= OnBackgroundMouseRightButtonUp;
                    _backgroundElement.MouseLeftButtonDown -= OnBackgroundMouseLeftButtonDown;
                    _backgroundElement.MouseLeftButtonUp -= OnBackgroundMouseLeftButtonUp;
                    _backgroundElement.MouseMove -= OnMouseMoving;
                    _backgroundElement.MouseLeave -= OnMouseLeave;
                    _backgroundElement.MouseWheel -= OnMouseWheeling;
                }

                _backgroundElement = element;

                if (_backgroundElement != null)
                {
                    _backgroundElement.MouseRightButtonDown += OnBackgroundMouseRightButtonDown;
                    _backgroundElement.MouseRightButtonUp += OnBackgroundMouseRightButtonUp;
                    _backgroundElement.MouseLeftButtonDown += OnBackgroundMouseLeftButtonDown;
                    _backgroundElement.MouseLeftButtonUp += OnBackgroundMouseLeftButtonUp;
                    _backgroundElement.MouseMove += OnMouseMoving;
                    _backgroundElement.MouseLeave += OnMouseLeave;
                    _backgroundElement.MouseWheel += OnMouseWheeling;
                }
            }
        }
        
        /// <summary>
        /// Called by other controls to let MouseHelpers handle the mouse event instead.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void PreprocessRightMouseButtonDown(IMouseElement element, object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup();

            lock (_mouseEventLock)
            {
                _rightMouseDown = true;
                if (_activeElement == null || !_activeElement.HasCapture)
                {
                    element.OnMouseRightButtonDown(sender,e);
                    return;
                }

                    
                _activeElement.OnMouseRightButtonDown(sender,e);
            }
        }

        /// <summary>
        /// Called by other controls to let MouseHelpers handle the mouse event instead.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void PreprocessLeftMouseButtonDown(IMouseElement element, object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup();

            lock (_mouseEventLock)
            {
                _leftMouseDown = true;

                if (_activeElement==null || !_activeElement.HasCapture)
                {
                    element.OnMouseLeftButtonDown(sender,e);
                    return;
                }

                _activeElement.OnMouseLeftButtonDown(sender,e);
            }
        }

        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            //lock (_mouseEventLock)
            //{
            //    if (_rightMouseDown)
            //    {
            //        if (_activeElement != null)
            //        {
            //            _activeElement.OnBackgroundMouseRightButtonUp(_activeElement, null);
            //        }
            //        _rightMouseDown = false;
            //    }

            //    if (_leftMouseDown)
            //    {
            //        if (_activeElement != null)
            //        {
            //            _activeElement.OnBackgroundMouseLeftButtonUp(sender, null);
            //            _leftMouseDown = false;
            //        }
            //    }
            //}
        }

		//TODO (CR May 2010): should have the tiles route their mouse moves to the helper, too.  That way, non-active tiles can highlight their ROIs even though a different one is selected.
        private static void OnMouseMoving(object sender, MouseEventArgs e)
        {
            lock (_mouseEventLock)
            {
                if (_activeElement != null)
                {
                    _activeElement.OnMouseMoving(sender, e);
                }
            }
        }

        private static void OnMouseWheeling(object sender, MouseWheelEventArgs e)
        {
            lock (_mouseEventLock)
            {
                if (_activeElement != null)
                    _activeElement.OnMouseWheeling(sender, e);
            }                  
        }

        private static void OnBackgroundMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup();
            //NOTE: we must set handled to true for double-click to work.
            e.Handled = true;

            lock (_mouseEventLock)
            {
                UIElement ui = _activeElement as UIElement;
                if (ui != null)
                {

                    if (_activeElement != null && _activeElement.HasCapture)
                    {
                        _leftMouseDown = true;
                        _activeElement.OnMouseLeftButtonDown(sender, e);
                    }
                }
            }
        }

        private static void OnBackgroundMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            lock (_mouseEventLock)
            {
                if (_activeElement != null && _leftMouseDown)
                    _activeElement.OnMouseLeftButtonUp(sender, e);

                _leftMouseDown = false; 
            }
        }

        private static void OnBackgroundMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup();
            //NOTE: we must set handled to true for double-click to work.
            e.Handled = true;

            lock (_mouseEventLock)
            {
                UIElement ui = _activeElement as UIElement;
                if (ui != null)
                {
                    if (_activeElement != null && _activeElement.HasCapture)
                    {
                        _rightMouseDown = true;
                        _activeElement.OnMouseRightButtonDown(sender, e);
                    }
                }
            }
        }

        private static void OnBackgroundMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            lock (_mouseEventLock)
            {
                if (_activeElement != null && _rightMouseDown)
                    _activeElement.OnMouseRightButtonUp(sender, e);
                _rightMouseDown = false;
            }
        }
    }
}
