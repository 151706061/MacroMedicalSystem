#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace Macro.ImageServer.Web.Application.Helpers
{
    public class SearchHelper
    {
        public static string TrailingWildCard(string searchText)
        {
            if(ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if (searchText.IndexOfAny(new[]{'*','%','?','_'} ) == -1)
                    return searchText + "%";
            }
            return searchText;
        }

        public static string LeadingWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if (searchText.IndexOfAny(new[] { '*', '%', '?', '_' }) == -1)
                    return "%" + searchText;
            }
            return searchText;
        }

        public static string LeadingAndTrailingWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if (searchText.IndexOfAny(new[] { '*', '%', '?', '_' }) == -1) 
                    return "%" + searchText + "%";
            }
            return searchText;
        }

        public static string NameWildCard(string searchText)
        {
            if (ConfigurationManager.AppSettings["UseWildcards"].ToLower().Equals("true"))
            {
                if (searchText.IndexOfAny(new[] { '*', '%', '?', '_' }) == -1)
                {
                    string[] names = searchText.Split(',');
                    if (names.Length == 2)
                    {
                        return names[0].Trim() + "%" + names[1].Trim() + "%";
                    }
                    return "%" + searchText + "%";
                }
            }
            return searchText;
        }
    }
}
