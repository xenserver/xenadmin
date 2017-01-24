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
using System.Text;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class DetachVirtualDiskAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VDI vdi;
        private VBD vbd;
        private bool takeVDILock = true;

        /// <summary>
        /// Detaches a virtual disk
        /// </summary>
        /// <param name="disk">The VDI to detach</param>
        /// <param name="vm">The VM to detach it from</param>
        /// <param name="takeVDILock">Whether it is necessary to lock the VDI (we may be part of another disk action)</param>
        public DetachVirtualDiskAction(VDI disk, VM vm, bool takeVDILock)
            : base(disk.Connection, string.Format(Messages.ACTION_DISK_DETACHING_TITLE, disk.Name, vm.Name, false))
        {
            this.takeVDILock = takeVDILock;
            vdi = disk;
            if (takeVDILock)
                vdi.Locked = true;
            VM = vm;
            VM.Locked = true;
            foreach (VBD v in Connection.ResolveAll<VBD>(VM.VBDs))
            {
                if (v.VDI.opaque_ref == vdi.opaque_ref)
                {
                    vbd = v;
                    vbd.Locked = true;
                }
            }

        }

        protected override void Run()
        {
            log.DebugFormat("Detaching VDI '{0}'", Helpers.GetName(vdi));
            Description = Messages.ACTION_DISK_DETACHING;
            try
            {
                if (vbd != null && vbd.currently_attached && XenAPI.VBD.get_allowed_operations(Session, vbd.opaque_ref).Contains(XenAPI.vbd_operations.unplug))
                {
                    RelatedTask = XenAPI.VBD.async_unplug(Session, vbd.opaque_ref);
                    PollToCompletion(0, 50);
                }
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                throw;
            }
            finally
            {
                PercentComplete = 50;
                RelatedTask = XenAPI.VBD.async_destroy(Session, vbd.opaque_ref);
                PollToCompletion(51, 100);
            }
            Description = Messages.ACTION_DISK_DETACHED;
            log.DebugFormat("Detached VDI '{0}'", Helpers.GetName(vdi));
        }

        protected override void Clean()
        {
            vbd.Locked = false;
            if (takeVDILock)
                vdi.Locked = false;
            VM.Locked = false;
        }
    }
}
