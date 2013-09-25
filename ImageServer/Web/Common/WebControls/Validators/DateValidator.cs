#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Helper class to parse a date input from the GUI
    /// </summary>
    public static class InputDateParser
    {
        private const string InputFormat = "dd?MM?yyyy";

        public static string DateFormat
        {
            get
            {
                return InputFormat.Replace("?", CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator);
            }
        }

        public static bool TryParse(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, DateFormat, null, DateTimeStyles.AssumeLocal, out result);
        }
    }

    /// <summary>
    /// Validate a given date against the current date ensuring the given date is not in the future.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example adds min password length validation. 
    /// </code>
    /// <uc1:InvalidInputIndicator ID="BirthDateIndicator" runat="server" 
    ///     ImageUrl="~/images/icons/HelpSmall.png"
    ///     Visible="true"/>
    ///                                                        
    /// <Macro:DateValidator runat="server" ID="DateValidator" 
    ///         ControlToValidate="BirthDateTextBox"
    ///         InputName="BirthDate" 
    ///         InvalidInputColor="#FAFFB5" 
    ///         InvalidInputIndicatorID="InvalidBirthDateIndicator"
    ///         MinLength="8"
    ///         ErrorMessage="Birth Date may not be in the future"
    ///         Display="None" ValidationGroup="vg1"/> 
    /// </code>
    /// </example>
    public class DateValidator : BaseValidator
    {
        #region Protected Methods

        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);
            if (String.IsNullOrEmpty(value))
            {
                if (IgnoreEmptyValue)
                    return true;
                else
                {
                    Text = "This field is required";
                    return false;
                }
            }

            DateTime result;
            if (!InputDateParser.TryParse(value, out result))
            {
                Text = string.Format("{0} is not a valid date for '{1}' format.", value, InputDateParser.DateFormat);
                return false;
            }

            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this, "Macro.ImageServer.Web.Common.WebControls.Validators.DateValidator.js");

            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");
            template.Replace("@@DATE_FORMAT@@", InputDateParser.DateFormat);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}