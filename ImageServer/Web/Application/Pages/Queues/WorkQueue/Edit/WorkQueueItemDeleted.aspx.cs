#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.ImageServer.Web.Application.Pages.Common;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public partial class WorkQueueItemDeleted : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ResetButton.Enabled = false;
            RescheduleToolbarButton.Enabled = false;
            DeleteButton.Enabled = false;
            ReprocessButton.Enabled = false;
        }
    }
}