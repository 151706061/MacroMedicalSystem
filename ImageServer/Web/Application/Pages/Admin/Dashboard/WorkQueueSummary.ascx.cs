#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.Model;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Dashboard
{
    public partial class WorkQueueSummary : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WorkQueueController controller = new WorkQueueController();
            WorkQueueDataList.DataSource = controller.GetWorkQueueOverview();
            DataBind();
        }

        protected void Item_DataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                WorkQueueInfo info = e.Item.DataItem as WorkQueueInfo;
                LinkButton button = e.Item.FindControl("WorkQueueLink") as LinkButton;
                button.PostBackUrl = ImageServerConstants.PageURLs.WorkQueuePage + "?ProcessorID=" + info.Server;
            }
        }
    }
}