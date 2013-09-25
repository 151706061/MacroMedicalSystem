#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Text.RegularExpressions;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate if an input control value matches a specified regular expression.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RegularExpressionFieldValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the IP Address. If the input is not an IP address, the IP address
    /// text box will be highlighted.
    /// 
    /// <Macro:RegularExpressionFieldValidator 
    ///                                ID="RegularExpressionFieldValidator1" runat="server"
    ///                                ControlToValidate="IPAddressTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
    ///                                ErrorMessage="The IP address is not valid." Display="None">
    /// </Macro:RegularExpressionFieldValidator>
    /// 
    /// </example>
    /// 
    public class RegularExpressionFieldValidator : BaseValidator
    {
        #region Public Properties

        /// <summary>
        /// Sets or gets the regular expression to validate the input.
        /// </summary>
        public string ValidationExpression { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);
            var regex = new Regex(ValidationExpression);
            
            return value != null ?  regex.IsMatch(value) : false;
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this,
                                   "Macro.ImageServer.Web.Common.WebControls.Validators.RegularExpressionValidator.js");
            template.Replace("@@REGULAR_EXPRESSION@@", ValidationExpression.Replace("\\", "\\\\").Replace("'", "\\'"));
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");


            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}