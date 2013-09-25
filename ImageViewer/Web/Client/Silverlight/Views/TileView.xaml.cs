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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Threading;
using Macro.Web.Client.Silverlight;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class TileView : IMouseElement, IDisposable
    {
        const string NormalTileBorderStyle = "NormalTileBorderStyle";
        const string SelectedTileBorderStyle = "SelectedTileBorderStyle";

        #region Private Members

        private ServerEventMediator _eventMediator;

        private readonly TimeSpan MOUSE_MOVE_STOP_MSG_DELAY = TimeSpan.FromMilliseconds(50);
                                  // send mouse move msg after user has stopped for 50 ms

        private readonly double DOUBLE_CLICK_MAX_MOUSE_DISPLACEMENT = 3;
                                // interpret as double click if two clicks separate within this distance

        private readonly TimeSpan _doubleClickTime = TimeSpan.FromMilliseconds(500);

        private readonly object _tileEventSync = new object();

        private int? _firstLeftClickTicks;
        private int? _firstRightClickTicks;

        private long _lastMouseMoveMessageTick = Environment.TickCount;
        private long _lastMouseMoveTick = Environment.TickCount;

        private readonly Image _serverCursorImage = new Image() {IsHitTestVisible = false};

        #region Cursor Handling

        private bool _mouseInside;

        #endregion

        private readonly object _mouseEventLock = new object();
        private IPopup _menu;
        private Point _rightClickPosition;
        private DispatcherTimer _timer;

        private DelayedEventPublisher<EventArgs> _fpsPublisher;
        private const bool _logPerformance = false; //ApplicationContext.Current.Parameters.LogPerformance;

        private Point _currentMousePosition;
        private System.Windows.Size _parentSize;

        private Point? _firstRightClickPos;
        private Point? _firstLeftClickPos;

        private bool _destroyed;

        private Tile _tileEntity;

        #endregion

        #region Properties

        public static Panel ApplicationRootVisual;

        public bool HasCapture
        {
            get { return ServerEntity.HasCapture; }
        }

        public bool IsSelected
        {
            get { return ServerEntity.Selected; }
        }

        public Tile ServerEntity
        {
            get { return _tileEntity; }
            private set
            {
                if (_tileEntity != value)
                {
                    if (_tileEntity != null)
                    {
                        _tileEntity.PropertyChanged -= ServerEntityPropertyChanged;
                    }

                    _tileEntity = value;
                    _tileEntity.PropertyChanged += ServerEntityPropertyChanged;

                }
            }
        }

        private Point CurrentMousePosition
        {
            set
            {
                if (_currentMousePosition == value)
                    return;

                _currentMousePosition = value;
                UpdateClientCursor();
            }
        }

        #endregion

        #region Constructor

        public TileView(Tile serverEntity, ServerEventMediator mediator)
        {
            IsTabStop = true; // allow focus

            ServerEntity = serverEntity;

            if (ServerEntity.Selected)
                MouseHelper.SetActiveElement(this);
            _eventMediator = mediator;

            _eventMediator.RegisterEventHandler(ServerEntity.Identifier, OnTileEvent);

            InitializeComponent();

            //TODO (CR May 2010): would mediator design pattern simplify this by giving the mousehelper more control?
            TileImage.MouseRightButtonDown += OnDirectMouseRightButtonDown;
            TileImage.MouseLeftButtonDown += OnDirectMouseLeftButtonDown;

            TileCanvas.MouseLeave += OnMouseLeave;
            TileCanvas.MouseEnter += OnMouseEnter;

            _eventMediator.TileHasCaptureChanged += EventBrokerTileHasCaptureChanged;
        }

        #endregion

        #region Public Methods

        public void SetParentSize(System.Windows.Size size)
        {
            if (_destroyed)
                return;

            _parentSize = size;
            UpdateSize();
            Draw(Guid.Empty, ServerEntity != null ? ServerEntity.Image : null);
        }


        public void Draw(Guid evid, byte[] imageBuffer)
        {
            if (_destroyed)
                return;

            if (imageBuffer != null)
            {
                using (var ms = new MemoryStream(imageBuffer))
                {
                    var bmp = new BitmapImage
                                  {CreateOptions = BitmapCreateOptions.IgnoreImageCache | BitmapCreateOptions.None};
                    bmp.SetSource(ms);

                    TileImage.Source = null;
                    TileImage.Source = bmp;

#if DEBUG
                    Platform.Log(LogLevel.Debug,
                                 "{0} : @@@@@@ tile image updated. size: {3}x{4}, {1} bytes, evid={2}\n",
                                 Environment.TickCount, imageBuffer.Length,
                                 evid,
                                 TileImage.ActualWidth, TileImage.ActualHeight);
#endif
                }

            }
            else
            {
                if (!double.IsNaN(TileCanvas.ActualHeight) && !double.IsNaN(TileCanvas.ActualWidth))
                {

                    var bitmap = new WriteableBitmap((int) TileCanvas.ActualWidth,
                                                     (int) TileCanvas.ActualHeight);
                    TileImage.Source = null;
                    TileImage.Source = bitmap;
#if DEBUG
                    Platform.Log(LogLevel.Debug, "{0} : @@@@@@ tile image size {1}x{2}", Environment.TickCount,
                                 bitmap.PixelHeight, bitmap.PixelWidth);
#endif
                }
            }


            UpdateBorder();
            OnTileImageDrawn();

        }

        public void Dispose()
        {            
            if (_tileEntity != null)
            {
                _tileEntity.PropertyChanged -= ServerEntityPropertyChanged;
            }
            TileImage.MouseRightButtonDown -= OnDirectMouseRightButtonDown;
            TileImage.MouseLeftButtonDown -= OnDirectMouseLeftButtonDown;

            TileCanvas.MouseLeave -= OnMouseLeave;
            TileCanvas.MouseEnter -= OnMouseEnter;

            TileImage.Source = null;

            Destroy();
        }
    
        public void Destroy()
        {
            _destroyed = true;
            if (_eventMediator != null)
            {
                _eventMediator.UnregisterEventHandler(ServerEntity.Identifier);

                _eventMediator.TileHasCaptureChanged -= EventBrokerTileHasCaptureChanged;
                _eventMediator = null;
            }

            if (_menu != null)
            {
                _menu.Dispose();
                _menu = null;
            }

            if (_fpsPublisher != null)
            {
                _fpsPublisher.Dispose();
                _fpsPublisher = null;
            }

            StopMouseMoveTimer();
        }

        #endregion

        #region Event Handlers


        private void EventBrokerTileHasCaptureChanged(object sender, EventArgs e)
        {
            var source = sender as Tile;
            if (source != null)
            {
                if (!source.Identifier.Equals(ServerEntity.Identifier) && source.HasCapture)
                    LayoutRoot.IsHitTestVisible = false;
                else
                    LayoutRoot.IsHitTestVisible = true;
            }
        }


        private void OnDirectMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;

            PerformanceMonitor.CurrentInstance.ResetClientStats();
            PerformanceMonitor.CurrentInstance.Begin(-1);

            MouseHelper.PreprocessRightMouseButtonDown(this, sender, e);
        }

        private void OnDirectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;
            PerformanceMonitor.CurrentInstance.ResetClientStats();
            PerformanceMonitor.CurrentInstance.Begin(-1);
            MouseHelper.PreprocessLeftMouseButtonDown(this, sender, e);
        }

        public void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;

            DebugInformation.Text = "";

            //TODO (CR May 2010): we probably don't need a lot of the locks in the client; could just remove them.
            lock (_mouseEventLock)
            {
                MouseRightButtonUp(e == null ? new Point() : e.GetPosition(TileImage));
            }
        }

        public void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;

            ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;

            lock (_mouseEventLock)
            {
                Focus();
                PerformanceMonitor.CurrentInstance.CurrentTile = this;

                Point curPos = e.GetPosition(TileImage);
                _rightClickPosition = curPos;

                bool isDoubleClick = InterpretAsRightDoubleClick(e);

                _eventMediator.DispatchMessage(new MouseMessage
                                                   {
                                                       Button = MouseButton.Right,
                                                       Identifier = Guid.NewGuid(),
                                                       MouseButtonState = MouseButtonState.Down,
                                                       ClickCount = isDoubleClick ? 2 : 1,
                                                       // Must send the local mouse position
                                                       MousePosition =
                                                           new Position {X = (int) curPos.X, Y = (int) curPos.Y},
                                                       TargetId = ServerEntity.Identifier
                                                   });

                _mouseInside = true;
                OnCursorChanged(ServerEntity.Cursor);

                MouseHelper.SetActiveElement(this);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_destroyed)
                return;

            lock (_mouseEventLock)
            {
                _mouseInside = true;
                OnCursorChanged(ServerEntity.Cursor);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_destroyed)
                return;

            lock (_mouseEventLock)
            {
                // TODO: Why was this commented out?
                // StopMouseMoveTimer();
                _mouseInside = false;
            }
        }


        public void OnMouseMoving(object sender, MouseEventArgs e)
        {
            if (_destroyed)
                return;


            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            long elapsed = Environment.TickCount - _lastMouseMoveMessageTick;

            lock (_mouseEventLock)
            {
                _lastMouseMoveTick = Environment.TickCount;

                try
                {
                    CurrentMousePosition = e.GetPosition(TileImage);
                }
                catch (ArgumentException)
                {
                    return; // Happens sometimes on layout changes
                }

                bool okToSend = false;

                ThrottleSettings settings = ThrottleSettings.Default;
                PerformanceMonitor p = PerformanceMonitor.CurrentInstance;


                // TODO: Should maxPendingAllowed be adjusted based on FPS? Image Size? and the current operation? 
                // Experiment shows that 2 provides the smoothest experience for pan, zoom, w/l and 4 is best for stacking (in office).
                // For big images (1x1 layout), pan and zoom work best when maxPendingAllowed=1.
                int maxPendingAllowed = settings.MaxPendingMouseMoveMsgAllowed;

                if (!ServerEntity.HasCapture)
                {
                    if (IsSelected)
                    {
                        // Although we are not tracking the mouse movement,
                        // we still send mouse position to the server to obtain the new image
                        // when the mouse is hovering over the tools.
                        if (_timer == null)
                        {
                            //TODO (CR May 2010): should we just start handling the mouse globally through the helper?  It might simplify the code.
                            //_lastMouseMoveMessageTick = Environment.TickCount;
                            //SendMouseMoveMessage();
                            StartMouseMoveTimer();
                        }
                        else
                        {
                            switch (settings.Strategy)
                            {
                                case ThrottleStrategy.WhenMouseMoveRspReceived:
                                    okToSend = elapsed >=
                                               p.AverageMouseMoveMsgRTTWithResponse/
                                               settings.MaxPendingMouseMoveMsgAllowed && p.SendLag < maxPendingAllowed &&
                                               p.RenderingLag < 5;
                                    break;
                            }


                            if (okToSend)
                            {
                                // TODO: REVIEW THIS
                                // Per MSDN:
                                // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
                                // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
                                _lastMouseMoveMessageTick = Environment.TickCount;
                                SendMouseMoveMessage();
                            }
                        }
                    }
                    return;
                }

                switch (settings.Strategy)
                {
                    case ThrottleStrategy.WhenMouseMoveRspReceived:
                        okToSend = elapsed >=
                                   p.AverageMouseMoveMsgRTTWithResponse/settings.MaxPendingMouseMoveMsgAllowed &&
                                   p.SendLag < maxPendingAllowed && p.RenderingLag < 5;
                        break;
                }

                if (okToSend)
                {
                    _lastMouseMoveMessageTick = Environment.TickCount;
                    SendMouseMoveMessage();

                }
            }
        }

        public void OnMouseWheeling(object sender, MouseWheelEventArgs e)
        {
            if (_destroyed)
                return;

            PerformanceMonitor p = PerformanceMonitor.CurrentInstance;
            bool okToSend = p.MouseWheelMsgCount <= ThrottleSettings.Default.MaxPendingMouseMoveMsgAllowed;

            if (okToSend)
            {
                PerformanceMonitor.CurrentInstance.Begin(1000);

                Message msg = new MouseWheelMessage
                                  {
                                      Identifier = Guid.NewGuid(),
                                      TargetId = ServerEntity.Identifier,
                                      Delta = e.Delta
                                  };

                _eventMediator.DispatchMessage(msg);
            }
        }

        //TODO (CR May 2010): both the same code, just different variables.  Consolidate?
        private bool InterpretAsLeftDoubleClick(MouseButtonEventArgs ev)
        {
            int now = Environment.TickCount;
            int diff = _firstLeftClickTicks != null ? (now - _firstLeftClickTicks.Value) : int.MaxValue;

            //TODO (CR May 2010): we use TileImage elsewhere to get the position.
            Point curMousePos = ev.GetPosition(this);

            bool doubleClick = _firstLeftClickPos != null && diff < _doubleClickTime.TotalMilliseconds &&
                               Math.Abs(curMousePos.X - _firstLeftClickPos.Value.X) <
                               DOUBLE_CLICK_MAX_MOUSE_DISPLACEMENT
                               &&
                               Math.Abs(curMousePos.Y - _firstLeftClickPos.Value.Y) <
                               DOUBLE_CLICK_MAX_MOUSE_DISPLACEMENT;

            if (doubleClick)
            {
                _firstLeftClickTicks = null;
                _firstLeftClickPos = null;
            }
            else
            {
                _firstLeftClickTicks = now;
                _firstLeftClickPos = curMousePos;
            }

            return doubleClick;
        }

        private bool InterpretAsRightDoubleClick(MouseButtonEventArgs ev)
        {

            int diff = _firstRightClickTicks.HasValue
                           ? Environment.TickCount - _firstRightClickTicks.Value
                           : int.MaxValue;

            Point curMousePos = ev.GetPosition(this);

            bool doubleClick = _firstRightClickPos != null && diff < _doubleClickTime.TotalMilliseconds &&
                               Math.Abs(curMousePos.X - _firstRightClickPos.Value.X) <
                               DOUBLE_CLICK_MAX_MOUSE_DISPLACEMENT
                               &&
                               Math.Abs(curMousePos.Y - _firstRightClickPos.Value.Y) <
                               DOUBLE_CLICK_MAX_MOUSE_DISPLACEMENT;

            if (doubleClick)
            {
                _firstRightClickTicks = null;
                _firstRightClickPos = null;
            }
            else
            {
                _firstRightClickTicks = Environment.TickCount;
                _firstRightClickPos = curMousePos;
            }

            return doubleClick;
        }


        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;

            ApplicationActivityMonitor.Instance.LastActivityTick = Environment.TickCount;

            Focus();
            PerformanceMonitor.CurrentInstance.CurrentTile = this;

            lock (_mouseEventLock)
            {

                bool isDoubleClick = InterpretAsLeftDoubleClick(e);

                Point pos = e.GetPosition(TileImage);
                Message msg = new MouseMessage
                                  {
                                      Identifier = Guid.NewGuid(),
                                      TargetId = ServerEntity.Identifier,
                                      Button = MouseButton.Left,
                                      MouseButtonState = MouseButtonState.Down,
                                      ClickCount = isDoubleClick ? 2 : 1,
                                      MousePosition = new Position {X = (int) pos.X, Y = (int) pos.Y}

                                  };
                _eventMediator.DispatchMessage(msg);

                _mouseInside = true;
                OnCursorChanged(ServerEntity.Cursor);
                MouseHelper.SetActiveElement(this);
            }
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_destroyed)
                return;

            DebugInformation.Text = "";

            lock (_mouseEventLock)
            {
                MouseLeftButtonUp(e == null ? new Point() : e.GetPosition(TileImage));
            }
        }


        private void OnTileEvent(object sender, ServerEventArgs e)
        {
            lock (_tileEventSync)
            {
                if (e.ServerEvent is TileUpdatedEvent)
                {
                    var ev = (TileUpdatedEvent) e.ServerEvent;

                    // TODO: review this. We are changing ServerEntity without notifying others. 
                    //
                    // This code makes the ImageBoxView and its TileViews all out-of-sync.
                    // The Tiles referenced by the ServerEntity of the ImageBoxView still references the old Tile entities.
                    // If another object observes PropertyChange event on the ImageBoxView.ServerEntity to detect
                    // changes, it may stop working after this.
                    //
                    ServerEntity = ev.Tile;

                    var performance = PerformanceMonitor.CurrentInstance;
                    if (performance.CurrentTile == this)
                    {
                        if (ev.Tile!=null&& ev.Tile.Image!=null)
                            performance.LogImageDraw(ev.Tile.Image.Length);
                    }

                    Draw(ev.Identifier, ev.Tile.Image);

                    performance.RenderingLag--;
                }
                else if (e.ServerEvent is ContextMenuEvent)
                {
                    OnContextMenuEvent((e.ServerEvent as ContextMenuEvent));
                }
                else if (e.ServerEvent is PropertyChangedEvent)
                {
                    var ev = (PropertyChangedEvent) e.ServerEvent;
                    if (ev.PropertyName == "Selected")
                    {
                        ServerEntity.Selected = (bool) ev.Value;
                        if (ServerEntity.Selected)
                            MouseHelper.SetActiveElement(this);

                        UpdateBorder();
                        return;
                    }
                    if (ev.PropertyName == "HasCapture")
                    {
                        ServerEntity.HasCapture = (bool) ev.Value;
                        return;
                    }
                    if (ev.PropertyName == "Image")
                    {
                        //ServerEntity.Image = (byte[])ev.Value;
                        Draw(ev.Identifier, (byte[]) ev.Value);
                        return;
                    }
                    if (ev.PropertyName == "Cursor")
                    {
                        OnCursorChanged(ev.Value as AppServiceReference.Cursor);
                        return;
                    }
                    if (ev.PropertyName == "MousePosition")
                    {
                        OnMousePositionChanged((Position) ev.Value);
                    }
                    if (ev.PropertyName == "HasCapture")
                    {
                        ServerEntity.HasCapture = (bool) ev.Value;
                        return;
                    }
                    if (ev.PropertyName == "InformationBox")
                    {
                        OnInformationBoxChanges(ev.Value as InformationBox);
                        return;
                    }
                }
            }
        }



        #endregion

        #region Private Methods

        private void ServerEntityPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("HasCapture"))
            {
                _eventMediator.OnTileHasCaptureChanged(sender as Tile);
            }
        }

        private void UpdateSize()
        {
            //TODO (CR May 2010): make this a bit bigger?
            if (_parentSize.Width > 4 || _parentSize.Height > 4)
            {
                var adjustedWidth = (int) (_parentSize.Width);
                var adjustedHeight = (int) (_parentSize.Height);

                // Must round-up the values to prevent SL from rendering fuzzy images (sub-pixel rendering)
                Width =
                    Math.Ceiling(adjustedWidth*
                                 (ServerEntity.NormalizedRectangle.Right - ServerEntity.NormalizedRectangle.Left));
                Height =
                    Math.Ceiling(adjustedHeight*
                                 (ServerEntity.NormalizedRectangle.Bottom - ServerEntity.NormalizedRectangle.Top));
                SetValue(Canvas.LeftProperty, Math.Ceiling(adjustedWidth*(double) ServerEntity.NormalizedRectangle.Left));
                SetValue(Canvas.TopProperty, Math.Ceiling(adjustedHeight*(double) ServerEntity.NormalizedRectangle.Top));
                Visibility = Visibility.Visible;
            }
            else
            {
                Width = 0;
                Height = 0;
                SetValue(Canvas.LeftProperty, (double) 0);
                SetValue(Canvas.TopProperty, (double) 0);
                Visibility = Visibility.Collapsed;
            }

            UpdateLayout();
            UpdateServerClientRectangle();
        }

        private void UpdateServerClientRectangle()
        {
            var left = (double) TileCanvas.GetValue(Canvas.LeftProperty);
            var top = (double) TileCanvas.GetValue(Canvas.TopProperty);

            _eventMediator.DispatchMessage(new UpdatePropertyMessage
                                               {
                                                   TargetId = ServerEntity.Identifier,
                                                   PropertyName = "ClientRectangle",
                                                   Value = new Rectangle
                                                               {
                                                                   Left = (int) left,
                                                                   Top = (int) top,
                                                                   Width = (int) (left + TileCanvas.ActualWidth),
                                                                   Height = (int) (top + TileCanvas.ActualHeight)
                                                               }
                                               });
        }

        private void UpdateClientCursor()
        {
            if (!ServerEntity.HasCapture)
            {
                var serverCursorAbsPos = new Point(Canvas.GetLeft(_serverCursorImage), Canvas.GetTop(_serverCursorImage));

                // Server cursor position is relative to the ApplicationRootVisual
                // Convert it to the relative to the tile and check for visibility
                GeneralTransform transform = ApplicationRootVisual.TransformToVisual(TileImage);
                Point serverCursorTilePos = transform.Transform(serverCursorAbsPos);

                _serverCursorImage.Visibility = this.ContainsPoint(serverCursorTilePos)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            }
            else
            {
                _serverCursorImage.Visibility = Visibility.Visible;
            }

            return;
        }

        private void OnMousePositionChanged(Position mousePosition)
        {
            if (ServerEntity.Cursor == null)
                ServerEntity.Cursor = new AppServiceReference.Cursor();

            ServerEntity.MousePosition = mousePosition;

            var theTilePoint = new Point((double) ServerEntity.MousePosition.X - ServerEntity.Cursor.HotSpot.X,
                                         (double) ServerEntity.MousePosition.Y - ServerEntity.Cursor.HotSpot.Y);
            var absolutePoint = TileImage.GetAbsolutePosition(theTilePoint);
            Point rootPoint = ApplicationRootVisual.GetAbsolutePosition(new Point());

            _serverCursorImage.SetValue(Canvas.LeftProperty, absolutePoint.X - rootPoint.X);
            _serverCursorImage.SetValue(Canvas.TopProperty, absolutePoint.Y - rootPoint.Y);

            _serverCursorImage.UpdateLayout();

            UpdateClientCursor();
        }

        //TODO (CR May 2010): we could simplify the mouse and cursor handling by putting it in the mousehelper.
        private void OnCursorChanged(AppServiceReference.Cursor cursor)
        {
            ServerEntity.Cursor = cursor ?? new AppServiceReference.Cursor();
            if (!_mouseInside)
            {
                _serverCursorImage.Source = null;
                ApplicationRootVisual.Children.Remove(_serverCursorImage);
            }
            else
            {
                var theTilePoint = new Point((double) ServerEntity.MousePosition.X - ServerEntity.Cursor.HotSpot.X,
                                             (double) ServerEntity.MousePosition.Y - ServerEntity.Cursor.HotSpot.Y);
                var absolutePoint = TileImage.GetAbsolutePosition(theTilePoint);
                var rootPoint = ApplicationRootVisual.GetAbsolutePosition(new Point());

                _serverCursorImage.SetValue(Canvas.LeftProperty, absolutePoint.X - rootPoint.X);
                _serverCursorImage.SetValue(Canvas.TopProperty, absolutePoint.Y - rootPoint.Y);

                if (ServerEntity.Cursor.Icon == null)
                {
                    if (_serverCursorImage.Source != null)
                    {
                        _serverCursorImage.Source = null;
                        ApplicationRootVisual.Children.Remove(_serverCursorImage);
                    }
                }
                else
                {
                    var bmp = new BitmapImage();
                    bmp.SetSource(new MemoryStream(ServerEntity.Cursor.Icon));
                    _serverCursorImage.Source = bmp;
                    _serverCursorImage.Stretch = Stretch.None;
                    if (!ApplicationRootVisual.Children.Contains(_serverCursorImage))
                        ApplicationRootVisual.Children.Add(_serverCursorImage);
                }

                _serverCursorImage.UpdateLayout();
            }

            //TODO (CR May 2010): we're already removing it from the AppRootVisual, but this is used in a number of places.
            UpdateClientCursor();
        }

        private void OnContextMenuEvent(ContextMenuEvent @event)
        {
            if (PopupHelper.IsPopupActive) return;

            if (_menu != null)
            {
                _menu.Dispose();
                _menu = null;
            }

            if (!IsSelected) //if we're not the selected one anymore, don't show it.
                return;

            _menu = MenuBuilder.BuildContextMenu(@event.ActionModelRoot, _eventMediator);
            _menu.Open(TransformToVisual(null).Transform(_rightClickPosition));
        }

        private void OnInformationBoxChanges(InformationBox box)
        {
            ServerEntity.InformationBox = box;
            if (ServerEntity.InformationBox == null)
            {
                TileToolTip.IsOpen = false;
                return;
            }

            if (ServerEntity.InformationBox.Visible)
            {
                ToolTipText.Text = ServerEntity.InformationBox.Data;
                if (!TileToolTip.IsOpen)
                    TileToolTip.IsOpen = true;
                else
                    UpdateTileTooltipPosition();
            }
            else
            {
                TileToolTip.IsOpen = false;
                return;
            }
        }

        private void UpdateBorder()
        {
            TileImageBorder.Style= ServerEntity.Selected
                                        ? System.Windows.Application.Current.Resources[SelectedTileBorderStyle] as Style
                                        : System.Windows.Application.Current.Resources[NormalTileBorderStyle] as Style;

        }

        private void OnTileImageDrawn()
        {
            if (_logPerformance)
            {

                if (HasCapture)
                {
                    // Log the speed when stacking/window-leveling
                    if (_fpsPublisher == null)
                    {
                        _fpsPublisher = new DelayedEventPublisher<EventArgs>((s, ev) =>
                                                                                 {
                                                                                     PerformanceMonitor p =
                                                                                         PerformanceMonitor.
                                                                                             CurrentInstance;
                                                                                     double fps = p.AverageClientFps;
                                                                                     _eventMediator.PublishPerformance(new PerformanceData
                                                                                                                           {
                                                                                                                               ClientIp
                                                                                                                                   =
                                                                                                                                   ApplicationContext
                                                                                                                                   .
                                                                                                                                   Current
                                                                                                                                   .
                                                                                                                                   Parameters
                                                                                                                                   .
                                                                                                                                   LocalIPAddress,
                                                                                                                               Name
                                                                                                                                   =
                                                                                                                                   "CLIENT_STACKING_SPEED",
                                                                                                                               Value
                                                                                                                                   =
                                                                                                                                   fps
                                                                                                                           });
                                                                                     BrowserWindow.SetStatus(
                                                                                         String.Format(
                                                                                             "Stacking Speed: {0:0} fps",
                                                                                             fps));
                                                                                 }, 1000);
                    }

                    _fpsPublisher.Publish(this, EventArgs.Empty);
                }
            }
        }

        private void StartMouseMoveTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer
                             {
                                 Interval =
                                     PerformanceMonitor.CurrentInstance.AverageClientFps > 10
                                         ? MOUSE_MOVE_STOP_MSG_DELAY
                                         : TimeSpan.FromMilliseconds(100)
                             };
                _timer.Tick += (s, ev) =>
                                   {
                                       if (_destroyed)
                                           return;

                                       long elapsedTime = Environment.TickCount - _lastMouseMoveTick;
                                       if (elapsedTime >= MOUSE_MOVE_STOP_MSG_DELAY.TotalMilliseconds)
                                       {
                                           StopMouseMoveTimer();

                                           _lastMouseMoveMessageTick = Environment.TickCount;
                                           SendMouseMoveMessage();
                                       }
                                   };

                _timer.Start();
            }


        }

        private void StopMouseMoveTimer()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer = null;
        }

        private void SendMouseMoveMessage()
        {
            if (_destroyed)
                return;

            Point pos = _currentMousePosition;
            Message msg = new MouseMoveMessage
                              {
                                  Identifier = Guid.NewGuid(),
                                  TargetId = ServerEntity.Identifier,
                                  MousePosition = new Position {X = (int) pos.X, Y = (int) pos.Y},
                                  Button = MouseButton.None,
                                  MouseButtonState = MouseButtonState.Up
                              };

            _eventMediator.DispatchMessage(msg);
        }

        private new void MouseLeftButtonUp(Point pos)
        {
            lock (_mouseEventLock)
            {
                StopMouseMoveTimer();
                Message msg = new MouseMessage
                                  {
                                      Identifier = Guid.NewGuid(),
                                      TargetId = ServerEntity.Identifier,
                                      Button = MouseButton.Left,
                                      MouseButtonState = MouseButtonState.Up,
                                      MousePosition = new Position {X = (int) pos.X, Y = (int) pos.Y}

                                  };
                _eventMediator.DispatchMessage(msg);
                OnCursorChanged(ServerEntity.Cursor);
            }
        }

        private new void MouseRightButtonUp(Point localMousePos)
        {
            lock (_mouseEventLock)
            {
                _rightClickPosition = localMousePos;

                _eventMediator.DispatchMessage(new MouseMessage
                                                   {
                                                       Button = MouseButton.Right,
                                                       Identifier = Guid.NewGuid(),
                                                       MouseButtonState = MouseButtonState.Up,
                                                       // Must send the local mouse position
                                                       MousePosition =
                                                           new Position
                                                               {X = (int) localMousePos.X, Y = (int) localMousePos.Y},
                                                       TargetId = ServerEntity.Identifier
                                                   });

                OnCursorChanged(ServerEntity.Cursor);
            }
        }


        private void TileToolTipOpened(object sender, EventArgs e)
        {
            UpdateTileTooltipPosition();
        }

        /// <summary>
        /// Adjust the tooltip position so that it's not obscured
        /// </summary>
        private void UpdateTileTooltipPosition()
        {
            if (TooltipContent.ActualHeight == 0 || TooltipContent.ActualWidth == 0)
                TileToolTip.UpdateLayout();

            const int offset = 5;
            var tooltipPt = new Point(ServerEntity.InformationBox.Location.X, ServerEntity.InformationBox.Location.Y);
            var tooltipAbsPt = this.GetAbsolutePosition(tooltipPt);

            if (tooltipAbsPt.X + TooltipContent.ActualWidth + offset <
                System.Windows.Application.Current.Host.Content.ActualWidth)
            {
                TileToolTip.HorizontalOffset = ServerEntity.InformationBox.Location.X + offset;
            }
            else
            {
                TileToolTip.HorizontalOffset = ServerEntity.InformationBox.Location.X - offset -
                                               TooltipContent.ActualWidth;
            }

            if (tooltipAbsPt.Y + TooltipContent.ActualHeight + offset <
                System.Windows.Application.Current.Host.Content.ActualHeight)
            {
                TileToolTip.VerticalOffset = ServerEntity.InformationBox.Location.Y + offset;
            }
            else
            {
                TileToolTip.VerticalOffset = ServerEntity.InformationBox.Location.Y - offset -
                                             TooltipContent.ActualHeight;
            }

        }

        private void TileToolTipSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTileTooltipPosition();
        }

        #endregion
    }
}

