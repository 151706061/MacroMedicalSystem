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
using System.Web.UI.WebControls;
using Macro.Dicom;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class ArchivePanel : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;
        private Study _study;
        private IList<ArchiveStudyStorage> _storage;

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

        public override void DataBind()
        {
            StudyController studyController = new StudyController();
            ArchiveQueueGridView.DataSource = studyController.GetArchiveQueueItems(_study);
            _storage = studyController.GetArchiveStudyStorage(_study);
            ArchiveStudyStorageGridView.DataSource = _storage;
            base.DataBind();
        }

        protected void ArchiveQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void ArchiveQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ArchiveQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void ArchiveStudyStorageGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                   int index = ArchiveStudyStorageGridView.PageIndex * ArchiveStudyStorageGridView.PageSize + e.Row.RowIndex;
                    ArchiveStudyStorage storage = _storage[index];

                    Label xmlLabel = e.Row.FindControl("XmlText") as Label;
                    if (xmlLabel != null && storage.ArchiveXml != null)
                    {
                        xmlLabel.Text = XmlUtils.GetXmlDocumentAsString(storage.ArchiveXml, true);    
                    }

                    Label stsLabel = e.Row.FindControl("ServerTranseferSyntax") as Label;
                    if (stsLabel != null && storage.ServerTransferSyntaxKey != null)
                    {
                        ServerTransferSyntaxAdaptor adaptor = new ServerTransferSyntaxAdaptor();
                        ServerTransferSyntax sts = adaptor.Get(storage.ServerTransferSyntaxKey);

                        if (sts != null)
                        {
                            stsLabel.Text = sts.Description;
                        }
                    }
                }
            }
        }       
}