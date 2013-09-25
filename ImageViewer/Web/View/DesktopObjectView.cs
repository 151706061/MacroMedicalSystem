#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Desktop;

namespace Macro.ImageViewer.Web.View
{
    /// <summary>
    /// Abstract base implementation of <see cref="IDesktopObjectView"/>.
    /// </summary>
    public abstract class DesktopObjectView : WebView, IDesktopObjectView
    {
        private bool _active;
        private bool _visible;
        private event EventHandler _activeChanged;
        private event EventHandler _visibleChanged;
        private event EventHandler _closeRequested;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DesktopObjectView()
        {
        }

        #region IDesktopObjectView Members

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property changes.
        /// </summary>
        public event EventHandler VisibleChanged
        {
            add { _visibleChanged += value; }
            remove { _visibleChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="Active"/> property changes.
        /// </summary>
        public event EventHandler ActiveChanged
        {
            add { _activeChanged += value; }
            remove { _activeChanged -= value; }
        }

        /// <summary>
        /// Occurs when the user has requested to close the view.
        /// </summary>
        public event EventHandler CloseRequested
        {
            add { _closeRequested += value; }
            remove { _closeRequested -= value; }
        }

        /// <summary>
        /// Sets the title that is displayed on the view.
        /// </summary>
        /// <param name="title"></param>
        public abstract void SetTitle(string title);

        /// <summary>
        /// Opens the view.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Shows the view.
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// Hides the view.
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// Activates the view.
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Gets a value indicating whether the view is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
        }

        /// <summary>
        /// Gets a value indicating whether the view is active.
        /// </summary>
        public bool Active
        {
            get { return _active; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sets the <see cref="Visible"/> property of this view.
        /// </summary>
        /// <param name="visible"></param>
        protected internal void SetVisibleStatus(bool visible)
        {
            if (_visible != visible)
            {
                _visible = visible;
                OnVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the <see cref="Active"/> property of this view.
        /// </summary>
        /// <param name="active"></param>
        protected internal void SetActiveStatus(bool active)
        {
            if (_active != active)
            {
                _active = active;
                OnActiveChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="CloseRequested"/> event.
        /// </summary>
        protected internal void RaiseCloseRequested()
        {
            OnCloseRequested(EventArgs.Empty);
        }

        #endregion

        #region Protected overridables

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // nothing to dispose of
        }

        /// <summary>
        /// Raises the <see cref="CloseRequested"/> event.
        /// </summary>
        protected virtual void OnCloseRequested(EventArgs e)
        {
            EventsHelper.Fire(_closeRequested, this, e);
        }

        /// <summary>
        /// Raises the <see cref="ActiveChanged"/> event.
        /// </summary>
        protected virtual void OnActiveChanged(EventArgs e)
        {
            EventsHelper.Fire(_activeChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            EventsHelper.Fire(_visibleChanged, this, e);
        }

        #endregion
    }
}
