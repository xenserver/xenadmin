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
using XenAdmin;
using XenAdmin.Network;


namespace XenAPI
{
    public partial class Subject
    {
        /// <summary>
        /// Extracts a key value pair list of information on the subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, String>> ExtractKvpInfo(Subject subject)
        {
            List<KeyValuePair<String, String>> kvpList = new List<KeyValuePair<string, string>>();
            Dictionary<string, string> info = new Dictionary<string, string>(subject.other_config);
            // Trim undesirable entries
            info.Remove("subject-gid");
            info.Remove("subject-uid");
            info.Remove("subject-sid");
            info.Remove("subject-is-group");
            info.Remove("subject-gecos");
            info.Remove("subject-upn");

            if (info.Count == 0)
                return kvpList;

            string s;
            // Try and extract the important info as headers. No need for a label, just put the value as the key of the kvp
            if (info.TryGetValue("subject-displayname", out s))
            {
                kvpList.Add(new KeyValuePair<string, string>(s, ""));
                info.Remove("subject-displayname");
            }
            if (info.TryGetValue("subject-name", out s))
            {
                kvpList.Add(new KeyValuePair<string, string>(s, ""));
                info.Remove("subject-name");
            }

            // Add a double blank entry which is drawn as a vertical space and then add remaining kvps
            kvpList.Add(new KeyValuePair<string, string>("", ""));
            foreach (KeyValuePair<string, string> kvp in info)
            {
                // Try to localise the property key name
                string keyText, valueText;
                keyText = XenAdmin.Core.PropertyManager.GetFriendlyName(string.Format("AD.PropertyKey-{0}", kvp.Key));
                if (keyText == null)
                    keyText = kvp.Key;

                if (kvp.Value == "true")
                    valueText = Messages.YES;
                else if (kvp.Value == "false")
                    valueText = Messages.NO;
                else
                    valueText = kvp.Value;

                keyText += Messages.GENERAL_PAGE_KVP_SEPARATOR;
                kvpList.Add(new KeyValuePair<string, string>(keyText, valueText));
            }
            return kvpList;
        }

        public override string Name
        {
            get { return SubjectName; }
        }

        public static Subject GetBySID(IXenConnection connection, string sid)
        {
            foreach (Subject s in connection.Cache.Subjects)
            {
                if (s.subject_identifier == sid)
                    return s;
            }
            return null;
        }
    }
}
