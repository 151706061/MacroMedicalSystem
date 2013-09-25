#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails;
using Macro.ImageServer.Web.Common.Data;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.WorkQueueGridView.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.WorkQueueGridView",
                       ResourcePath = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.WorkQueueGridView.js")]
    
    /// <summary>
    /// list panel within the <see cref="StudyDetailsPanel"/> that contains all of the work queue
    /// entries for this study.
    /// </summary>
    public partial class WorkQueueGridView : ScriptUserControl
    {
        #region Private members

        private Study _study = null;

    	#endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }      

        public Web.Common.WebControls.UI.GridView WorkQueueListControl
        {
            get { return StudyWorkQueueGridView; }
        }

        #endregion Public properties

        #region Constructors

        public WorkQueueGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        protected void StudyWorkQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void StudyWorkQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyWorkQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion Protected methods

        #region Public methods

        public override void DataBind()
        {
            if (Study != null)
            {
                StudyController controller = new StudyController();
                StudyWorkQueueGridView.DataSource = controller.GetWorkQueueItems(Study);
            }

            base.DataBind();
        }

        #endregion Public methods

    }
}