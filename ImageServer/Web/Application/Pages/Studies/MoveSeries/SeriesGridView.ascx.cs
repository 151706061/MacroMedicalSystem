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

namespace Macro.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    public partial class SeriesGridView : System.Web.UI.UserControl
    {
        private IList<Series> _seriesList = new List<Series>();
        private ServerPartition _partition;
        private Study _study;

        public IList<Series> SeriesList
        {
            get { return _seriesList; }
            set { _seriesList = value;}
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            SeriesListControl.DataSource = _seriesList;
            SeriesListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in SeriesListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = SeriesListControl.PageIndex * SeriesListControl.PageSize + row.RowIndex;
                    Series series = SeriesList[index];

                    if (series != null)
                    {

                        row.Attributes.Add("instanceuid", series.SeriesInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}
