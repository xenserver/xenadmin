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
using System.Threading;
using System.Xml;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Asks the server for a list of NetApp aggregates and SRs on a given filer.
    /// </summary>
    public class SrScanAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string hostname;
        private readonly string username;
        private readonly string password;
        private readonly SR.SRTypes type;

        private List<SR.SRInfo> srs;

        private List<NetAppAggregate> aggregates;
        private List<DellStoragePool> storagePools;

        public SrScanAction(IXenConnection connection, string hostname, string username, 
            string password, SR.SRTypes type)
            : base(connection, String.Format(Messages.ACTION_SR_SCAN_NAME, hostname),
            String.Format(Messages.ACTION_SR_SCAN_DESCRIPTION, XenAPI.SR.getFriendlyTypeName(type), hostname), true)
        {
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.type = type;
        }

        /// <summary>
        /// Will be null if the scan has not yet successfully returned.
        /// </summary>
        public List<SR.SRInfo> SRs
        {
            get
            {
                return srs;
            }
        }

        /// <summary>
        /// Will be null if the scan has not yet successfully returned.
        /// </summary>
        public List<NetAppAggregate> Aggregates
        {
            get
            {
                return aggregates;
            }
        }

        /// <summary>
        /// Will be null if the scan has not yet successfully returned.
        /// </summary>
        public List<DellStoragePool> StoragePools
        {
            get
            {
                return storagePools;
            }
        }

        protected override void Run()
        {
            // Fill in dictionary params
            Dictionary<String, String> dconf = new Dictionary<String, String>();
            dconf.Add("target", hostname);
            dconf.Add("username", username);
            dconf.Add("password", password);

            log.DebugFormat("Attempting to find SRs on {0} filer {1}.", type, hostname);

            RelatedTask = XenAPI.SR.async_probe(Session, Helpers.GetMaster(Connection).opaque_ref,
                dconf, type.ToString(), new Dictionary<String, String>());
            this.PollToCompletion();
            srs = XenAPI.SR.ParseSRListXML(this.Result);

            log.DebugFormat("Attempting to find aggregates on {0} filer {1}.", type, hostname);

            try
            {
                RelatedTask = XenAPI.SR.async_create(Session, hostname, dconf, 0,
                    Helpers.GuiTempObjectPrefix, "", type.ToString(), "", true, 
                    new Dictionary<String, String>());

                this.PollToCompletion(50, 100);

                throw new BadServerResponse(hostname);
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription.Count < 1)
                    throw new BadServerResponse(hostname);
    
                // We expect a particular sort of failure, whose error details contain
                // the list of aggregates on the filer in XML.

                switch (exn.ErrorDescription[0])
                {
                    case "SR_BACKEND_FAILURE_123":
                        // We want a custom error if the server returns no iqns.
                        if (exn.ErrorDescription.Count < 4 || exn.ErrorDescription[3].Length == 0)
                            break;

                        aggregates = ParseAggregateXML(exn.ErrorDescription[3], hostname);

                        break;

                    case "SR_BACKEND_FAILURE_163":
                        // We want a custom error if the server returns no iqns.
                        if (exn.ErrorDescription.Count < 4 || exn.ErrorDescription[3].Length == 0)
                            break;

                        storagePools = ParseDellStoragePoolsXML(exn.ErrorDescription[3], hostname);

                        break;

                    default:
                        throw;
                }

                if (ListIsNullOrEmpty(srs) 
                    && ListIsNullOrEmpty(aggregates) 
                    && ListIsNullOrEmpty(storagePools))
                    throw new NoExistingAndNowhereToCreateException(hostname);                    
            }
        }

        public static bool ListIsNullOrEmpty<T>(List<T> list)
        {
            return list == null || list.Count <= 0;
        }

        private static List<NetAppAggregate> ParseAggregateXML(String xml, String hostname)
        {
            List<NetAppAggregate> results = new List<NetAppAggregate>();

            try
            {
                // Parse XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                foreach (XmlNode aggr in doc.GetElementsByTagName("Aggr"))
                {
                    string name = "";
                    long size = -1;
                    int disks = -1;
                    string raidType = "";
                    bool asisCapable = false;
                    foreach (XmlNode info in aggr.ChildNodes)
                    {
                        // use name for build < 7040, aggregate after this
                        if (info.Name.ToLowerInvariant() == "name" || info.Name.ToLowerInvariant() == "aggregate")
                        {
                            name = info.InnerText.Trim();
                        }
                        else if (info.Name.ToLowerInvariant() == "size")
                        {
                            size = long.Parse(info.InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (info.Name.ToLowerInvariant() == "disks")
                        {
                            disks = int.Parse(info.InnerText.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (info.Name.ToLowerInvariant() == "raidtype")
                        {
                            raidType = info.InnerText.Trim();
                        }
                        else if (info.Name.ToLowerInvariant() == "asis_dedup")
                        {
                            asisCapable = bool.Parse(info.InnerText.Trim().ToLowerInvariant());
                        }
                    }

                    results.Add(new NetAppAggregate(name, size, disks, raidType, asisCapable));
                }

                results.Sort();
            }
            catch
            {
                throw new BadServerResponse(hostname);
            }

            return results;
        }

        private static List<DellStoragePool> ParseDellStoragePoolsXML(String xml, String hostname)
        {
            List<DellStoragePool> results = new List<DellStoragePool>();

            try
            {
                // Parse XML
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                foreach (XmlNode aggr in doc.GetElementsByTagName("StoragePool"))
                {
                    String Name = "";
                    bool Default = false;
                    int Members = 0;
                    int Volumes = 0;
                    long Capacity = 0;
                    long Freespace = 0;

                    foreach (XmlNode info in aggr.ChildNodes)
                    {
                        switch(info.Name.ToLowerInvariant())
                        {
                            case "name":
                                Name = info.InnerText.Trim();
                                break;

                            case "default":
                                bool.TryParse(info.InnerText.Trim(), out Default);
                                break;

                            case "members":
                                int.TryParse(info.InnerText.Trim(), out Members);
                                break;

                            case "volumes":
                                int.TryParse(info.InnerText.Trim(), out Volumes);
                                break;

                            case "capacity":
                                long.TryParse(info.InnerText.Trim(), out Capacity);
                                break;

                            case "freespace":
                                long.TryParse(info.InnerText.Trim(), out Freespace);
                                break;
                        }
                    }

                    results.Add(new DellStoragePool(Name, Default, Members, Volumes, Capacity, Freespace));
                }

                results.Sort();
            }
            catch
            {
                throw new BadServerResponse(hostname);
            }

            return results;
        }
    }

    public struct NetAppAggregate : IComparable<NetAppAggregate>
    {
        public readonly string Name;
        public readonly long Size;
        public readonly int Disks;
        public readonly string RaidType;
        public readonly bool AsisCapable;

        public NetAppAggregate(string name, long size, int disks, string raidType, bool asisCapable)
        {
            this.Name = name;
            this.Size = size;
            this.Disks = disks;
            this.RaidType = raidType;
            this.AsisCapable = asisCapable;
        }

        public int CompareTo(NetAppAggregate other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public struct DellStoragePool : IComparable<DellStoragePool>
    {
        public readonly String Name;
        public readonly bool Default;
        public readonly int Members;
        public readonly int Volumes;
        public readonly long Capacity;
        public readonly long Freespace;

        public DellStoragePool(String Name, bool Default, int Members, 
            int Volumes, long Capacity, long Freespace)
        {
            this.Name = Name;
            this.Default = Default;
            this.Members = Members;
            this.Volumes = Volumes;
            this.Capacity = Capacity;
            this.Freespace = Freespace;
        }

        public int CompareTo(DellStoragePool other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public class NoExistingAndNowhereToCreateException : Exception
    {
        private String host;

        public NoExistingAndNowhereToCreateException(String host)
        {
            this.host = host;
        }

        public override string Message
        {
            get
            {
                return Messages.NEWSR_NOWHERE_TO_CREATE;
            }
        }
    }
}
