#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Controls
{
    public partial class UsersGuideLink : System.Web.UI.UserControl
    {
        public string TopicID { get; set; }
        public string SubTopicID { get; set; }
        public string Target { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Link.NavigateUrl = UsersGuideLinkHelper.GetUrlTo(TopicID, SubTopicID);
            Link.Target = Target;
        }
    }
}
