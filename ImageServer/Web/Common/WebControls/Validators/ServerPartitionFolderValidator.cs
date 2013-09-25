#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common.Data;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    public class ServerPartitionFolderValidator : BaseValidator
    {
        private string _originalFolder = "";

        public string OriginalPartitionFolder
        {
            get { return _originalFolder; }
            set { _originalFolder = value; }
        }

        protected override void RegisterClientSideValidationExtensionScripts()
        {
        }

        protected override bool OnServerSideEvaluate()
        {
            String partitionFolder = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(partitionFolder))
            {
                ErrorMessage = ValidationErrors.PartitionFolderCannotBeEmpty;
                return false;
            }

            if (OriginalPartitionFolder.Equals(partitionFolder))
                return true;

            var controller = new ServerPartitionConfigController();
            var criteria = new ServerPartitionSelectCriteria();
            criteria.PartitionFolder.EqualTo(partitionFolder);

            IList<ServerPartition> list = controller.GetPartitions(criteria);

            if (list.Count > 0)
            {
                ErrorMessage = String.Format(ValidationErrors.PartitionFolderIsInUse, partitionFolder);
                return false;
            }

            return true;
        }
    }
}