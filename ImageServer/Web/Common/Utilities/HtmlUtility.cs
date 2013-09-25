#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Macro.Common.Utilities;
using Macro.ImageServer.Common;
using Macro.ImageServer.Common.Utilities;

namespace Macro.ImageServer.Web.Common.Utilities
{
    public static class HtmlUtility
    {
        public static string ConditionalString(bool condition, string s1, string s2)
        {
            return condition ? s1 : s2;
        }

    	///
    	/// Encode a string so that it is suitable for rendering in an Html page.
    	/// Also ensure all Xml escape characters are encoded properly.
        public static string Encode(string text)
        {
            if (text == null) return string.Empty;
            String encodedText = new SecurityElement("dummy", SecurityElement.Escape(text)).Text; //decode any escaped xml characters.
            return HttpUtility.HtmlEncode(encodedText).Replace(Environment.NewLine, "<BR/>");
            
        }

        /// <summary>
        /// Returns the <see cref="EnumInfoAttribute"/> of an enum value, if it's defined.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumInfoAttribute GetEnumInfo<TEnum>(TEnum value)
            where TEnum:struct
        {
            FieldInfo field = typeof (TEnum).GetField(value.ToString());
            if (field != null)
            {
                return AttributeUtils.GetAttribute<EnumInfoAttribute>(field);
            }
            else
                return null;
        }

        public static string GetEvalValue(object item, string itemName, string defaultValue)
        {
            string value = DataBinder.Eval(item, itemName, "");

            if (value == null || value.Equals("")) return defaultValue;
            else return value;
        }

         public static void AddCssClass(WebControl control, string cssClass)
         {
             control.CssClass += " " + cssClass;
         }
         
        public static void RemoveCssClass(WebControl control, string cssClass)
        {
            control.CssClass = control.CssClass.Replace(" " + cssClass, "");
        }

        public static String ResolveStudyDetailsUrl(Page page, String serverAE, String studyUid)
        {
            return String.Format("{0}?serverae={1}&siuid={2}",
                page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage),
                serverAE, studyUid);
        }

        public static String ResolveWorkQueueDetailsUrl(Page page, String workQueueKey)
        {
            return String.Format("{0}?uid={1}",
                page.ResolveClientUrl(ImageServerConstants.PageURLs.WorkQueueItemDetailsPage), workQueueKey);
        }
    }


}
