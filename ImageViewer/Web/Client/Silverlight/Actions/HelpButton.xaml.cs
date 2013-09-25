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
using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Media.Imaging;
using System.Reflection;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;
using Macro.ImageViewer.Web.Client.Silverlight.Views;
using System.Windows.Media;

namespace Macro.ImageViewer.Web.Client.Silverlight.Actions
{
    public partial class HelpButton : UserControl, IToolstripButton
    {
        private MouseEvent _mouseEnterEvent;
        private MouseEvent _mouseLeaveEvent;
        private AppServiceReference.WebIconSize _iconSize;

        public HelpButton()
        {
            InitializeComponent();
        }

        #region IToolstripButton Members

        public void RegisterOnMouseEnter(MouseEvent @event)
        {
            _mouseEnterEvent += @event;
        }

        public void RegisterOnMouseLeave(MouseEvent @event)
        {
            _mouseLeaveEvent += @event;
        }

        #endregion

        private void ButtonComponent_Click(object sender, RoutedEventArgs e)
        {
            PopupHelper.PopupContent(DialogTitles.About, new HelpDialogContent());
        }

        private void ButtonComponent_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseLeaveEvent != null)
                _mouseLeaveEvent(this);
        }

        private void ButtonComponent_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseEnterEvent != null)
                _mouseEnterEvent(this);
        }



        #region IToolstripButton Members

        public void SetIconSize(AppServiceReference.WebIconSize iconSize)
        {
            _iconSize = iconSize;
            switch (_iconSize)
            {
                case AppServiceReference.WebIconSize.Large:

                    HelpIcon.Source = Application.Current.Resources["HelpButtonImageLarge"] as ImageSource;
                    break;

                case AppServiceReference.WebIconSize.Medium:

                    HelpIcon.Source = Application.Current.Resources["HelpButtonImageMedium"] as ImageSource;
                    break;

                case AppServiceReference.WebIconSize.Small:

                    HelpIcon.Source = Application.Current.Resources["HelpButtonImageSmall"] as ImageSource;
                   break;
            }

        }

        #endregion
    }
}
