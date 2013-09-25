#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate a required Web UI input control containing value based on the state of another checkbox control.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RequiredFieldValidator"/>.
    /// Users can use this control for required field validation based on state of a checkbox on the UI. When the 
    /// condition is satisfied, the control will validate the input field contains a value. Developers 
    /// can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the SIN if the citizen checkbox is checked:
    /// 
    /// <Macro:ConditionalRequiredFieldValidator 
    ///                                ID="RequiredFieldValidator2" runat="server" 
    ///                                ControlToValidate="SINTextBox"
    ///                                ConditionalCheckBoxID="IsCitizenCheckedBox" 
    ///                                RequiredWhenChecked="true"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                EnableClientScript="true"
    ///                                ErrorMessage="SIN is required for citizen!!">
    /// </Macro:ConditionalRequiredFieldValidator>
    /// 
    /// </example>
    /// 
    public class ConditionalRequiredFieldValidator : BaseValidator
    {
        #region Public Properties

        #endregion Public Properties

        #region Protected Methods

        //protected override void RegisterClientSideValidationExtensionScripts()
        //{
        //    if (ConditionalCheckBoxID != null)
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "Macro.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate_Conditional.js");
        //        template.Replace("@@CLIENTID@@", ClientID);
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@CONDITIONAL_CONTROL_CLIENTID@@", GetControlRenderID(ConditionalCheckBoxID));
        //        template.Replace("@@REQUIRED_WHEN_CHECKED@@", RequiredWhenChecked.ToString().ToLower());
        //        template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue.ToString().ToLower());
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //    else
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "Macro.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate.js");
        //        template.Replace("@@CLIENTID@@", ClientID); 
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //}

        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }


            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            RegisterClientSideBaseValidationScripts();

            var template =
                new ScriptTemplate(this,
                                   "Macro.ImageServer.Web.Common.WebControls.Validators.ConditionalRequiredFieldValidator.js");

            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}