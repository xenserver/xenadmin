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


namespace XenAdmin.Actions
{
    public class DestroyDiskAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool AllowRunningVMDelete = false;

        private VDI Disk;

        public DestroyDiskAction(VDI disk)
            : base(disk.Connection, string.Format(Messages.ACTION_DISK_DELETING_TITLE, disk.Name, disk.Connection.Resolve<SR>(disk.SR).NameWithoutHost), false)
        {
            Disk = disk;
            Disk.Locked = true;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("vbd.unplug");
            ApiMethodsToRoleCheck.Add("vbd.destroy");
            ApiMethodsToRoleCheck.Add("vdi.destroy");
            #endregion

            SR = Connection.Resolve(Disk.SR);

            // If there is only one VM involved here we can filter it under the VM
            // Otherwise just filter under the SR
            VM relevantVM = null;
            foreach (VBD vbd in SR.Connection.ResolveAll(Disk.VBDs))
            {
                VM vm = SR.Connection.Resolve<VM>(vbd.VM);
                if (vm == null)
                    continue;
                if (relevantVM == null)
                    relevantVM = vm;
                else
                {
                    // more than one relevant VM, just give it the SR as an owner
                    relevantVM = null;
                    break;
                }
            }
            if (relevantVM != null)
                VM = relevantVM;       
        }

        protected override void Run()
        {
            log.DebugFormat("Deleting VDI '{0}'", Helpers.GetName(Disk));
            Description = Messages.ACTION_VDI_DELETING;
            foreach (VBD vbd in SR.Connection.ResolveAll(Disk.VBDs))
            {
                VM vm = Connection.Resolve<VM>(vbd.VM);
                if (vm == null)
                    continue;
                if (vbd.currently_attached && !AllowRunningVMDelete)
                {
                    throw new Exception(
                        string.Format(Messages.CANNOT_DELETE_VDI_ACTIVE_ON, 
                        Helpers.GetName(vm).Ellipsise(20)));
                }
                DetachVirtualDiskAction action = new DetachVirtualDiskAction(Disk, vm, false);
                action.RunExternal(Session);
            }

            RelatedTask = XenAPI.VDI.async_destroy(Session, Disk.opaque_ref);
            PollToCompletion();
            Description = Messages.ACTION_VDI_DELETED;
            log.DebugFormat("Deleted VDI '{0}'", Helpers.GetName(Disk));        
        }

        protected override void Clean()
        {
            Disk.Locked = false;
        }
    }
}
