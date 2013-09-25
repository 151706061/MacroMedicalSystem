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
using Macro.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class SeriesDeleteChangeLog : System.Web.UI.UserControl
    {
        private SeriesDeletionChangeLog _changeLog;

        public SeriesDeletionChangeLog ChangeLog
        {
            get { return _changeLog; }
            set { _changeLog = value; }
        }

        public string GetReason(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason[0];
        }

        public string GetComment(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            if (reason.Length == 1) return SR.NoneSpecified;
            return reason[1];
        }

        protected string ChangeSummaryText
        {
            get
            {
                return ChangeLog.Series.Count == 1
                    ? string.Format(SR.StudyDetails_History_OneSeriesDeletedBy, ChangeLog.Series.Count, ChangeLog.UserId ?? SR.Unknown)
                    : string.Format(SR.StudyDetails_History_MultipleSeriesDeletedBy, ChangeLog.Series.Count, ChangeLog.UserId ?? SR.Unknown);
            }
        }
    }
}