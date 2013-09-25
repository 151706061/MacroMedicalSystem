#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
	public partial class StandardButton : IActionUpdate, IToolstripButton, IDisposable
	{
	    private MouseEvent _mouseEnterEvent;
        private MouseEvent _mouseLeaveEvent;
        private readonly WebClickAction _actionItem;
        private ActionDispatcher _actionDispatcher;
        private WebIconSize _iconSize;
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

        public StandardButton(ActionDispatcher dispatcher, WebClickAction icon, WebIconSize iconSize)
		{
			InitializeComponent();

			_actionItem = icon;
            _actionDispatcher = dispatcher;

            _actionDispatcher.Register(_actionItem.Identifier, this);

            SetIconSize(iconSize); 
            SetIcon();

			ToolTipService.SetToolTip(ButtonComponent, _actionItem.ToolTip);

			ButtonComponent.Click += OnClick;

            Visibility = _actionItem.DesiredVisiblility;

			ButtonComponent.IsEnabled = _actionItem.Enabled;
            ButtonComponent.MouseEnter += ButtonComponentMouseEnter;
            ButtonComponent.MouseLeave += ButtonComponentMouseLeave;

			IndicateChecked(_actionItem.IsCheckAction && _actionItem.Checked);

            OverlayCheckedIndicator.Opacity = _actionItem.IconSet.HasOverlay ? 1 : 0;
		}

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_actionDispatcher != null)
                {
                    _actionDispatcher.Remove(_actionItem.Identifier);
                    _actionDispatcher = null;
                }

                ButtonComponent.MouseEnter -= ButtonComponentMouseEnter;
                ButtonComponent.MouseLeave -= ButtonComponentMouseLeave;
                ButtonComponent.Click -= OnClick;
                _disposed = true;
            }
        }

        void ButtonComponentMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseLeaveEvent != null)
                _mouseLeaveEvent(this);
        }

        void ButtonComponentMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
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
                if (_actionItem.Checked && !_actionItem.Enabled)
                {
                    CheckedIndicator.Opacity = 0.25;
                    if(_actionItem.IconSet.HasOverlay) OverlayCheckedIndicator.Opacity = 0.25;
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
				ToolTipService.SetToolTip(ButtonComponent, _actionItem.ToolTip);
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

        
        // TODO: Refactor this.
        // This code is replicated (almost) in all toolbar button classes
        // Also consider using Visual State Manager when doing it
        private void IndicateChecked(bool isChecked) {
            if (isChecked)
            {
                var outerGlow = new DropShadowEffect
                                    {
                                        ShadowDepth = 0,
                                        BlurRadius = 20,
                                        Opacity = 1,
                                        Color = MacroStyle.MacroCheckedButtonGlow
                                    };
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


                OverlayCheckedIndicator.Width = ButtonComponent.Width /2;
                OverlayCheckedIndicator.Height = ButtonComponent.Height /2;
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