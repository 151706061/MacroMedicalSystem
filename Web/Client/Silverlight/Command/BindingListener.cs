#region License (non-CC)
// ****************************************************************************
// <copyright file="BindingListener.cs" company="Microsoft">
// (c) Copyright Microsoft Corporation  
// </copyright>
// ****************************************************************************
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
// ****************************************************************************
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Macro.Web.Client.Silverlight.Command
{
    /// <summary>
    /// Helper class for adding Bindings to non-FrameworkElements
    /// </summary>
    public class BindingListener
    {
        /// <summary>
        /// Delegate for when the binding listener has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ChangedHandler(object sender, BindingChangedEventArgs e);

        private static readonly List<DependencyPropertyListener> FreeListeners = new List<DependencyPropertyListener>();

        private readonly ChangedHandler _changedHandler;

        private Binding _binding;

        private DependencyPropertyListener _listener;

        private DependencyObject _target;

        private object _value;

        /// <summary>
        /// The context of the binding.
        /// </summary>
        public object Context { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context of the binding.</param>
        /// <param name="changedHandler">Callback whenever the value of this binding has changed.</param>
        public BindingListener(object context, ChangedHandler changedHandler)
        {
            Context = context;
            _changedHandler = changedHandler;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BindingListener()
        {
        }

        /// <summary>
        /// The Binding which is to be evaluated
        /// </summary>
        public Binding Binding
        {
            get { return _binding; }
            set
            {
                _binding = value;
                Attach();
            }
        }

        /// <summary>
        /// The element to be used as the context on which to evaluate the binding.
        /// </summary>
        public DependencyObject Element
        {
            get { return _target; }
            set
            {
                _target = value;
                Attach();
            }
        }

        /// <summary>
        /// The current value of this binding.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                if (_listener != null)
                {
                    _listener.SetValue(value);
                }
            }
        }

        private void Attach()
        {
            Detach();

            if (_target != null
                && _binding != null)
            {
                _listener = GetListener();
                _listener.Attach(_target, _binding);
            }
        }

        private void Detach()
        {
            if (_listener != null)
            {
                ReturnListener();
            }
        }

        private DependencyPropertyListener GetListener()
        {
            DependencyPropertyListener listener;

            if (FreeListeners.Count != 0)
            {
                listener = FreeListeners[FreeListeners.Count - 1];
                FreeListeners.RemoveAt(FreeListeners.Count - 1);

                return listener;
            }
            listener = new DependencyPropertyListener();

            listener.Changed += HandleValueChanged;

            return listener;
        }

        private void HandleValueChanged(object sender, BindingChangedEventArgs e)
        {
            _value = e.EventArgs.NewValue;

            if (_changedHandler != null)
            {
                _changedHandler(this, e);
            }
        }

        private void ReturnListener()
        {
            _listener.Changed -= HandleValueChanged;
            _listener.Detach();

            FreeListeners.Add(_listener);

            _listener = null;
        }

        private class DependencyPropertyListener
        {
            private readonly DependencyProperty _property;

            private static int _index;

            private DependencyObject _target;

            public DependencyPropertyListener()
            {
                _property = DependencyProperty.RegisterAttached("DependencyPropertyListener" + _index++,
                                                                    typeof (object),
                                                                    typeof (DependencyPropertyListener),
                                                                    new PropertyMetadata(null, HandleValueChanged));
            }

            public event EventHandler<BindingChangedEventArgs> Changed;

            public void Attach(DependencyObject element, Binding binding)
            {
                if (_target != null)
                {
                    throw new Exception("Cannot attach an already attached listener");
                }

                _target = element;

                BindingOperations.SetBinding(_target, _property, binding);
            }

            public void Detach()
            {
                _target.ClearValue(_property);
                _target = null;
            }

            public void SetValue(object value)
            {
                _target.SetValue(_property, value);
            }

            private void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (Changed != null)
                {
                    Changed(this, new BindingChangedEventArgs(e));
                }
            }
        }
    }

    /// <summary>
    /// Event args for when binding values change.
    /// </summary>
    public class BindingChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e"></param>
        public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            EventArgs = e;
        }

        /// <summary>
        /// Original event args.
        /// </summary>
        public DependencyPropertyChangedEventArgs EventArgs { get; private set; }
    }
}
