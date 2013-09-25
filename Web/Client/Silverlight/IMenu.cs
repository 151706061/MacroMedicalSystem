#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections;

namespace Macro.Web.Client.Silverlight
{
    public interface IMenuItem : IMenu
    {
        IMenuItem ParentMenuItem { get; }

        bool IsTopLevel { get; }

        bool IsChecked { get; set; }
        ImageSource Icon { get; set; }

        bool IsHighlighted { get; set; }

        bool IsExpanded { get; set; }

        event EventHandler Click;
    }

    public interface IMenu
    {
        bool IsRoot { get; }
        MenuBase RootMenu { get; }
        IMenu ParentMenu { get; set; }

        bool HasItems { get; }
        IList Items { get; }

        IMenuItemCoordinator ItemCoordinator { get; }
        FrameworkElement ItemsContainer { get; }
    }
}
