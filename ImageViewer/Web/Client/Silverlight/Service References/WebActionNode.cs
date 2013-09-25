#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace Macro.ImageViewer.Web.Client.Silverlight.AppServiceReference
{
    public partial class WebActionNode
    {
        /// <summary>
        /// Gets the desired visibility of the node
        /// </summary>
        public Visibility DesiredVisiblility
        {
            get
            {
                if (this is WebAction)
                {
                    var actionNode = (this as WebAction);
                    if (actionNode.Available && actionNode.Visible)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
                else
                {
                	if (this.Children != null)
                    {
                    	// Visible if any of the children is visible
                        if (this.Children.Any(n => n.DesiredVisiblility == Visibility.Visible))
                            return Visibility.Visible;

                        return Visibility.Collapsed;
                    }

                    return Visibility.Visible;
                }
            }
        }

    }
}
