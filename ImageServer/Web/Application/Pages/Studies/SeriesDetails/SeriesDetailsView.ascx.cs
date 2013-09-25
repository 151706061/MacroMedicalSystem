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

namespace Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// The series details view panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public partial class SeriesDetailsView : System.Web.UI.UserControl
    {
        #region Private members
        private Model.Series _series;

        #endregion Private members

        #region Public Properties

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series!=null)
            {
                IList<SeriesDetails> seriesDetails = new List<SeriesDetails>();
                seriesDetails.Add(SeriesDetailsAssembler.CreateSeriesDetails(Series));
                DetailsView1.DataSource = seriesDetails;
                DetailsView1.DataBind();
            }
        }

        #endregion Protected Methods

        protected void DetailsView1_DataBound(object sender, EventArgs e)
        {
            SeriesDetails series = DetailsView1.DataItem as SeriesDetails;
        }
    }
}