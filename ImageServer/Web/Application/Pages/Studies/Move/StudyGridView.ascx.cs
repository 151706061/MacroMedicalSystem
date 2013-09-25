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
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Application.Pages.Studies.Move
{
    public partial class StudyGridView : System.Web.UI.UserControl
    {
        private IList<Study> _studyList = new List<Study>();
        private ServerPartition _partition;

        public IList<Study> StudyList
        {
            get { return _studyList; }
            set { _studyList = value;
                  StudyListControl.DataSource = _studyList;
            }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            StudyListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + row.RowIndex;
                    Study study = StudyList[index];

                    if (study != null)
                    {

                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        bool deleted = controller.IsScheduledForDelete(study);
                        if (deleted)
                            row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}