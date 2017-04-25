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
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
    public class SummaryDetails
    {
        public SummaryDetails(string key, string value, bool errors)
        {
            Key = key;
            Value = value;
            Errors = errors;
        }

        public SummaryDetails(string key, string value)
            : this(key, value, false)
        {
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
        public bool Errors { get; private set; }
    }

    /// <summary>
    /// Concrete component class for decorators
    /// </summary>
    public class VMMappingSummary : MappingSummary
    {
        public override List<SummaryDetails> Details
        {
            get { return new List<SummaryDetails>(); }
        }
    }

    #region Decorator classes to convert VM Mappings to a summary
    public abstract class TitleSummary : MappingSummaryDecorator
    {
        private readonly VmMapping mapping;
        public TitleSummary(MappingSummary summary, VmMapping mapping)
            : base(summary)
        {
            this.mapping = mapping;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                decoratedSummary.Add(new SummaryDetails(SummaryKeyText, mapping.VmNameLabel));
                return decoratedSummary;
            }
        }

        protected abstract string SummaryKeyText
        {
            get;
        }
    }

    public class VmTitleSummary : TitleSummary
    {
        public VmTitleSummary(MappingSummary summary, VmMapping mapping)
            : base(summary, mapping)
        {
        }

        protected override string SummaryKeyText
        {
            get { return Messages.CPM_SUMMARY_KEY_MIGRATE_VM; }
        }
    }

    public class TemplateTitleSummary : TitleSummary
    {
        public TemplateTitleSummary(MappingSummary summary, VmMapping mapping)
            : base(summary, mapping)
        {
        }

        protected override string SummaryKeyText
        {
            get { return Messages.CPM_SUMMARY_KEY_MIGRATE_TEMPLATE; }
        }
    }


    public class DestinationPoolSummary : MappingSummaryDecorator
    {
        private readonly VmMapping mapping;
        private readonly IXenConnection connection;

        public DestinationPoolSummary(MappingSummary summary, VmMapping mapping, IXenConnection connection)
            : base(summary)
        {
            this.mapping = mapping;
            this.connection = connection;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                decoratedSummary.Add(new SummaryDetails(Messages.CPM_SUMMARY_KEY_DESTINATION, ResolveLabel()));
                return decoratedSummary;
            }
        }

        private string ResolveLabel()
        {
            if (mapping.XenRef is XenRef<Host>)
            {
                Host targetHost = connection.Resolve(mapping.XenRef as XenRef<Host>);

                if(targetHost == null)
                {
                    return Messages.UNKNOWN;
                }

                Pool targetPool = Helpers.GetPool(targetHost.Connection);
                if (targetPool != null)
                {
                    return targetPool.Name;
                }

                return mapping.TargetName;
            }

            return mapping.TargetName;
        }
    }

    public class TargetServerSummary : MappingSummaryDecorator
    {
        private readonly VmMapping mapping;
        private readonly IXenConnection connection;

        public TargetServerSummary(MappingSummary summary, VmMapping mapping, IXenConnection connection)
            : base(summary)
        {
            this.mapping = mapping;
            this.connection = connection;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                decoratedSummary.Add(new SummaryDetails(Messages.CPM_SUMMARY_KEY_TARGET_SERVER, ResolveLabel()));
                return decoratedSummary;
            }
        }

        private string ResolveLabel()
        {
            if (mapping.XenRef is XenRef<Host>)
            {
                Host targetHost = connection.Resolve(mapping.XenRef as XenRef<Host>);

                if (targetHost == null)
                {
                    return Messages.UNKNOWN;
                }

                return mapping.TargetName;
            }

            return Messages.CPM_SUMMARY_UNSET;
        }
    }

    public class StorageSummary : MappingSummaryDecorator
    {
        private readonly VmMapping mapping;
        private readonly IXenConnection connection;
        private const string separatorText = " -> ";

        public StorageSummary(MappingSummary summary, VmMapping mapping, IXenConnection connection)
            : base(summary)
        {
            this.mapping = mapping;
            this.connection = connection;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                AddStorageMappings(ref decoratedSummary);
                return decoratedSummary;
            }
        }

        private void AddStorageMappings(ref List<SummaryDetails> decoratedSummary)
        {
            foreach (var pair in mapping.Storage)
            {
                VDI vdi = connection.Resolve(new XenRef<VDI>(pair.Key));
                if (vdi == null)
                    continue;

                string valueToAdd = vdi.Name + separatorText + pair.Value.Name;

                if (pair.Key == mapping.Storage.First().Key)
                {
                    decoratedSummary.Add(new SummaryDetails(Messages.CPM_SUMMARY_KEY_STORAGE, valueToAdd));
                }
                else
                {
                    decoratedSummary.Add(new SummaryDetails(String.Empty, valueToAdd));
                }
            }
        }
    }

    public class NetworkSummary : MappingSummaryDecorator
    {
        private readonly VmMapping mapping;
        private readonly IXenConnection connection;
        private const string separatorText = " -> ";

        public NetworkSummary(MappingSummary summary, VmMapping mapping, IXenConnection connection)
            : base(summary)
        {
            this.mapping = mapping;
            this.connection = connection;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                AddStorageMappings(ref decoratedSummary);
                return decoratedSummary;
            }
        }

        private void AddStorageMappings(ref List<SummaryDetails> decoratedSummary)
        {
            bool addSummaryKey = true;
            foreach (var pair in mapping.Networks)
            {
                XenAPI.Network net = connection.Resolve(new XenRef<XenAPI.Network>(pair.Key));
                bool networkNotFound = net == null;
                string valueToAdd = networkNotFound ? Messages.CPM_SUMMARY_NETWORK_NOT_FOUND : net.Name;
                valueToAdd += separatorText + pair.Value.Name;
                decoratedSummary.Add(addSummaryKey
                                         ? new SummaryDetails(Messages.CPM_SUMMARY_KEY_NETWORK, valueToAdd, networkNotFound)
                                         : new SummaryDetails(String.Empty, valueToAdd, networkNotFound));
                addSummaryKey = false;
            }
        }
    }

    /// <summary>
    /// Decorator class adding a splitter to the formatting
    /// </summary>
    public class SummarySplitter : MappingSummaryDecorator
    {
        public SummarySplitter(MappingSummary summary) : base(summary) { }
        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                decoratedSummary.Add(new SummaryDetails(String.Empty, String.Empty));
                return decoratedSummary;
            }
        }
    }

    /// <summary>
    /// Decorator class adding a splitter to the formatting
    /// </summary>
    public class TransferNetworkSummary : MappingSummaryDecorator
    {
        private readonly string networkName;
        public TransferNetworkSummary(MappingSummary summary, string networkName)
            : base(summary)
        {
            this.networkName = networkName;
        }

        public override List<SummaryDetails> Details
        {
            get
            {
                List<SummaryDetails> decoratedSummary = summary.Details;
                if (!string.IsNullOrEmpty(networkName))
                    decoratedSummary.Add(new SummaryDetails(Messages.CPM_SUMMARY_KEY_TRANSFER_NETWORK, networkName));
                return decoratedSummary;
            }
        }
    } 
    #endregion

    /// <summary>
    /// Base class for decorator and component classes
    /// </summary>
    public abstract class MappingSummary
    {
        public abstract List<SummaryDetails> Details { get; }
    }

    /// <summary>
    /// Base decorator class
    /// </summary>
    public abstract class MappingSummaryDecorator : MappingSummary
    {
        protected MappingSummary summary;

        protected MappingSummaryDecorator(MappingSummary summary)
        {
            this.summary = summary;
        }
 
        public override List<SummaryDetails> Details
        {
            get { return summary != null ? summary.Details : new List<SummaryDetails>(); }
        }
    }
}
