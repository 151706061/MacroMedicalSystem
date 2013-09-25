#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Utilities;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.FileSystemQueueGridView.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.FileSystemQueueGridView",
                       ResourcePath = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.FileSystemQueueGridView.js")]
    
    /// <summary>
    /// list panel within the <see cref="StudyDetailsPanel"/> that contains all of the File System queue
    /// entries for this study.
    /// </summary>
    public partial class FileSystemQueueGridView : ScriptUserControl
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

        public Web.Common.WebControls.UI.GridView FSQueueListControl
        {
            get { return FSQueueGridView; }
        }

        #endregion Public properties

        #region Constructors

        public FileSystemQueueGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        protected void FSQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void FSQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FSQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void FSQueueGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            FilesystemQueue fsq = e.Row.DataItem as FilesystemQueue;
            if (fsq != null)
            {
                Label xmlText = e.Row.FindControl("XmlText") as Label;
                if (xmlText != null && fsq.QueueXml != null)
                {
                    xmlText.Text = XmlUtils.GetXmlDocumentAsString(fsq.QueueXml, true);
                }
            }
        }

        #endregion Protected methods

        #region Public methods

        public override void DataBind()
        {
            if (Study != null)
            {
                StudyController controller = new StudyController();
                FSQueueGridView.DataSource = controller.GetFileSystemQueueItems(Study);
            }

            base.DataBind();
        }

        #endregion Public methods

    }
}