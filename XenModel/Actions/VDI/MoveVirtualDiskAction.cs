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
using System.Linq;
using System.Text;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class MoveVirtualDiskAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private XenAPI.VDI vdi;

        public MoveVirtualDiskAction(IXenConnection connection, XenAPI.VDI vdi, SR sr)
            : base(connection, string.Format(Messages.ACTION_MOVING_VDI_TITLE, Helpers.GetName(vdi), Helpers.GetName(sr)))
        {
            this.vdi = vdi;
            SR = sr;
            vdi.Locked = true;
            sr.Locked = true;
            ApiMethodsToRoleCheck.Add("vdi.destroy");
            ApiMethodsToRoleCheck.Add("vdi.copy");
            if (vdi.type == vdi_type.suspend) 
                ApiMethodsToRoleCheck.Add("vm.set_suspend_VDI");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
        }

        protected override void Run()
        {
            Description = string.Format(Messages.ACTION_MOVING_VDI_STATUS, Helpers.GetName(vdi));
            PercentComplete = 10;
            log.DebugFormat("Moving VDI '{0}'", Helpers.GetName(vdi));
            RelatedTask = XenAPI.VDI.async_copy(Session, vdi.opaque_ref, SR.opaque_ref);
            PollToCompletion(PercentComplete, 60);
            XenAPI.VDI newVDI = Connection.WaitForCache(new XenRef<VDI>(Result));

            // if the original is a suspend VDI, link the suspended VM to the new VDI
            if (vdi.type == vdi_type.suspend)
            {
                var suspendedVm = (from vm in Connection.Cache.VMs
                                         let suspendVdi = Connection.Resolve<VDI>(vm.suspend_VDI)
                                         where suspendVdi != null && suspendVdi.uuid == vdi.uuid
                                         select vm).FirstOrDefault();
                if (suspendedVm != null)
                {
                    XenAPI.VM.set_suspend_VDI(Session, suspendedVm.opaque_ref, newVDI.opaque_ref);
                }
            }
            PercentComplete = 70;

            XenAPI.VDI.destroy(Session, vdi.opaque_ref);
            PercentComplete = 100;
            Description = Messages.COMPLETED;
            log.DebugFormat("Moved VDI '{0}'", Helpers.GetName(vdi));
        }

        protected override void Clean()
        {
            SR.Locked = false;
            vdi.Locked = false;
        }
    }
}
