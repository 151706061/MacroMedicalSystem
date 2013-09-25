#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Macro.Common.Statistics;
using Macro.Common.Utilities;
using Macro.Desktop;
using Macro.Desktop.Actions;
using Macro.Dicom;
using Macro.ImageViewer.StudyManagement;
using Macro.ImageViewer.Web.Common;
using Macro.ImageViewer.Web.Common.Events;
using Macro.ImageViewer.Web.Utiltities;
using Macro.Web.Common;
using Macro.Web.Services;
using TileEntity = Macro.ImageViewer.Web.Common.Entities.Tile;
using Macro.ImageViewer.Rendering;
using Macro.ImageViewer.InputManagement;
using System.Drawing;
using System.Drawing.Imaging;
using Macro.ImageViewer.Web.Common.Messages;
using Message=Macro.Web.Common.Message;
using MouseWheelMessage=Macro.ImageViewer.Web.Common.Messages.MouseWheelMessage;
using Rectangle=System.Drawing.Rectangle;
using Macro.ImageViewer.Web.Common.Entities;
using Macro.Web.Common.Messages;
using Macro.Common;
using Cursor=Macro.ImageViewer.Web.Common.Entities.Cursor;

namespace Macro.ImageViewer.Web.EntityHandlers
{
	internal class ContextMenuContainer : IDisposable
	{
		private readonly List<ActionNodeEntityHandler> _contextMenuHandlers;

		public ContextMenuContainer(ActionModelNode modelNode)
		{
			_contextMenuHandlers = ActionNodeEntityHandler.Create(modelNode.ChildNodes);
		}

		public WebActionNode[] GetWebActions()
		{
			return CollectionUtils.Map(_contextMenuHandlers,
				(ActionNodeEntityHandler handler) => (WebActionNode)handler.GetEntity()).ToArray();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_contextMenuHandlers == null)
				return;

			foreach (ActionNodeEntityHandler handler in _contextMenuHandlers)
				handler.Dispose();

			_contextMenuHandlers.Clear();
		}

		#endregion
	}

    public interface IQFactorStrategy
    {
        long GetOptimalQFactor(int imageWidth, int imageHeight, IImageSopProvider sop);
    }

    public class QFactorExtensionPoint : ExtensionPoint<IQFactorStrategy> { }

    public class TileEntityHandler : EntityHandler<TileEntity>
	{
		private readonly string _mimeType = "image/jpeg";

        // TODO: Should this be at the application level?
        // So that all images are at the same quality during sync stacking
        // How can other tiles actively refresh themselves when this tile does?
        private readonly long _defaultJpegQFactor = WebViewerServices.Default.JpegQualityFactor;
        private long _quality = WebViewerServices.Default.JpegQualityFactor;

		private StatisticsSet _stats = new StatisticsSet("AverageRender");

		private Tile _tile;
		private Point _mousePosition;
		private bool _hasCapture;
		private WebTileInputTranslator _tileInputTranslator;
		private TileController _tileController;

		private IRenderingSurface _surface;
		private Bitmap _bitmap;

		[ThreadStatic]private static ContextMenuContainer _contextMenu;

	    private readonly IQFactorStrategy _qFactorPlugin;

        private bool _isMouseDown;
        private readonly object _drawLock = new object();
        private readonly object _refreshLock = new object();
        private bool _refreshingClient = false;

        // timer to push image to the client when dynamic image quality is used
        private System.Threading.Timer _refreshTimer; 
        
        public TileEntityHandler()
		{
            if (WebViewerServices.Default.CompressionType.ToLower().Equals("jpeg"))
                _mimeType = "image/jpeg";
            else if (WebViewerServices.Default.CompressionType.ToLower().Equals("png"))
                _mimeType = "image/png";
            else
                _mimeType = "image/jpeg"; // Default to jpeg

		    _quality = _defaultJpegQFactor;

            try
            {
                _qFactorPlugin = new QFactorExtensionPoint().CreateExtension() as IQFactorStrategy;
            }
            catch (Exception)
            {
                _qFactorPlugin = new DefaultQFactorStrategy();
            }
		}

	    private long GetOptimalQFactor(int w, int h, IImageSopProvider sop)
        {
            if (_qFactorPlugin!=null)
            {
                return _qFactorPlugin.GetOptimalQFactor(w, h, sop);
            }

	        return 80L; //default
        }

        private bool IsDynamicImageQualityEnabled
        {
            get
            {
                //TODO: Cache this and update whenever the client changes it?

                const string key = "DynamicImageQualityEnabled";

                if (ApplicationContext.HasProperty(key))
                {
                    string value;
                    if (ApplicationContext.TryGetValue(key, out value))
                    {
                        bool isEnabled;
                        if (Boolean.TryParse(value, out isEnabled))
                            return isEnabled;
                    }
                }
                return false;
            }
        }

		private Rectangle ClientRectangle
		{
			get { return _tileController.TileClientRectangle; }
			set
			{
				if (_tileController.TileClientRectangle.Equals(value))
					return;

			    _tileController.TileClientRectangle = value;
				OnClientRectangleChanged();
			}
		}

		private Point MousePosition
		{
			get { return _mousePosition; }
			set
			{
				if (_mousePosition.Equals(value))
					return;

				_mousePosition = value;
				NotifyEntityPropertyChanged("MousePosition", new Position(_mousePosition));
			}
		}

		private bool HasCapture
		{
			get { return _hasCapture; }	
			set
			{
				if (value == _hasCapture)
					return;

				_hasCapture = value;
				NotifyEntityPropertyChanged("HasCapture", _hasCapture);
			}
		}

		private IRenderer Renderer
		{
			get { return CurrentImage != null ? CurrentImage.ImageRenderer : null; }
		}

		private IRenderingSurface Surface
		{
			get
			{
				if (_surface == null && Renderer != null && ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
					_surface = Renderer.GetRenderingSurface(IntPtr.Zero, ClientRectangle.Width, ClientRectangle.Height);
				
				return _surface;
			}
		}

		private Bitmap Bitmap
		{
			get
			{
				if (_bitmap == null && _surface != null)
					_bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);

				return _bitmap;
			}
		}

		private PresentationImage CurrentImage
		{
			get { return _tile.PresentationImage as PresentationImage; }
		}

		private Common.Entities.InformationBox CreateInformationBox()
		{
			if (_tile.InformationBox == null)
				return null;

			return new Common.Entities.InformationBox
									{
										Data = _tile.InformationBox.Data,
										Visible = _tile.InformationBox.Visible,
										Location = _tile.InformationBox.DestinationPoint
									};
		}

		private void DisposeSurface()
		{
			if (_surface != null)
			{
				_surface.Dispose();
				_surface = null;
			}

			if (_bitmap != null)
			{
				_bitmap.Dispose();
				_bitmap = null;
			}
		}

		public override void  SetModelObject(object modelObject)
		{
			_tile = (Tile)modelObject;
			_tileInputTranslator = new WebTileInputTranslator();
			_tileController = new TileController(_tile, ((ImageViewerComponent)_tile.ImageViewer).ShortcutManager);

			_tileController.CursorTokenChanged += OnCursorTokenChanged;
			_tileController.ContextMenuRequested += OnContextMenuRequested;

			_tile.Drawing += OnTileDrawing;
			_tile.RendererChanged += OnRendererChanged;
			_tile.SelectionChanged += OnSelectionChanged;
			_tileController.CaptureChanging += OnCaptureChanging;
		    _tile.InformationBoxChanged += OnInformationBoxChanged;
		}

		protected override void UpdateEntity(Common.Entities.Tile entity)
		{
			entity.NormalizedRectangle = _tile.NormalizedRectangle;
			entity.ClientRectangle = ClientRectangle;
			entity.MousePosition = MousePosition;
			entity.Selected = _tile.Selected;
			entity.HasCapture = HasCapture;
			entity.Cursor = CreateCursor();
			entity.InformationBox = CreateInformationBox();
			entity.Image = CreateImage();
		}

		private void OnContextMenuRequested(object sender, ItemEventArgs<Point> e)
		{
			FireContextMenuEvent();
		}

		private void FireContextMenuEvent()
		{
			if (_tileController.ContextMenuProvider != null && _tileController.ContextMenuEnabled)
			{
				ActionModelNode actionModelNode = _tileController.ContextMenuProvider.GetContextMenuModel(_tileController);
				if (_contextMenu != null)
					_contextMenu.Dispose();

				_contextMenu = new ContextMenuContainer(actionModelNode);

				ApplicationContext.FireEvent(new ContextMenuEvent
				{
					Identifier = Guid.NewGuid(),
					SenderId = Identifier,
					ActionModelRoot = new WebActionNode { Children = _contextMenu.GetWebActions() }
				});
			}
		}

        private void OnInformationBoxUpdated(object sender, EventArgs e)
        {
            if (_tile.InformationBox == null)
            {
                NotifyEntityPropertyChanged("InformationBox", null);
                return;
            }

            NotifyEntityPropertyChanged("InformationBox", CreateInformationBox());
        }

	    private void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
	    {
            if (e.InformationBox != null)
				e.InformationBox.Updated += OnInformationBoxUpdated;
			else
                NotifyEntityPropertyChanged("InformationBox", null);
	    }

	    private void OnRendererChanged(object sender, EventArgs e)
		{
			DisposeSurface();
		}

		private void OnClientRectangleChanged()
		{
			DisposeSurface();
			_tile.Draw();
		}

		private void OnCursorTokenChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("MousePosition", new Position(_mousePosition));
			NotifyEntityPropertyChanged("Cursor", CreateCursor());
		}

		private Cursor CreateCursor()
		{
			CursorToken token = _tileController.CursorToken;
			if (token == null)
				return null;

			Cursor webCursor = new Cursor();
			Bitmap bitmap;
			if (token.IsSystemCursor)
			{
				PropertyInfo propertyInfo = typeof(Cursors).GetProperty(token.ResourceName, BindingFlags.Static | BindingFlags.Public);
				var cursor = (System.Windows.Forms.Cursor)propertyInfo.GetValue(null, null);
				bitmap = new Bitmap(cursor.Size.Width, cursor.Size.Height, PixelFormat.Format32bppArgb);
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
					cursor.Draw(g, new Rectangle(Point.Empty, cursor.Size));

				webCursor.HotSpot = new Position(cursor.HotSpot);
			}
			else
			{
				bitmap = new Bitmap(token.Resolver.OpenResource(token.ResourceName));
				webCursor.HotSpot = new Position { X = bitmap.Width / 2, Y = bitmap.Height / 2 };
			}

			using (MemoryStream stream = new MemoryStream())
			{
				bitmap.Save(stream, ImageFormat.Png);
				stream.Position = 0;

				webCursor.Icon = stream.GetBuffer();
				stream.Close();
			}

			return webCursor;
		}

		private void OnCaptureChanging(object sender, ItemEventArgs<IMouseButtonHandler> e)
		{
			HasCapture = e.Item != null;
		}

		private void OnSelectionChanged(object sender, ItemEventArgs<ITile> e)
		{
			NotifyEntityPropertyChanged("Selected", _tile.Selected);
		}

		private void OnTileDrawing(object sender, EventArgs e)
		{
			Draw(false);
		}

		public void Draw(bool refresh)
		{
            lock (_drawLock)
            {
                if (refresh)
                    _refreshingClient = true;

                Event ev = new TileUpdatedEvent
                               {
                                   Identifier = Guid.NewGuid(),
                                   SenderId = Identifier,
                                   Tile = GetEntity(),
                                   MimeType = _mimeType,
                                   Quality = _quality
                               };

                ApplicationContext.FireEvent(ev);
               
                if (refresh) 
                    _refreshingClient = false;
            }
		}

        private byte[] CreateImage()
		{
			if (_tile.PresentationImage == null)
				DisposeSurface();

			if (Surface == null)
				return null;

            if (_refreshingClient)
            {
                return GetCurrentTileBitmap();
            }

            IImageSopProvider sop = _tile.PresentationImage as IImageSopProvider;
            if (sop != null)
            {
                //TODO (CR May 2010): sops are shared between users and threads.  This will be an issue
                //for dynamic quality changes.
                DicomAttribute attrib = sop.ImageSop[DicomTags.LossyImageCompression];
                DicomAttribute ratioAttrib = sop.ImageSop[DicomTags.LossyImageCompressionRatio];
                bool lossy = false;
                if (_mimeType.Equals("image/jpeg"))
                    lossy = true;
                if (lossy)
                {
                    attrib.SetStringValue("01");
                }
                else
                {
                    if (ratioAttrib.IsEmpty)
                    {
                        attrib.SetEmptyValue();
                    }
                }
            }

            WebViewStudyStatistics stats = new WebViewStudyStatistics(_mimeType);

            //long t0 = Environment.TickCount;
            stats.DrawToBitmapTime.Start();

            Bitmap bitmap = Bitmap;

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                Surface.ContextID = graphics.GetHdc();
                Surface.ClipRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                DrawArgs drawArgs = new DrawArgs(Surface, null, Rendering.DrawMode.Render);
                CurrentImage.Draw(drawArgs);
                drawArgs = new DrawArgs(Surface, null, Rendering.DrawMode.Refresh);
                CurrentImage.Draw(drawArgs);
                graphics.ReleaseHdc(Surface.ContextID);
            }

            stats.DrawToBitmapTime.End();

            Bitmap bmp1 = null;
            if (DiagnosticsSettings.Default.CompareImageQuality)
            {
                // make a copy in case Bitmap.Save() has any side effects.
                bmp1 = (Bitmap)Bitmap.Clone();
            }

            //TODO (CR May 2010): should be in using and/or closed.  Separate function?
            MemoryStream ms = new MemoryStream();

            _quality = _defaultJpegQFactor;
            if (_isMouseDown && IsDynamicImageQualityEnabled)
            {
                _quality = GetOptimalQFactor(Bitmap.Width, Bitmap.Height, sop);

                InitOrUpdateRefreshClientTimer();
            }

            stats.SaveTime.Start();
            if (_mimeType.Equals("image/jpeg"))
            {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, _quality);
                ImageCodecInfo ici = GetEncoderInfo(_mimeType);

                bitmap.Save(ms, ici, eps);

            }
            else if (_mimeType.Equals("image/png"))
            {
                bitmap.Save(ms, ImageFormat.Png);
            }
            stats.SaveTime.End();

            byte[] imageBuffer = ms.ToArray();

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                Platform.Log(LogLevel.Debug, "Render Frame #{0}. Size= {1}bytes. Q={2} {3}. Highbit={4}",
                    sop.ImageSop.InstanceNumber, imageBuffer.Length, _quality, _isMouseDown && IsDynamicImageQualityEnabled ? "[Dynamic]" : "",
                    sop.Frame.HighBit);

            ms.Position = 0;
            stats.ImageSize = (ulong)imageBuffer.LongLength;
            _stats.AddSubStats(stats);

            //StatisticsLogger.Log(LogLevel.Info, false, stats);
            if (_stats.SubStatistics.Count > 20)
            {
                _stats.CalculateAverage();
                //StatisticsLogger.Log(LogLevel.Info, false, _stats);
                _stats = new StatisticsSet("AverageRender");
            }

            //Console.WriteLine("Tile {0} : DrawToBitmap (size: {3}, mime: {2}):{1}ms", tile.Identifier,Environment.TickCount - t0,mimeType, ms.Length);

            //TODO (CR May 2010): #if DEBUG?
            if (DiagnosticsSettings.Default.CompareImageQuality)
            {
                Bitmap bmp2 = new Bitmap(ms);
                ImageComparisonResult result = BitmapComparison.Compare(ref bmp1, ref bmp2);
                //TODO (CR May 2010): ConsoleHelper
                Console.WriteLine("BMP vs {0} w/ client size: {1}x{2}", _mimeType, bmp2.Height, bmp2.Width);
                Console.WriteLine("\tR: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[0].MeanError, result.Channels[0].StdDeviation, Math.Abs(result.Channels[0].MinError), Math.Abs(result.Channels[0].MaxError));
                Console.WriteLine("\tG: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[1].MeanError, result.Channels[1].StdDeviation, Math.Abs(result.Channels[1].MinError), Math.Abs(result.Channels[1].MaxError));
                Console.WriteLine("\tB: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[2].MeanError, result.Channels[2].StdDeviation, Math.Abs(result.Channels[2].MinError), Math.Abs(result.Channels[2].MaxError));

            }

            return imageBuffer;
		}

        private void InitOrUpdateRefreshClientTimer()
        {
            // If user is drawing an ellipse, the ROI info will be updated a short time after
            // user stops moving the mouse. If we don't wait long enough user will see
            // a high-res image, the low res and another high-res image again. 
            // 500 ms is picked arbitrarily.
            TimeSpan refreshTime = TimeSpan.FromMilliseconds(500);

            lock (_refreshLock)
            {
                // If the timer is running then just postpone it
                if (_refreshTimer != null)
                {
                    _refreshTimer.Change(refreshTime, TimeSpan.Zero);
                }
                else
                {
                    _refreshTimer = new System.Threading.Timer((ignore) =>
                                                                   {
                                                                       try
                                                                       {
                                                                           // Note: the image will not go to the client until it polls
                                                                           // The assumption is the client continues polling if the user
                                                                           // stops moving the mouse
                                                                           Draw(true);
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           // catch exceptions to prevent crashing
                                                                           Platform.Log(LogLevel.Error, ex);
                                                                       }
                                                                       finally
                                                                       {
                                                                           lock (_refreshLock)
                                                                           {
                                                                               _refreshTimer.Dispose();
                                                                               _refreshTimer = null;
                                                                           }
                                                                       }
                                                                   }, null, refreshTime, TimeSpan.Zero);
                }
            }

        }

        private byte[] GetCurrentTileBitmap()
        {
            Bitmap bitmap = Bitmap;
            MemoryStream ms = new MemoryStream();

            _quality = _defaultJpegQFactor;

            if (_mimeType.Equals("image/jpeg"))
            {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, _quality);
                ImageCodecInfo ici = GetEncoderInfo(_mimeType);

                bitmap.Save(ms, ici, eps);

            }
            else if (_mimeType.Equals("image/png"))
            {
                bitmap.Save(ms, ImageFormat.Png);
            }

            return ms.ToArray();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			//TODO (CR May 2010): cache the encoder
			int j;
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType.Equals(mimeType))
					return encoders[j];
			}
			return null;
		}

		public override void ProcessMessage(Message message)
		{
            if (message is MouseMoveMessage)
			{
				ProcessMouseMoveMessage((MouseMoveMessage)message);
				//TODO: ideally, the tilecontroller would have an event and the handler would listen
				MousePosition = _tileController.Location;
				return;
			}
			if (message is MouseMessage)
			{
				ProcessMouseMessage((MouseMessage)message);
				//TODO: ideally, the tilecontroller would have an event and the handler would listen
				MousePosition = _tileController.Location;
				return;
			}
			if (message is MouseWheelMessage)
			{
				ProcessMouseWheelMessage((MouseWheelMessage)message);
				return;
			}
			if (message is UpdatePropertyMessage)
			{
				ProcessUpdateMessage((UpdatePropertyMessage)message);
			}

			//throw new InvalidOperationException("Unexpected message");
		}

		private void ProcessUpdateMessage(UpdatePropertyMessage message)
		{
			switch(message.PropertyName)
			{
				case "ClientRectangle":
					ClientRectangle = (Common.Rectangle)message.Value;
					break;
				default: break;
			}
		}

		private void ProcessMouseWheelMessage(MouseWheelMessage message)
		{
			MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 1, 0, 0, message.Delta);
			object msg = _tileInputTranslator.OnMouseWheel(e);
			_tileController.ProcessMessage(msg);
		}

		private void ProcessMouseMoveMessage(MouseMoveMessage message)
		{
			//TODO (CR May 2010): should we remove this code?  We should be very careful if we do.
			//Theoretically, this could limit us to 10 fps.

            Event ack = new MouseMoveProcessedEvent()
            {
                Identifier = Guid.NewGuid(),
                SenderId = this.Identifier
            };

            ApplicationContext.FireEvent(ack);

			MouseButtons mouseButtons = MouseButtons.None;

			switch (message.Button)
			{
				case MouseButton.Left: mouseButtons = MouseButtons.Left; break;
				case MouseButton.Right: mouseButtons = MouseButtons.Right; break;
			}

			MouseEventArgs e = new MouseEventArgs(mouseButtons, 0, message.MousePosition.X, message.MousePosition.Y, 0);
			object msg = _tileInputTranslator.OnMouseMove(e);
			_tileController.ProcessMessage(msg);
		}

		private void ProcessMouseMessage(MouseMessage message)
		{
			if (message.Button == MouseButton.Left)
			{
				if (message.MouseButtonState == MouseButtonState.Down)
				{
					MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, message.ClickCount, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseDown(e);
				    _tileController.ProcessMessage(msg);
                    _isMouseDown = true;
				}
				else if (message.MouseButtonState == MouseButtonState.Up)
				{
					MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseUp(e);
					_tileController.ProcessMessage(msg);

					//do a mouse move to set the focus state of graphics
					e = new MouseEventArgs(MouseButtons.None, 0, message.MousePosition.X, message.MousePosition.Y, 0);
					msg = _tileInputTranslator.OnMouseMove(e);
					_tileController.ProcessMessage(msg);
                    _isMouseDown = false;
				}
			}
			else if (message.Button == MouseButton.Right)
			{
				if (message.MouseButtonState == MouseButtonState.Down)
				{
					MouseEventArgs e = new MouseEventArgs(MouseButtons.Right, message.ClickCount, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseDown(e);
					_tileController.ProcessMessage(msg);
                    _isMouseDown = false;
				}
				else if (message.MouseButtonState == MouseButtonState.Up)
				{
					//TODO (CR May 2010): should we be calling this when the tilecontroller fires an event?
					FireContextMenuEvent();
					MouseEventArgs e = new MouseEventArgs(MouseButtons.Right, 1, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseUp(e);
					_tileController.ProcessMessage(msg);

					//do a mouse move to set the focus state of graphics
					e = new MouseEventArgs(MouseButtons.None, 0, message.MousePosition.X, message.MousePosition.Y, 0);
					msg = _tileInputTranslator.OnMouseMove(e);
					_tileController.ProcessMessage(msg);
                    _isMouseDown = true;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_tile.Drawing -= OnTileDrawing;
				_tile.RendererChanged -= OnRendererChanged;
				_tile.SelectionChanged -= OnSelectionChanged;
				_tileController.CursorTokenChanged -= OnCursorTokenChanged;
				_tileController.ContextMenuRequested -= OnContextMenuRequested;
				_tileController.CaptureChanging -= OnCaptureChanging;
				_tile.InformationBoxChanged -= OnInformationBoxChanged;

				if (_contextMenu != null)
				{
					_contextMenu.Dispose();
					_contextMenu = null;
				}

				try
				{
					DisposeSurface();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}
		}
	}
}
