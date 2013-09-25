#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System;

namespace Macro.ImageViewer.Web.Client.Silverlight.Controls
{
	//TODO (CR May 2010): still use this class?
	public class StudyInfo
    {
        public String StudyInstanceUid { get; set; }
        public String PatientsName { get; set; }
        public String Modality { get; set; }
    }
}