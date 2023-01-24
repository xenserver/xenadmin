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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAPI;

namespace XenAdmin.Actions.VMActions
{
    public class VMCrossPoolMigrateAction : PureAsyncAction
    {
        private readonly VmMapping mapping;
        private readonly XenAPI.Network transferNetwork;
        private readonly bool copy;

        /// <summary>
        /// Cross pool migration action. Can also be used to copy a VM across pools, by setting the "copy" parameter to true
        /// </summary>
        /// <param name="vm">the VM to be migrated</param>
        /// <param name="destinationHost">the destination host</param>
        /// <param name="transferNetwork">the network used for the VM migration</param>
        /// <param name="mapping">the storage and networking mappings</param>
        /// <param name="copy">weather this should be a cross-pool copy (true) or migrate (false) operation</param>
        public VMCrossPoolMigrateAction(VM vm, Host destinationHost, XenAPI.Network transferNetwork, VmMapping mapping, bool copy)
            : base(vm.Connection, GetTitle(vm, destinationHost, copy))
        {
            Session = vm.Connection.Session;
            VM = vm;
            Host = destinationHost;
            Pool = Helpers.GetPool(vm.Connection);
            this.mapping = mapping;
            this.transferNetwork = transferNetwork;
            this.copy = copy;
        }

        public static RbacMethodList StaticRBACDependencies
        {
            get
            {
                RbacMethodList list = new RbacMethodList( "Host.migrate_receive",
                                                          "VM.migrate_send", 
                                                          "VM.async_migrate_send",
                                                          "VM.assert_can_migrate");
                list.AddRange(Role.CommonTaskApiList);
                return list;
            }
        }

        public static string GetTitle(VM vm, Host toHost, bool copy)
        {
            if (copy)
                return string.Format(Messages.ACTION_VM_CROSS_POOL_COPY_TITLE,
                    vm.NameWithLocation(),
                    Helpers.GetPool(vm.Connection)?.Name() ?? vm.Connection.Name,
                    toHost.NameWithLocation());

            Host residentOn = vm.Connection.Resolve(vm.resident_on);

            return residentOn == null
                ? string.Format(Messages.ACTION_VM_MIGRATING_NON_RESIDENT, vm.NameWithLocation(), toHost.NameWithLocation())
                : string.Format(Messages.ACTION_VM_MIGRATING_RESIDENT, vm.Name(), residentOn.NameWithLocation(), toHost.NameWithLocation());
        }

        protected override void Run()
        {
            try
            {
                PercentComplete = 0;
                Session session = Host.Connection.DuplicateSession();
                var sendData = Host.migrate_receive(session, Host.opaque_ref,
                    transferNetwork.opaque_ref, new Dictionary<string, string>());
                PercentComplete = 5;

                var options = copy
                    ? new Dictionary<string, string> {{"copy", "true"}}
                    : new Dictionary<string, string>();

                RelatedTask = VM.async_migrate_send(Session, VM.opaque_ref, sendData,
                    true, GetVdiMap(mapping), GetVifMap(mapping, VM), options);

                PollToCompletion(PercentComplete, 100);
            }
            catch (CancelledException)
            {
                Description = string.Format(copy
                        ? Messages.ACTION_VM_CROSS_POOL_COPY_CANCELLED
                        : Messages.ACTION_VM_MIGRATE_CANCELLED,
                    VM.NameWithLocation());
                throw;
            }
            catch (Failure ex)
            {
                Description = ex.Message;
                throw;
            }
            Description = copy ? Messages.ACTION_VM_COPIED: Messages.ACTION_VM_MIGRATED;
        }

        /// <summary>
        /// VDI ref to SR ref Map
        /// </summary>
        private Dictionary<XenRef<VDI>, XenRef<SR>> GetVdiMap(VmMapping vmMap)
        {
            return vmMap.Storage.ToDictionary(pair => new XenRef<VDI>(pair.Key),
                pair => new XenRef<SR>(pair.Value));
        }

        /// <summary>
        /// VIF ref to remote Network ref
        /// </summary>
        private Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>> GetVifMap(VmMapping vmMap, VM vm) 
        { 
            var map = new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>();
            var vmVifs = vm.Connection.ResolveAll(vm.VIFs);
            var vmMacs = vmVifs.Select(vif => vif.MAC);

            // CA-359124: add VIFs that are present in the VM's snapshots, but not the VM
            var snapVIFs = vm.snapshots
                .Select(Connection.Resolve)
                .SelectMany(snap => Connection.ResolveAll(snap.VIFs))
                // use MAC to identify VIFs that are not in the VM, opaque_ref differentiates between VM and snapshot VIFs
                .Where(snapVif => !vmMacs.Contains(snapVif.MAC))
                .ToList();

            var allVifs = vmVifs.Concat(snapVIFs).ToList();
            foreach (var pair in vmMap.VIFs)
            {
                var vifMac = pair.Key;

                var vif = allVifs.FirstOrDefault(v => v.MAC.Equals(vifMac));
                if (vif == null)
                {
                    continue;
                }
                var vifRef = new XenRef<VIF>(vif);
                var networkRef = new XenRef<XenAPI.Network>(pair.Value);
                map.Add(vifRef, networkRef);
            }

            return map;
        }
    }
}
