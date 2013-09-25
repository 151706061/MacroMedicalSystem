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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections;

namespace Macro.Web.Client.Silverlight
{
    [TemplatePart(Name = IconTemplateName, Type = typeof(Image))]
    [TemplatePart(Name = ContentTemplateName, Type = typeof(ContentControl))]
    [TemplatePart(Name = PopupTemplateName)]
    [TemplatePart(Name = DependentContainerTemplateName)]

    [TemplateVisualState(GroupName = CheckStatesGroup, Name = UnCheckedState)]
    [TemplateVisualState(GroupName = CheckStatesGroup, Name = CheckedNoIconState)]
    [TemplateVisualState(GroupName = CheckStatesGroup, Name = CheckedWithIconState)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = NormalState)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = DisabledState)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = HighlightedState)]
    [TemplateVisualState(GroupName = CommonStatesGroup, Name = ExpandedState)]
    [TemplateVisualState(GroupName = HasItemsStatesGroup, Name = HasItemsState)]
    [TemplateVisualState(GroupName = HasItemsStatesGroup, Name = NoItemsState)]
    public class MenuItem : HeaderedItemsControl, IMenuItem
    {
        public const string IconTemplateName = "IconElement";
        public const string ContentTemplateName = "ContentElement";
        public const string PopupTemplateName = "PopupElement";
        public const string DependentContainerTemplateName = "DependentContainerElement";

        private const string CommonStatesGroup = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string HighlightedState = "Highlighted";
        private const string ExpandedState = "Expanded";

        private const string CheckStatesGroup = "CheckStates";
        private const string UnCheckedState = "UnChecked";
        private const string CheckedNoIconState = "CheckedNoIcon";
        private const string CheckedWithIconState = "CheckedWithIcon";

        private const string HasItemsStatesGroup = "HasItemsStates";
        private const string HasItemsState = "HasItems";
        private const string NoItemsState = "NoItems";

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked",
            typeof(bool), typeof(MenuItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
            typeof(ImageSource), typeof(MenuItem), new PropertyMetadata(null, new PropertyChangedCallback(OnIconPropertyChanged)));

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted",
            typeof(bool), typeof(MenuItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsHighlightedPropertyChanged)));

        public static readonly DependencyProperty MenuPositioningStrategyProperty = DependencyProperty.Register("MenuPositioningStrategy",
            typeof(IMenuPositioningStrategy), typeof(MenuItem), new PropertyMetadata(new DefaultMenuPositioningStrategy(), new PropertyChangedCallback(OnMenuPositioningStrategyChanged)));

        #region Template Members
        private Image _tIcon;
        private ContentControl _tContent;
        private IPopup _tPopup;
        #endregion
       
        private IMenu _parentMenu;
        private event EventHandler _click;

        public MenuItem()
        {
            this.DefaultStyleKey = typeof(MenuItem);
        }

        private IPopup Popup
        {
            get { return _tPopup; }
        }

        #region Public

        public IMenuPositioningStrategy MenuPositioningStrategy
        {
            get { return GetValue(MenuPositioningStrategyProperty) as IMenuPositioningStrategy; }
            set { SetValue(MenuPositioningStrategyProperty, value); }
        }

        #region IMenuItem Members

        public bool IsRoot { get { return false; } }

        public MenuBase RootMenu
        {
            get
            {
                if (_parentMenu is IMenuItem)
                    return ((IMenuItem)_parentMenu).RootMenu;

                return _parentMenu as MenuBase;
            }
        }

        public IMenu ParentMenu
        {
            get { return _parentMenu; }
            set
            {
                if (ReferenceEquals(value, _parentMenu))
                    return;

                IsHighlighted = false;
                Collapse();

                _parentMenu = value;
                foreach (var item in Items.OfType<IMenuItem>())
                    item.ParentMenu = value;

                OnParentMenuChanged();
            }
        }

        public IMenuItem ParentMenuItem
        {
            get { return ParentMenu as IMenuItem; }
        }

        public bool IsTopLevel
        {
            get { return _parentMenu == RootMenu && RootMenu != null; }
        }

		public bool IsChecked
		{ 
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

        public ImageSource Icon
        {
            get { return GetValue(IconProperty) as ImageSource; }
            set { SetValue(IconProperty, value); }
        }

        public bool IsHighlighted
		{
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public bool HasItems
        {
            get { return Items.Count > 0; }
        }

        IList IMenu.Items { get { return base.Items; }}

        public bool IsExpanded
	    {
            get { return _tPopup != null && _tPopup.IsOpen; }
            set
            {
                if (value)
                    Expand();
                else
                    Collapse();
            }
	    }

        protected IMenuItemCoordinator ItemCoordinator { get { return _parentMenu != null ? _parentMenu.ItemCoordinator : null; } }
        protected FrameworkElement ItemsContainer
        { 
            get { return (_tPopup != null) ? (FrameworkElement)_tPopup.Content : null; } 
        }

        IMenuItemCoordinator IMenu.ItemCoordinator {  get { return ItemCoordinator; } }

        FrameworkElement IMenu.ItemsContainer { get { return ItemsContainer; } }

        public event EventHandler Click
        {
            add { _click += value; }
            remove { _click -= value; }
        }

        #endregion 

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InitializePopup((FrameworkElement)GetTemplateChild(PopupTemplateName));
            _tContent = (ContentControl)GetTemplateChild(ContentTemplateName);
            _tIcon = (Image)GetTemplateChild(IconTemplateName);

            var dependentContainer = GetTemplateChild(DependentContainerTemplateName) as IMenuItemsContainer;
            if (dependentContainer != null)
                dependentContainer.Parent = this;

            if (_tContent != null && Header != null)
                _tContent.Content = Header;

            var icon = Icon;
            if (_tIcon != null && icon != null)
                _tIcon.Source = icon;

            if (!HasItems)
                Collapse();

            ChangeVisualState();
        }

        public override string ToString()
        {
            if (Header != null)
                return String.Format("{0} ({1} items)", Header, Items.Count);

            return String.Format("{0} items", Items.Count);
        }

        #endregion
        #endregion

        #region Protected Methods
        #region Overrides

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            IsHighlighted = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsHighlighted = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!HasItems && ItemCoordinator != null)
                this.ItemCoordinator.OnMenuItemSelected(this);

            if (_click != null)
                _click(this, EventArgs.Empty);
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);

            if (_tContent != null)
                _tContent.Content = newHeader;
        }

        protected override void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
            base.OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
            if (_tContent != null)
                _tContent.ContentTemplate = newHeaderTemplate;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<IMenuItem>())
                    item.ParentMenu = this;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.NewItems.OfType<IMenuItem>())
                    item.ParentMenu = null;
            }

            ChangeVisualState();
        }

        protected sealed override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            MenuItem menuItem = element as MenuItem;
            if (menuItem != null)
                menuItem.ParentMenu = this;

            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            MenuItem menuItem = element as MenuItem;
            if (menuItem == null)
                menuItem.ParentMenu = null;

            base.ClearContainerForItemOverride(element, item);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

        #endregion

        #region Virtual

        protected virtual void OnParentMenuChanged()
        {
        }

        protected virtual void OnIsHighlightedChanged(bool newValue)
        {
            ChangeVisualState();

            if (ItemCoordinator != null)
                ItemCoordinator.OnMenuItemHighlightingChanged(this);
        }

        protected virtual void OnIsCheckedPropertyChanged(bool newValue)
        {
            ChangeVisualState();
        }

        protected virtual void OnIconChanged(ImageSource newValue)
        {
            if (_tIcon != null)
                _tIcon.Source = newValue;

            ChangeVisualState();
        }

        protected virtual void OnIsExpandedChanged()
	    {
            ChangeVisualState();
        }

        #endregion
        #endregion

        #region Private/Internal Methods

        #region Event Handlers

        private void OnPopupOpened(object sender, EventArgs e)
        {
            OnIsExpandedChanged();
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            OnIsExpandedChanged();
        }

        #endregion

        private void InitializePopup(FrameworkElement newPopup)
        {
            if (ReferenceEquals(newPopup, _tPopup))
                return;

            if (_tPopup != null)
            {
                _tPopup.Opened -= OnPopupOpened;
                _tPopup.Closed -= OnPopupClosed;
            }

            _tPopup = newPopup.AsPopup();
            if (_tPopup != null)
            {
                _tPopup.Opened += OnPopupOpened;
                _tPopup.Closed += OnPopupClosed;
            }
        }

        private void Expand()
        {
            if (ItemCoordinator == null)
                return;

            bool cancel;
            ItemCoordinator.OnMenuItemExpanding(this, out cancel);

            if (cancel || !HasItems || IsExpanded)
                return;

            if (MenuPositioningStrategy != null)
                MenuPositioningStrategy.BeforeOpenPopup(this, Popup);

            Popup.IsOpen = true;
            UpdateLayout();

            if (MenuPositioningStrategy != null)
                MenuPositioningStrategy.AfterOpenPopup(this, Popup);
        }

        private void Collapse()
        {
            if (ItemCoordinator == null)
                return;

            ItemCoordinator.OnMenuItemCollapsing(this);

            foreach (var item in Items.OfType<IMenuItem>())
                item.IsExpanded = false;

            if (!IsExpanded)
                return;

            Popup.IsOpen = false;
        }

        internal virtual void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, HasItems ? HasItemsState : NoItemsState, true);

            if (!IsChecked)
                VisualStateManager.GoToState(this, UnCheckedState, true);
            else if (Icon != null)
                VisualStateManager.GoToState(this, CheckedWithIconState, true);
            else
                VisualStateManager.GoToState(this, CheckedNoIconState, true);
            
            if (IsHighlighted)
                VisualStateManager.GoToState(this, HighlightedState, true);
            else if (IsExpanded)
                VisualStateManager.GoToState(this, ExpandedState, true);
            else
                VisualStateManager.GoToState(this, NormalState, true);

            if (!IsEnabled)
            {
                // Note: this visual state must be appplied LAST
                VisualStateManager.GoToState(this, DisabledState, true);
            }
        }

        #endregion

        #region Dependency Property Methods

        private static void OnIsHighlightedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)obj).OnIsHighlightedChanged((bool)e.NewValue);
        }

        private static void OnIconPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)obj).OnIconChanged(e.NewValue as ImageSource);
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)obj).OnIsCheckedPropertyChanged((bool)e.NewValue);
        }

        private static void OnMenuPositioningStrategyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion
    }
}