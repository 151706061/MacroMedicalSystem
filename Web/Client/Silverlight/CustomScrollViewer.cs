#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Macro.Web.Client.Silverlight
{
    [TemplatePart(Name=ScrollViewerTemplateName, Type=typeof(ScrollViewer))]
    public class CustomScrollViewer : ContentControl
    {
        public const string ScrollViewerTemplateName = "ScrollViewerElement";

        public static readonly DependencyProperty ScrollViewerStyleProperty = DependencyProperty.Register("ScrollViewerStyle",
            typeof(Style), typeof(CustomScrollViewer), new PropertyMetadata(null, new PropertyChangedCallback(OnScrollViewerStylePropertyChanged)));

        private ScrollViewer _scrollViewer;
        private List<ScrollBar> _scrollBars;
        private event EventHandler _scrolling;

        public CustomScrollViewer()
        {
            this.DefaultStyleKey = typeof(CustomScrollViewer);
        }

        public Style ScrollViewerStyle
        {
            get { return GetValue(ScrollViewerStyleProperty) as Style; }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }

        public event EventHandler Scrolling
        {
            add { _scrolling += value; }
            remove { _scrolling -= value; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_scrollBars != null)
            {
                foreach (var scrollBar in _scrollBars)
                    scrollBar.Scroll -= OnScrolling;
                
                _scrollBars = null;
            }

            _scrollViewer = (ScrollViewer)GetTemplateChild(ScrollViewerTemplateName);
            if (ScrollViewerStyle != null)
                _scrollViewer.Style = ScrollViewerStyle;

            _scrollViewer.LayoutUpdated += ScrollViewerLayoutUpdated;
        }

        protected virtual void OnScrolling() { }

        private void ScrollViewerLayoutUpdated(object sender, EventArgs e)
        {
            if (_scrollBars == null)
            {
                foreach (var scrollBar in _scrollViewer.FindChildren<ScrollBar>())
                {
                    if (_scrollBars == null)
                        _scrollBars = new List<ScrollBar>();

                    _scrollBars.Add(scrollBar);
                    scrollBar.Scroll += OnScrolling;
                }
            }

            if (_scrollBars != null)
                _scrollViewer.LayoutUpdated -= ScrollViewerLayoutUpdated;
        }

        private void OnScrolling(object sender, ScrollEventArgs e)
        {
            OnScrolling();

            if (_scrolling != null)
                _scrolling(this, EventArgs.Empty);
        }

        private void OnScrollViewerStylePropertyChanged(System.Windows.Style oldStyle, System.Windows.Style newStyle)
        {
            if (_scrollViewer != null)
                _scrollViewer.Style = newStyle;
        }

        private static void OnScrollViewerStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((CustomScrollViewer)obj).OnScrollViewerStylePropertyChanged(e.OldValue as Style, e.NewValue as Style);
        }
    }
}
