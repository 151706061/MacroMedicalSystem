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

namespace Macro.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Defines the interface of the class that returns the customize
    /// control to display information in different types of
    /// <see cref="StudyHistory"/> record.
    /// 
    /// </summary>
    internal interface IStudyHistoryColumnControlFactory
    {
        /// <summary>
        /// Returns the <see cref="Control"/> that displays the content of 
        /// the ChangeDescription column of the specified <see cref="StudyHistory"/> record.
        /// </summary>
        /// <param name="historyRecord"></param>
        /// <returns></returns>
        Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord);
    }
}