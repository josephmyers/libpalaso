﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Palaso.Network {


	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	public sealed partial class ProxyCredentialSettings : global::System.Configuration.ApplicationSettingsBase {

		private static ProxyCredentialSettings defaultInstance = ((ProxyCredentialSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ProxyCredentialSettings())));

		public static ProxyCredentialSettings Default {
			get {
				return defaultInstance;
			}
		}

		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		public string UserName {
			get {
				return ((string)(this["UserName"]));
			}
			set {
				this["UserName"] = value;
			}
		}

		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		public string Password {
			get {
				return ((string)(this["Password"]));
			}
			set {
				this["Password"] = value;
			}
		}
	}
}
