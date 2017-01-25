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

namespace XenAdmin.Core
{
    public class XenCenterVersion
    {
        public Version Version;
        public string Name;
        public bool IsLatest;
        public string Url;
        public string Lang;
        public DateTime TimeStamp;

        public XenCenterVersion(string version_lang, string name, bool is_latest, string url, string timestamp)
        {
            ParseVersion(version_lang);
            Name = name;
            IsLatest = is_latest;
            if (url.StartsWith("/XenServer"))
                url = XenServerVersion.UpdateRoot + url;
            Url = url;
            DateTime.TryParse(timestamp, out TimeStamp);
        }

        private void ParseVersion(string version_lang)
        {
            string[] bits = version_lang.Split('.');
            List<string> ver = new List<string>();
            foreach (string bit in bits)
            {
                int num;
                if (Int32.TryParse(bit, out num))
                    ver.Add(bit);
                else
                    Lang = bit;
            }
            Version = new Version(string.Join(".", ver.ToArray()));
        }

        public override string ToString()
        {
            return Name;
        }

        public string VersionAndLang
        {
            get
            {
                if (string.IsNullOrEmpty(Lang))
                    return Version.ToString();
                return string.Format("{0}.{1}", Version.ToString(), Lang);
            }
        }
    }
}