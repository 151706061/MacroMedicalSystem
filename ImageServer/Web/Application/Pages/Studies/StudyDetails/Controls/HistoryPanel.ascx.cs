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
using Macro.Common.Utilities;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Data.DataSource;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    
    public partial class HistoryPanel : System.Web.UI.UserControl
    {
        private IList<StudyHistory> _historyList; 
        public StudySummary TheStudySummary;
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            // load history
            LoadHistory();

            StudyHistoryGridView.DataSource = _historyList;
            base.DataBind();
        }

        private void LoadHistory()
        {
            StudyHistoryeAdaptor adaptor = new StudyHistoryeAdaptor();
            StudyHistorySelectCriteria criteria = new StudyHistorySelectCriteria();
            criteria.DestStudyStorageKey.EqualTo(TheStudySummary.TheStudyStorage.GetKey());
            criteria.InsertTime.SortDesc(0);
            _historyList = CollectionUtils.Select(adaptor.Get(criteria),
                        delegate(StudyHistory history)
                            {
                                // only include reconciliation records that result in updating the current study
                                if (history.StudyHistoryTypeEnum==StudyHistoryTypeEnum.StudyReconciled)
                                {
                                    ReconcileHistoryRecord desc = StudyHistoryRecordDecoder.ReadReconcileRecord(history);
                                    switch(desc.UpdateDescription.Action)
                                    {
                                        case StudyReconcileAction.CreateNewStudy:
                                        case StudyReconcileAction.Merge:
                                        case StudyReconcileAction.ProcessAsIs:
                                            return true;
                                    }
                                    return false;
                                }
                                return true;
                            });

        }

        protected void StudyHistoryGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
        }

        protected void StudyHistoryGridView_PageIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        protected void StudyHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                StudyHistory item = e.Row.DataItem as StudyHistory;
                
                StudyHistoryChangeDescPanel panel = e.Row.FindControl("StudyHistoryChangeDescPanel") as StudyHistoryChangeDescPanel;
                panel.HistoryRecord = item;
                panel.DataBind();
            }
        }
    }
}