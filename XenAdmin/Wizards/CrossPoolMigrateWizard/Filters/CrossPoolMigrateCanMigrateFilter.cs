/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard.Filters
{
    public class CrossPoolMigrateCanMigrateFilter : ReasoningFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly WizardMode _wizardMode;
        private readonly List<VM> _preSelectedVMs;
        private Dictionary<string, Dictionary<string, string>> _cache;
        private bool _canceled;
        private static readonly object _cacheLock = new object();

        public CrossPoolMigrateCanMigrateFilter(IXenObject itemAddedToFilterOn, List<VM> preSelectedVMs, WizardMode wizardMode, Dictionary<string, Dictionary<string, string>> cache = null)
            : base(itemAddedToFilterOn)
        {
            _wizardMode = wizardMode;
            _cache = cache ?? new Dictionary<string, Dictionary<string, string>>();
            _preSelectedVMs = preSelectedVMs ?? throw new ArgumentNullException(nameof(preSelectedVMs));
        }

        public override void Cancel()
        {
            _canceled = true;
        }

        protected override bool FailureFoundFor(IXenObject itemToFilterOn, out string failureReason)
        {
            //Rules:
            //- if at least one VM cannot be migrated to the target itemToFilterOn, this method returns true
            //- if the itemToFilterOn is a pool, the VM is considered migratable if it can be migrated
            //  to at least one of the pool hosts

            failureReason = string.Empty;
            List<Host> targets = CollateHosts(itemToFilterOn, out var targetPool);

            foreach (VM vm in _preSelectedVMs)
            {
                log.InfoFormat("Asserting can migrate VM {0} to {1}...", vm.Name(), itemToFilterOn);

                //CA-220218: for intra-pool motion of halted VMs we do a move, so no need to assert we can migrate
                Pool vmPool = Helpers.GetPoolOfOne(vm.Connection);
                if (_wizardMode == WizardMode.Move && vmPool != null && targetPool != null && vmPool.opaque_ref == targetPool.opaque_ref)
                    continue;

                // obtain the cache data for a vm
                Dictionary<string, string> vmCache;
                lock (_cacheLock)
                {
                    if (!_cache.TryGetValue(vm.opaque_ref, out vmCache))
                    {
                        vmCache = new Dictionary<string, string>();
                        _cache[vm.opaque_ref] = vmCache;
                    }
                }

                foreach (Host host in targets)
                {
                    if (_canceled)
                        return false;

                    if (vmCache.TryGetValue(host.opaque_ref, out var reason) && !string.IsNullOrEmpty(reason))
                    {
                        failureReason = reason;
                        continue;
                    }

                    try
                    {
                        //Skip the resident host as there's a filter for it and 
                        //if not then you could exclude intrapool migration
                        //CA-205799: do not offer the host the VM is currently on
                        Host homeHost = vm.Home();
                        if (homeHost != null && homeHost.opaque_ref == host.opaque_ref)
                            continue;

                        //if pool_migrate can be done, then we will allow it in the wizard, even if storage migration is not allowed (i.e. users can use the wizard to live-migrate a VM inside the pool)
                        if (_wizardMode == WizardMode.Migrate && vmPool != null && targetPool != null && vmPool.opaque_ref == targetPool.opaque_ref)
                        {
                            failureReason = VMOperationHostCommand.GetVmCannotBootOnHostReason(vm, host, vm.Connection.Session, vm_operations.pool_migrate);

                            if (string.IsNullOrEmpty(failureReason))
                                break;
                            else
                                continue;
                        }

                        //check if the destination host is older than the source host
                        var destinationVersion = Helpers.HostPlatformVersion(host);
                        var sourceVersion = Helpers.HostPlatformVersion(vm.Home() ?? Helpers.GetCoordinator(vmPool));

                        if (Helpers.ProductVersionCompare(destinationVersion, sourceVersion) < 0)
                        {
                            failureReason = Messages.OLDER_THAN_CURRENT_SERVER;
                            continue;
                        }

                        if (Host.RestrictDMC(host) &&
                            (vm.power_state == vm_power_state.Running ||
                             vm.power_state == vm_power_state.Paused ||
                             vm.power_state == vm_power_state.Suspended) &&
                            (vm.memory_static_min > vm.memory_dynamic_min || //corner case, probably unlikely
                             vm.memory_dynamic_min != vm.memory_dynamic_max ||
                             vm.memory_dynamic_max != vm.memory_static_max))
                        {
                            failureReason = FriendlyErrorNames.DYNAMIC_MEMORY_CONTROL_UNAVAILABLE;
                            continue;
                        }

                        PIF managementPif = host.Connection.Cache.PIFs.First(p => p.management);
                        XenAPI.Network managementNetwork = host.Connection.Cache.Resolve(managementPif.network);

                        Session session = host.Connection.DuplicateSession();
                        Dictionary<string, string> receiveMapping = Host.migrate_receive(session, host.opaque_ref, managementNetwork.opaque_ref, new Dictionary<string, string>());

                        var targetSrs = host.Connection.Cache.SRs.Where(sr => sr.SupportsStorageMigration()).ToList();
                        var targetNetwork = GetANetwork(host);

                        VM.assert_can_migrate(vm.Connection.Session,
                            vm.opaque_ref,
                            receiveMapping,
                            true,
                            GetVdiMap(vm, targetSrs),
                            vm.Connection == host.Connection ? new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>() : GetVifMap(vm, targetNetwork),
                            new Dictionary<string, string>());

                        break;
                    }
                    catch (Failure failure)
                    {
                        // CA-359124 VM is migratable if a snapshot has more VIFs than the VM. As long as the mapping takes this into account. 
                        if (failure.ErrorDescription.Count > 0 && failure.ErrorDescription[0] == Failure.VIF_NOT_IN_MAP &&
                            SnapshotsContainExtraVIFs(vm))
                            break;

                        if (failure.ErrorDescription.Count > 0 && failure.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                            failureReason = failure.Message.Split('\n')[0].TrimEnd('\r'); // we want the first line only
                        else if (failure.ErrorDescription.Count > 1 && failure.ErrorDescription[1].Contains(Failure.DYNAMIC_MEMORY_CONTROL_UNAVAILABLE))
                            failureReason = FriendlyErrorNames.DYNAMIC_MEMORY_CONTROL_UNAVAILABLE;
                        else
                            failureReason = failure.Message;

                        log.InfoFormat("VM {0} cannot be migrated to {1}. Reason: {2};", vm.Name(), host.Name(), failure.Message);
                    }
                    catch (Exception e)
                    {
                        log.Error($"There was an error while asserting the VM {vm.Name()} can be migrated to {itemToFilterOn.Name()}:", e);
                        failureReason = Messages.HOST_MENU_UNKNOWN_ERROR;
                    }
                    finally
                    {
                        lock (_cacheLock)
                        {
                            if (!string.IsNullOrEmpty(failureReason))
                                vmCache[host.opaque_ref] = failureReason;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(failureReason))
                    return true;
            }

            failureReason = string.Empty;
            return false;
        }

        /// <summary>
        /// Check if a VM's snapshots contain a reference to a VIF not present in the VM.
        /// </summary>
        /// <param name="vm">The VM</param>
        /// <returns>true if at least one snapshot contains a VIF not present in the VM</returns>
        private static bool SnapshotsContainExtraVIFs(VM vm)
        {
            var snapVIFs = vm.snapshots.Select(vm.Connection.Resolve).SelectMany(snap => snap.VIFs);
            return snapVIFs.Any(snapVIF => !vm.VIFs.Contains(snapVIF));
        }

        private List<Host> CollateHosts(IXenObject itemToFilterOn, out Pool thePool)
        {
            thePool = null;

            List<Host> target = new List<Host>();
            if (itemToFilterOn is Host)
            {
                target.Add(itemToFilterOn as Host);
                thePool = Helpers.GetPoolOfOne(itemToFilterOn.Connection);
            }

            if (itemToFilterOn is Pool)
            {
                Pool pool = itemToFilterOn as Pool;
                target.AddRange(pool.Connection.Cache.Hosts);
                target.Sort();
                thePool = pool;
            }
            return target;
        }

        private Dictionary<XenRef<VDI>, XenRef<SR>> GetVdiMap(VM vm, List<SR> targetSrs)
        {
            var vdiMap = new Dictionary<XenRef<VDI>, XenRef<SR>>();

            foreach (var vbdRef in vm.VBDs)
            {
                VBD vbd = vm.Connection.Resolve(vbdRef);
                if (vbd != null)
                {
                    VDI vdi = vm.Connection.Resolve(vbd.VDI);
                    if (vdi != null)
                    {
                        SR sr = vm.Connection.Resolve(vdi.SR);
                        if (sr != null && sr.GetSRType(true) != SR.SRTypes.iso)
                        {
                            // CA-220218: select a storage other than the VDI's current storage to ensure that
                            // both source and target SRs will be checked to see if they support migration
                            // (when sourceSR == targetSR, the server side skips the check)
                            var targetSr = targetSrs.FirstOrDefault(s => s.opaque_ref != sr.opaque_ref);
                            if (targetSr != null)
                                vdiMap.Add(new XenRef<VDI>(vdi.opaque_ref), new XenRef<SR>(targetSr));
                        }
                    }
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
