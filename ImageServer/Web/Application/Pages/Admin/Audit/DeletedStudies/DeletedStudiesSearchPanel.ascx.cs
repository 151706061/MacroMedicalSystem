#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Macro.Common.Utilities;
using Macro.ImageServer.Enterprise.Authentication;
using Resources;
using Macro.ImageServer.Web.Application.Helpers;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Data.Model;
using Macro.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js",
        "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// Represents an event fired when the View Details button is clicked
    /// </summary>
    public class DeletedStudyViewDetailsClickedEventArgs : EventArgs
    {
        public DeletedStudyInfo DeletedStudyInfo { get; set; }
    }

    /// <summary>
    /// Represents an event fired when the Delete button is clicked
    /// </summary>
    public class DeletedStudyDeleteClickedEventArgs : EventArgs
    {
        public DeletedStudyInfo SelectedItem { get; set; }
    }

    [ClientScriptResource(
        ComponentType =
            "Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel",
        ResourcePath =
            "Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js")]
    public partial class DeletedStudiesSearchPanel : AJAXScriptControl
    {
        #region Private Fields

        private EventHandler<DeletedStudyDeleteClickedEventArgs> _deleteClicked;
        private EventHandler<DeletedStudyViewDetailsClickedEventArgs> _viewDetailsClicked;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when user clicks on the View Details button
        /// </summary>
        public event EventHandler<DeletedStudyViewDetailsClickedEventArgs> ViewDetailsClicked
        {
            add { _viewDetailsClicked += value; }
            remove { _viewDetailsClicked -= value; }
        }

        /// <summary>
        /// Occurs when user clicks on the Delete button
        /// </summary>
        public event EventHandler<DeletedStudyDeleteClickedEventArgs> DeleteClicked
        {
            add { _deleteClicked += value; }
            remove { _deleteClicked -= value; }
        }

        #endregion

        #region Ajax Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ListClientID")]
        public string ListClientID
        {
            get { return SearchResultGridView1.GridViewControl.ClientID; }
        }

        #endregion

        #region Private Methods

        private void DataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            var dataSource = e.ObjectInstance as DeletedStudyDataSource;
            if (dataSource != null)
            {
                dataSource.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.Text);
                dataSource.PatientsName = SearchHelper.NameWildCard(PatientName.Text);
                dataSource.PatientId = SearchHelper.TrailingWildCard(PatientId.Text);
                dataSource.StudyDescription = SearchHelper.LeadingAndTrailingWildCard(StudyDescription.Text);
                dataSource.DeletedBy = DeletedBy.Text;
                if (!String.IsNullOrEmpty(StudyDate.Text))
                {
                    DateTime value;
                    if (DateTime.TryParseExact(StudyDate.Text, StudyDateCalendarExtender.Format,
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out value))
                    {
                        dataSource.StudyDate = value;
                    }
                }
            }
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearStudyDateButton.OnClientClick = ScriptHelper.ClearDate(StudyDate.ClientID,
                                                                        StudyDateCalendarExtender.ClientID);

            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem, Labels.GridPagerQueueMultipleItems,
                                             SearchResultGridView1.GridViewControl,
                                             () => SearchResultGridView1.ResultCount,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            SearchResultGridView1.Pager = GridPagerTop;
            GridPagerTop.Reset();

            SearchResultGridView1.DataSourceContainer.ObjectCreated += DataSource_ObjectCreated;

            DeleteButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.Delete;
            ViewStudyDetailsButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.View;
        }

        #endregion

        #region Protected Methods

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            Refresh();
        }

        protected void ViewDetailsButtonClicked(object sender, ImageClickEventArgs e)
        {
            var args = new DeletedStudyViewDetailsClickedEventArgs
                           {
                               DeletedStudyInfo = SearchResultGridView1.SelectedItem
                           };
            EventsHelper.Fire(_viewDetailsClicked, this, args);
        }

        protected void DeleteButtonClicked(object sender, ImageClickEventArgs e)
        {
            var args = new DeletedStudyDeleteClickedEventArgs
                           {
                               SelectedItem = SearchResultGridView1.SelectedItem
                           };
            EventsHelper.Fire(_deleteClicked, this, args);
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            SearchResultGridView1.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        #endregion
    }
}