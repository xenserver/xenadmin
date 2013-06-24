/* Copyright (c) Citrix Systems Inc. 
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
using XenAPI;
using System.IO;
using System.Xml;
using System.Net;
using XenAdmin.Core;

namespace XenAdmin.Actions
{
    public class DownloadUpdatesXmlAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string XenCenterVersionsNode = "xencenterversions";
        private const string XenServerVersionsNode = "serverversions";
        private const string PatchesNode = "patches";
        private const string UpdateXmlUrl = @"http://updates.xensource.com/XenServer/updates.xml";

        public List<XenCenterVersion> XenCenterVersions { get; set; }
        public List<XenServerVersion> XenServerVersions { get; set; }
        public List<XenServerPatch> XenServerPatches { get; set; }

        public DownloadUpdatesXmlAction()
            : base(null, "_get_updates", "_get_updates", true)
        {
            XenServerPatches = new List<XenServerPatch>();
            XenServerVersions = new List<XenServerVersion>();
            XenCenterVersions = new List<XenCenterVersion>();
        }

        protected override void Run()
        {
            this.Description = Messages.AVAILABLE_UPDATES_SEARCHING;

            XmlDocument xdoc = FetchCheckForUpdatesXml(UpdateXmlUrl);

            // XenCenter Versions
            foreach (XmlNode versions in xdoc.GetElementsByTagName(XenCenterVersionsNode))
            {
                foreach (XmlNode version in versions.ChildNodes)
                {
                    string version_lang = "";
                    string name = "";
                    bool is_latest = false;
                    string url = "";
                    string timestamp = "";

                    foreach (XmlAttribute attrib in version.Attributes)
                    {
                        if (attrib.Name == "value")
                            version_lang = attrib.Value;
                        else if (attrib.Name == "name")
                            name = attrib.Value;
                        else if (attrib.Name == "latest")
                            is_latest = attrib.Value.ToUpperInvariant() == bool.TrueString.ToUpperInvariant();
                        else if (attrib.Name == "url")
                            url = attrib.Value;
                        else if (attrib.Name == "timestamp")
                            timestamp = attrib.Value;
                    }

                    XenCenterVersions.Add(new XenCenterVersion(version_lang, name, is_latest, url, timestamp));
                }
            }

            // Patches
            foreach (XmlNode versions in xdoc.GetElementsByTagName(PatchesNode))
            {
                foreach (XmlNode version in versions.ChildNodes)
                {
                    string uuid = "";
                    string name = "";
                    string description = "";
                    string guidance = "";
                    string patchVersion = "";
                    string url = "";
                    string patchUrl = "";
                    string timestamp = "";
                    string priority = "";

                    foreach (XmlAttribute attrib in version.Attributes)
                    {
                        if (attrib.Name == "uuid")
                            uuid = attrib.Value;
                        else if (attrib.Name == "name-label")
                            name = attrib.Value;
                        else if (attrib.Name == "name-description")
                            description = attrib.Value;
                        else if (attrib.Name == "after-apply-guidance")
                            guidance = attrib.Value;
                        else if (attrib.Name == "version")
                            patchVersion = attrib.Value;
                        else if (attrib.Name == "url")
                            url = attrib.Value;
                        else if (attrib.Name == "patch-url")
                            patchUrl = attrib.Value;
                        else if (attrib.Name == "timestamp")
                            timestamp = attrib.Value;
                        else if (attrib.Name == "priority")
                            priority = attrib.Value;
                    }

                    XenServerPatches.Add(new XenServerPatch(uuid, name, description, guidance, patchVersion, url,
                                                            patchUrl, timestamp, priority));
                }
            }

            // XenServer Versions
            foreach (XmlNode versions in xdoc.GetElementsByTagName(XenServerVersionsNode))
            {
                foreach (XmlNode version in versions.ChildNodes)
                {
                    string version_oem = "";
                    string name = "";
                    bool is_latest = false;
                    string url = "";
                    string timestamp = "";
                    string buildNumber = "";

                    foreach (XmlAttribute attrib in version.Attributes)
                    {
                        if (attrib.Name == "value")
                            version_oem = attrib.Value;
                        else if (attrib.Name == "name")
                            name = attrib.Value;
                        else if (attrib.Name == "latest")
                            is_latest = attrib.Value.ToUpperInvariant() == bool.TrueString.ToUpperInvariant();
                        else if (attrib.Name == "url")
                            url = attrib.Value;
                        else if (attrib.Name == "timestamp")
                            timestamp = attrib.Value;
                        else if (attrib.Name == "build-number")
                            buildNumber = attrib.Value;
                    }

                    List<XenServerPatch> patches = new List<XenServerPatch>();

                    foreach (XmlNode childnode in version.ChildNodes)
                    {
                        if (childnode.Name != "patch")
                            continue;
                        XenServerPatch patch = XenServerPatches.Find(new Predicate<XenServerPatch>(delegate(XenServerPatch item) { return item.Uuid == childnode.Attributes["uuid"].Value; }));
                        if (patch == null)
                            continue;
                        patches.Add(patch);
                    }

                    XenServerVersions.Add(new XenServerVersion(version_oem, name, is_latest, url, patches, timestamp,
                                                               buildNumber));
                }
            }
        }

        protected virtual XmlDocument FetchCheckForUpdatesXml(string location)
        {
            XmlDocument xdoc;
            using (Stream xmlstream = HTTPHelper.GET(new Uri(location), Connection, false))
            {
                xdoc = Helpers.LoadXmlDocument(xmlstream);
            }
            return xdoc;
        }
    }
}
