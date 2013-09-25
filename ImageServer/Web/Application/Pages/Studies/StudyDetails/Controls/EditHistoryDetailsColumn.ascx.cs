#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Edit;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Utilities;
using Resources;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditHistoryDetailsColumn : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;
        private WebEditStudyHistoryChangeDescription _description;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public StudyHistory HistoryRecord
        {
            set { _historyRecord = value; }
        }

        public WebEditStudyHistoryChangeDescription EditHistory
        {
            get
            {
                if (_description == null && _historyRecord != null)
                {
                    _description = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(_historyRecord.ChangeDescription.DocumentElement);
                }
                return _description;
            }
        }

        public string GetReason(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason.Length > 0 ? reason[0] : string.Empty;
        }

        public string GetComment(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return SR.NoneSpecified;
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason.Length > 1 ? reason[1] : string.Empty;
        }

        protected string ChangeSummaryText
        {
            get{
                return String.Format(SR.EditBy, EditTypeTranslator.Translate(EditHistory.EditType), EditHistory.UserId ?? SR.Unknown);
            }
        }
    }

    public static class EditTypeTranslator{
        public static string Translate(EditType type)
        {
            switch (type)
            {
                case EditType.WebEdit:
                    return HtmlUtility.Encode(SR.StudyDetails_WebEdit_Description);
                case EditType.WebServiceEdit:
                    return HtmlUtility.Encode(SR.StudyDetails_WebServiceEdit_Description);
            }

            return HtmlUtility.Encode(HtmlUtility.GetEnumInfo(type).LongDescription);
        }
    }
}