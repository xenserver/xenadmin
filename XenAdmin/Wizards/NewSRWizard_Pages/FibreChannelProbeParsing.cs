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
using System.Xml;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    class FibreChannelProbeParsing
    {
        internal static List<FibreChannelDevice> ProcessXML(string p)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(p);

            var devices = new List<FibreChannelDevice>();

            foreach (XmlNode device in doc.GetElementsByTagName("BlockDevice"))
            {
                var dev = ParseDevice(device);
                devices.Add(dev);
            }
            return devices;
        }

        private static FibreChannelDevice ParseDevice(XmlNode device)
        {
            string vendor = "";
            long size = 0;
            string serial = "";
            string path = "";
            string scsiid = "";
            string adapter = "";
            string channel = "";
            string id = "";
            string lun = "";
            string name_label = "";
            string name_description = "";
            bool pool_metadata_detected = false;
            string eth = "";

            foreach (XmlNode node in device.ChildNodes)
            {
                if (node.Name.ToLowerInvariant() == "vendor")
                    vendor = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "size")
                    size = ParseSizeWithUnits(node.InnerText.Trim());
                if (node.Name.ToLowerInvariant() == "serial")
                    serial = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "path")
                    path = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "scsiid")
                    scsiid = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "adapter")
                    adapter = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "channel")
                    channel = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "id")
                    id = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "lun")
                    lun = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "name_label")
                    name_label = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "name_description")
                    name_description = node.InnerText.Trim();
                if (node.Name.ToLowerInvariant() == "pool_metadata_detected")
                    pool_metadata_detected = bool.Parse(node.InnerText.Trim());
                if (node.Name.ToLowerInvariant() == "eth")
                    eth = node.InnerText.Trim();
            }

            return new FibreChannelDevice(serial, path, vendor, size, 
                scsiid, adapter, channel, id, lun, name_label, name_description, pool_metadata_detected, eth);
        }

        /// <summary>
        /// Sometimes, the XML that is returned contains units.  I think that we're not doing this in
        /// Miami GA.
        /// </summary>
        private static long ParseSizeWithUnits(string p)
        {
            p = p.ToLowerInvariant();

            if (p.Contains("kb"))
            {
                return long.Parse(p.Replace("kb", "")) * Util.BINARY_KILO;
            }
            else if (p.Contains("mb"))
            {
                return long.Parse(p.Replace("mb", "")) * Util.BINARY_MEGA;
            }
            else if (p.Contains("gb"))
            {
                return long.Parse(p.Replace("gb", "")) * Util.BINARY_GIGA;
            }
            else
            {
                return long.Parse(p);
            }
        }
    }
}
