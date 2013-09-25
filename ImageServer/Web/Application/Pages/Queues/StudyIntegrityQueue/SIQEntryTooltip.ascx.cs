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
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public partial class SIQEntryTooltip : System.Web.UI.UserControl
    {
        public StudyIntegrityQueueSummary SIQItem { get; set; }

        public string CssClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Container.CssClass = CssClass;
        }

        public override void DataBind()
        {
            base.DataBind();

            if (SIQItem != null)
            {
                FilesystemPath.Text = SIQItem.GetFilesystemStudyPath();
                ReconcilePath.Text = SIQItem.GetReconcileFolderPath();

                if (!SIQItem.CanReconcile)
                {
                    Note.Text = SIQItem.GetNotReconcilableReason();
                }

                if (SIQItem.StudyExists)
                {
                    StudyLink.NavigateUrl = HtmlUtility.ResolveStudyDetailsUrl(Page,
                                                SIQItem.StudySummary.ThePartition.AeTitle, SIQItem.StudyInstanceUid);

                    
                    StudyLink.Text = string.Format("{0}, {1}",
                                        SIQItem.StudySummary.AccessionNumber, 
                                        SIQItem.StudySummary.StudyDescription);

                }

            }

            
        }
    }
}