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
using System.Web.UI.WebControls;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : GridViewPanel
    {
		#region Delegates
		public delegate void StudyDataSourceCreated(StudyDataSource theSource);
		public event StudyDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudySummary> _studies;
        private ServerPartition _partition;
        private Unit _height;
    	private StudyDataSource _dataSource;
        private Dictionary<string, StudySummary> _studyDictionary = new Dictionary<string, StudySummary>();
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyDataSource();

					_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
											{
												Studies = newlist;
											};
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);
					_dataSource.SelectCount();
				}
				if (_dataSource.ResultCount == 0)
				{
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);

					_dataSource.SelectCount();
				}
				return _dataSource.ResultCount;
			}
		}

        public ServerPartition Partition
        {
            set { _partition = value; }
            get { return _partition; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<StudySummary> SelectedStudies
        {
            get
            {
                if(!StudyListControl.IsDataBound) StudyListControl.DataBind();

                if (_studyDictionary.Count == 0)
                    return null;

                string[] rows = StudyListControl.SelectedDataKeys;
                if (rows == null || rows.Length == 0)
                    return null;

				IList<StudySummary> studies = new List<StudySummary>();
                for(int i=0; i<rows.Length; i++)
                {
                    studies.Add(_studyDictionary[rows[i]]);
                }

                return studies;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
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
                foreach(StudySummary study in _studies)
                {
                    _studyDictionary.Add(study.Key.ToString(), study);
                }
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

        public void SetDataSource()
        {
            StudyListControl.DataSource = StudyDataSourceObject;
        }

        public bool IsDataSourceSet()
        {
            return StudyListControl.DataSource != null;
        }

        public bool DisplayVetTags()
        {
            return Thread.CurrentPrincipal.IsInRole(Enterprise.Authentication.AuthorityTokens.Study.VetTags);
        }

        #endregion
  
  
        #region protected methods
     
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = StudyListControl;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            if (DisplayVetTags())
            {
                foreach (DataControlField o in StudyListControl.Columns)
                {
                    var t = o as TemplateField;
                    if (t != null)
                    {
                        // TODO: This is a bit of a Hack, need something better for this in the future.
                        if (t.Visible == false)
                            t.Visible = true;
                        continue;
                    }

                    var f = o as BoundField;
                    if (f == null) continue;

                    if (f.DataField.Equals("ResponsiblePerson"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsibleOrganization"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Species"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Breed"))
                        f.Visible = true;
                }
            } 


            if(IsPostBack || Page.IsAsync)
            {
                StudyListControl.DataSource = StudyDataSourceObject;    
            } 
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

        

            if (Studies == null)
            {
                return;
            } 

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    StudySummary study = Studies[row.RowIndex];

                    if (study != null)
                    {
                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", study.ThePartition.AeTitle);

                        string reason;
                        if (study.CanScheduleDelete(out reason))
                            row.Attributes.Add("candelete", "true");

                        if (study.CanScheduleMove(out reason))
                            row.Attributes.Add("canmove", "true");

                        if (study.CanScheduleRestore(out reason))
                            row.Attributes.Add("canrestore", "true");

                        if (study.CanViewImages(out reason))
                        {
                            row.Attributes.Add("canviewimages", "true");
                            
                        } else
                        {
                            row.Attributes.Add("canviewimagesreason", reason);
                        }

                        var status = (Label) row.FindControl("StudyStatusEnum");
                        status.Text = ServerEnumDescription.GetLocalizedDescription(study.StudyStatusEnum);

                        LinkButton button = (LinkButton) row.FindControl("ReconcileLinkButton");
                        Label label = (Label)row.FindControl("SeparatorLabel");

                        if (study.IsReconcileRequired)
                        {
                            button.Visible = true;
							label.Visible = true;

                            button.PostBackUrl = ImageServerConstants.PageURLs.StudyIntegrityQueuePage +
                                                 "?PatientID=" + study.PatientId + "&PatientName=" + study.PatientsName + "&PartitionKey=" + study.ThePartition.GetKey();

                            button.Enabled = Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.StudyIntegrityQueue.Search);
                        }
                        else
                        {
                            button.Visible = false;
                            label.Visible = false;
                        }

                        button = (LinkButton) row.FindControl("QueueLinkButton");
                        label = (Label) row.FindControl("QueueSeparatorLabel");

                        if(study.IsLocked)
                        {
                            button.Visible = true;
									
							if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.RestoreScheduled))
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.RestoreQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.RestoreQueue.Search);
							}
							else if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ArchiveScheduled))
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.ArchiveQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.ArchiveQueue.Search);
							}
							else
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.WorkQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(Enterprise.Authentication.AuthorityTokens.WorkQueue.Search);
							}
                        } else
                        {
                            button.Visible = false;
                        }

                        label.Visible = button.Visible;
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
                    message.Message = TheGrid.DataSource == null ? SR.PleaseEnterSearchCriteriaToFindStudies: SR.NoStudiesFound;
                }
            } 
        }

		protected void GetStudyDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new StudyDataSource();

				_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
										{
											Studies = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        #endregion

    }

}
