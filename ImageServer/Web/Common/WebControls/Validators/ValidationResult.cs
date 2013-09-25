#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Contains result of a validation.
    /// </summary>
    [DataContract]
    public class ValidationResult
    {
        public const int ERRORCODE_SERVICENOTAVAILABLE = -5000;

        #region Public Properties

        /// <summary>
        /// Indicate the validation passes or fails.
        /// </summary>
        [DataMember(Name = "Success")]
        public bool Success { get; set; }

        /// <summary>
        /// Validation failure code.
        /// </summary>
        [DataMember]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Validation failture message (reason)
        /// </summary>
        [DataMember]
        public string ErrorText { get; set; }

        #endregion Public Properties
    }
}