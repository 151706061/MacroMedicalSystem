#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Macro.Web.Client.Silverlight
{
    /// <summary>
    /// Class to extend a DataGridTextColumn so that it's visibility is bindable
    /// </summary>
    /// <remarks>
    /// This code came from here:
    /// http://stackoverflow.com/questions/1045014/silverlight-how-to-bind-datagridcolumn-visibility/3974035#3974035
    /// </remarks>
    public class ExtendedDataGridTextColumn : DataGridTextColumn
    {
        private readonly Notifier _n;

        private Binding _visibilityBinding;
        public Binding VisibilityBinding
        {
            get { return _visibilityBinding; }
            set
            {
                _visibilityBinding = value;
                _n.SetBinding(Notifier.VisibilityProxyProperty, _visibilityBinding);
            }
        }

        public ExtendedDataGridTextColumn()
        {
            _n = new Notifier();
            _n.PropertyChanged += ToggleVisibility;
        }

        private void ToggleVisibility(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
                this.Visibility = _n.VisibilityProxy;
        }

        //Notifier class is just used to pass the property changed event back to the column container Dependency Object, leaving it as a private inner class for now
        public class Notifier : FrameworkElement, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public Visibility VisibilityProxy
            {
                get { return (Visibility)GetValue(VisibilityProxyProperty); }
                set { SetValue(VisibilityProxyProperty, value); }
            }

            public static readonly DependencyProperty VisibilityProxyProperty = DependencyProperty.Register("VisibilityProxy", typeof(Visibility), typeof(Notifier), new PropertyMetadata(VisibilityProxyChanged));

            private static void VisibilityProxyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var n = d as Notifier;
                if (n != null)
                {
                    n.PropertyChanged(n, new PropertyChangedEventArgs("Visibility"));
                }
            }
        }
    }
}
