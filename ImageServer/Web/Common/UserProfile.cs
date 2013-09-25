#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.Security;
using Macro.Enterprise.Common;
using Macro.ImageServer.Web.Common.Security;
using ImageServerAuthorityTokens = Macro.ImageServer.Enterprise.Authentication.AuthorityTokens;
namespace Macro.ImageServer.Web.Common
{
    public static class UserProfile
    {
        /// <summary>
        /// Returns the default url for the current user
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultUrl()
        {
            //TODO: Use a mapping file similar to SiteMap to specify the default home page based on the authority tokens that user has.
            
            if (SessionManager.Current == null)
            {
                // user has not logged in
                FormsAuthentication.RedirectToLoginPage();
				// Need Response.End here, per this link:  http://www.neilpullinger.co.uk/2007/07/always-use-responseend-after.html
            	return null;
            }

			if (SessionManager.Current.User == null)
				return ImageServerConstants.PageURLs.SearchPage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.Admin.Dashboard.View))
                return ImageServerConstants.PageURLs.DashboardPage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.Study.Search))
                return ImageServerConstants.PageURLs.SearchPage;

            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Admin.Security.User))
                return ImageServerConstants.PageURLs.AdminUserPage;
            
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.WorkQueue.Search))
                return ImageServerConstants.PageURLs.WorkQueuePage;
           
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.StudyIntegrityQueue.Search))
                return ImageServerConstants.PageURLs.StudyIntegrityQueuePage;
                    
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.ArchiveQueue.Search))
                return ImageServerConstants.PageURLs.ArchiveQueuePage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.RestoreQueue.Search))
                return ImageServerConstants.PageURLs.RestoreQueuePage;

            return ImageServerConstants.PageURLs.AboutPage;
            
        }
    }
}
