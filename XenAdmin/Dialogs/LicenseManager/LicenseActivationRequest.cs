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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public interface ILicenseActivationRequest
    {
        ReadOnlyCollection<Host> HostsThatCanBeActivated { get; }
        MemoryStream CreateRequestBestEffort();
        Encoding RequestEncoding { get; }
        List<Host> Hosts { set; }
        bool AllHostsCanBeActivated { get; }
    }

    public class LicenseActivationRequest : ILicenseActivationRequest
    {
        public List<Host> Hosts { private get; set; }

        public LicenseActivationRequest(){}

        public LicenseActivationRequest(List<Host> hosts)
        {
            Hosts = hosts;
        }

        public static bool CanActivate(Host host)
        {
            return host.IsFreeLicense() && !Helpers.ClearwaterOrGreater(host);
        }

        public static bool CanActivate(Pool pool)
        {
            return pool.Connection.Cache.Hosts.Any(CanActivate);
        }

        public ReadOnlyCollection<Host> HostsThatCanBeActivated
        {
            get
            {
                return Hosts.Where(CanActivate).ToList().AsReadOnly();
            }
        }

        public bool AllHostsCanBeActivated
        {
            get
            {
                return Hosts.TrueForAll(CanActivate);
            }
        }

        public MemoryStream CreateRequestBestEffort()
        {
            return Request(HostsThatCanBeActivated);
        }

        public Encoding RequestEncoding { get; private set; }

        private MemoryStream Request(IEnumerable<Host> selection)
        {
            MemoryStream ms = new MemoryStream();
            RequestEncoding = new UTF8Encoding(false);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = RequestEncoding;
            XmlWriter writer = XmlWriter.Create(ms, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("reactivation_request");
            foreach (Host host in selection)
            {
                writer.WriteStartElement("host");
                writer.WriteAttributeString("uuid", host.uuid);
                foreach (KeyValuePair<String, String> kvp in host.software_version)
                {
                    writer.WriteStartElement("software_version_element");
                    writer.WriteAttributeString("key", kvp.Key);
                    writer.WriteAttributeString("value", kvp.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return ms;
        }

    }
}
