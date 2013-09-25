#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// The view control to display detail information of a <see cref="WorkQueue"/> item of type 'Web-Move-Study' inside the <see cref="WorkQueueItemDetailsPanel"/>
    /// </summary>
    public partial class TierMigrationWorkQueueDetailsView : WorkQueueDetailsViewBase
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the width of work queue details view panel
        /// </summary>
        public override Unit Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                TierMigrationStudyDetailsView.Width = value;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        #endregion Protected Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                var detailsList = new List<WorkQueueDetails>();
                detailsList.Add(WorkQueueDetailsAssembler.CreateWorkQueueDetail(WorkQueue));
                TierMigrationStudyDetailsView.DataSource = detailsList;
            }
            else
            {
                TierMigrationStudyDetailsView.DataSource = null;
            }

            base.DataBind();
        }

        #endregion Public Methods
    }
}