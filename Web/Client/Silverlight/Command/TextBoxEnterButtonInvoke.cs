#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
// 
// For information about the licensing and copyright of this software please
// contact Macro, Inc. at info@Macro.ca

#endregion

// Modified from this post:  http://geekswithblogs.net/SilverBlog/archive/2009/09/21/behaviors-textbox-enter-button-invoke-targetedtriggeraction.aspx

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Macro.Web.Client.Silverlight.Command
{
    public class TextBoxEnterButtonInvoke : TargetedTriggerAction<ButtonBase>
    {
        /// <summary>
        /// Gets or sets the peer.
        /// </summary>
        /// <value>The peer.</value>
        private AutomationPeer Peer { get; set; }

        /// <summary>
        /// Gets or sets the target button
        /// </summary>
        private ButtonBase TargetedButton { get; set; }

        /// <summary>
        /// Called after the TargetedTriggerAction is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            TargetedButton = Target;
            if (null == TargetedButton)
            {
                return;
            }

            // set peer
            Peer = FrameworkElementAutomationPeer.FromElement(TargetedButton) ??
                   FrameworkElementAutomationPeer.CreatePeerForElement(TargetedButton);
        }

        /// <summary>
        /// Called after targeted Button change.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the new targeted Button.</remarks>
        protected override void OnTargetChanged(ButtonBase oldTarget, ButtonBase newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            TargetedButton = newTarget;
            if (null == TargetedButton)
            {
                return;
            }

            // set peer
            Peer = FrameworkElementAutomationPeer.FromElement(TargetedButton) ??
                   FrameworkElementAutomationPeer.CreatePeerForElement(TargetedButton);
        }

        /// <summary>
        /// Invokes the targeted Button when Enter key is pressed inside TextBox.
        /// </summary>
        /// <param name="parameter">KeyEventArgs with Enter key</param>
        protected override void Invoke(object parameter)
        {
            var keyEventArgs = parameter as KeyEventArgs;
            if (null != keyEventArgs && keyEventArgs.Key == Key.Enter)
            {
                if (null != Peer)
                {
                    // Force the TextBox to be rebound here.  The KeyUp event is triggered
                    // Before the textBox's value is rebound to the ViewModel, so this forces
                    // the value to be rebound before the enter is processed.
                    var tb = AssociatedObject as TextBox;
                    if (tb != null)
                    {
                        var bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);

                        if (bindingExpression != null)
                        {
                            bindingExpression.UpdateSource();
                        }
                    }

                    var invokeProvider = Peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProvider != null) invokeProvider.Invoke();
                }
            }
        }
    }
}
