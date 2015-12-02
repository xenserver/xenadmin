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
using System.Linq;
using System.Text;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard.Filters
{
    public class CrossPoolMigrateCanMigrateFilter : ReasoningFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<VM> preSelectedVMs;
        private readonly List<FailureReason> failureReasons = new List<FailureReason>();

        /// <summary>
        /// Helper class used for determining if the hosts in the failure 
        /// reason are equal. Used in the distinct function of the enumerable classes
        /// </summary>
        private class FailingHostComparer : IEqualityComparer<FailureReason>
        {
            public bool Equals(FailureReason x, FailureReason y)
            {
                return x.Host.Equals(y.Host);
            }

            public int GetHashCode(FailureReason obj)
            {
                return obj.Host.GetHashCode();
            }
        }

        public CrossPoolMigrateCanMigrateFilter(IXenObject itemAddedToComboBox, List<VM> preSelectedVMs)
            : base(itemAddedToComboBox)
        {
            this.preSelectedVMs = preSelectedVMs;
        }

        private struct FailureReason
        {
            public string Reason;
            public VM Vm;
            public Host Host;
        }

        public override bool FailureFound
        {
            get
            {
                List<Host> targets = CollateHosts();
                failureReasons.Clear();

                foreach (Host host in targets)
                {
                    if (preSelectedVMs == null)
                        throw new NullReferenceException("Pre-selected VMs are null");

                    var targetSR = GetDefaultSROrAny(host);
                    var targetNetwork = GetANetwork(host);

                    foreach (VM vm in preSelectedVMs)
                    {
                        try
                        {
                            //Skip the resident host as there's a filter for it and 
                            //if not then you could exclude intrapool migration
                            if (vm.resident_on == host.opaque_ref)
                                continue;

                            PIF managementPif = host.Connection.Cache.PIFs.First(p => p.management);
                            XenAPI.Network network = host.Connection.Cache.Resolve(managementPif.network);

                            Session session = host.Connection.DuplicateSession();
                            Dictionary<string, string> receiveMapping = Host.migrate_receive(session, host.opaque_ref, network.opaque_ref, new Dictionary<string, string>());
                            VM.assert_can_migrate(vm.Connection.Session,
                                                  vm.opaque_ref,
                                                  receiveMapping,
                                                  true,
                                                  GetVdiMap(vm, targetSR),
                                                  vm.Connection == host.Connection ? new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>() : GetVifMap(vm, targetNetwork),
                                                  new Dictionary<string, string>());
                        }
                        catch (Failure failure)
                        {
                            failureReasons.Add(new FailureReason{Reason = failure.Message, Host = host, Vm = vm});
                        }
                    }
                }

                return DetermineIfFailureGivenReasons();
            }
        }

        /// <summary>
        /// If we have a pool, then we need to fail only when there are failures for 
        /// each of the hosts. Otherwise any failure counts.
        /// </summary>
        /// <returns></returns>
        private bool DetermineIfFailureGivenReasons()
        {
            List<FailureReason> distinctFails = failureReasons.Distinct(new FailingHostComparer()).ToList();

            if (ItemToFilterOn is Pool)
            {
                Pool pool = ItemToFilterOn as Pool;
                return distinctFails.Count >= pool.Connection.Cache.HostCount;
            }

            return failureReasons.Count > 0;
        }

        private List<Host> CollateHosts()
        {
            List<Host> target = new List<Host>();
            if (ItemToFilterOn is Host)
            {
                target.Add(ItemToFilterOn as Host);
            }

            if (ItemToFilterOn is Pool)
            {
                Pool pool = ItemToFilterOn as Pool;
                target.AddRange(pool.Connection.Cache.Hosts);
            }
            return target;
        }

        public override string Reason
        {
            get
            {
                if(failureReasons.Count > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (FailureReason pair in failureReasons)
                    {
                        sb.AppendLine(string.Format("VM: {0}, Host: {1} - Reason: {2};", 
                                      pair.Vm.opaque_ref, pair.Host.opaque_ref, pair.Reason));
                    }

                    log.ErrorFormat("{0} not suitable for migration for multiple reasons: {1}", 
                                    ItemToFilterOn, sb); 
                }

                //It's expected that FailureFound is called prior to the Reason property
                //If not this should throw an exception
                return failureReasons.FirstOrDefault().Reason;
                
            }
        }

        private SR GetDefaultSROrAny(Host host)
        {
            // try default SR or any other SR that supports VDI create
            var pool = Helpers.GetPoolOfOne(host.Connection);
            if (pool != null)
                return host.Connection.Resolve(pool.default_SR) ?? host.Connection.Cache.SRs.FirstOrDefault(sr => sr.SupportsVdiCreate());

            return null;
        }

        private Dictionary<XenRef<VDI>, XenRef<SR>> GetVdiMap(VM vm, SR targetSR)
        {
            var vdiMap = new Dictionary<XenRef<VDI>, XenRef<SR>>();

            if (targetSR != null)
            {
                List<VDI> vdis = vm.Connection.ResolveAll(vm.VBDs).Select(v => vm.Connection.Resolve(v.VDI)).ToList();
                vdis.RemoveAll(vdi => vdi == null || vm.Connection.Resolve(vdi.SR).GetSRType(true) == SR.SRTypes.iso);

                foreach (var vdi in vdis)
                {
                    vdiMap.Add(new XenRef<VDI>(vdi.opaque_ref), new XenRef<SR>(targetSR));
                }
            }
            return vdiMap;
        }

        private XenAPI.Network GetANetwork(Host host)
        {
            return host.Connection.Cache.Networks.FirstOrDefault(network => host.CanSeeNetwork(network));
        }

        private Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>> GetVifMap(VM vm, XenAPI.Network targetNetwork)
        {
            var vifMap = new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>();

            if (targetNetwork != null)
            {
                List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);

                foreach (var vif in vifs)
                {
                    vifMap.Add(new XenRef<VIF>(vif.opaque_ref), new XenRef<XenAPI.Network>(targetNetwork));
                }
            }
            return vifMap;
        }
    }
}
