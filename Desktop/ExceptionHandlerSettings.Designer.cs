﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.296
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Macro.Desktop {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class ExceptionHandlerSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ExceptionHandlerSettings defaultInstance = ((ExceptionHandlerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ExceptionHandlerSettings())));
        
        public static ExceptionHandlerSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Indicates whether exception stack trace is shown in a dialog box.  For security reasons, this should not be enabled in production.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Indicates whether exception stack trace is shown in a dialog box.  For security r" +
            "easons, this should not be enabled in production.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowStackTraceInDialog {
            get {
                return ((bool)(this["ShowStackTraceInDialog"]));
            }
        }
    }
}
