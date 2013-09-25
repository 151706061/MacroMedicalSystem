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
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Media.Effects;
using System.IO;
using System.Windows.Media.Imaging;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
    public partial class LayoutDropDown : IToolstripDropdownButton, IActionUpdate, IDisposable
    {
        private MouseEvent _mouseEnterEvent;
        private MouseEvent _mouseLeaveEvent;
        private readonly WebDropDownAction _actionItem;
        private IPopup _popup;
        private WebIconSize _iconSize;
        private ActionDispatcher _actionDispatcher;
        private bool _disposed = false;

        private WebIconSize IconSize
        {
            set
            {
                if (_iconSize != value)
                {
                    _iconSize = value;
                    SetIcon();
                }
            }
        }

        public LayoutDropDown(ActionDispatcher dispatcher, WebDropDownAction action, WebIconSize iconSize)
		{
			InitializeComponent();
            
            _iconSize = iconSize;
            _actionDispatcher = dispatcher;
            _actionItem = action;
            _popup = new LayoutPopup(dispatcher, action.DropDownActions).AsSingleton();

			dispatcher.Register(_actionItem.Identifier, this);

            SetIcon();

            ToolTipService.SetToolTip(LayoutDropDownButton, _actionItem.ToolTip);

			ButtonComponent.Click += OnDropClick;

            LayoutDropDownButton.MouseEnter += ButtonComponent_MouseEnter;
            LayoutDropDownButton.MouseLeave += ButtonComponent_MouseLeave;

            Visibility = _actionItem.DesiredVisiblility;

			ButtonComponent.IsEnabled = _actionItem.Enabled;

            IndicateChecked(false); //This button doesn't have a checked state.
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
                LayoutDropDownButton.MouseEnter -= ButtonComponent_MouseEnter;
                LayoutDropDownButton.MouseLeave -= ButtonComponent_MouseLeave;
                ButtonComponent.Click -= OnDropClick;

                _popup.Dispose();
                _popup = null;

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

        private void OnDropClick(object o, RoutedEventArgs args)
        {
            PopupManager.CloseActivePopup();

            if (IsVisible)
                Hide();
            else
                Show();
        }

		public void Update(PropertyChangedEvent e)
		{
            if (e.PropertyName.Equals("Available"))
            {
                _actionItem.Available = (bool)e.Value;
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
			}
			else if (e.PropertyName.Equals("IconSet"))
			{
				_actionItem.IconSet = e.Value as WebIconSet;
			    SetIcon();
			}
			else if (e.PropertyName.Equals("ToolTip"))
			{
				_actionItem.ToolTip = e.Value as string;
                ToolTipService.SetToolTip(LayoutDropDownButton, _actionItem.ToolTip);
			}
			else if (e.PropertyName.Equals("Label"))
			{
				_actionItem.Label = e.Value as string;
			}
            UpdateLayout();
		}

        public void SetIcon()
        {
            if (_actionItem == null)
                return;

            var bi = new BitmapImage();

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

            var theImage = new Image
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
            Point p = StackPlaceHolder.TransformOriginToRootVisual();
            p.X = p.X - 1;
            p.Y = p.Y;
            _popup.Open(p);
        }

        public void Hide()
        {
            _popup.IsOpen = false;
        }

        public bool IsVisible
        {
            get { return _popup.IsOpen; }
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
                CheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.GetPredefinedColor("Yellow"));
                CheckedIndicator.Fill = new SolidColorBrush(MacroStyle.GetPredefinedColor("Yellow"));
                CheckedIndicator.Opacity = 1;
            }
            else
            {
                ButtonComponent.Effect = null;
                CheckedIndicator.Stroke = new SolidColorBrush(MacroStyle.GetPredefinedColor("Silver"));
                CheckedIndicator.Fill = new SolidColorBrush(MacroStyle.GetPredefinedColor("Silver"));
                CheckedIndicator.Opacity = 0.25;

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
