#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Windows;
using System.Windows.Controls.Primitives;

namespace Macro.Web.Client.Silverlight
{
    public interface IMenuPositioningStrategy
    {
        void BeforeOpenPopup(IMenu popupOwner, IPopup popup);
        void AfterOpenPopup(IMenu popupOwner, IPopup popup);
    }

    public abstract class MenuPositioningStrategy : IMenuPositioningStrategy
    {
        protected IMenu Owner { get; set; }
        protected IPopup Popup { get; set; }

        public void BeforeOpenPopup(IMenu popupOwner, IPopup popup)
        {
            Owner = popupOwner;
            Popup = popup;
            BeforeOpenPopup();
        }

        public void AfterOpenPopup(IMenu popupOwner, IPopup popup)
        {
            Owner = popupOwner;
            Popup = popup;
            AfterOpenPopup();
        }

        protected abstract void BeforeOpenPopup();
        protected abstract void AfterOpenPopup();
    }

    public class DefaultMenuPositioningStrategy : MenuPositioningStrategy
    {
        public DefaultMenuPositioningStrategy()
        {
            ConstrainMenuHeight = true;
        }

        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }
        public bool ConstrainMenuHeight { get; set; }
        public double MenuHeightPadding { get; set; }

        private Size? RootVisualSize { get; set; }
        private Rect? MenuContentBounds { get; set; }
        private Rect? StartItemsContainerBounds { get; set; }
        private Rect? ParentItemsContainerBounds { get; set; }
        
        private double OffsetX { get; set; }
        private double OffsetY { get; set; }

        private Rect? ItemsContainerBounds
        { 
            get 
            {
                if (!StartItemsContainerBounds.HasValue)
                    return null;

                if (OffsetX == OffsetY && OffsetY == 0)
                    return StartItemsContainerBounds.Value;

                return new Rect(new Point(StartItemsContainerBounds.Value.Left + OffsetX, StartItemsContainerBounds.Value.Top + OffsetY),
                    new Size(StartItemsContainerBounds.Value.Width, StartItemsContainerBounds.Value.Height));
            }
        }

        protected override void BeforeOpenPopup()
        {
            SetMaxMenuHeight();
        }

        protected override void AfterOpenPopup()
        {
            Initialize();

            if (Owner.IsRoot)
                SetRootMenuPopupPosition();
            else
                SetSubMenuPopupPosition();

            Popup.HorizontalOffset += OffsetX;
            Popup.VerticalOffset += OffsetY;
        }

        private void Initialize()
        {
            OffsetX = OffsetY = 0;

            var menuElement = Owner as FrameworkElement;
            if (menuElement == null)
                return;

            var rootVisual = Extensions.RootVisual;
            if (rootVisual != null)
                RootVisualSize = new Size(rootVisual.ActualWidth, rootVisual.ActualHeight);

            MenuContentBounds = menuElement.GetBoundsRelativeTo(rootVisual);
            StartItemsContainerBounds = Owner.ItemsContainer.GetBoundsRelativeTo(rootVisual);
            if (Owner.ParentMenu != null)
                ParentItemsContainerBounds = Owner.ParentMenu.ItemsContainer.GetBoundsRelativeTo(rootVisual);
        }

        private void SetMaxMenuHeight()
        {
            if (!ConstrainMenuHeight)
                return;

            var itemsContainer = Owner.ItemsContainer;
            if (itemsContainer == null)
                return;

            var rootVisual = Extensions.RootVisual;
            if (rootVisual == null)
                return;

            itemsContainer.MaxHeight = rootVisual.ActualHeight - 2 * MenuHeightPadding;
        }

        private void SetRootMenuPopupPosition()
        {
            OffsetX += HorizontalOffset;
            OffsetY += VerticalOffset;
            KeepInsideRootVisual();
        }

        private void SetSubMenuPopupPosition()
        {
            OptimizeSubMenuAlignment();
            KeepInsideRootVisual();
        }

        private void OptimizeSubMenuAlignment()
        {
            AlignVertically();
            AlignRight();
            AlignLeftIfBeyondRightEdge();
        }

        private void AlignVertically()
        {
            if (ItemsContainerBounds.HasValue && MenuContentBounds.HasValue)
            {
                OffsetY += MenuContentBounds.Value.Top - ItemsContainerBounds.Value.Top;
                OffsetY += VerticalOffset;
                if (ConstrainMenuHeight)
                    OffsetY += MenuHeightPadding;
            }
        }

        private void AlignRight()
        {
            if (ItemsContainerBounds.HasValue && ParentItemsContainerBounds.HasValue)
            {
                OffsetX += ParentItemsContainerBounds.Value.Right - ItemsContainerBounds.Value.Left;
                OffsetX += HorizontalOffset;
            }
        }

        private void AlignLeftIfBeyondRightEdge()
        {
            if (ItemsContainerBounds.HasValue && ParentItemsContainerBounds.HasValue && ItemsContainerBounds.Value.Right > RootVisualSize.Value.Width)
            {
                OffsetX += ParentItemsContainerBounds.Value.Left - ItemsContainerBounds.Value.Right;
                OffsetX -= HorizontalOffset;
            }
        }

        private void KeepInsideRootVisual()
        {
            if (!ItemsContainerBounds.HasValue || !RootVisualSize.HasValue)
                return;

            if (ItemsContainerBounds.Value.Right > RootVisualSize.Value.Width)
                OffsetX += RootVisualSize.Value.Width - ItemsContainerBounds.Value.Right;

            if (ItemsContainerBounds.Value.Bottom > RootVisualSize.Value.Height)
            {
                OffsetY += RootVisualSize.Value.Height - ItemsContainerBounds.Value.Bottom;
                if (ConstrainMenuHeight)
                    OffsetY -= MenuHeightPadding;
            }

            if (ItemsContainerBounds.Value.Top < 0)
            {
                OffsetY -= ItemsContainerBounds.Value.Top;
                if (ConstrainMenuHeight)
                    OffsetY += MenuHeightPadding;
            }

            if (ItemsContainerBounds.Value.Left < 0)
                OffsetX -= ItemsContainerBounds.Value.Left;
        }
    }
}
