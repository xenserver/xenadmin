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
using System.Diagnostics;

namespace XenAdmin.Core
{
    [DebuggerDisplay("XenServerVersion (Name={Name}; Patches.Count={Patches.Count}; MinimalPatches.Count={MinimalPatches.Count})")]
    public class XenServerVersion
    {
        public Version Version;
        public string Name;
        public bool Latest;
        public string Url;
        public string Oem;
        public List<XenServerPatch> Patches;
        
        /// <summary>
        /// A host of this version is considered up-to-date when it has all the patches in this list installed on it
        /// <value>null</value> means that the list is not known (automated updates are not supported)
        /// </summary>
        public List<XenServerPatch> MinimalPatches;
        
        public DateTime TimeStamp;
        public const string UpdateRoot = @"http://updates.xensource.com";
        public string BuildNumber;

        /// <summary>
        /// Defines metadata for a XenServer version
        /// </summary>
        /// <param name="version_oem"></param>
        /// <param name="name"></param>
        /// <param name="latest"></param>
        /// <param name="url"></param>
        /// <param name="patches"></param>
        /// <param name="minimumPatches">can be null (see <paramref name="MinimalPatches"/></param>
        /// <param name="timestamp"></param>
        /// <param name="buildNumber"></param>
        public XenServerVersion(string version_oem, string name, bool latest, string url, List<XenServerPatch> patches, List<XenServerPatch> minimumPatches,
            string timestamp, string buildNumber)
        {
            ParseVersion(version_oem);
            Name = name;
            Latest = latest;
            if (url.StartsWith("/XenServer"))
                url = UpdateRoot + url;
            Url = url;
            Patches = patches;
            MinimalPatches = minimumPatches;
            DateTime.TryParse(timestamp, out TimeStamp);
            BuildNumber = buildNumber;
        }

        private void ParseVersion(string version_oem)
        {
            string[] bits = version_oem.Split('.');
            List<string> ver = new List<string>();
            foreach (string bit in bits)
            {
                int num;
                if (Int32.TryParse(bit, out num))
                    ver.Add(bit);
                else
                    Oem = bit;
            }
            Version = new Version(string.Join(".", ver.ToArray()));
        }

        public string VersionAndOEM
        {
            get
            {
                if (string.IsNullOrEmpty(Oem))
                    return Version.ToString();
                return string.Format("{0}.{1}", Version.ToString(), Oem);
            }
        }



    }
}