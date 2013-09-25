#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace Macro.Web.Client.Silverlight
{
    public static class MenuHelper
    {
        public static IMenuItem GetHighlightedItem(this IMenu item)
        {
            return GetHighlightedItem(item.Items.OfType<IMenuItem>());
        }

        public static IMenuItem GetHighlightedItem(this IMenuItem item)
        {
            if (item.IsHighlighted)
                return item;

            return GetHighlightedItem(item.Items.OfType<IMenuItem>());
        }

        public static IMenuItem GetHighlightedItem(IEnumerable<IMenuItem> items)
        {
            foreach (var item in items)
            {
                var highlightedItem = GetHighlightedItem(item);
                if (highlightedItem != null)
                    return highlightedItem;
            }

            return null;
        }

        internal static void CreateTest(this IMenu menu, int height, int depth)
        {
            menu.Items.Clear();
            for (int h = 0; h < height; ++h)
            {
                var item = new MenuItem(){Header = String.Format("Item {0}", h+1) };
                item.Click += (sender, e) => MessageBox.Show(item.Header.ToString());
                menu.Items.Add(item);
                if (h % 2 == 0)
                {
                    if (depth > 0)
                    {
                        var l = depth == 1 ? 10 : height;
                        CreateTest(item, l, depth - 1);
                    }
                }
                else
                {
                    item.IsChecked = true;
                }
            }
        }
    }
}
