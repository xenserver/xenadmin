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
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions.HostActions
{
    public class VMsWhichCanBeMigratedAction : PureAsyncAction
    {
        private List<string> VmsRefsNotMigratable = new List<string>();
        public List<VM> MigratableVms = new List<VM>();

        public VMsWhichCanBeMigratedAction(IXenConnection connection, Host host)
            : base(connection, "vms which prevent evacuation", true)
        {
            Host = host;
        }

        protected override void Run()
        {
            Session session = Host.Connection.DuplicateSession();
            Dictionary<XenRef<XenAPI.VM>, string[]> dict = XenAPI.Host.get_vms_which_prevent_evacuation(session, Host.opaque_ref);

            foreach (KeyValuePair<XenRef<VM>, string[]> pair in dict)
            {
                if (pair.Value[0] == Failure.HOST_NOT_ENOUGH_FREE_MEMORY)
                    continue;

                if (!VmsRefsNotMigratable.Contains(pair.Key.opaque_ref))
                    VmsRefsNotMigratable.Add(pair.Key.opaque_ref);
            }

            foreach(VM vm in Connection.ResolveAll(Host.resident_VMs))
            {
                if (!vm.is_a_real_vm)
                    continue;

                if (VmsRefsNotMigratable.Contains(vm.opaque_ref))
                    continue;

                if (!MigratableVms.Contains(vm))
                    MigratableVms.Add(vm);
            }
        }
    }
}
