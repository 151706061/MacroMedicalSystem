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
using System.Windows.Controls.Primitives;
using FrameworkPopup = System.Windows.Controls.Primitives.Popup;

namespace Macro.Web.Client.Silverlight
{
    public interface IPopup
    {
        UIElement Content { get; set; }

        double HorizontalOffset { get; set; }
        double VerticalOffset { get; set; }

        void Open(Point point);
        bool IsOpen { get; set; }

        event EventHandler Opened;
        event EventHandler Closed;
    }

    public class PopupProxy : IPopup
    {
        public PopupProxy(IPopup popup)
        {
            RealPopup = popup;
        }

        protected IPopup RealPopup { get; private set; }

        public virtual UIElement Content
        {
            get { return RealPopup.Content; }
            set { RealPopup.Content = value; }
        }

        public double HorizontalOffset
        {
            get { return RealPopup.HorizontalOffset; }
            set { RealPopup.HorizontalOffset = value; }
        }

        public double VerticalOffset
        {
            get { return RealPopup.VerticalOffset; }
            set { RealPopup.VerticalOffset = value; }
        }

        public virtual bool IsOpen
        {
            get { return RealPopup.IsOpen; }
            set { RealPopup.IsOpen = value; }
        }

        public event EventHandler Opened
        {
            add { RealPopup.Opened += value; }
            remove { RealPopup.Opened -= value; }
        }

        public event EventHandler Closed
        {
            add { RealPopup.Closed += value; }
            remove { RealPopup.Closed -= value; }
        }

        public virtual void Open(Point point)
        {
            RealPopup.Open(point);
        }
    }

    public class FrameworkPopupProxy : IPopup
    {
        public FrameworkPopupProxy()
            : this(new FrameworkPopup())
        {
        }

        public FrameworkPopupProxy(Popup popup)
        {
            RealPopup = popup;
        }

        protected FrameworkPopup RealPopup { get; private set; }

        public virtual UIElement Content
        {
            get { return RealPopup.Child; }
            set { RealPopup.Child = value; }
        }

        public double HorizontalOffset
        {
            get { return RealPopup.HorizontalOffset; }
            set { RealPopup.HorizontalOffset = value; }
        }

        public double VerticalOffset
        {
            get { return RealPopup.VerticalOffset; }
            set { RealPopup.VerticalOffset = value; }
        }

        public virtual bool IsOpen
        {
            get { return RealPopup.IsOpen; }
            set { RealPopup.IsOpen = value; }
        }

        public event EventHandler Opened
        {
            add { RealPopup.Opened += value; }
            remove { RealPopup.Opened -= value; }
        }

        public event EventHandler Closed
        {
            add { RealPopup.Closed += value; }
            remove { RealPopup.Closed -= value; }
        }

        public virtual void Open(Point point)
        {
            HorizontalOffset = point.X;
            VerticalOffset = point.Y;
            IsOpen = true;
        }
    }
}
