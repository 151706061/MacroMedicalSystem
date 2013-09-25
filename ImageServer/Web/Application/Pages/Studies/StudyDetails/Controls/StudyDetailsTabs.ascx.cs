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
using System.Threading;
using System.Web.UI;
using AjaxControlToolkit;
using Macro.Common.Utilities;
using Macro.ImageServer.Enterprise.Authentication;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

[assembly: WebResource("Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.StudyDetailsTabs.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [ClientScriptResource(ComponentType = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs",
                          ResourcePath = "Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.StudyDetailsTabs.js")]    
    public partial class StudyDetailsTabs : ScriptUserControl
    {
        private EventHandler<StudyDetailsTabsDeleteSeriesClickEventArgs> _deleteSeriesClickedHandler;

        public class StudyDetailsTabsDeleteSeriesClickEventArgs : EventArgs
        { }

        #region Private Members

        [Serializable]
        private class DeleteRequest
        {
            private Study _study;
            private ServerPartition _partition;
            private IList<Series> _series;
            private string _reason;

            public Study SelectedStudy
            {
                get { return _study; }
                set { _study = value; }
            }

            public ServerPartition Partition
            {
                get { return _partition; }
                set { _partition = value; }
            }

            public IList<Series> Series
            {
                get { return _series; }
                set { _series = value; }
            }

            public string Reason
            {
                get { return _reason; }
                set { _reason = value; }
            }
        }

        private StudySummary _study;
        private ServerPartition _partition;
        
        #endregion Private Members

        #region Events
        public event EventHandler<StudyDetailsTabsDeleteSeriesClickEventArgs> DeleteSeriesClicked
        {
            add { _deleteSeriesClickedHandler += value; }
            remove { _deleteSeriesClickedHandler -= value; }
        }
        #endregion

        #region Public Members

        [ExtenderControlProperty]
        [ClientPropertyName("ViewSeriesButtonClientID")]
        public string ViewSeriesButtonClientID
        {
            get { return ViewSeriesButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SeriesListClientID")]
        public string SeriesListClientID
        {
            get { return SeriesGridView.SeriesListControl.ClientID; }
        }
        
        [ExtenderControlProperty]       
        [ClientPropertyName("OpenSeriesPageUrl")]
        public string OpenSeriesPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.SeriesDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SendSeriesPageUrl")]
        public string SendSeriesPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.MoveSeriesPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("MoveSeriesButtonClientID")]
        public string MoveSeriesButtonClientID
        {
            get { return MoveSeriesButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteSeriesButtonClientID")]
        public string DeleteSeriesButtonClientID
        {
            get { return DeleteSeriesButton.ClientID; }
        }

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get { return _study; }
            set 
            { 
                _study = value;
                UpdateAuthorityGroupDialog.Study = value;
            }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public IList<Series> SelectedSeries
        {
            get { return SeriesGridView.SelectedItems; }
        }

        #endregion Public Members

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteConfirmation.Confirmed += delegate
                                                {
                                                  ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                                                                      "alertScript", "self.close();",
                                                                                      true);

                                                  DeleteRequest deleteRequest = DeleteConfirmation.Data as DeleteRequest;

                                                  StudyController studyController = new StudyController();
                                                  studyController.DeleteSeries(deleteRequest.SelectedStudy, deleteRequest.Series, deleteRequest.Reason);
                                              };

            DeleteSeriesButton.Roles = AuthorityTokens.Study.Delete;
            MoveSeriesButton.Roles = AuthorityTokens.Study.Move;

            UpdateAuthorityGroupDialog.AuthorityGroupsEdited += UpdateAuthorityGroupDialog_AuthorityGroupsEdited;

            //DataAccessTabPanel.Visible = Thread.CurrentPrincipal.IsInRole(Macro.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup);
            DataAccessTabPanel.Enabled = Thread.CurrentPrincipal.IsInRole(Macro.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess);
            
        }

        private void UpdateAuthorityGroupDialog_AuthorityGroupsEdited(object sender, EventArgs e)
        {
            DataAccessPanel.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            string reason;
            int[] selectedSeriesIndices = SeriesGridView.SeriesListControl.SelectedIndices;
            ViewSeriesButton.Enabled = selectedSeriesIndices != null && selectedSeriesIndices.Length > 0;
            MoveSeriesButton.Enabled = selectedSeriesIndices != null && selectedSeriesIndices.Length > 0;
            DeleteSeriesButton.Enabled = selectedSeriesIndices != null && selectedSeriesIndices.Length > 0 && _study.CanScheduleSeriesDelete(out reason);

            base.OnPreRender(e);
        }

        protected void DeleteSeriesButton_Click(object sender, ImageClickEventArgs e)
        {
            EventsHelper.Fire(_deleteSeriesClickedHandler, this, new StudyDetailsTabsDeleteSeriesClickEventArgs());
/*
            DeleteConfirmation.Message = DialogHelper.createConfirmationMessage(string.Format(App_GlobalResources.SR.DeleteSeriesMessage));

            IList<Series> items = SeriesGridView.SelectedItems;

            DeleteConfirmation.Message += DialogHelper.createSeriesTable(items);
            DeleteConfirmation.Message += "<div style='text-align: right; padding-top: 5px; font-weight: bold; color: #336699'>Reason: &nbsp;<input type='Text' name='ReasonText' width='450'></div>";

            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.OKCANCEL;

            // Create the delete request
            DeleteRequest deleteData = new DeleteRequest();
            deleteData.SelectedStudy = SeriesGridView.Study.TheStudy;
            deleteData.Series = SeriesGridView.SelectedItems;
            deleteData.Partition = SeriesGridView.Partition;
            deleteData.Reason = "";
            DeleteConfirmation.Data = deleteData;
            
            DeleteConfirmation.Show();
 */
        }

        protected void UpdateAuthorityGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UpdateAuthorityGroupDialog.Study = Study;
            UpdateAuthorityGroupDialog.Show();
        }

        #endregion Protected Methods

        public StudyDetailsTabs()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                StudyDetailsView.Studies.Add(Study);

                SeriesGridView.Partition = Partition;
                SeriesGridView.Study = Study;

                WorkQueueGridView.Study = Study.TheStudy;
                FSQueueGridView.Study = Study.TheStudy;
                StudyStorageView.Study = Study.TheStudy;
                ArchivePanel.Study = Study.TheStudy;
                HistoryPanel.TheStudySummary = Study;
                StudyIntegrityQueueGridView.Study = Study;
                DataAccessPanel.Study = Study;
                DataAccessPanel.Partition = Partition;
            }

            base.DataBind();
        }
    }
}