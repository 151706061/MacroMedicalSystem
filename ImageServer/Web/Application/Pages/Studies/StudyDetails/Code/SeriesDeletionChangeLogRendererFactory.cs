#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory;
using Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded of a "SeriesDeleted"
    /// StudyHistory record.
    /// </summary>
    internal class SeriesDeletionChangeLogRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            SeriesDeleteChangeLog control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/SeriesDeleteChangeLog.ascx") as SeriesDeleteChangeLog;
            control.ChangeLog = XmlUtils.Deserialize<SeriesDeletionChangeLog>(historyRecord.ChangeDescription);
            return control;
        }
    }
}