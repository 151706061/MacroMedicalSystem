#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Macro.Common;
using System.Reflection;
using Macro.Common.Utilities;

namespace Macro.ImageServer.Web.Common
{
    public class ImageServerHttpApplication:HttpApplication
    {
        public override void Init()
        {
            try
            {
                object[] extensions = new HttpApplicationExtensionPoint().CreateExtensions();

                foreach (IHttpApplicationExtension ext in extensions)
                {
                    if (AttributeUtils.HasAttribute<ImageServerModuleAttribute>(ext.GetType(), true))
                    {
                        string methodName = AttributeUtils.GetAttribute<ImageServerModuleAttribute>(ext.GetType()).RegisterMethod;
                        if (String.IsNullOrEmpty(methodName))
                        {
                            Platform.Log(LogLevel.Warn, "ImageServerModuleAttribute RegisterMethod is missing in {0}", ext.GetType().FullName);
                        }
                        else
                        {
                            System.Reflection.MethodInfo method = ext.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                            if (method != null)
                            {
                                method.Invoke(null, new[] { this });
                            }
                        }
                    }
                    else
                    {
                        Platform.Log(LogLevel.Warn, "{0} must be decorated with ImageServerModuleAttribute", ext.GetType().FullName);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Warn, ex);
            }

            
            base.Init();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ImageServerModuleAttribute : Attribute
    {
        public string RegisterMethod { get; set; }
    }
}
