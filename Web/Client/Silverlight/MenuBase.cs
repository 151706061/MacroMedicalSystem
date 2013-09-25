#region License (non-CC)

// The SL3Menu project (http://sl3menu.codeplex.com/) and the Silverlight toolkit (http://silverlight.codeplex.com/)
// were used as a reference when writing this code.  As such this is considered a derived work and licensed under Ms-PL.
//
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the software, you accept this license. If you 
// do not accept the license, do not use the software.
//
// 1. Definitions
//
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under
// U.S. copyright law.
//
// A "contribution" is the original software, or any additions or changes to the software.
//
// A "contributor" is any person that distributes its contribution under this license.
//
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
//
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in 
//     section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to
//     reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or 
//     any derivative works that you create.
//
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in 
//     section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed 
//     patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution 
//     in the software or derivative works of the contribution in the software.
//
// 3. Conditions and Limitations
//
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or
//     trademarks.
//
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the 
//     software, your patent license from such contributor to the software ends automatically.
//
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and
//     attribution notices that are present in the software.
//
// (D) If you distribute any portion of the software in source code form, you may do so only under this license 
//     by including a complete copy of this license with your distribution. If you distribute any portion of the 
//     software in compiled or object code form, you may only do so under a license that complies with this license.
//
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties,
//     guarantees or conditions. You may have additional consumer rights under your local laws which this license
//     cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties
//     of merchantability, fitness for a particular purpose and non-infringement.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace Macro.Web.Client.Silverlight
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MenuItem))]
    public abstract partial class MenuBase : ItemsControl, IMenu
    {
        public static DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle",
                typeof(Style), typeof(MenuBase), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStylePropertyChanged)));

        private IMenuItemCoordinator _itemCoordinator;

        protected MenuBase()
        {
            _itemCoordinator = new MenuItemCoordinator(this);
        }

        public override string ToString()
        {
            return String.Format("Menu ({0} items)", Items.Count);
        }

        #region Protected

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<IMenuItem>())
                {
                    item.ParentMenu = this;
                    SetItemContainerStyle(item as MenuItem);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<IMenuItem>())
                    item.ParentMenu = null;
            }
        }

        protected sealed override DependencyObject GetContainerForItemOverride()
        {
            var menuItem = new MenuItem();
            SetItemContainerStyle(menuItem);
            return menuItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            MenuItem menuItem = (MenuItem)element;
            menuItem.ParentMenu = this;
            SetItemContainerStyle(menuItem);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            MenuItem menuItem = (MenuItem)element;
            menuItem.ParentMenu = null;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

        protected virtual void OnItemContainerStylePropertyChanged(System.Windows.Style style)
        {
            ItemContainerStyle = style;
            SetItemContainerStyle();
        }

        protected virtual void OnClose() { }

        #endregion

        public abstract bool IsVisible { get; }

        public Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        protected IMenuItemCoordinator ItemCoordinator { get { return _itemCoordinator; } }

        protected FrameworkElement ItemsContainer { get { return this; } }
        
        #region IMenu Members

        bool IMenu.IsRoot { get { return true; } }

        MenuBase IMenu.RootMenu { get { return this; } }

        IMenu IMenu.ParentMenu
        { 
            get { return null; }
            set { throw new InvalidOperationException("MenuBase class cannot have a parent."); }
        }

        public bool HasItems { get { return Items.Count > 0; } }

        IList IMenu.Items { get { return base.Items; }}

        IMenuItemCoordinator IMenu.ItemCoordinator { get { return ItemCoordinator; } }

        FrameworkElement IMenu.ItemsContainer { get { return ItemsContainer; } }

        #endregion

        public void Close()
        {
            foreach (var item in Items.OfType<IMenuItem>())
                item.IsExpanded = false;

            OnClose();
        }

        #region Private

        #region Dependency Property Methods

        private static void OnItemContainerStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((MenuBase)obj).OnItemContainerStylePropertyChanged(e.NewValue as Style);
        }

        #endregion

        private void SetItemContainerStyle()
        {
            SetItemContainerStyle(Items.OfType<MenuItem>());
        }

        private void SetItemContainerStyle(IEnumerable<MenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
                SetItemContainerStyle(menuItem);
        }

        private void SetItemContainerStyle(MenuItem menuItem)
        {
            if (menuItem == null)
                return;

            menuItem.Style = ItemContainerStyle;
            SetItemContainerStyle(menuItem.Items.OfType<MenuItem>());
        }

        #endregion
    }
}
