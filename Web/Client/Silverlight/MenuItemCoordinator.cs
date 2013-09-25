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

namespace Macro.Web.Client.Silverlight
{
    public interface IMenuItemCoordinator
    {
        void OnMenuVisibilityChanged();
        void OnMenuItemHighlightingChanged(IMenuItem item);
        
        void OnMenuItemExpanding(IMenuItem item, out bool cancel);
        void OnMenuItemCollapsing(IMenuItem item);

        void OnMenuItemSelected(IMenuItem item);
    }

    internal class MenuItemCoordinator : IMenuItemCoordinator
    {
        private DelayedEventPublisher<EventArgs> _delayedEventPublisher;
        private IMenuItem _highlightedItem;

        internal MenuItemCoordinator(MenuBase menu)
        {
            Menu = menu;
            _delayedEventPublisher = new DelayedEventPublisher<EventArgs>(SetExpandedItem, TimeSpan.FromMilliseconds(350));
        }

        private MenuBase Menu { get; set; }

        private bool ValidateHighlightedItem()
        {
            if (_highlightedItem != null && (!_highlightedItem.IsHighlighted || !BelongsToRootMenu(_highlightedItem)))
                _highlightedItem = null;

            //IsHighlighted is guaranteed to be true if it's not null.
            return _highlightedItem != null;
        }

        #region IMenuItemCoordinator Members

        public void OnMenuVisibilityChanged()
        {
            CancelDelayedEvent();

            if (Menu.IsVisible || !ValidateHighlightedItem())
                return;

            //When the entire menu closes, it's possible for an item to be left highlighted.
            _highlightedItem.IsHighlighted = false;
            _highlightedItem = null;
        }

        public void OnMenuItemHighlightingChanged(IMenuItem item)
        {
            ValidateHighlightedItem();
            if (!item.IsHighlighted)
                return;

            //Most of the time this will not happen because the mouseenter/mouseleave on the item itself takes care of it, but it doesn't hurt.
            if (_highlightedItem != null && !ReferenceEquals(item, _highlightedItem))
                _highlightedItem.IsHighlighted = false;

            _highlightedItem = item;
            _delayedEventPublisher.Publish(this, EventArgs.Empty);
        }

        public void OnMenuItemExpanding(IMenuItem item, out bool cancel)
        {
            CancelDelayedEvent(); 
            
            if (item.ParentMenu != null)
            {
                foreach (var child in item.ParentMenu.Items.OfType<IMenuItem>())
                {
                    //Collapse all the other items at the same level as the one expanding.
                    if (!ReferenceEquals(child, item))
                        child.IsExpanded = false;
                }
            }

            cancel = false;

            if (item.IsHighlighted && item.IsExpanded)
            {
                //When an item is highlighted and *already* expanded, its children should be collapsed.  Standard menu behaviour.
                foreach(var child in item.Items.OfType<IMenuItem>())
                    child.IsExpanded = false;
            }
            else
            {
                var root = item.RootMenu;
                if (root == null || !root.IsVisible)
                {
                    //Can't expand an item that has no visible root menu.
                    cancel = true;
                }
                else
                {
                    //When expanded via code, we have to expand all the parents starting at the top.
                    //Simply expanding the immediate parent here will cause it to expand its parent and so on.
                    if (item.ParentMenuItem != null)
                        item.ParentMenuItem.IsExpanded = true;

                    if (item.IsTopLevel || item.ParentMenuItem.IsExpanded)
                        return;

                    //(above if statement) If the parent menu (provided there is one) is not expanded, this item should not expand - cancel.
                    cancel = true;
                }
            }
        }

        public void OnMenuItemCollapsing(IMenuItem item)
        {
            CancelDelayedEvent();

            //When an item collapses and the currently highlighted item is below it (belongs to it's tree), then unhighlight it.
            if (ValidateHighlightedItem() && IsHighlightedItemChildOf(item))
                _highlightedItem.IsHighlighted = false;
        }

        public void OnMenuItemSelected(IMenuItem item)
        {
			item.RootMenu.Close();
        }

        #endregion

        private void CancelDelayedEvent()
        {
            //Shouldn't even be trying to expand items when the menu has closed or an item expands.
            _delayedEventPublisher.Cancel();
        }

        private void SetExpandedItem(object sender, EventArgs e)
        {
            var highlightedItem = Menu.GetHighlightedItem();
            if (highlightedItem == null)
                return;

            highlightedItem.IsExpanded = true;
        }

        private bool IsHighlightedItemChildOf(IMenuItem item)
        {
            if (_highlightedItem == null)
                return false;

            var parent = _highlightedItem.ParentMenu;
            while (parent != null)
            {
                if (ReferenceEquals(parent, item))
                    return true;

                parent = parent.ParentMenu;
            }

            return false;
        }

        private bool BelongsToRootMenu(IMenuItem item)
        {
            return ReferenceEquals(item.RootMenu, Menu);
        }
    }
}
