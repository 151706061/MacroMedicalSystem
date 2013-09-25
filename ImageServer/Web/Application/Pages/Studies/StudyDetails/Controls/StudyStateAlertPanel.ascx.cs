#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyStateAlertPanel : System.Web.UI.UserControl
    {
        private StudySummary _studySummary;

        /// <summary>
        /// Message displayed
        /// </summary>
        protected Label Message;

        public StudySummary Study
        {
            get { return _studySummary; }
            set { _studySummary = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Visible = false;
            Message.Text = String.Empty;
            base.OnInit(e);
        }

        public override void DataBind()
        {
            if (_studySummary!=null)
            {
                if (_studySummary.IsProcessing)
                {
                    ShowAlert(SR.StudyBeingProcessed);
                }
                else if (_studySummary.IsLocked)
                {
                    ShowAlert(ServerEnumDescription.GetLocalizedLongDescription(_studySummary.QueueStudyStateEnum));
                }
                else if (_studySummary.IsNearline)
                {
                    ShowAlert(SR.StudyIsNearline);
                }
                else if (_studySummary.IsReconcileRequired)
                {
                    ShowAlert(SR.StudyRequiresReconcilie);
                }
                else if (_studySummary.HasPendingExternalEdit)
                {
                    ShowAlert(SR.StudyScheduledForEdit);
                }
            }

            base.DataBind();
        }

        private void ShowAlert(string message)
        {
            Message.Text = message;
            Visible = true;
        }
    }
}