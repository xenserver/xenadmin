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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class CreateVMPolicy : AsyncAction
    {
        private VMSS _record;
        private List<VM> _vms;
        private bool _runNow = false;
        public CreateVMPolicy(VMSS record, List<VM> vms, bool runNow)
            : base(record.Connection, Messages.CREATE_POLICY)
        {
            _record = record;
            _vms = vms;
            _runNow = runNow;
            Pool = Helpers.GetPool(record.Connection);
            ApiMethodsToRoleCheck.Add("VMSS.async_create");
            ApiMethodsToRoleCheck.Add("VM.set_snapshot_schedule");
            ApiMethodsToRoleCheck.Add("VMSS.snapshot_now");
        }

        protected override void Run()
        {
            Description = string.Format(Messages.CREATING_VMSS, _record.Name());
            RelatedTask = VMSS.async_create(Session, _record);
            PollToCompletion();
            var vmssref = new XenRef<VMSS>(Result);
            Connection.WaitForCache(vmssref);
            foreach (var selectedVM in _vms)
            {
                VM.set_snapshot_schedule(Session, selectedVM.opaque_ref, vmssref.opaque_ref);
            }

            Tick(60, string.Format(Messages.CREATED_VMSS, _record.Name()));

            if (_runNow)
                VMSS.snapshot_now(Session, vmssref);
            PercentComplete = 100;
        }
    }
}
