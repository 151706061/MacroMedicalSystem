#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Drawing;
using Macro.Common.Utilities;
using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.Imaging;
using Macro.ImageViewer.InteractiveGraphics;

namespace Macro.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable(false)]
	internal partial class FusionOverlayCompositeGraphic : CompositeGraphic, IVoiLutProvider, IColorMapProvider, ILayerOpacityProvider
	{
		[CloneIgnore]
		private IFusionOverlayFrameDataReference _overlayFrameDataReference;

		[CloneIgnore]
		private GrayscaleImageGraphic _overlayImageGraphic;

		private VoiLutManagerProxy _voiLutManagerProxy;
		private ColorMapManagerProxy _colorMapManagerProxy;

		public FusionOverlayCompositeGraphic(FusionOverlayFrameData overlayFrameData)
		{
			_overlayFrameDataReference = overlayFrameData.CreateTransientReference();
			_overlayFrameDataReference.FusionOverlayFrameData.Unloaded += HandleOverlayFrameDataUnloaded;
			_voiLutManagerProxy = new VoiLutManagerProxy();
			_colorMapManagerProxy = new ColorMapManagerProxy();

			// install an initial VOI window if one is available in the overlay headers
			var initialVoiWindow = CollectionUtils.FirstElement(overlayFrameData.OverlayData.VoiWindows);
			if (initialVoiWindow != null)
				VoiLutManager.InstallVoiLut(new BasicVoiLutLinear(initialVoiWindow.Width, initialVoiWindow.Center));
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionOverlayCompositeGraphic(FusionOverlayCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_overlayFrameDataReference = source._overlayFrameDataReference.Clone();
			_overlayFrameDataReference.FusionOverlayFrameData.Unloaded += HandleOverlayFrameDataUnloaded;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_overlayImageGraphic = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is GrayscaleImageGraphic);
			if (_overlayImageGraphic != null)
			{
				_voiLutManagerProxy.SetRealVoiLutManager(_overlayImageGraphic.VoiLutManager);
				_colorMapManagerProxy.SetRealColorMapManager(_overlayImageGraphic.ColorMapManager);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_overlayImageGraphic = null;
				_voiLutManagerProxy = null;
				_colorMapManagerProxy = null;

				if (_overlayFrameDataReference != null)
				{
					_overlayFrameDataReference.FusionOverlayFrameData.Unloaded -= HandleOverlayFrameDataUnloaded;
					_overlayFrameDataReference.Dispose();
					_overlayFrameDataReference = null;
				}
			}

			base.Dispose(disposing);
		}

		public IVoiLutManager VoiLutManager
		{
			get { return _voiLutManagerProxy; }
		}

		public IColorMapManager ColorMapManager
		{
			get { return _colorMapManagerProxy; }
		}

		public ILayerOpacityManager LayerOpacityManager
		{
			get { return _colorMapManagerProxy.LayerOpacityManager; }
		}

		public FusionOverlayFrameData OverlayFrameData
		{
			get { return _overlayFrameDataReference.FusionOverlayFrameData; }
		}

		public GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayImageGraphic; }
			private set
			{
				if (_overlayImageGraphic != value)
				{
					if (_overlayImageGraphic != null)
					{
						base.Graphics.Remove(_overlayImageGraphic);
						_voiLutManagerProxy.SetRealVoiLutManager(null);
						_colorMapManagerProxy.SetRealColorMapManager(null);

						// disposal must be last so that the proxy objects have a chance to grab a memento in case we need to reload it later
						_overlayImageGraphic.Dispose();
					}

					_overlayImageGraphic = value;

					if (_overlayImageGraphic != null)
					{
						_voiLutManagerProxy.SetRealVoiLutManager(_overlayImageGraphic.VoiLutManager);
						_colorMapManagerProxy.SetRealColorMapManager(_overlayImageGraphic.ColorMapManager);
						base.Graphics.Insert(0, _overlayImageGraphic);
					}
				}
			}
		}

		public override void OnDrawing()
		{
			//TODO (CR Sept 2010): we already uncovered a bug related to this class and the MemoryManager.
			//We need to figure out a way to synchronize the functionality in this class along with the
			//volume/frame data being loaded and unloaded.

			if (_overlayImageGraphic == null)
			{
				_overlayFrameDataReference.FusionOverlayFrameData.Lock();
				try
				{
					if (this.ParentPresentationImage == null || !this.ParentPresentationImage.Visible)
					{
						// we're drawing to an offscreen buffer, so force the frame data to load synchronously now (progress bars must be visible to be useful)
						_overlayFrameDataReference.FusionOverlayFrameData.Load();
					}

					var progressGraphic = (ProgressGraphic) CollectionUtils.SelectFirst(this.Graphics, g => g is ProgressGraphic);

					//TODO (CR Sept 2010): as mentioned in the progress graphic code, this API is unclear
					//and doesn't guarantee that the image won't be unloaded before CreateImageGraphic is called.
					float progress;
					string message;
					if (_overlayFrameDataReference.FusionOverlayFrameData.BeginLoad(out progress, out message))
					{
						OverlayImageGraphic = _overlayFrameDataReference.FusionOverlayFrameData.CreateImageGraphic();

						if (progressGraphic != null)
						{
							this.Graphics.Remove(progressGraphic);
							progressGraphic.Dispose();
						}
					}
					else if (progressGraphic == null)
					{
						this.Graphics.Add(new ProgressGraphic(_overlayFrameDataReference.FusionOverlayFrameData, true, ProgressBarGraphicStyle.Continuous));
					}
				}
				finally
				{
					_overlayFrameDataReference.FusionOverlayFrameData.Unlock();
				}
			}
			base.OnDrawing();
		}

		private void HandleOverlayFrameDataUnloaded(object sender, EventArgs e)
		{
			OverlayImageGraphic = null;
		}

		//TODO (CR Sept 2010): Remove if unused.
		public GrayscaleImageGraphic CreateStaticOverlayImageGraphic(bool forceLoad)
		{
			_overlayFrameDataReference.FusionOverlayFrameData.Lock();
			try
			{
				if (!_overlayFrameDataReference.FusionOverlayFrameData.IsLoaded)
				{
					if (!forceLoad)
						return null;

					_overlayFrameDataReference.FusionOverlayFrameData.Load();
				}

				if (OverlayImageGraphic == null)
					OverlayImageGraphic = _overlayFrameDataReference.FusionOverlayFrameData.CreateImageGraphic();

				var staticClone = new GrayscaleImageGraphic(
					OverlayImageGraphic.Rows, OverlayImageGraphic.Columns,
					OverlayImageGraphic.BitsPerPixel, OverlayImageGraphic.BitsStored, OverlayImageGraphic.HighBit,
					OverlayImageGraphic.IsSigned, OverlayImageGraphic.Invert,
					OverlayImageGraphic.RescaleSlope, OverlayImageGraphic.RescaleIntercept,
					OverlayImageGraphic.PixelData.Raw);
				staticClone.VoiLutManager.SetMemento(OverlayImageGraphic.VoiLutManager.CreateMemento());
				staticClone.ColorMapManager.SetMemento(OverlayImageGraphic.ColorMapManager.CreateMemento());
				staticClone.SpatialTransform.SetMemento(OverlayImageGraphic.SpatialTransform.CreateMemento());
				return staticClone;
			}
			finally
			{
				_overlayFrameDataReference.FusionOverlayFrameData.Unlock();
			}
		}
	}
}