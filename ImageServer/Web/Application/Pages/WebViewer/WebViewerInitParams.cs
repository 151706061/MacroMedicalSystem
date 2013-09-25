#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.ObjectModel;

namespace Macro.ImageServer.Web.Application.Pages.WebViewer
{
    public class WebViewerInitParams
    {
        private Collection<string> _accessionNumbers = new Collection<string>();
        private Collection<string> _patientIds = new Collection<string>();
        private Collection<string> _studyInstanceUids = new Collection<string>();

        public Collection<string> AccessionNumbers
        {
            get { return _accessionNumbers; }
        }

        public Collection<string> PatientIds
        {
            get { return _patientIds; }
        }

        public Collection<string> StudyInstanceUids
        {
            get { return _studyInstanceUids; }
        }

        public string AeTitle
        {
            get;
            set;
        }

    }
}
