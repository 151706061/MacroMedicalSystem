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
using System.Windows.Input;

namespace Macro.Web.Client.Silverlight
{
    [TemplatePart(Name = DependentContainerTemplateName)]
    public partial class ContextMenu : MenuBase, IPopup
    {
        public const string DependentContainerTemplateName = "DependentContainerElement";

        public static readonly DependencyProperty MenuPositioningStrategyProperty = DependencyProperty.Register("MenuPositioningStrategy",
            typeof(IMenuPositioningStrategy), typeof(ContextMenu), new PropertyMetadata(new DefaultMenuPositioningStrategy(), OnMenuPositioningStrategyChanged));
        
        private DependencyObject _owner;
        event EventHandler _opened;
        event EventHandler _closed;

        public ContextMenu()
        {
            DefaultStyleKey = typeof(ContextMenu);
            AutoClose = MenuManager.AutoCloseMenus;

            PopupManager.RegisterSingletonPopup(this);
        }

        private IPopup Popup { get; set; }

        internal DependencyObject Owner
        {
            get { return _owner; }
            set 
            {
                if (ReferenceEquals(_owner, value))
                    return;

                UIElement owner = _owner as UIElement;
                if (owner != null)
                {
                    owner.MouseRightButtonDown -= OnOwnerMouseRightButtonDown;
                    owner.MouseLeftButtonDown -= OnOwnerMouseLeftButtonDown;
                    owner.MouseRightButtonUp -= OnOwnerMouseRightButtonUp;
                }

                _owner = value;

                owner = _owner as UIElement;
                if (owner != null)
                {
                    owner.MouseRightButtonDown += OnOwnerMouseRightButtonDown;
                    owner.MouseLeftButtonDown += OnOwnerMouseLeftButtonDown;
                    owner.MouseRightButtonUp += OnOwnerMouseRightButtonUp;
                }
            }
        }

        public IMenuPositioningStrategy MenuPositioningStrategy
        {
            get { return GetValue(MenuPositioningStrategyProperty) as IMenuPositioningStrategy; }
            set { SetValue(MenuPositioningStrategyProperty, value); }
        }

        public bool AutoClose 
        {
            get { return Popup is BlackoutPopup; }
            set
            {
                var old = Popup;
                if (value)
                {
                    if (Popup is BlackoutPopup)
                        return;

                    var @new = new BlackoutPopup();
                    @new.ClickOutsideChild += (sender, e) => IsOpen = false;
                    Popup = @new;
                }
                else
                {
                    if (Popup is FrameworkPopupProxy)
                        return;

                    Popup = new FrameworkPopupProxy();
                }

                if (old != null)
                {
                    Popup.HorizontalOffset = old.HorizontalOffset;
                    Popup.VerticalOffset = old.VerticalOffset;
                }
            }
        }

        UIElement IPopup.Content
        {
            get { throw new NotSupportedException("ContextMenu does not have content that can be inspected or set."); }
            set { throw new NotSupportedException("ContextMenu does not have content that can be inspected or set."); }
        }

        public override bool IsVisible
        {
            get { return IsOpen; }
        }

        public bool IsOpen
        {
            get { return Popup.IsOpen; }
            set 
            {
                if (value)
                    Open();
                else
                    Close();
            }
        }

        public double HorizontalOffset
        {
            get { return Popup.HorizontalOffset; }
            set { Popup.HorizontalOffset = value; }
        }

        public double VerticalOffset
        {
            get { return Popup.VerticalOffset; }
            set { Popup.VerticalOffset = value; }
        }

        public event EventHandler Opened
        {
            add { _opened += value; }
            remove { _opened -= value; }
        }

        public event EventHandler Closed
        {
            add { _closed += value; }
            remove { _closed -= value; }
        }

        public void Open(Point point)
        {
            Popup.HorizontalOffset = point.X;
            Popup.VerticalOffset = point.Y;

            Open();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var dependentContainer = GetTemplateChild(DependentContainerTemplateName) as IMenuItemsContainer;
            if (dependentContainer != null)
                dependentContainer.Parent = this;
        }

        protected override void OnClose()
        {
            if (!IsOpen)
                return;

            Popup.IsOpen = false;
            ItemCoordinator.OnMenuVisibilityChanged();
            Popup.Content = null;

            if (_closed != null)
                _closed(this, EventArgs.Empty);
        }

        private void Open()
        {
            if (!HasItems || IsOpen)
                return;

            this.IsTabStop = true;    
            
            Popup.Content = this;
            UpdateLayout();

            var strategy = MenuPositioningStrategy;
            if (strategy != null)
                strategy.BeforeOpenPopup(this, Popup);

            Popup.IsOpen = true;
            this.Focus();

            ItemCoordinator.OnMenuVisibilityChanged();
            
            UpdateLayout();

            if (strategy != null)
                strategy.AfterOpenPopup(this, Popup);

            if (_opened != null)
                _opened(this, EventArgs.Empty);
        }

        private void OnOwnerMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Open(e.GetPosition(null));
        }

        private void OnOwnerMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup();
            e.Handled = true;
        }

        private void OnOwnerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupManager.CloseActivePopup(); //try to close the active one, but never handle the left click.
        }

        private static void OnMenuPositioningStrategyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
