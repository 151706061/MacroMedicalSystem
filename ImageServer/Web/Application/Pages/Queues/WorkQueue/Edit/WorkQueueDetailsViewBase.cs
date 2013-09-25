#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Base class for controls that display the Work Queue Item details view.
    /// </summary>
    public class WorkQueueDetailsViewBase : UserControl
    {
        /// <summary>
        /// Sets or gets the list of studies whose information are displayed
        /// </summary>
        public Model.WorkQueue WorkQueue { get; set; }

        /// <summary>
        /// Sets or gets the width of control
        /// </summary>
        public virtual Unit Width { get; set; }
    }
}