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
using System.Text;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Web.Common.Data
{
    class DeleteStudyRecordAdaptor : BaseAdaptor<StudyDeleteRecord,
        IStudyDeleteRecordEntityBroker, StudyDeleteRecordSelectCriteria, StudyDeleteRecordUpdateColumns>
    {
    }
}
