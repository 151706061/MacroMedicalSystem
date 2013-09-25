#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;

namespace Macro.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Model object behind the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public class SeriesDetails
    {
        private string _seriesInstanceUid;
        private string _modality;
        private string _seriesNumber;
        private string _seriesDescription;
        private int _numberOfSeriesRelatedInstances;
        private string _performedDate;
        private string _performedTime;
        private string _sourceApplicationEntityTitle;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfSeriesRelatedInstances; }
            set { _numberOfSeriesRelatedInstances = value; }
        }

        public string PerformedDate
        {
            get { return _performedDate; }
            set { _performedDate = value; }
        }

        public string SourceApplicationEntityTitle
        {
            get { return _sourceApplicationEntityTitle; }
            set { _sourceApplicationEntityTitle = value; }
        }

        public string PerformedTime
        {
            get { return _performedTime; }
            set { _performedTime = value; }
        }
    }
}
