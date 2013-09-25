#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    static public class SeriesDetailsAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="SeriesDetails"/> for a <see cref="Series"/>.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public SeriesDetails CreateSeriesDetails(Model.Series series)
        {
            SeriesDetails details = new SeriesDetails();

            details.Modality = series.Modality;
            details.NumberOfSeriesRelatedInstances = series.NumberOfSeriesRelatedInstances;
            details.PerformedDate = series.PerformedProcedureStepStartDate;
            details.PerformedTime = series.PerformedProcedureStepStartTime;
            details.SeriesDescription = series.SeriesDescription;
            details.SeriesInstanceUid = series.SeriesInstanceUid;
            details.SeriesNumber = series.SeriesNumber;
            details.SourceApplicationEntityTitle = series.SourceApplicationEntityTitle;

            return details;
        }
    }
}
