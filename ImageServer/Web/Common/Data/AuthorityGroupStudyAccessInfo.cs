#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Enterprise.Common.Admin.AuthorityGroupAdmin;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Wraps <seealso cref="AuthorityGroupDetail"/> for display purposes. This class also contains additional properties related to data access.
    /// </summary>
    public class AuthorityGroupStudyAccessInfo
    {
        #region Public Properties

        public string AuthorityOID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group can access to all server partitions
        /// </summary>
        public bool CanAccessToAllPartitions { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group can access to all studies within a given partition
        /// </summary>
        public bool CanAccessToAllStudies { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group is a Data Access group
        /// </summary>
        public bool IsDataAccessAuthorityGroup { get; private set; }

        public StudyDataAccess StudyDataAccess { get; set; }

        #endregion


        #region Constructor

        public AuthorityGroupStudyAccessInfo(AuthorityGroupDetail detail)
        {
            AuthorityOID = detail.AuthorityGroupRef.ToString(false, false);
            Name = detail.Name;
            Description = detail.Description;
            IsDataAccessAuthorityGroup = detail.DataGroup;

            CanAccessToAllPartitions = HasToken(detail.AuthorityTokens,
                                                Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions);


            CanAccessToAllStudies= HasToken(detail.AuthorityTokens,
                                            Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies);
        }

        #endregion


        #region  Private Methods

        private static bool HasToken(List<AuthorityTokenSummary> tokens, string token)
        {
            return tokens.Exists(t => t.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}