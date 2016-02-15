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
using System.Text;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class ChangePolicyEnabledAction<T> : PureAsyncAction where T : XenObject<T>
    {
        private T _policy;
        public ChangePolicyEnabledAction(T policy)
            : base(policy.Connection, string.Format(Messages.CHANGE_POLICY_STATUS, policy.Name))
        {
            _policy = policy;
            Pool = Helpers.GetPool(_policy.Connection);
        }

        protected override void Run()
        {
            if (typeof(T) == typeof(VMPP))
            {
                var vmpp = _policy as VMPP;
                bool value = !vmpp.is_policy_enabled;
                Description = value ? string.Format(Messages.ENABLING_VMPP, vmpp.Name) : string.Format(Messages.DISABLING_VMPP, vmpp.Name);

                VMPP.set_is_policy_enabled(Session, vmpp.opaque_ref, !vmpp.is_policy_enabled);
                Description = value ? string.Format(Messages.ENABLED_VMPP, vmpp.Name) : string.Format(Messages.DISABLED_VMPP, vmpp.Name);
            }
            else
            {
                var vmss = _policy as VMSS;
                bool value = !vmss.enabled;
                Description = value ? string.Format(Messages.ENABLING_VMSS, vmss.Name) : string.Format(Messages.DISABLING_VMSS, vmss.Name);

                VMSS.set_enabled(Session, vmss.opaque_ref, !vmss.enabled);
                Description = value ? string.Format(Messages.ENABLED_VMSS, vmss.Name) : string.Format(Messages.DISABLED_VMSS, vmss.Name);
            }
            
        }
    }
}
