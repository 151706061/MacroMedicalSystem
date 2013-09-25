#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyHistoryChangeDescPanel : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;

        public StudyHistory HistoryRecord
        {
            get { return _historyRecord; }
            set { _historyRecord = value; }
        }

        public override void DataBind()
        {
            if (_historyRecord != null)
            {
                // Use different control to render the content of the ChangeDescription column.
                IStudyHistoryColumnControlFactory render = GetColumnControlFactory(_historyRecord);
                Control ctrl = render.GetChangeDescColumnControl(this, _historyRecord);
                SummaryPlaceHolder.Controls.Add(ctrl);
            }
            base.DataBind();
        } 

        private static IStudyHistoryColumnControlFactory GetColumnControlFactory(StudyHistory record)
        {
            if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
                return new ReconcileStudyRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited || record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.ExternalEdit)
                return new StudyEditRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Duplicate)
                return new ProcessDuplicateChangeLogRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Reprocessed)
                return new StudyReprocessedChangeLogRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.SeriesDeleted)
                return new SeriesDeletionChangeLogRendererFactory();
            else
                return new DefaultStudyHistoryRendererFactory();
        }

    }
}