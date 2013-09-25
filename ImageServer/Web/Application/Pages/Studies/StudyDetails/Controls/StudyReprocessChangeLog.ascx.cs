#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Model;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyReprocessChangeLogControl : System.Web.UI.UserControl
    {
        private ReprocessStudyChangeLog _changeLog;

        public StudyHistory HistoryRecord;

        protected ReprocessStudyChangeLog ChangeLog
        {
            get
            {
                if (_changeLog == null)
                {
                    _changeLog = XmlUtils.Deserialize<ReprocessStudyChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

        protected string ChangeDescription
        {
            get
            {
                if (!String.IsNullOrEmpty(ChangeLog.User))
                {
                    return String.Format(SR.StudyDetails_History_Reconcile_ReprocessedBecause, ChangeLog.Reason);
                }
                else
                {
                    return String.Format(SR.StudyDetails_History_Reconcile_ReprocessedByBecause, ChangeLog.User, ChangeLog.Reason);
                }
            }
        }

    }
}