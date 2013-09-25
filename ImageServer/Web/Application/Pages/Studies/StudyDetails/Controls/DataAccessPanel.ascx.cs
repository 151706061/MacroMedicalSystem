#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using System.Web.UI.WebControls;
using Macro.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DataAccessPanel : System.Web.UI.UserControl
    {
        #region Private members

        #endregion Private members

        #region Public properties

        /// <summary>
        /// The Study to get the DataAccess Groups for.
        /// </summary>
        public StudySummary Study { get; set; }

        public ServerPartition Partition { get; set; }

        #endregion Public properties

        #region Overridden Public Methods

        public override void DataBind()
        {
            if (Study != null)
            {
                if (Thread.CurrentPrincipal.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess))
                {
                    StudyDataAccessController controller = new StudyDataAccessController();

                    var dataAccessGroupList = CollectionUtils.Sort(controller.ListDataAccessGroupsForStudy(Study.TheStudyStorage.Key), Compare);
                    UpdatableDataAccessGroupsGridView.DataSource = dataAccessGroupList;


                    var tokenAccessGroupList = CollectionUtils.Sort(controller.ListAuthorityGroupsForStudyViaToken(Study.TheStudyStorage), Compare);
                    OtherGroupsWithAccessGridView.DataSource = tokenAccessGroupList;
                    OtherGroupsListing.Visible = tokenAccessGroupList.Count > 0;

                    LinkToOtherGroupListing.Visible = dataAccessGroupList.Count > 10;

                }
            }

            base.DataBind();
        }
        
        #endregion

        #region Protected methods

        
        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UpdatableDataAccessGroupsGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion Protected methods

        #region Helper Methods

        private static int Compare(AuthorityGroupStudyAccessInfo x, AuthorityGroupStudyAccessInfo y)
        {
            if (x.CanAccessToAllStudies && !y.CanAccessToAllStudies)
                return -1; // x first

            if (!x.CanAccessToAllStudies && y.CanAccessToAllStudies)
                return 1; // y first

            return x.Name.CompareTo(y.Name); // alphabetically
        }

        #endregion

    }

}