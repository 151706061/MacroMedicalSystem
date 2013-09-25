#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Macro.Common;
using System.Web.UI;

namespace Macro.ImageServer.Web.Common.Extensions
{
    public sealed class ExtensibleAttribute : Attribute
    {
        public Type ExtensionPoint;
    }

    public interface ILoginPage
    {
        Control SplashScreenControl { get; }
    }

    public interface ILoginPageExtension
    {
        void OnLoginPageInit(Page page);
    }

    [ExtensionPoint]
    public class LoginPageExtensionPoint : ExtensionPoint<ILoginPageExtension>
    {

    }


    public interface IAboutPage
    {
        Control ExtensionContentParent { get; }
    }

    public interface IAboutPageExtension
    {
        void OnAboutPageInit(Page page);
    }

    [ExtensionPoint]
    public class AboutPageExtensionPoint : ExtensionPoint<IAboutPageExtension>
    {

    }
}
