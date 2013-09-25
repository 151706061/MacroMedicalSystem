#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using Macro.Dicom.Utilities.Rules;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Model;
using Macro.ImageServer.Rules;
using Macro.ImageServer.Web.Common.Data;
using Macro.ImageServer.Web.Common.Utilities;
using Macro.ImageServer.Web.Common.WebControls.Validators;

namespace Macro.ImageServer.Web.Application.Services
{
    /// <summary>
    /// Provides data validation services
    /// </summary>
    [WebService(Namespace = "http://www.ClearCanvas.ca/ImageServer/Services/ValidationServices.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [GenerateScriptType(typeof (ValidationResult))]
    [ScriptService]
    public class ValidationServices : WebService
    {
        /// <summary>
        /// Validate the existence of the specified path on the network.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateFilesystemPath(string path)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(path))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = ValidationErrors.FilesystemPathCannotBeEmpty ;
                return result;
            }

            FilesystemInfo fsInfo = null;
            result.Success = false;
            try
            {
                fsInfo = ServerUtility.GetFilesystemInfo(path);
                if (fsInfo!=null && fsInfo.Exists)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ErrorText = String.Format(ValidationErrors.FilesystemPathInvalidOrUnreachable, path);
                }
                return result;
            }
            catch(Exception e)
            {
                result.Success = false;
                result.ErrorCode = 100;
                result.ErrorText = String.Format(ValidationErrors.UnableToValidatePath, path, e.Message);
            }

            return result;
        }

        /// <summary>
        /// Validate the existence of a user name.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="originalUsername"></param>/// 
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateUsername(string username, string originalUsername)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(username))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = ValidationErrors.UsernameCannotBeEmpty;
                return result;
            }

            UserManagementController controller = new UserManagementController();

            if (controller.ExistsUsername(username) && !username.Equals(originalUsername))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = String.Format(ValidationErrors.UsernameAlreadyExists, username);
                return result;
                
            } else
            {
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// Validate the existence of a user group.
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <param name="originalGroupName"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateUserGroupName(string userGroupName, string originalGroupName)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(userGroupName))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText =  ValidationErrors.UserGroupCannotBeEmpty;
                return result;
            }

            UserManagementController controller = new UserManagementController();

            if (controller.ExistsUsergroup(userGroupName) && !userGroupName.Equals(originalGroupName))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = String.Format(ValidationErrors.UserGroupAlreadyExists, userGroupName); ;
                return result;
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// Validate a ServerRule for proper formatting.
        /// </summary>
        /// <param name="serverRule">A string representing the rule.</param>
        /// <param name="ruleType">An string enumerated value of <see cref="ServerRuleTypeEnum"/></param>
        /// <returns>The result of the validation.</returns>
        [WebMethod]
        public ValidationResult ValidateServerRule(string serverRule, string ruleType)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrEmpty(serverRule))
            {
                result.ErrorText = ValidationErrors.ServerRuleXMLIsMissing;
                result.Success = false;
                result.ErrorCode = -5000;
                return result;
            }

        	ServerRuleTypeEnum type;
			try
			{
				type = ServerRuleTypeEnum.GetEnum(ruleType);
			}
			catch (Exception e)
			{
				result.ErrorText = String.Format(ValidationErrors.UnableToParseServerRuleXML, e.Message);
				result.Success = false;
				result.ErrorCode = -5000;
				return result;
			}

        	XmlDocument theDoc = new XmlDocument();

            try
            {
                string xml = Microsoft.JScript.GlobalObject.unescape(serverRule);
                
                if (type.Equals(ServerRuleTypeEnum.DataAccess))
                {
                    // Validated DataAccess rules only have the condition.  Make a fake 
                    // rule that includes a non-op action
                    xml = String.Format("<rule>{0}<action><no-op/></action></rule>", xml);
                }

                theDoc.LoadXml(xml);
            }
            catch (Exception e)
            {
                result.ErrorText = String.Format(ValidationErrors.UnableToParseServerRuleXML, e.Message);
                result.Success = false;
                result.ErrorCode = -5000;
                return result;
            }

            string error;
            if (false == Rule<ServerActionContext, ServerRuleTypeEnum>.ValidateRule(type, theDoc, out error))
            {
                result.ErrorText = error;
                result.Success = false;
                result.ErrorCode = -5000;
            }
            else
                result.Success = true;

            return result;
        }
    }
}
