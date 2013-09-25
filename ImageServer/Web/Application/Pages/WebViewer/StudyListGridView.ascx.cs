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
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;
using GridView = Macro.ImageServer.Web.Common.WebControls.UI.GridView;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : UserControl
    {
        #region Private members
        // list of studies to display
        private IList<StudySummary> _studies;
        private Unit _height;
        #endregion Private members

        #region Public properties

        /// <summary>
        /// Gets/Sets the list of studies rendered on the screen.
        /// </summary>
        public IList<StudySummary> Studies
        {
            get
            {
                return _studies;
            }
            set
            {
                _studies = value;
                StudyListControl.DataSource = _studies; 
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get
            {
            	if (ContainerTable != null)
                    return ContainerTable.Height;
            	return _height;
            }
        	set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        public GridView TheGrid
        {
            get { return StudyListControl;}
        }

        public GridPager Pager
        {
            get;
            set;
        }

        public void Refresh()
        {
            TheGrid.DataBind();
        }

        #endregion
        
        #region protected methods
     
        protected override void OnInit(EventArgs e)
        {
            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Studies == null)
            {
                return;
            }

            new ServerPartitionDataAdapter();

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    StudySummary study = Studies[row.RowIndex];

                    if (study != null)
                    {                    
                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", study.ThePartition.AeTitle);
                        row.Attributes.Add("canviewimages", "true");
                    }  
                }
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    message.Message = "No studies found matching the provided criteria.";
                }
            } 
        }

        #endregion

    }

}
