#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    public partial class ThrottleSettingsTab : UserControl
    {
        private const short Unlimited = -1;

        public short MaxConnections
        {
            get
            {
                return String.IsNullOrEmpty(MaxConnectionTextBox.Text) ? Unlimited : short.Parse(MaxConnectionTextBox.Text);
            }
            set { MaxConnectionTextBox.Text = value == Unlimited ? "" : value.ToString(); }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UnlimitedCheckBox.Checked = MaxConnections == Unlimited;
            LimitedCheckBox.Checked = !UnlimitedCheckBox.Checked;
        }
    }
}