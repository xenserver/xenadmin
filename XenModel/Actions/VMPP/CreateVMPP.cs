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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class CreateVMPolicy<T> : AsyncAction where T : XenObject<T>
    {
        private IVMPolicy _record;
        private List<VM> _vms;
        private bool _runNow = false;
        public CreateVMPolicy(IVMPolicy record, List<VM> vms, bool runNow)
            : base(record.Connection, Messages.CREATE_POLICY)
        {
            _record = record;
            _vms = vms;
            _runNow = runNow;
            Pool = Helpers.GetPool(record.Connection);
            if (typeof(T) == typeof(VMPP))
            {
                ApiMethodsToRoleCheck.Add("VMPP.async_create");
                ApiMethodsToRoleCheck.Add("VM.set_protection_policy");
                ApiMethodsToRoleCheck.Add("VMPP.protect_now");
            }
            else
            {
                ApiMethodsToRoleCheck.Add("VMSS.async_create");
                ApiMethodsToRoleCheck.Add("VM.set_snapshot_schedule");
                ApiMethodsToRoleCheck.Add("VMSS.snapshot_now");
            }
        }

        protected override void Run()
        {
            Description = string.Format(typeof(T) == typeof(VMPP) ? Messages.CREATING_VMPP : Messages.CREATING_VMSS, _record.Name);
            RelatedTask = _record.async_task_create(Session);
            PollToCompletion();
            var vmppref = new XenRef<T>(Result);
            Connection.WaitForCache(vmppref);
            foreach (var selectedVM in _vms)
            {
                _record.set_policy(Session, selectedVM.opaque_ref, vmppref.opaque_ref);
            }
            Description = string.Format(typeof(T) == typeof(VMPP) ? Messages.CREATED_VMPP : Messages.CREATED_VMSS, _record.Name);
            PercentComplete = 60;
            if (_runNow)
                _record.run_now (Session, vmppref);
            PercentComplete = 100;
        }
    }
}
