/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Diagnostics;
using System.Reflection;
using System.Resources;
using XenAdmin.Properties;


namespace XenAdmin.Core
{
    public class BrandManager
    {
        private static readonly ResourceManager Branding = new ResourceManager("XenAdmin.Branding", typeof(BrandManager).Assembly);

        static BrandManager()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var customBranding = (CustomBrandingAttribute)assembly.GetCustomAttribute(typeof(CustomBrandingAttribute));

            BrandConsole = customBranding.BrandConsole;
            BrandConsoleNoSpace = customBranding.BrandConsoleNoSpace;
            CompanyNameShort = customBranding.CompanyNameShort;
            ProductBrand = customBranding.ProductBrand;
            ProductVersionPost82 = customBranding.ProductVersionText;
            XcUpdatesUrl = customBranding.XcUpdatesUrl;
            CfuUrl = customBranding.CfuUrl;
            VmTools = customBranding.VmTools;
            XenHost = customBranding.XenHost;
        }


        public static readonly string BrandConsole;

        public static readonly string BrandConsoleNoSpace;

        public static readonly string Cis = Get("CIS");

        public static readonly string CompanyNameLegacy = Get("COMPANY_NAME_LEGACY");

        public static readonly string CompanyNameShort;

        public static readonly string Copyright = Get("COPYRIGHT");

        public static readonly string ExtensionBackup = Get("EXTENSION_BACKUP");

        public static readonly string ExtensionSearch = Get("EXTENSION_SEARCH");

        public static readonly string ExtensionUpdate = Get("EXTENSION_UPDATE");

        public static readonly string HelpPath = Get("HELP_PATH");

        public static readonly string LegacyProduct = Get("LEGACY_PRODUCT");

        public static readonly string PerfAlertMailDefaultLanguage = Get("PERF_ALERT_MAIL_DEFAULT_LANGUAGE");

        public static readonly string ProductBrand;

        public static readonly string ProductVersion70 = Get("PRODUCT_VERSION_7_0");

        public static readonly string ProductVersion70Short = Get("PRODUCT_VERSION_7_0_SHORT");

        public static readonly string ProductVersion712 = Get("PRODUCT_VERSION_7_1_2");

        public static readonly string ProductVersion712Short = Get("PRODUCT_VERSION_7_1_2_SHORT");

        public static readonly string ProductVersion80 = Get("PRODUCT_VERSION_8_0");

        public static readonly string ProductVersion81 = Get("PRODUCT_VERSION_8_1");

        public static readonly string ProductVersion82 = Get("PRODUCT_VERSION_8_2");

        public static readonly string ProductVersion82Short = Get("PRODUCT_VERSION_8_2_SHORT");

        public static readonly string ProductVersion821 = Get("PRODUCT_VERSION_8_2_1");

        public static readonly string ProductVersionPost82;

        public static readonly string Trademarks = Get("TRADEMARKS");

        public static readonly string XcUpdatesUrl;

        public static readonly string CfuUrl;

        public static readonly string VmTools;

        public static readonly string XenHost;


        /// <summary>
        /// Returns null if no match is found.
        /// </summary>
        public static string Get(string s)
        {
            var result = Branding.GetString(s);
#if DEBUG
            Debug.Assert(result != null, $"{s} doesn't exist in Branding");
#endif
            return result;
        }
    }
}
