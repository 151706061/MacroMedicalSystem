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
using Macro.Dicom;
using Macro.ImageViewer;
using Macro.ImageViewer.StudyManagement;
using System.Diagnostics;

namespace Macro.ImageViewer.Web.Server.ImageServer
{
    internal delegate void NotifyChangedDelegate();

    internal class ImageBoxFrameSelectionStrategy : IDisposable
    {
        private readonly IImageBox _imageBox;
        private readonly int _window;
        private readonly NotifyChangedDelegate _notifyChanged;

        private readonly object _syncLock = new object();
        private readonly Queue<Frame> _frames;

        private int _currentIndex = -1;

        public ImageBoxFrameSelectionStrategy(IImageBox imageBox, int window, NotifyChangedDelegate notifyChanged)
        {
            _notifyChanged = notifyChanged;
            _imageBox = imageBox;
            _imageBox.Drawing += OnImageBoxDrawing;

            _window = window;
            _frames = new Queue<Frame>();
            Refresh(true);
        }

        private void OnImageBoxDrawing(object sender, EventArgs e)
        {
            Refresh(false);
        }

        public void OnDisplaySetChanged()
        {
            Refresh(true);
        }

        private void Refresh(bool force)
        {
            lock (_syncLock)
            {
                if (_imageBox.DisplaySet == null || _imageBox.DisplaySet.PresentationImages.Count == 0)
                {
                    _frames.Clear();
                    return;
                }

                if (!force && _currentIndex == _imageBox.TopLeftPresentationImageIndex)
                    return;

                _frames.Clear();

                _currentIndex = _imageBox.TopLeftPresentationImageIndex;
                int numberOfImages = _imageBox.DisplaySet.PresentationImages.Count;
                int lastImageIndex = numberOfImages - 1;

                int selectionWindow;
                if (_window >= 0)
                {
                    selectionWindow = 2 * _window + 1;
                }
                else
                {
                    //not terribly efficient, but by default will end up including all images.
                    selectionWindow = 2 * numberOfImages + 1;
                }

                int offsetFromCurrent = 0;
                for (int i = 0; i < selectionWindow; ++i)
                {
                    int index = _currentIndex + offsetFromCurrent;

                    if (index >= 0 && index <= lastImageIndex)
                    {
                        IPresentationImage image = _imageBox.DisplaySet.PresentationImages[index];
                        if (image is IImageSopProvider)
                        {
                            Frame frame = ((IImageSopProvider)image).Frame;

                            if (frame.ParentImageSop.DataSource is ImageServerSopDataSource)
                            {
                                ImageServerSopDataSource dataSource = (ImageServerSopDataSource)frame.ParentImageSop.DataSource;
                                if (!dataSource.SopLoaded)
                                    _frames.Enqueue(frame);
                            }
                        }
                    }

                    if (offsetFromCurrent == 0)
                        ++offsetFromCurrent;
                    else if (offsetFromCurrent > 0)
                        offsetFromCurrent = -offsetFromCurrent;
                    else
                        offsetFromCurrent = -offsetFromCurrent + 1;
                }

                string message = String.Format("Current: {0}, Window: {1}, Queue: {2}", _currentIndex, selectionWindow, _frames.Count);
                Trace.WriteLine(message);
            }

            //trigger another round of retrievals.
            _notifyChanged();
        }

        public Frame GetNextFrame()
        {
            lock (_syncLock)
            {
                if (_frames.Count == 0)
                    return null;

                return _frames.Dequeue();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _imageBox.Drawing -= OnImageBoxDrawing;
        }

        #endregion
    }
}