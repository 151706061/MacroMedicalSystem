#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    [ToolboxData("<{0}:TextBox runat=server></{0}:TextBox>")]
    [Themeable(true)]
    public class TextBox : System.Web.UI.WebControls.TextBox
    {
        private bool _readOnly;

        #region Public Properties

        /// <summary>
        /// Sets or gets the ReadOnly property. Fixes a bug that prevents data
        /// changed in a textbox programatically from posting back to the server.
        /// </summary>
        public new bool ReadOnly
        {
            get
            {
                return _readOnly;               
            }

            set
            {
                _readOnly = value;
                if (_readOnly)
                    Attributes.Add("readonly", "readonly");
                else
                    Attributes.Remove("readonly"); 
            }
        }

        #endregion Public Properties

    }
}
