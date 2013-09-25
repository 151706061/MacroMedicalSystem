#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate length of the input.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example adds min password length validation. 
    /// </code>
    /// <uc1:InvalidInputIndicator ID="InvalidZipCodeIndicator" runat="server" 
    ///     ImageUrl="~/images/icons/HelpSmall.png"
    ///     Visible="true"/>
    ///                                                        
    /// <Macro:FilesystemPathValidator runat="server" ID="PasswordValidator" 
    ///         ControlToValidate="PasswordTextBox"
    ///         InputName="Password" 
    ///         InvalidInputColor="#FAFFB5" 
    ///         InvalidInputIndicatorID="InvalidPasswordIndicator"
    ///         MinLength="8"
    ///         ErrorMessage="Invalid password"
    ///         Display="None" ValidationGroup="vg1"/> 
    /// </code>
    /// </example>
    public class LengthValidator : BaseValidator
    {
        private int _maxLength = Int32.MaxValue;
        private int _minLength = Int32.MinValue;

        #region Public Properties

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override bool OnServerSideEvaluate()
        {
            //String value = GetControlValidationValue(ControlToValidate);
            //if (value == null || value.Length < MinLength || value.Length>MaxLength)
            //{
            //    ErrorMessage = String.Format("Must be at least {0} and no more than {1} characters.", MinLength, MaxLength);
            //    return false;
            //}
            //else
            //    return true;
            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            var template =
                new ScriptTemplate(this, "Macro.ImageServer.Web.Common.WebControls.Validators.LengthValidator.js");

            template.Replace("@@MIN_LENGTH@@", MinLength.ToString());
            template.Replace("@@MAX_LENGTH@@", MaxLength.ToString());
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}