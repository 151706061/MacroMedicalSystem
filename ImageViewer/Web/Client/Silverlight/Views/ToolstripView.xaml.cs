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
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Actions;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class ToolstripView : IDisposable
    {
        private readonly Dictionary<Guid, IToolstripButton> _buttonLookup = new Dictionary<Guid, IToolstripButton>();
        private ActionDispatcher _dispatcher;
        private ServerEventMediator _eventMediator;
        WebIconSize _desiredIconSize = WebIconSize.Medium;
        private bool _disposed = false;

        public ServerEventMediator EventDispatcher
        {
            set
            {
                _eventMediator = value;
                _dispatcher = new ActionDispatcher(_eventMediator);
                _eventMediator.TileHasCaptureChanged += EventBrokerTileHasCapture;
            }
        }

        public ToolstripView()
        {
            InitializeComponent();
            System.Windows.Application.Current.Host.Content.Resized += OnApplicationResized;
        }

        public void OnLoseFocus(object sender, EventArgs e)
        {
             foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                IToolstripDropdownButton dropButton = stripButton as IToolstripDropdownButton;
                if (dropButton == null) continue;

                if (dropButton.IsVisible)
                    dropButton.Hide();
            }
        }

        public void OnLayoutUpdated(object sender, EventArgs e)
        {
            // Had problems here when the initial rendering is off screen, and then it is rendered on screen.  This fixes the heights.
            if (Height == 0.0 || Width == 0.0)
                OnApplicationResized(sender, e);
        }

        private void OnApplicationResized(object sender, EventArgs e)
        {
            if (Height != LayoutRoot.DesiredSize.Height && LayoutRoot.DesiredSize.Height > 0)
            {
                Height = LayoutRoot.DesiredSize.Height;
                UpdateLayout();
            }
            if (Height != LayoutRoot.ActualHeight && LayoutRoot.ActualHeight > 0)
            {
                Height = LayoutRoot.ActualHeight;
                UpdateLayout();
            }
        }

        public void SetActionModel(IEnumerable<WebActionNode> actionModel)
        {
            LayoutRoot.Children.Clear();

			foreach (WebActionNode action in actionModel)
            {
				//TODO: what if there are children?
				if (action is WebDropDownButtonAction)
                {
                    var theButton = new DropDownButton(_dispatcher, action as WebDropDownButtonAction,_desiredIconSize);

					_buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
                else if (action is WebDropDownAction)
                {

                    var theButton = new LayoutDropDown(_dispatcher, action as WebDropDownAction,_desiredIconSize);

                    _buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
                else
                {
                    var theButton = new StandardButton(_dispatcher, action as WebClickAction, _desiredIconSize);

                    _buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
            }

            OnActionModelChanged();
        }

        private void OnActionModelChanged()
        {
            AddHelpButton();

            UpdateLayout();
            if (LayoutRoot.ActualHeight != 0)
            {
                Height = LayoutRoot.ActualHeight;
                UpdateLayout();
            }
        }

        private void AddHelpButton()
        {
            var theButton = new HelpButton();
            theButton.SetIconSize(_desiredIconSize);
            LayoutRoot.Children.Add(theButton);
            theButton.RegisterOnMouseEnter(OnMouseEnter);
            theButton.RegisterOnMouseLeave(OnMouseLeave);
        }
        
        void EventBrokerTileHasCapture(object sender, EventArgs e)
        {
            var tile = sender as Tile;
            LayoutRoot.IsHitTestVisible = !tile.HasCapture;
        }

		private void OnMouseEnter(IToolstripButton button)
        {
            foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                if (stripButton == button) continue;

                IToolstripDropdownButton dropButton = stripButton as IToolstripDropdownButton;
                if (dropButton == null) continue;

                if (dropButton.IsVisible)
                    dropButton.Hide();
            }
        }

        private void OnMouseLeave(IToolstripButton button)
        {
            
        }

        internal void SetIconSize(WebIconSize webIconSize)
        {
            _desiredIconSize = webIconSize;
            foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                stripButton.SetIconSize(_desiredIconSize);
            }
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
                System.Windows.Application.Current.Host.Content.Resized -= OnApplicationResized;

                if (disposing)
                {
                    foreach (var c in LayoutRoot.Children)
                    {
                        var d = c as IDisposable;
                        if (d!= null)
                            d.Dispose();
                    }
                } 
                
                if (_dispatcher != null)
                {
                    _dispatcher.Dispose();
                    _dispatcher = null;
                }

                if (_eventMediator != null)
                {
                    _eventMediator.TileHasCaptureChanged -= EventBrokerTileHasCapture;
                    _eventMediator = null;
                }
             

                LayoutRoot.Children.Clear();

                _disposed = true;
            }
        }
    }
}