#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;

namespace Macro.Web.Client.Silverlight
{
    public static class PopupManager
    {
        public static event EventHandler PopupOpened;
        public static event EventHandler PopupClosed;

        private static IPopup _activePopup;

        internal static IPopup ActivePopup
        {
            get { return _activePopup; }
            set
            {
                if (ReferenceEquals(value, _activePopup))
                    return;

                var old = _activePopup;
                _activePopup = value;
                if (old != null)
                    old.IsOpen = false;
            }
        }

        public static void FirePopupOpened()
        {
            if (PopupOpened != null) PopupOpened(null,EventArgs.Empty);
        }

        public static void FirePopupClosed()
        {
            if (PopupClosed != null) PopupClosed(null, EventArgs.Empty);
        }

        public static void CloseActivePopup()
        {
            if (_activePopup != null)
                _activePopup.IsOpen = false;
        }

        internal static void RegisterSingletonPopup(IPopup popup)
        {
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;
        }

        private static void OnPopupOpened(object sender, EventArgs e)
        {
            ActivePopup = (IPopup)sender;
            if (PopupOpened != null) PopupOpened(sender, e);
        }

        private static void OnPopupClosed(object sender, EventArgs e)
        {
            var popup = (IPopup)sender;
            if (ReferenceEquals(_activePopup, popup))
            {
                ActivePopup = null;
                if (PopupClosed != null) PopupClosed(sender, e);
            }
        }
   }
}