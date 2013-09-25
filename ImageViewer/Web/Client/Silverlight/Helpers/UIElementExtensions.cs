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

namespace Macro.ImageViewer.Web.Client.Silverlight.Helpers
{
    public static class UIElementExtensions
    {
        private static readonly Point _zero = new Point(0, 0);
        public static Point TransformOriginToRootVisual(this UIElement element)
        {
            try
            {
                MatrixTransform globalTransform = (MatrixTransform)element.TransformToVisual(Application.Current.RootVisual);
                return globalTransform.Matrix.Transform(_zero);
            }
            catch { }
            return _zero;
        }

        public static Point GetAbsolutePosition(this UIElement element, Point point)
        {
            GeneralTransform globalTransform = (GeneralTransform)element.TransformToVisual(Application.Current.RootVisual);
            return globalTransform.Transform(point);
        }

        /// <summary>
        /// Returns a value indicating whether a point is inside the bounding box of the element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="pointRelativeToElement"></param>
        /// <returns></returns>
        public static bool ContainsPoint(this FrameworkElement element, Point pointRelativeToElement)
        {
            //TODO: Silverlight allows more complex geometries than Rectangle.
            Rect rect = new Rect { X = 0, Y = 0, Width = element.ActualWidth, Height=element.ActualHeight };
            
            return rect.Contains(pointRelativeToElement);            
        }
    }
}
