﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XenOvfTransport.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("P2V Automatically created.")]
        public string xenP2VDiskName {
            get {
                return ((string)(this["xenP2VDiskName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BIOS order")]
        public string xenBootOptions {
            get {
                return ((string)(this["xenBootOptions"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("order=dc;")]
        public string xenBootParams {
            get {
                return ((string)(this["xenBootParams"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("nx=true;acpi=true;apic=true;pae=true;stdvga=0;")]
        public string xenPlatformSetting {
            get {
                return ((string)(this["xenPlatformSetting"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("network=")]
        public string xenNetworkKey {
            get {
                return ((string)(this["xenNetworkKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xenbr0")]
        public string xenDefaultNetwork {
            get {
                return ((string)(this["xenDefaultNetwork"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/var/run/sr-mount/{0}")]
        public string xenISOMount {
            get {
                return ((string)(this["xenISOMount"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/var/opt/xen/iso_import")]
        public string xenISOTools {
            get {
                return ((string)(this["xenISOTools"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xenserver-linuxfixup-disk.iso")]
        public string XenFixupLabel {
            get {
                return ((string)(this["XenFixupLabel"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sr=")]
        public string xenSRKey {
            get {
                return ((string)(this["xenSRKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("device=")]
        public string xenDeviceKey {
            get {
                return ((string)(this["xenDeviceKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("XenNetwork=")]
        public string xenNetworkUuidKey {
            get {
                return ((string)(this["xenNetworkUuidKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.0.0")]
        public string OVFVersion {
            get {
                return ((string)(this["OVFVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^(http|https|file|ftp)://*")]
        public string uriRegex {
            get {
                return ((string)(this["uriRegex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DoNotUse_iSCSI_target")]
        public string iSCSITargetName {
            get {
                return ((string)(this["iSCSITargetName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Pool-wide network associated with eth0</string>
  <string>Network 0</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection iSCSINetworkName {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["iSCSINetworkName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int iSCSIConnectRetry {
            get {
                return ((int)(this["iSCSIConnectRetry"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Transfer VM for VDI {0}")]
        public string iSCSITransferVM {
            get {
                return ((string)(this["iSCSITransferVM"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.5,1.6,LATEST")]
        public string xenSupportedVersions {
            get {
                return ((string)(this["xenSupportedVersions"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>network_config=auto #REQUIRED: MUST BE FIRST Values:  ""auto"" ""manual""</string>
  <string>network_mode=dhcp #REQUIRED: MUST BE SECOND Values: ""dhcp"" ""manual""</string>
  <string>network_port=3260 #OPTIONAL: port for config is manual</string>
  <string>network_mac=00:00:00:00:00:00 #OPTIONAL: set mac address for config is manual</string>
  <string>network_ip=192.168.2.69 #OPTIONAL: Required if mode is manual</string>
  <string>network_mask=255.255.255.0 #OPTIONAL: Required if mode is manual</string>
  <string>network_gateway=192.168.2.1 #OPTIONAL: Required if mode is manual</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection iSCSITransferVMNetwork {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["iSCSITransferVMNetwork"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en-us")]
        public string DEFAULT_CULTURE {
            get {
                return ((string)(this["DEFAULT_CULTURE"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>\PAGEFILE.SYS</string>
  <string>\HIBERFIL.SYS</string>
  <string>\SYSTEM VOLUME INFORMATION</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection EXCLUDED_FILES_COPY {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["EXCLUDED_FILES_COPY"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".bz2")]
        public string bzip2ext {
            get {
                return ((string)(this["bzip2ext"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("management  #Replace with Network UUID to change iSCSI network utilization.")]
        public string iSCSITransferVMMgtNetwork {
            get {
                return ((string)(this["iSCSITransferVMMgtNetwork"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CreateApplianceFolder {
            get {
                return ((bool)(this["CreateApplianceFolder"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ApplianceFolderPath {
            get {
                return ((string)(this["ApplianceFolderPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("256")]
        public long FixupOsMemorySizeAsMB {
            get {
                return ((long)(this["FixupOsMemorySizeAsMB"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public int FixupDurationAsSeconds {
            get {
                return ((int)(this["FixupDurationAsSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int FixupPollTimeAsMs {
            get {
                return ((int)(this["FixupPollTimeAsMs"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[XenServer product] P2V (Orela) Server")]
        public string p2vTemplate {
            get {
                return ((string)(this["p2vTemplate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[Citrix VM Tools]")]
        public string xenTools {
            get {
                return ((string)(this["xenTools"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[XenServer product] Transfer")]
        public string iSCSITransferVMName {
            get {
                return ((string)(this["iSCSITransferVMName"]));
            }
        }
    }
}
