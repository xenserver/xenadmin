﻿/* Copyright (c) Citrix Systems, Inc. 
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
    public class DestroyPolicyAction: PureAsyncAction
    {
        private List<VMSS> _selectedToDelete;
        public DestroyPolicyAction(IXenConnection connection,List<VMSS> deletePolicies) : base(connection, Messages.DELETE_POLICIES)
        {
            _selectedToDelete = deletePolicies;
            Pool = Helpers.GetPool(connection);
        }

        protected override void Run()
        {

            foreach (var policy in _selectedToDelete)
            {
                Description = string.Format(Messages.DELETING_VMSS, policy.Name());
                foreach (var vmref in policy.VMs)
                {
                    VM.set_snapshot_schedule(Session, vmref.opaque_ref, null);
                }
                try
                {
                    VMSS.destroy(Session, policy.opaque_ref);
                }
                catch (Exception e)
                {
                    if (!e.Message.StartsWith("Object has been deleted"))
                        throw e;
                }
            }
            Description =Messages.DELETED_VMSS;
        }
    }
}
