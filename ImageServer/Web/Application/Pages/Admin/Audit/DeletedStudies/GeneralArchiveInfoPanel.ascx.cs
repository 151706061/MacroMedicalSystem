#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.ImageServer.Common.Utilities;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class GeneralArchiveInfoPanel : BaseDeletedStudyArchiveUIPanel
    {
        public override void DataBind()
        {
            ArchiveXml.Text = XmlUtils.GetXmlDocumentAsString(ArchiveInfo.ArchiveXml, true);
            base.DataBind();
        }
    }
}