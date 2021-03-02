﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Diagnostics;
using System.Resources;


namespace XenAdmin.Core
{
    public class BrandManager
    {
        private static readonly ResourceManager Branding = new ResourceManager("XenAdmin.Branding", typeof(BrandManager).Assembly);


        public static readonly string BrandConsole = Get("BRAND_CONSOLE");

        public static readonly string CompanyNameLegal = Get("COMPANY_NAME_LEGAL");

        public static readonly string CompanyNameShort = Get("COMPANY_NAME_SHORT");

        public static readonly string ExtensionBackup = Get("EXTENSION_BACKUP");

        public static readonly string ExtensionSearch = Get("EXTENSION_SEARCH");

        public static readonly string ExtensionUpdate = Get("EXTENSION_UPDATE");

        public static readonly string LegacyProduct = Get("LEGACY_PRODUCT");

        public static readonly string PerfAlertMailDefaultLanguage = Get("PERF_ALERT_MAIL_DEFAULT_LANGUAGE");

        public static readonly string ProductBrand = Get("PRODUCT_BRAND");

        public static readonly string ProductBrandWithCompany = Get("PRODUCT_BRAND_WITH_COMPANY");

        public static readonly string ProductServer = Get("PRODUCT_SERVER");

        public static readonly string ProductVersion56 = Get("PRODUCT_VERSION_5_6");

        public static readonly string ProductVersion65 = Get("PRODUCT_VERSION_6_5");

        public static readonly string ProductVersion70 = Get("PRODUCT_VERSION_7_0");

        public static readonly string ProductVersion712 = Get("PRODUCT_VERSION_7_1_2");

        public static readonly string ProductVersion80 = Get("PRODUCT_VERSION_8_0");

        public static readonly string ProductVersion81 = Get("PRODUCT_VERSION_8_1");

        public static readonly string ProductVersion82 = Get("PRODUCT_VERSION_8_2");

        public static readonly string ProductVersionPost82 = Get("PRODUCT_VERSION_POST_8_2");

        public static readonly string ProductVersionText = Get("PRODUCT_VERSION_TEXT");

        public static readonly string UpdatesUrl = Get("UPDATES_URL");

        public static readonly string VmTools = Get("VM_TOOLS");

        public static readonly string XenCenterVersion = Get("XENCENTER_VERSION");


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
