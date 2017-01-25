/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace XenAdmin.Core
{
    public class Registry
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static bool AllowCredentialSave
        {
            get
            {
                return ReadBool(ALLOW_CREDENTIAL_SAVE, true);
            }
        }

        internal static bool VMPRFeatureEnabled
        {
            get
            {
                return ReadBool(VMPR_ENABLED, true);
            }
        }

        internal static bool DisablePlugins
        {
            get
            {
                return ReadBool(DISABLE_PLUGINS, false);
            }
        }

        internal static bool ForceSystemFonts
        {
            get
            {
                return ReadBool(FORCE_SYSTEM_FONTS, false);
            }
        }

        internal static bool DontSudo
        {
            get
            {
                return ReadBool(DONT_SUDO, false);  // CA-38045
            }
        }

        public static SSLCertificateTypes AlwaysShowSSLCertificates
        {
            get
            {
                try
                {
                    RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(XENCENTER_LOCAL_KEYS);
                    if (masterKey == null)
                        return SSLCertificateTypes.None;

                    try
                    {
                        object v = masterKey.GetValue(SSL_CERTIFICATES_KEY);
                        if (v != null)
                        {
                            if (v.ToString().ToUpperInvariant() == SSL_CERTIFICATES_CHANGED_ONLY)
                                return SSLCertificateTypes.Changed;

                            if (v.ToString().ToUpperInvariant() == SSL_CERTIFICATES_ALL)
                                return SSLCertificateTypes.All;
                        }
                        return SSLCertificateTypes.None;
                    }
                    finally
                    {
                        masterKey.Close();
                    }
                }
                catch
                {
                    return SSLCertificateTypes.None;
                }
            }
        }

        public static bool IsPowerShellInstalled()
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(PowerShellKey);
                if (masterKey == null)
                    return false;

                try
                {
                    object v = masterKey.GetValue(PowerShellStamp);
                    if (v != null && v is int && (int)v == 1)
                            return true;
                    return false;
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.Debug("It looks like PowerShell is not installed on your machine!", e);
            }
            return false;
        }

        public static bool IsXenServerSnapInInstalled()
        {
            return !string.IsNullOrEmpty(XenServerSnapInLocation());
        }

        public static bool IsExecutionPolicyNotRestricted()
        {
            return PowerShellExecutionPolicy() != "Restricted";
        }

        public static string XenServerSnapInLocation()
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(PSConsoleKey);
                if (masterKey == null)
                {
                    masterKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PSConsoleKey);
                    if (masterKey == null)
                    {
                        return "";
                    }
                }

                try
                {
                    object v = masterKey.GetValue(PSConsoleName);
                    if (v != null)
                        return v.ToString();
                    return "";
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.Debug("Failed to find XenServerPSSnapIn Console file. Assuming the XenServerPSSnapIn is not installed.", e);
            }
            return "";
        }

        public static string PowerShellExecutionPolicy()
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(PSExecutionPolicyKey);
                if (masterKey == null)
                {
                    masterKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PSExecutionPolicyKey);
                    if (masterKey == null)
                    {
                        return "";
                    }
                }

                try
                {
                    object v = masterKey.GetValue(PSExecutionPolicyName);
                    if (v != null)
                        return v.ToString();
                    return "";
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.Debug("Failed to find XenServerPSSnapIn Console file. Assuming the XenServerPSSnapIn is not installed.", e);
            }
            return "";
        }

        public static void CheckPowershell()
        {
            if (!Registry.IsPowerShellInstalled())
                throw new I18NException(I18NExceptionType.PowerShellNotPresent);

        }

        public static void CheckXenServerPSSnapIn()
        {
            CheckPowershell();

            if (!Registry.IsXenServerSnapInInstalled())
                throw new I18NException(I18NExceptionType.PowerShellSnapInNotPresent);
            
            if (!Registry.IsExecutionPolicyNotRestricted())
                throw new I18NException(I18NExceptionType.PowerShellExecutionPolicyRestricted);
        }

        /// <summary>
        /// Read a true/false key from XENCENTER_LOCAL_KEYS\k.
        /// </summary>
        /// <returns>False if the value is "false" (case insensitive), true if the key is present but not
        /// "false", def otherwise.</returns>
        private static bool ReadBool(string k, bool def)
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(XENCENTER_LOCAL_KEYS);
                if (masterKey == null)
                    return def;

                try
                {
                    object v = masterKey.GetValue(k);
                    if (v == null)
                        return def;
                    else
                        return "FALSE" != v.ToString().ToUpperInvariant();
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.DebugFormat(@"Failed to read {0}\{1} from registry; assuming {1} is {2}.", XENCENTER_LOCAL_KEYS, k, def);
                log.Debug(e, e);
                return def;
            }
        }

        /// <summary>
        /// Reads a key from HKEY_LOCAL_MACHINE\XENCENTER_LOCAL_KEYS\k.
        /// </summary>
        private static string ReadKey(string k)
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(XENCENTER_LOCAL_KEYS);
                if (masterKey == null)
                    return null;

                try
                {
                    var v = masterKey.GetValue(k);
                    return (v != null) ? v.ToString() : null;
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.DebugFormat(@"Failed to read {0}\{1} from registry; assuming NULL.", XENCENTER_LOCAL_KEYS, k);
                log.Debug(e, e);
                return null;
            }
        }

        /// <summary>
        /// Reads a key from hKey\XENCENTER_LOCAL_KEYS\k, targeting the 32-bit registry view
        /// </summary>
        private static string ReadKey(string k, RegistryHive hKey)
        {
            try
            {
                RegistryKey masterKey = RegistryKey.OpenBaseKey(hKey, RegistryView.Registry32);
                masterKey = masterKey.OpenSubKey(XENCENTER_LOCAL_KEYS) ?? null;

                if (masterKey == null)
                    return null;

                try
                {
                    var v = masterKey.GetValue(k);
                    return (v != null) ? v.ToString() : null;
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.DebugFormat(@"Failed to read {0}\{1} from registry; assuming NULL.", XENCENTER_LOCAL_KEYS, k);
                log.Debug(e, e);
                return null;
            }
        }

        /// <summary>
        /// Reads a key from XENCENTER_LOCAL_KEYS\k, trying CurrentUser first and then LocalMachine
        /// </summary>
        private static string ReadInstalledKey(string k)
        {
            var v = ReadKey(k, RegistryHive.CurrentUser);
            return (v != null) ? v : ReadKey(k, RegistryHive.LocalMachine);
        }

        public static string HealthCheckIdentityTokenDomainName
        {
            get { return ReadKey(HEALTH_CHECK_IDENTITY_TOKEN_DOMAIN_NAME); }
        }

        public static string HealthCheckUploadTokenDomainName
        {
            get { return ReadKey(HEALTH_CHECK_UPLOAD_TOKEN_DOMAIN_NAME); }
        }

        public static string HealthCheckUploadGrantTokenDomainName
        {
            get { return ReadKey(HEALTH_CHECK_UPLOAD_GRANT_TOKEN_DOMAIN_NAME); }
        }

        public static string HealthCheckUploadDomainName
        {
            get { return ReadKey(HEALTH_CHECK_UPLOAD_DOMAIN_NAME); }
        }

        public static string HealthCheckDiagnosticDomainName
        {
            get { return ReadKey(HEALTH_CHECK_DIAGNOSTIC_DOMAIN_NAME); }
        }

        public static string HealthCheckProductKey
        {
            get { return ReadKey(HEALTH_CHECK_PRODUCT_KEY); }
        }

        public static string HiddenFeatures
        {
            get { return ReadInstalledKey(HIDDEN_FEATURES); }
        }

        public static string AdditionalFeatures
        {
            get { return ReadInstalledKey(ADDITIONAL_FEATURES); }
        }

        public static string CustomUpdatesXmlLocation
        {
            get { return ReadKey(CUSTOM_UPDATES_XML_LOCATION); }
        }

        private const string SSL_CERTIFICATES_CHANGED_ONLY = "CHANGED";
        private const string SSL_CERTIFICATES_ALL = "ALL";
        private const string SSL_CERTIFICATES_KEY = "ForceSSLCertificates";
        private const string ALLOW_CREDENTIAL_SAVE = "AllowCredentialSave";
        private const string FORCE_SYSTEM_FONTS = "ForceSystemFonts";
        private const string DISABLE_PLUGINS = "DisablePlugins";
        private const string VMPR_ENABLED = "VMPREnabled";
        private const string DONT_SUDO = "DontSudo";
        private const string XENCENTER_LOCAL_KEYS = @"SOFTWARE\" + Branding.COMPANY_NAME_SHORT + @"\" + Branding.BRAND_CONSOLE;
        private const string PSConsoleKey = @"Software\" + Branding.COMPANY_NAME_SHORT + @"\XenServerPSSnapIn";
        private const string PSConsoleName = "ConsoleFile";
        private const string PSExecutionPolicyKey = @"Software\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell";
        private const string PSExecutionPolicyName = "ExecutionPolicy";
        private const string PowerShellKey = @"Software\Microsoft\PowerShell\1";
        private const string PowerShellStamp = "Install";
        private const string HEALTH_CHECK_IDENTITY_TOKEN_DOMAIN_NAME = "HealthCheckIdentityTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_TOKEN_DOMAIN_NAME = "HealthCheckUploadTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_GRANT_TOKEN_DOMAIN_NAME = "HealthCheckUploadGrantTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_DOMAIN_NAME = "HealthCheckUploadDomainName";
        private const string HEALTH_CHECK_DIAGNOSTIC_DOMAIN_NAME = "HealthCheckDiagnosticDomainName";
        private const string HEALTH_CHECK_PRODUCT_KEY = "HealthCheckProductKey";
        private const string HIDDEN_FEATURES = "HiddenFeatures";
        private const string ADDITIONAL_FEATURES = "AdditionalFeatures";
        private const string CUSTOM_UPDATES_XML_LOCATION = "CheckForUpdatesXmlLocationOverride";
    }

    public enum SSLCertificateTypes { None, Changed, All }
}
