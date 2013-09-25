#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Enterprise.Common;
using Resources;

namespace Macro.ImageServer.Web.Application.Helpers
{
    internal static class ExceptionTranslator
    {
        public static string Translate(Exception ex)
        {
            if (ex.GetType().Equals(typeof(AuthorityGroupIsNotEmptyException)))
            {
                AuthorityGroupIsNotEmptyException exception = ex as AuthorityGroupIsNotEmptyException;
                return exception.UserCount == 1
                           ? string.Format(ErrorMessages.ExceptionAuthorityGroupIsNotEmpty_OneUser, exception.GroupName)
                           : string.Format(ErrorMessages.ExceptionAuthorityGroupIsNotEmpty_MultipleUsers, exception.GroupName, exception.UserCount);
            }

            return ex.Message;
        }
    }
}
