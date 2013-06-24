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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.VMProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    public class AssertCanEvacuateUpgradeCheck : AssertCanEvacuateCheck
    {
        public AssertCanEvacuateUpgradeCheck(Host host)
            : base(host)
        {
        }

        public override Problem RunCheck()
        {
            if (!Host.IsLive)
                return new HostNotLive(this, Host);

            //Storage link check
            var storageLinkProblem = GetStorageLinkProblem();
            if (storageLinkProblem != null)
                return storageLinkProblem;

            return base.RunCheck();
        }

        private Problem GetStorageLinkProblem()
        {
            if (Helpers.BostonOrGreater(Host))
                return null;

            var vmsInStorageLink = new List<VM>();
            foreach (var vm in Host.Connection.Cache.VMs.Where(vm => vm.is_a_real_vm))
            {
                foreach (var sr in vm.SRs.Where(sr => sr.GetSRType(true) == SR.SRTypes.cslg))
                {
                    //Check if it is a supported adapter
                    if (IsSupportedAdapter(sr))
                    {
                        if (vm.power_state == vm_power_state.Suspended || vm.power_state == vm_power_state.Running)
                        {
                            vmsInStorageLink.Add(vm);
                            break;
                        }
                    }
                    else
                    {
                        return new IsInStorageLinkLegacySR(this, vm);
                    }
                }
            }
            if (vmsInStorageLink.Count > 0)
                return new IsInStorageLinkLegacySR(this, vmsInStorageLink);

            return null;
        }

        private bool IsSupportedAdapter(SR sr)
        {
            bool isSMIS = false;
            bool isNetApp = false;
            bool isDell = false;
            try
            {
                var storageRepository = sr.StorageLinkRepository(Program.StorageLinkConnections);

                if (storageRepository == null)
                {
                    // The CSLG is down, maybe because it was on local storage:
                    // try the other_config instead: see CA-63607.
                    string adapter;
                    if (sr.other_config.TryGetValue("CSLG-adapter", out adapter))
                    {
                        isSMIS = (adapter == "SMI-S");
                        isNetApp = adapter.ToLower().Contains("netapp");
                        isDell = adapter.ToLower().Contains("equallogic");
                    }
                }

                else
                {
                    // The CSLG is up: look up the adapter type, and save it away in the other_config
                    var storageSystem = storageRepository.StorageLinkSystem;
                    isSMIS = storageSystem.StorageLinkAdapter.IsSMIS;
                    isNetApp = storageSystem.StorageLinkAdapter.AdapterName.ToLower().Contains("netapp");
                    isDell = storageSystem.StorageLinkAdapter.AdapterName.ToLower().Contains("equallogic");
                    Helpers.SetOtherConfig(sr.Connection.Session, sr, "CSLG-adapter",
                                           isSMIS ? "SMI-S" : storageSystem.StorageLinkAdapter.AdapterName);
                }
            }
            catch
            {
            }

            return (isNetApp || isDell || isSMIS);
        }
    }
}