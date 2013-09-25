#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using Macro.Common.Utilities;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using Macro.ImageServer.Web.Common.Data.DataSource;
using AuthorityTokens=Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public class StudyDetailsPanelDeleteStudyClickEventArgs : EventArgs
    {}

    public class StudyDetailsPanelEditStudyClickEventArgs : EventArgs
    { }

    public class StudyDetailsPanelReprocessStudyClickEventArgs : EventArgs
    { }

    /// <summary>
    /// Main panel within the <see cref="Default"/>
    /// </summary>
    public partial class StudyDetailsPanel : UserControl
    {
        #region Private Members
        private StudySummary _study;
        private EventHandler<StudyDetailsPanelDeleteStudyClickEventArgs> _deleteStudyClickedHandler;
        private EventHandler<StudyDetailsPanelEditStudyClickEventArgs> _editStudyClickedHandler;
        private EventHandler<StudyDetailsPanelReprocessStudyClickEventArgs> _reprocessStudyClickedHandler;
       
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public StudySummary Study
        {
            get { return _study; }
            set { _study = value; }
        }

        #endregion Public Properties

        #region Events
        public event EventHandler<StudyDetailsPanelDeleteStudyClickEventArgs> DeleteStudyClicked
        {
            add { _deleteStudyClickedHandler += value; }
            remove { _deleteStudyClickedHandler -= value; }
        }
        public event EventHandler<StudyDetailsPanelEditStudyClickEventArgs> EditStudyClicked
        {
            add { _editStudyClickedHandler += value; }
            remove { _editStudyClickedHandler -= value; }
        }

        public event EventHandler<StudyDetailsPanelReprocessStudyClickEventArgs> ReprocessStudyClicked
        {
            add { _reprocessStudyClickedHandler += value; }
            remove { _reprocessStudyClickedHandler -= value; }
        }

        public StudyDetailsTabs StudyDetailsTabsControl
        {
            get { return StudyDetailsTabs; }
        }
        #endregion

        #region Protected Methods

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                PatientSummaryPanel.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(Study.TheStudy);

                StudyDetailsTabs.Partition = Study.ThePartition;
                StudyDetailsTabs.Study = Study;
                StudyStateAlertPanel.Study = Study;
            } 
            
            base.DataBind();
        }

        public void Page_Load(Object sender, EventArgs e)
        {
            DeleteStudyButton.Roles = AuthorityTokens.Study.Delete;
            EditStudyButton.Roles = AuthorityTokens.Study.Edit;
            ReprocessStudyButton.Roles = AuthorityTokens.Study.Reprocess;
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI(); 
            
            base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            if (Study!=null)
            {
                string reason;
                DeleteStudyButton.Enabled = Study.CanScheduleDelete(out reason);
                if (!DeleteStudyButton.Enabled)
                    DeleteStudyButton.ToolTip = reason;

                EditStudyButton.Enabled = Study.CanScheduleEdit(out reason);
                if (!EditStudyButton.Enabled)
                    EditStudyButton.ToolTip = reason;

                ReprocessStudyButton.Enabled = Study.CanReprocess(out reason);
                if (!ReprocessStudyButton.Enabled)
                    ReprocessStudyButton.ToolTip = reason;

                
            }

            SearchUpdatePanel.Update();// force update
        }

        protected void DeleteStudyButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_deleteStudyClickedHandler, this, new StudyDetailsPanelDeleteStudyClickEventArgs());
        }

        protected void EditStudyButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_editStudyClickedHandler, this, new StudyDetailsPanelEditStudyClickEventArgs());
        }

        #endregion Protected Methods

        protected void ReprocessButton_Click(object sender, ImageClickEventArgs e)
        {
            EventsHelper.Fire(_reprocessStudyClickedHandler, this, new StudyDetailsPanelReprocessStudyClickEventArgs());
        }

    }
}