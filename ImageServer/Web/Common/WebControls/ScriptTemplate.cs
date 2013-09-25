#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Macro.ImageServer.Web.Common.WebControls.Validators;

namespace Macro.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Provides convenience mean to load a javascript template from embedded resource.
    /// </summary>
    public  class ScriptTemplate
    {
        #region Private Members
        private String _script;

        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ScriptTemplate"/>
        /// </summary>
        /// <param name="validator">The validator to which the validator belongs.</param>
        /// <param name="name">Fully-qualified name of the javascript template (including the namespace)</param>
        /// <remarks>
        /// 
        /// </remarks>
        public ScriptTemplate(BaseValidator validator,  string name):
            this(validator.GetType().Assembly, name)
        {
            Replace("@@CLIENTID@@", validator.ClientID);
            Replace("@@INPUT_NAME@@", validator.InputName);
            Replace("@@INPUT_CLIENTID@@", validator.InputControl.ClientID);
            Replace("@@INPUT_NORMAL_BKCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalColor));
            Replace("@@INPUT_INVALID_BKCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputColor));
            Replace("@@INPUT_NORMAL_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalBorderColor));
            Replace("@@INPUT_INVALID_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputBorderColor));
            Replace("@@INPUT_NORMAL_CSS@@", validator.InputNormalCSS);
            Replace("@@INPUT_INVALID_CSS@@", validator.InvalidInputCSS);            
            Replace("@@INVALID_INPUT_INDICATOR_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.Container.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabel.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabelContainer.ClientID);
            Replace("@@ERROR_MESSAGE@@", validator.Text);
            Replace("@@IGNORE_EMPTY_VALUE@@", validator.IgnoreEmptyValue? "true":"false");
            
        }

        public ScriptTemplate(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
                throw new Exception(String.Format("Resource not found: {0}", name));
            StreamReader reader = new StreamReader(stream);
            _script = reader.ReadToEnd();
            stream.Close();
            reader.Dispose();

        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        public string Script
        {
            get { return _script; }
        }

        #endregion Public properties

        #region Public Methods

        /// <summary>
        /// Replaces a token in the script with the specified value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Replace(string key, string value)
        {
            _script = _script.Replace(key, value);
        }

        #endregion Public Methods
    }
}
