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
using Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using Macro.Web.Client.Silverlight;


namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
	public partial class StudyView : UserControl, IDisposable
    {
        private List<ImageBoxView> _imageBoxViews;
        private DelayedEventPublisher<EventArgs> _resizePublisher;
        private readonly ServerEventMediator _eventMediator;
	    private bool _disposed = false;

        public StudyView(ServerEventMediator eventMediator)
        {
            InitializeComponent();
            _eventMediator = eventMediator;
            _imageBoxViews = new List<ImageBoxView>();

			SizeChanged += OnSizeChanged;
            _resizePublisher = new DelayedEventPublisher<EventArgs>(ResizeEvent, 350);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_resizePublisher != null)
                {
                    if (disposing)
                        _resizePublisher.Dispose();
                    _resizePublisher = null;
                }
                SizeChanged -= OnSizeChanged;

                DestroyImageBoxViews();
                _disposed = true;
            }
        }

        #region Private Methods

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
            //Dispatcher.BeginInvoke(() => SetImageBoxesParentSize());
            _resizePublisher.Publish(null,null);
		}

        private void ResizeEvent(object s, EventArgs a)
        {
            Dispatcher.BeginInvoke(SetImageBoxesParentSize);
        }
		
        private void SetImageBoxesParentSize()
		{
            if (_imageBoxViews != null)
            {
                foreach (ImageBoxView boxView in _imageBoxViews)
                    boxView.SetParentSize(new System.Windows.Size(StudyViewCanvas.ActualWidth, StudyViewCanvas.ActualHeight));
            }
		}

		private void DestroyImageBoxViews()
		{
			StudyViewCanvas.Children.Clear();

			if (_imageBoxViews != null)
			{
				foreach (ImageBoxView imageBox in _imageBoxViews)
					imageBox.Destroy();

				_imageBoxViews = null;
			}
		}

        #endregion

        #region Public Methods

        public void SetImageBoxes(IEnumerable<ImageBox> imageBoxes)
        {
            DestroyImageBoxViews();
            _imageBoxViews = new List<ImageBoxView>();

            foreach (ImageBox box in imageBoxes)
            {
                var boxView = new ImageBoxView(box,_eventMediator);
                StudyViewCanvas.Children.Add(boxView);
                _imageBoxViews.Add(boxView);

            }

            SetImageBoxesParentSize();
        }
        
        #endregion
    }
}