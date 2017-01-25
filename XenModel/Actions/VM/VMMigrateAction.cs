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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public class VMMigrateAction : PureAsyncAction
    {

        public VMMigrateAction(VM vm, Host destinationHost)
            : base(vm.Connection, GetTitle(vm, destinationHost))
        {
            this.Description = Messages.ACTION_PREPARING;
            this.VM = vm;
            this.Host = destinationHost;
            this.Pool = Core.Helpers.GetPool(vm.Connection);
        }

        private static string GetTitle(VM vm, Host toHost)
        {
            Host residentOn = vm.Connection.Resolve(vm.resident_on);
            
            return residentOn == null
                ? string.Format(Messages.ACTION_VM_MIGRATING_NON_RESIDENT, vm.Name, toHost.Name)
                : string.Format(Messages.ACTION_VM_MIGRATING_RESIDENT, vm.Name, Helpers.GetName(residentOn), toHost.Name);
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_MIGRATING;
            RelatedTask = XenAPI.VM.async_live_migrate(Session, VM.opaque_ref, Host.opaque_ref);
            try
            {
                PollToCompletion();
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count >= 5 && f.ErrorDescription[0] == "VM_MIGRATE_FAILED"
                    && f.ErrorDescription[4].Contains("VDI_MISSING"))
                {
                    throw new Exception(Messages.MIGRATE_EJECT_TOOLS_ON_UPGRADE);
                }
                else
                {
                    throw;
                }
            }

            this.Description = Messages.ACTION_VM_MIGRATED;
        }
    }
}
