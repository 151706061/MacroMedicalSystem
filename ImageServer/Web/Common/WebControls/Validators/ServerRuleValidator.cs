#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using Macro.Dicom.Utilities.Rules;
using Macro.ImageServer.Model;
using Macro.ImageServer.Rules;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validator for Server Rules.  Note that this control only works with client side validation.
    /// It will not work properly with just server side validation.
    /// </summary>
    public class ServerRuleValidator : WebServiceValidator
    {
        public string RuleTypeControl
        {
            get
            {
                return ViewState["RULE_TYPE"].ToString();
            } 
            set
            {
                ViewState["RULE_TYPE"] = value;
            }
        }

        protected override bool OnServerSideEvaluate()
        {
            String ruleXml = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(ruleXml))
            {
                ErrorMessage = ValidationErrors.ServerRuleXMLIsMissing;
                return false;
            }

            if (RuleTypeControl.Equals(ServerRuleTypeEnum.DataAccess.Lookup))
            {
                // Validated DataAccess rules only have the condition.  Make a fake 
                // rule that includes a non-op action
                ruleXml = String.Format("<rule>{0}<action><no-op/></action></rule>", ruleXml);
            }

            var theDoc = new XmlDocument();

            try
            {
                theDoc.LoadXml(ruleXml);
            }
            catch (Exception e)
            {
                ErrorMessage = String.Format(ValidationErrors.UnableToParseServerRuleXML, e.Message);
                return false;
            }

            string error;
                if (false == Rule<ServerActionContext,ServerRuleTypeEnum>.ValidateRule(ServerRuleTypeEnum.GetEnum(RuleTypeControl), theDoc, out error))
                {
                    ErrorMessage = error;
                    return false;
                }

            return true;
        }
    }
}