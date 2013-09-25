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
using Macro.Common.Statistics;
using Macro.Dicom;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class StudyStorageView : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;
        private Study _study;

        #endregion Private members
        
        #region Public Properties

        /// <summary>
        /// Sets or gets the Study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public override void DataBind()
        {           
            StudyStorageViewControl.DataSource = null;
            StudyController studyController = new StudyController();
            StudyStorageViewControl.DataSource = studyController.GetStudyStorageLocation(_study);    
            
            base.DataBind();
        }

        protected void StudyStorageView_DataBound(object sender, EventArgs e)
        {
            StudyStorageLocation ssl = (StudyStorageViewControl.DataItem) as StudyStorageLocation;
            if (ssl != null)
            {
                Label statusLabel = StudyStorageViewControl.FindControl("Status") as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text =  ServerEnumDescription.GetLocalizedDescription(ssl.StudyStatusEnum);
                }
				Label queueStateLable = StudyStorageViewControl.FindControl("QueueState") as Label;
				if (queueStateLable != null)
				{
				    queueStateLable.Text = ServerEnumDescription.GetLocalizedDescription(ssl.QueueStudyStateEnum);
				}
				Label studyFolder = StudyStorageViewControl.FindControl("StudyFolder") as Label;
                if (studyFolder != null)
                {
                    studyFolder.Text = ssl.GetStudyPath();
                }
                Label transferSyntaxUID = StudyStorageViewControl.FindControl("TransferSyntaxUID") as Label;
                if (transferSyntaxUID != null)
                {
                    transferSyntaxUID.Text = TransferSyntax.GetTransferSyntax(ssl.TransferSyntaxUid).Name;
                }
				Label tier = StudyStorageViewControl.FindControl("Tier") as Label;
				if (tier != null)
				{
					tier.Text =  ServerEnumDescription.GetLocalizedDescription(ssl.FilesystemTierEnum);
				}

                Label studySize = StudyStorageViewControl.FindControl("StudySize") as Label;
                if (studySize!=null)
                {
                    ulong sizeInBytes = (ulong) (ssl.Study.StudySizeInKB*1024);
                    studySize.Text = ByteCountFormatter.Format(sizeInBytes);
                }
			}
        }

        #endregion Protected Methods

    }
}