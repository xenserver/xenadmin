﻿/* Copyright (c) Citrix Systems Inc. 
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
using System.Diagnostics;
using System.Threading;
using XenAPI;
using System.Linq;

namespace XenAdmin.Actions
{
    public class PvsProxyCreateAction : AsyncAction
    {
        private VM vm;
        private PVS_site site;
        private VIF vif;

        public PvsProxyCreateAction(VM vm, PVS_site site, VIF vif)
            : base(vm.Connection, string.Format(Messages.ACTION_ENABLE_PVS_READ_CACHING_FOR, vm, site))
        {
            this.site = site;
            this.vif = vif;

            this.Session = vm.Connection.Session;
            this.VM = vm;

            this.Description = Messages.WAITING;
            SetRBACPermissions();
        }

        private void SetRBACPermissions()
        {
            //AddCommonAPIMethodsToRoleCheck();
            //ApiMethodsToRoleCheck.Add("host.destroy");
            //ApiMethodsToRoleCheck.Add("sr.forget");

            // TODO: what to use here?
        }

        protected override void Run()
        {
            try
            {
                PVS_proxy.async_create(Session, site.opaque_ref, vif.opaque_ref, false); //prepopulate is being removed
                this.Description = Messages.ENABLED;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);               
            }
        }
    }
}
