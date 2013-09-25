#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.Common;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Common.Data.DataSource
{
    public interface IStudySummaryAssembler
    {
        void PopulateStudy(StudySummary summary, Study theStudy);
    }

    public class StudySummaryAssemblerExtensionPoint : ExtensionPoint<IStudySummaryAssembler>
    {
    }
}