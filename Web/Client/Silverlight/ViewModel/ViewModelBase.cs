#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
// 
// For information about the licensing and copyright of this software please
// contact Macro, Inc. at info@Macro.ca

// Note, most of this code was taken from the MVVM Lite Toolkit, which is licensed here:   http://www.galasoft.ch/license_MIT.txt
#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;

namespace Macro.Web.Client.Silverlight.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Design Mode
        private static bool? _isInDesignMode;


        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
       public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }

     
        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = DesignerProperties.IsInDesignTool;

                }

                return _isInDesignMode.Value;
            }
        }

        #endregion

        #region Dependency Property Related

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Provides access to the PropertyChanged event handler to derived classes.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// <para>This method is only active in DEBUG mode.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        //[DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();
            if (myType.GetProperty(propertyName) == null)
            {
                throw new ArgumentException(@"Property not found", propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                return;
            }

            var handler = PropertyChanged;

            if (handler != null)
            {
                var body = propertyExpression.Body as MemberExpression;
                if (body != null)
                {
                    var expression = body.Expression as ConstantExpression;
                    if (expression != null)
                        handler(expression.Value, new PropertyChangedEventArgs(body.Member.Name));
                }
            }
        }

        #endregion
    }
}
