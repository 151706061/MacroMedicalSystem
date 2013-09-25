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
using System.Globalization;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    public class DeviceValidator : BaseValidator
    {
        private string _originalAeTitle = String.Empty;
        private ServerEntityKey _partition;

        public string OriginalAeTitle
        {
            get { return _originalAeTitle; }
            set { _originalAeTitle = value; }
        }

        public ServerEntityKey Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        protected override void RegisterClientSideValidationExtensionScripts()
        {
        }

        protected override bool OnServerSideEvaluate()
        {
            String deviceAE = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(deviceAE))
            {
                ErrorMessage = ValidationErrors.AETitleCannotBeEmpty;
                return false;
            }

            if (OriginalAeTitle.Equals(deviceAE))
                return true;

            var controller = new DeviceConfigurationController();
            var criteria = new DeviceSelectCriteria();
            criteria.AeTitle.EqualTo(deviceAE);
            criteria.ServerPartitionKey.EqualTo(Partition);

            IList<Device> list = controller.GetDevices(criteria);
            foreach (var d in list)
            {
                if (string.Compare(d.AeTitle, deviceAE, false, CultureInfo.InvariantCulture) == 0)
                {
                    ErrorMessage = String.Format(ValidationErrors.AETitleIsInUse, deviceAE);
                    return false;
                }
            }

            return true;
        }
    }
}
