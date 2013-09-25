#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
    public partial class DropDownButton : UserControl, IActionUpdate, IToolstripDropdownButton, IDisposable
	{
        private MouseEvent _mouseEnterEvent;
        private MouseEvent _mouseLeaveEvent;
        private readonly WebDropDownButtonAction _actionItem;
        private IPopup _dropMenu;
        private ActionDispatcher _actionDispatcher;
        private WebIconSize _iconSize;
        private bool _disposed = false;

        private WebIconSize IconSize {
            get { return _iconSize; }
            set
            {
                if (_iconSize != value)
                {
                    _iconSize = value;
                    SetIcon();
                }
            }
        }

		public DropDownButton(ActionDispatcher dispatcher, WebDropDownButtonAction action, WebIconSize iconSize)
		{
			InitializeComponent();

            _iconSize = iconSize;
            _actionDispatcher = dispatcher;
			_actionItem = action;
			
			dispatcher.Register(_actionItem.Identifier, this);

            SetIcon();

            ToolTipService.SetToolTip(StackPanelVerticalComponent, _actionItem.ToolTip);

			ButtonComponent.Click += OnClick;
		    DropButtonComponent.Height = ButtonComponent.Height;
		    DropButtonComponent.Click += OnDropClick;
            
            _dropMenu = MenuBuilder.BuildContextMenu(action, _actionDispatcher);

            StackPanelVerticalComponent.MouseEnter += ButtonComponent_MouseEnter;
            StackPanelVerticalComponent.MouseLeave += ButtonComponent_MouseLeave;

            Visibility = _actionItem.DesiredVisiblility;

			ButtonComponent.IsEnabled = _actionItem.Enabled;
			DropButtonComponent.IsEnabled = _actionItem.Enabled;

            IndicateChecked(_actionItem.IsCheckAction && _actionItem.Checked);

            OverlayCheckedIndicator.Opacity = _actionItem.IconSet.HasOverlay ? 1 : 0;
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_actionDispatcher != null)
                {
                    _actionDispatcher.Remove(_actionItem.Identifier);
                    _actionDispatcher = null;
                }

                StackPanelVerticalComponent.MouseEnter -= ButtonComponent_MouseEnter;
                StackPanelVerticalComponent.MouseLeave -= ButtonComponent_MouseLeave;
                ButtonComponent.Click -= OnClick;
                DropButtonComponent.Click -= OnDropClick;
            
                if (_dropMenu != null)
                {
                    if (disposing)
                        _dropMenu.Dispose();
                    _dropMenu = null;
                }

                _disposed = true;
            }
        }

        void ButtonComponent_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_mouseLeaveEvent != null)
                _mouseLeaveEvent(this);
        }

        void ButtonComponent_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_mouseEnterEvent != null)
                _mouseEnterEvent(this);
        }

	    private void OnClick(object o, RoutedEventArgs args)
		{
            PopupManager.CloseActivePopup();

			_actionDispatcher.EventDispatcher.DispatchMessage(
				new ActionClickedMessage
				{
					TargetId = _actionItem.Identifier,
					Identifier = Guid.NewGuid()
				});
		}

        private void OnDropClick(object o, RoutedEventArgs args)
        {
            PopupManager.CloseActivePopup();

            if (_dropMenu.IsOpen)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

		public void Update(PropertyChangedEvent e)
		{
            if (e.PropertyName.Equals("Available"))
            {
                _actionItem.Visible = (bool)e.Value;
                Visibility = _actionItem.DesiredVisiblility;
            }
            else if (e.PropertyName.Equals("Visible"))
			{
				_actionItem.Visible = (bool)e.Value;
                Visibility = _actionItem.DesiredVisiblility;
			}
			else if (e.PropertyName.Equals("Enabled"))
			{
				_actionItem.Enabled = (bool)e.Value;
				ButtonComponent.IsEnabled = _actionItem.Enabled;
			    DropButtonComponent.IsEnabled = _actionItem.Enabled;
                if (_actionItem.Checked && !_actionItem.Enabled)
                {
                    CheckedIndicator.Opacity = 0.25;
                    if (_actionItem.IconSet.HasOverlay) OverlayCheckedIndicator.Opacity = 0.25;
                }
                else if (_actionItem.Checked)
                {
                    CheckedIndicator.Opacity = 1;
                    if (_actionItem.IconSet.HasOverlay) OverlayCheckedIndicator.Opacity = 1;
                }
                else IndicateChecked(false);
			}
			else if (e.PropertyName.Equals("IconSet"))
			{
				_actionItem.IconSet = e.Value as WebIconSet;
			    SetIcon();
			}
			else if (e.PropertyName.Equals("ToolTip"))
			{
				_actionItem.ToolTip = e.Value as string;
                ToolTipService.SetToolTip(StackPanelVerticalComponent, _actionItem.ToolTip);
			}
			else if (e.PropertyName.Equals("Label"))
			{
				_actionItem.Label = e.Value as string;
			}
			else if (e.PropertyName.Equals("Checked"))
			{
				_actionItem.Checked = (bool)e.Value;
                IndicateChecked(_actionItem.Checked);
			}
			else if (e.PropertyName.Equals("DropDownActions"))
            {
				_actionItem.DropDownActions = e.Value as Collection<WebActionNode>;
                _dropMenu.Dispose();
                _dropMenu = MenuBuilder.BuildContextMenu(_actionItem, _actionDispatcher);
            }
			UpdateLayout();
		}

        public void SetIcon()
        {
            if (_actionItem == null)
                return;

            BitmapImage bi = new BitmapImage();

            if (_actionItem.IconSet != null)
            {
                switch (_iconSize)
                {
                    case WebIconSize.Large:
                        bi.SetSource(new MemoryStream(_actionItem.IconSet.LargeIcon));
                        break;

                    case WebIconSize.Medium:
                        bi.SetSource(new MemoryStream(_actionItem.IconSet.MediumIcon));
                        break;

                    case WebIconSize.Small:
                        bi.SetSource(new MemoryStream(_actionItem.IconSet.SmallIcon));
                        break;

                    default:
                        bi.SetSource(new MemoryStream(_actionItem.IconSet.MediumIcon));
                        break;
                }
            }

            Image theImage = new Image
            {
                Source = bi
            };

            ButtonComponent.Content = theImage;
            ButtonComponent.Height = bi.PixelHeight;
            ButtonComponent.Width = bi.PixelWidth;
        }

        public void RegisterOnMouseEnter(MouseEvent @event)
        {
            _mouseEnterEvent += @event;
        }

        public void RegisterOnMouseLeave(MouseEvent @event)
        {
            _mouseLeaveEvent += @event;
        }

        public void Show()
        {
            System.Windows.Point p = StackPlaceHolder.TransformOriginToRootVisual();
            p.X = p.X - 1;
            p.Y = p.Y;
            _dropMenu.Open(p);
        }

        public void Hide()
        {
            _dropMenu.IsOpen = false;
        }

        public bool IsVisible
        {
            get { return _dropMenu.IsOpen; }
        }

        // TODO: Refactor this.
        // This code is replicated (almost) in all toolbar button classes
        // Also consider using Visual State Manager when doing it
        private void IndicateChecked(bool isChecked)
        {
            if (isChecked)
            {
                var outerGlow = new DropShadowEffect();
                outerGlow.ShadowDepth = 0;
                outerGlow.BlurRadius = 20;
                outerGlow.Opacity = 1;
                outerGlow.Color = MacroStyle.MacroCheckedButtonGlow;
                ButtonComponent.Effect = outerGlow;
                CheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.MacroButtonOutlineChecked);
                CheckedIndicator.Fill = new SolidColorBrush(MacroStyle.MacroButtonOutlineChecked);
                OverlayCheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.MacroButtonOutlineChecked);
                OverlayCheckedIndicator.Fill = new SolidColorBrush(MacroStyle.MacroButtonOutlineChecked);
                OverlayCheckedIndicator.Width = ButtonComponent.Width / 2;
                OverlayCheckedIndicator.Height = ButtonComponent.Height / 2;
            }
            else
            {
                ButtonComponent.Effect = null;
                CheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.MacroButtonOutlineUnchecked);
                CheckedIndicator.Fill = new SolidColorBrush(MacroStyle.MacroButtonOutlineUnchecked);
                OverlayCheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.MacroButtonOutlineUnchecked);
                OverlayCheckedIndicator.Fill = new SolidColorBrush(MacroStyle.MacroButtonOutlineUnchecked);
                OverlayCheckedIndicator.Width = ButtonComponent.Width / 2;
                OverlayCheckedIndicator.Height = ButtonComponent.Height / 2;
            }
        }

        #region IToolstripButton Members

        public void SetIconSize(WebIconSize iconSize)
        {
            IconSize = iconSize;
        }

        #endregion
	}
}