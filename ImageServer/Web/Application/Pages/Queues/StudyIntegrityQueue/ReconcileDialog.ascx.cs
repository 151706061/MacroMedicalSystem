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
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Helpers;
using Resources;
using Macro.ImageServer.Web.Application.Controls;
using Macro.ImageServer.Web.Common;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Macro.ImageServer.Web.Common.Utilities;
using SR=Resources.SR;

namespace Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class ReconcileDialog : UserControl
    {
        private const string HighlightCssClass = " ConflictField ";
        private readonly StudyIntegrityQueueController _controller = new StudyIntegrityQueueController();

        #region Nested type: ComparisonCallback

        private delegate void ComparisonCallback(bool different);

        #endregion

        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.

        private Model.StudyIntegrityQueue _item;
        private IList<ServerPartition> _partitions = new List<ServerPartition>();
        protected internal bool CanReconcile { get; set; }
        private List<uint> _tagsMustBeTruncated = new List<uint>();

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            set { _partitions = value; }

            get { return _partitions; }
        }

        /// <summary>
        /// Sets or gets the StudyIntegrity Item Value
        /// </summary>
        public Model.StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
            set
            {
                _item = value;
                ViewState["StudyIntegrityQueueItem"] = _item.GetKey();
            }
        }

        /// <summary>
        /// Sets or gets the Reconcile Item Value
        /// </summary>
        public ReconcileDetails ReconcileDetails { get; set; }


        public bool DataWillBeTruncated { get { return _tagsMustBeTruncated.Count > 0; } }

        #endregion // public members


        #region Events

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            var itemKey = ViewState["StudyIntegrityQueueItem"] as ServerEntityKey;

            try
            {
                if (MergeUsingExistingStudy.Checked)
                {
                    _controller.MergeStudy(itemKey, true);
                }
                else if (MergeUsingConflictingStudy.Checked)
                {
                    _controller.MergeStudy(itemKey, false);
                }
                else if (CreateNewStudy.Checked)
                {
                    _controller.CreateNewStudy(itemKey);
                }
                else if (DiscardStudy.Checked)
                {
                    _controller.Discard(itemKey);
                }
                else if (IgnoreConflict.Checked)
                {
                    _controller.IgnoreDifferences(itemKey);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Message = String.Format(ErrorMessages.ActionNotAllowedAtThisTime, ex.Message);
                MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();
            }

            //((Default) Page).UpdateUI();
            Close();
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();
            HighlightDifferences();
            Page.Validate();
            TabContainer.ActiveTabIndex = 0;

            CheckDataForPossibleTruncation();
            ReconcileItemModalDialog.Show();
        }

        public override void DataBind()
        {
            ExistingPatientSeriesGridView.DataSource = ReconcileDetails.ExistingStudy.Series;
            ConflictingPatientSeriesGridView.DataSource = ReconcileDetails.ConflictingStudyInfo.Series;

            StudyStorage storage =
                StudyStorage.Load(HttpContextData.Current.ReadContext, StudyIntegrityQueueItem.StudyStorageKey);

            IList<StudyStorageLocation> studyLocations = StudyStorageLocation.FindStorageLocations(storage);
            StudyStorageLocation location = studyLocations[0];
            StudyLocation.Text = location.GetStudyPath();

            ConflictingStudyLocation.Text = ReconcileDetails != null
                                                ? ReconcileDetails.GetFolderPath()
                                                : SR.NotSpecified;

            string reason;
            CanReconcile = _controller.CanReconcile(location, out reason);
            MessagePanel.Visible = !CanReconcile;
            AlertMessage.Text = reason;
            OKButton.Enabled = CanReconcile;
            OptionRow.Visible = CanReconcile;
            base.DataBind();
        }

        private void CheckDataForPossibleTruncation()
        {
            _tagsMustBeTruncated  = DataLengthValidation.CheckDataLength(ReconcileDetails.ConflictingStudyInfo);
            FieldsMayTruncate.Value = _tagsMustBeTruncated.Count>0 ? "true" : "false";
        }

        private void HighlightDifferences()
        {
            if (ReconcileDetails != null)
            {
                Compare(ReconcileDetails.ExistingStudy.Patient.Name, ReconcileDetails.ConflictingStudyInfo.Patient.Name,
                        different => Highlight(ConflictingNameLabel, different));

                Compare(ReconcileDetails.ExistingStudy.Patient.PatientID,
                        ReconcileDetails.ConflictingStudyInfo.Patient.PatientID,
                        different => Highlight(ConflictingPatientIDLabel, different));
                Compare(ReconcileDetails.ExistingStudy.Patient.IssuerOfPatientID,
                        ReconcileDetails.ConflictingStudyInfo.Patient.IssuerOfPatientID,
                        different => Highlight(ConflictingPatientIssuerOfPatientID, different));
                Compare(ReconcileDetails.ExistingStudy.Patient.BirthDate,
                        ReconcileDetails.ConflictingStudyInfo.Patient.BirthDate,
                        different => Highlight(ConflictingPatientBirthDate, different));
                Compare(ReconcileDetails.ExistingStudy.Patient.Sex, ReconcileDetails.ConflictingStudyInfo.Patient.Sex,
                        different => Highlight(ConflictingPatientSex, different));
                Compare(ReconcileDetails.ExistingStudy.StudyDate, ReconcileDetails.ConflictingStudyInfo.StudyDate,
                        different => Highlight(ConflictingStudyDate, different));
                Compare(ReconcileDetails.ExistingStudy.AccessionNumber,
                        ReconcileDetails.ConflictingStudyInfo.AccessionNumber,
                        different => Highlight(ConflictingAccessionNumberLabel, different));

                UnknownSexWarning.Visible = !DicomValueValidator.IsValidDicomPatientSex(ReconcileDetails.ConflictingStudyInfo.Patient.Sex);
            }
        }

        private static void Compare(string value1, string value2, ComparisonCallback del)
        {
            if (!StringUtils.AreEqual(value1, value2, StringComparison.InvariantCultureIgnoreCase))
                del(true);
            else
                del(false);
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            TabContainer.ActiveTabIndex = 0;
            ReconcileItemModalDialog.Hide();
        }

        #endregion Public methods

        private static void Highlight(WebControl control, bool highlight)
        {
            if (highlight)
                HtmlUtility.AddCssClass(control, HighlightCssClass);
            else
                HtmlUtility.RemoveCssClass(control, HighlightCssClass);
        }
    }
}