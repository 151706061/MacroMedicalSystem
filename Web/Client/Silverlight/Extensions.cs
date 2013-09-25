#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace Macro.Web.Client.Silverlight
{
    public static class Extensions
    {
        internal static FrameworkElement RootVisual
        {
            get { return Application.Current.RootVisual as FrameworkElement; }
        }

        public static FrameworkElement GetRootVisual(this UIElement uiElement)
        {
            return RootVisual;
        }

        public static GeneralTransform TransformToRootVisual(this UIElement uiElement)
        {
            return uiElement.TransformToVisual(RootVisual);
        }

        public static GeneralTransform TransformFromRootVisual(this UIElement uiElement)
        {
            return RootVisual.TransformToVisual(uiElement);
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject obj) where T : class
        {
            foreach (var child in FindChildren<T>(obj, true))
                yield return child;
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject obj, bool recursive) where T : class
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    yield return child as T;

                if (!recursive)
                    continue;

                foreach (var sub in FindChildren<T>(child))
                    yield return sub;
            }
        }

        public static IPopup AsSingleton(this IPopup popup)
        {
            PopupManager.RegisterSingletonPopup(popup);
            return popup;
        }

        internal static IPopup AsPopup(this FrameworkElement popup)
        {
            if (popup != null)
            {
                if (popup is IPopup)
                    return (IPopup)popup;

                if (popup is Popup)
                    return new FrameworkPopupProxy((Popup)popup);
            }

            return null;
        }
    }
}
