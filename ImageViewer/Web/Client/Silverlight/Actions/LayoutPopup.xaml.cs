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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.Web.Client.Silverlight;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
    /// <summary>
    /// Popup component for the Layout tool.
    /// </summary>
    public partial class LayoutPopup : UserControl, IPopup, IDisposable
    {

        //TODO: Convert this into a control. Combine Box and inner Rectangle.
        private class Box 
        {
            static string Key_ToolLayout_PopupBorderStyle = "ToolLayout_PopupBorderStyle";
            static string Key_ToolLayout_BoxBorderStyle = "ToolLayout_BoxBorderStyle";
            static string Key_ToolLayout_BoxNormalStyle = "ToolLayout_BoxNormalStyle";
            static string Key_ToolLayout_BoxHoverStyle = "ToolLayout_BoxHoverStyle";

            public delegate void ClickDelegate(WebLayoutChangerAction action, int row, int column);

            public int Row {get;set;}
            public int Column {get;set;}
            public Border TheBorder {get;set;}
            public System.Windows.Shapes.Rectangle TheRectangle {get;set;}
            private readonly IList<Box> _boxList;
            private readonly Grid _grid;
            private readonly TextBlock _textBox;
            private readonly WebLayoutChangerAction _action;

            public ClickDelegate OnClick;
 
            public Box(int row, int col, Grid containingGrid, IList<Box> boxList, TextBlock textBox, WebLayoutChangerAction action)
            {
                _boxList = boxList;
                _grid = containingGrid;
                _textBox = textBox;
                _action = action;

                Row = row;
                Column = col;

                TheBorder = new Border();
                TheBorder.Style = System.Windows.Application.Current.Resources[Key_ToolLayout_BoxBorderStyle] as Style;

                TheRectangle = new System.Windows.Shapes.Rectangle();
                TheRectangle.Style = System.Windows.Application.Current.Resources[Key_ToolLayout_BoxNormalStyle] as Style;

                TheBorder.Child = TheRectangle;
                TheBorder.SetValue(Grid.RowProperty, row);
                TheBorder.SetValue(Grid.ColumnProperty, Column);
            
                _grid.Children.Add(TheBorder);
                
                TheRectangle.MouseEnter += TheRectangle_MouseEnter;
                _grid.MouseLeave += Grid_MouseLeave;

                TheRectangle.MouseLeftButtonUp += TheRectangle_MouseLeftButtonUp;
            }

            void TheRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                if (OnClick != null)
                    OnClick(_action, Row+1, Column+1);
            }

            public void Grid_MouseLeave(object sender, MouseEventArgs e)
            {
                _textBox.Text = _action.Label;
                TheRectangle.Style = System.Windows.Application.Current.Resources[Key_ToolLayout_BoxNormalStyle] as Style;
            }

            void TheRectangle_MouseEnter(object sender, MouseEventArgs e)
            {
                _textBox.Text = string.Format("{0}:{1}x{2}", _action.Label, Row + 1, Column + 1);

                foreach (Box theBox in _boxList)
                {
                    if (theBox.Row <= Row && theBox.Column <= Column)
                    {
                        theBox.TheRectangle.Style = System.Windows.Application.Current.Resources[Key_ToolLayout_BoxHoverStyle] as Style;

                    }
                    else
                    {
                        //theBox.TheRectangle.Fill = new SolidColorBrush(Colors.Transparent);
                        theBox.TheRectangle.Style = System.Windows.Application.Current.Resources[Key_ToolLayout_BoxNormalStyle] as Style;

                    }
                }
            }
        }

        #region DependencyProperty Members
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen",
            typeof(bool), typeof(LayoutPopup), new PropertyMetadata(false, OnIsOpenPropertyChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset",
            typeof(double), typeof(LayoutPopup), new PropertyMetadata(0.0,
                                                                      OnHorizontalOffsetPropertyChanged));
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset",
            typeof(double), typeof(LayoutPopup), new PropertyMetadata(0.0, OnVerticalOffsetPropertyChanged));
        #endregion

        private readonly List<Box> _imageBoxList = new List<Box>();
        private readonly List<Box> _tileList = new List<Box>();
        private readonly WebLayoutChangerAction _imageBoxAction;
        private readonly WebLayoutChangerAction _tilesAction;
        private ActionDispatcher _actionDispatcher;
        private bool _disposed = false;

        private event EventHandler _opened;
        private event EventHandler _closed;

        public LayoutPopup(ActionDispatcher dispatcher, IEnumerable<WebActionNode> actions)
        {
            InitializeComponent();


            Popup.IsOpen = false;

            IsTabStop = true; // allow focus

            _actionDispatcher = dispatcher;

            //because the layout popup is just like a context menu, handle the right mouse explicitly.
            HostRoot.MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e) { e.Handled = true; Hide(); };
            HostRoot.MouseRightButtonUp += (sender, e) => e.Handled = true;

            foreach (WebLayoutChangerAction node in actions)
            {
                if (node.ActionID.Equals("chooseBoxLayout"))
                    _imageBoxAction = node;
                else if (node.ActionID.Equals("chooseTileLayout"))
                    _tilesAction = node;
            }

            ImageBoxLayoutText.Text = _imageBoxAction.Label = Labels.MenuImageBoxLayout;
            TileLayoutText.Text = _tilesAction.Label = Labels.MenuTileLayout;

            CreateGridElements();
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
                _actionDispatcher = null;                

                foreach (Box theBox in _imageBoxList)
                {
                    theBox.OnClick -= OnClick;
                }
                _imageBoxList.Clear();

                _disposed = true;
            }
        }

        public void CreateGridElements()
        {
            for (int cols = 0; cols < _imageBoxAction.MaxColumns; cols++)
            {
                var coldef = new ColumnDefinition {Width = GridLength.Auto};
                ImageBoxGrid.ColumnDefinitions.Add(coldef);
            }

            for (int rows = 0; rows < _imageBoxAction.MaxRows; rows++)
            {
                //do this for each row
                var rowDef = new RowDefinition {Height = GridLength.Auto};
                ImageBoxGrid.RowDefinitions.Add(rowDef);
            }

            // Fill in the individual boxes in the grid
            for (int cols = 0; cols < _imageBoxAction.MaxColumns; cols++)
                for (int rows = 0; rows < _imageBoxAction.MaxRows; rows++)
                {
                    var theBox = new Box(rows, cols, ImageBoxGrid, _imageBoxList, ImageBoxLayoutText,_imageBoxAction);
                    theBox.OnClick += OnClick;
                    _imageBoxList.Add(theBox);
                }

            for (int cols = 0; cols < _tilesAction.MaxColumns; cols++)
            {
                var coldef = new ColumnDefinition {Width = GridLength.Auto};
                TileGrid.ColumnDefinitions.Add(coldef);
            }

            for (int rows = 0; rows < _tilesAction.MaxRows; rows++)
            {
                //do this for each row
                var rowDef = new RowDefinition {Height = GridLength.Auto};
                TileGrid.RowDefinitions.Add(rowDef);
            }

            // Fill in the individual boxes in the grid
            for (int cols = 0; cols < _tilesAction.MaxColumns; cols++)
                for (int rows = 0; rows < _tilesAction.MaxColumns; rows++)
                {
                    var theBox = new Box(rows, cols, TileGrid, _tileList, TileLayoutText,_tilesAction);
                    theBox.OnClick += OnClick;
                    _tileList.Add(theBox);
                }
        }

        private void OnClick(WebLayoutChangerAction action, int rows, int columns)
        {
            Hide();

            var msg = new SetLayoutActionMessage
                          {
                              Columns = columns,
                              Rows = rows,
                              TargetId = action.Identifier,
                              Identifier = action.Identifier
                          };

            _actionDispatcher.EventDispatcher.DispatchMessage(msg);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            Hide();
        }

        #region IPopup Members

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

        #endregion

        public void Open(Point point)
        {
            HorizontalOffset = point.X;
            VerticalOffset = point.Y;

            IsOpen = true;
        }

        public void Hide()
        {
            IsOpen = false;
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        #region PropertyChanged Members
        private void OnIsOpenPropertyChanged(bool newValue)
        {
            if (Popup == null || Popup.IsOpen == newValue)
                return;

            foreach (Box b in _tileList)
                b.Grid_MouseLeave(null, null);
            foreach (Box b in _imageBoxList)
                b.Grid_MouseLeave(null, null);
            
            Popup.IsOpen = newValue;

            if (Popup.IsOpen)
            {
                Popup.HorizontalOffset = HorizontalOffset;
                Popup.VerticalOffset = VerticalOffset;
                Popup.Visibility = Visibility.Visible;
                Focus();

                System.Windows.Application.Current.Host.Content.Resized += Window_Resized;

                if (_opened != null)
                    _opened(this, EventArgs.Empty);
            }
            else
            {
                System.Windows.Application.Current.Host.Content.Resized -= Window_Resized;

                if (_closed != null)
                    _closed(this, EventArgs.Empty);
            }
        }

        private void OnHorizontalOffsetPropertyChanged(double newValue)
        {
            if (Popup != null)
                Popup.HorizontalOffset = newValue;
        }

        private void OnVerticalOffsetPropertyChanged(double newValue)
        {
            if (Popup != null)
                Popup.VerticalOffset = newValue;
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutPopup)d).OnIsOpenPropertyChanged((bool)e.NewValue);
        }

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutPopup)d).OnHorizontalOffsetPropertyChanged((double)e.NewValue);
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutPopup)d).OnVerticalOffsetPropertyChanged((double)e.NewValue);
        }
        #endregion

        private void Window_Resized(object sender, EventArgs ev)
        {
            Hide();
        }
    }
}
