#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Detailed view of a <see cref="Study"/> in the context of the WorkQueue configuration UI.
    /// </summary>
    /// <remarks>
    /// A <see cref="StudyDetails"/> contains detailed information of a <see cref="Study"/> and related information 
    /// to be displayed within the WorkQueue configuration UI.
    /// <para>
    /// A <see cref="StudyDetails"/> can be created using a <see cref="StudyDetailsAssembler"/> object.
    /// </para>
    /// </remarks>
    /// <seealso cref="WorkQueueDetails"/>
    public class StudyDetails
    {
        #region Public Properties

        public string StudyInstanceUID { get; set; }

        public string Status { get; set; }

        public string PatientName { get; set; }

        public string AccessionNumber { get; set; }

        public string PatientID { get; set; }

        public string StudyDescription { get; set; }

        public string StudyDate { get; set; }

        public string StudyTime { get; set; }

        public string Modalities { get; set; }

        public bool? WriteLock { get; set; }

		public short ReadLock { get; set; }

        #endregion Public Properties
    }
}