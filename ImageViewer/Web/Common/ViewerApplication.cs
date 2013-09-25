#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using Macro.Web.Common;
using Macro.ImageViewer.Web.Common.Entities;
using System;

namespace Macro.ImageViewer.Web.Common
{
    [DataContract]
    public class LayoutConfiguration
    {
        [DataMember(IsRequired = true)]
        public int Rows { get; set; }

        [DataMember(IsRequired = true)]
        public int Columns { get; set; }
    }

    [DataContract]
    public class LoadStudyOptions
    {
        [DataMember(IsRequired = false)]
		public bool KeyImagesOnly {get; set;}
        /// <summary>
        /// Exclude priors
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool ExcludePriors { get; set; }

        [DataMember(IsRequired = false)]
        public LayoutConfiguration PreferredLayout { get; set; }
    }

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class StartViewerApplicationRequest : StartApplicationRequest
	{
		[DataMember(IsRequired = true)]
		public string AeTitle { get; set; }

		[DataMember(IsRequired = false)]
		public string[] StudyInstanceUid { get; set; }

		[DataMember(IsRequired = false)]
		public string[] AccessionNumber { get; set; }

		[DataMember(IsRequired = false)]
		public string[] PatientId { get; set; }

        [DataMember(IsRequired = false)]
        public LoadStudyOptions LoadStudyOptions { get; set; }

        [DataMember(IsRequired = false)]
        public string ApplicationName { get; set; }
	}

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class ViewerApplication : Application
	{
        [DataMember(IsRequired = true)]
        public string VersionString { get; set; }

		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}
}