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

using System.Collections.Generic;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.StorageLinkAPI;


namespace XenAdmin.Actions
{
    public class SrCslgAdaptersScanAction : PureAsyncAction
    {
        public SrCslgAdaptersScanAction(IXenConnection connection)
            : base(connection, Messages.SCANNING_ADAPTERS, Messages.SCANNING_ADAPTERS)
        {
        }

        protected override void Run()
        {
            RelatedTask = XenAPI.SR.async_probe(Session, Helpers.GetMaster(Connection).opaque_ref,
                                                new Dictionary<string, string>(), "cslg",
                                                new Dictionary<string, string>());
            PollToCompletion();
            Description = Messages.COMPLETED;
        }

        private IEnumerable<StorageLinkAdapterBoston> ParseStorageSystemsXmlBoston()
        {

            var output = new List<StorageLinkAdapterBoston>();
            var doc = new XmlDocument();
            doc.LoadXml(Util.GetContentsOfValueNode(Result));

            foreach (XmlNode adapter in doc.GetElementsByTagName("Adapter"))
            {
                string id = Util.GetXmlNodeInnerText(adapter, "Name").Trim();
                string name = Util.GetXmlNodeInnerText(adapter, "FriendlyName").Trim();
                string description = Util.GetXmlNodeInnerText(adapter, "Description").Trim();
                string type = Util.GetXmlNodeInnerText(adapter, "Type").Trim();
                string replicationtype = Util.GetXmlNodeInnerText(adapter, "ReplicationType").Trim();
                string versionmajor = Util.GetXmlNodeInnerText(adapter, "VersionMajor").Trim();
                string versionminor = Util.GetXmlNodeInnerText(adapter, "VersionMinor").Trim();


                output.Add(new StorageLinkAdapterBoston(id, name, description, type, replicationtype, versionmajor, versionminor));
            }
            return output;
        }

        public IEnumerable<StorageLinkAdapterBoston> GetAdapters()
        {
            return ParseStorageSystemsXmlBoston();
        }
    }
}