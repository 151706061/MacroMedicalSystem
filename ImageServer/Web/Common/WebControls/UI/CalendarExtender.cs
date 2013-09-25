#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Macro.ImageServer.Web.Common.Utilities;

namespace Macro.ImageServer.Web.Common.WebControls.UI
{
    public class CalendarExtender : AjaxControlToolkit.CalendarExtender
    {
        public CalendarExtender()
        {
            SetPropertyValue("Format", DateTimeFormatter.DefaultDateFormat);
        }
    }
}