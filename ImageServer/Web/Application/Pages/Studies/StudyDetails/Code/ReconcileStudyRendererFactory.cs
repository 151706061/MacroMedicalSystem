#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    internal class ReconcileStudyRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            ReconcileHistoryDetailsColumn control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/ReconcileHistoryDetailsColumn.ascx") as ReconcileHistoryDetailsColumn;
            control.HistoryRecord = historyRecord;
            return control;
        }
    }

    internal class ReconcileHistoryRecord : StudyHistoryRecordBase
    {
        #region Private Fields

        private StudyReconcileDescriptor _updateDescription;
        #endregion

        #region Public Properties

        public StudyReconcileDescriptor UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }

        #endregion
    }

}