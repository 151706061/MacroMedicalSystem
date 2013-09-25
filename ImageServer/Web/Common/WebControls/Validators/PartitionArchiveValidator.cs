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
    public class PartitionArchiveValidator : BaseValidator
    {
        private string _originalAeTitle = "";

        public string OriginalAeTitle
        {
            get { return _originalAeTitle; }
            set { _originalAeTitle = value; }
        }

        protected override void RegisterClientSideValidationExtensionScripts()
        {
        }

        protected override bool OnServerSideEvaluate()
        {
            String aeTitle = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(aeTitle))
            {
                ErrorMessage = "AE Title must be specified";
                return false;
            }

            if (OriginalAeTitle.Equals(aeTitle))
                return true;

            var controller = new ServerPartitionConfigController();
            var criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.EqualTo(aeTitle);

            IList<ServerPartition> list = controller.GetPartitions(criteria);

            if (list.Count > 0)
            {
                ErrorMessage = String.Format(ValidationErrors.AETitleIsInUse, aeTitle);
                return false;
            }

            return true;
        }
    }
}