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

        public static string UpdatesUrl => Get("UPDATES_URL");

        public static string PerfAlertMailDefaultLanguage => Get("PERF_ALERT_MAIL_DEFAULT_LANGUAGE");

        public static string ExtensionSearch => Get("EXTENSION_SEARCH");

        public static string ExtensionUpdate => Get("EXTENSION_UPDATE");

        public static string ExtensionBackup => Get("EXTENSION_BACKUP");

        public static string LegacyProduct => Get("LEGACY_PRODUCT");

        public static string ProductVersion56 => Get("PRODUCT_VERSION_5_6");

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
