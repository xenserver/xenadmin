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


namespace XenAdmin.Actions
{
    /// <summary>
    /// Saves changes on a VBD, then tries to plug the VBD into a VM.
    /// </summary>
    public class VbdSaveAndPlugAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Action _ShowMustRebootBoxCD;
        private readonly Action _ShowVBDWarningBox;
        private readonly XenAPI.VBD vbd;

        private bool InstallingTools = false;

        public VbdSaveAndPlugAction(VM vm, XenAPI.VBD vbd, string vdiName, XenAPI.Session session, bool installingTools, bool supress, Action showMustRebootBoxCD, Action showVBDWarningBox)
            : this(vm, vbd, vdiName, session, supress,showMustRebootBoxCD,showVBDWarningBox)
        {
            InstallingTools = installingTools;
        }

        public VbdSaveAndPlugAction(VM vm, XenAPI.VBD vbd, string vdiName, XenAPI.Session session, bool supress, Action showMustRebootBoxCD, Action showVBDWarningBox)
            : base(vm.Connection, string.Format(Messages.ATTACHING_VIRTUAL_DISK, vdiName, vm.Name), "", supress)
        {
            _ShowVBDWarningBox = showVBDWarningBox;
            _ShowMustRebootBoxCD = showMustRebootBoxCD;
            VM = vm;
            this.vbd = vbd;
            // Preserve existing session if provided.
            if (session != null)
                this.Session = session;

            ApiMethodsToRoleCheck.Add("vbd.async_plug");
            ApiMethodsToRoleCheck.Add("vbd.set_userdevice");
        }

        protected override void Run()
        {
            // First, save changes to the VBD.
            
            string vbdServerRef = vbd.SaveChanges(Session, null, null);

            if (!VM.IsHVM && vbd.empty)
            {
                // PV guest require no further action 
                return;
            }

            // Then if we can plug the vbd in, do so...
            if (vbdServerRef != null && 
                XenAPI.VBD.get_allowed_operations(Session, vbdServerRef).Contains(XenAPI.vbd_operations.plug))
            {

                log.DebugFormat("Attempting to hot plug VBD {0}.", vbd.uuid);
                this.RelatedTask = XenAPI.VBD.async_plug(Session, vbdServerRef);
                PollToCompletion();
                this.Description = Messages.ATTACHDISKWIZARD_ATTACHED;
            }
            else
            {
                XenAPI.VM vm = this.Connection.Resolve<XenAPI.VM>(vbd.VM);
                if (vm != null && vm.power_state != XenAPI.vm_power_state.Halted)
                {
                    if (InstallingTools)
                    {
                        //Program.Invoke(Program.MainWindow, ShowMustRebootBox);
                        // this is now done inside InstallToolsCommand
                    }
                    else if (vbd.type == XenAPI.vbd_type.CD)
                    {
                        _ShowMustRebootBoxCD();
                    }
                    else
                    {
                        // Plug could not happen straight away: show warning.
                        _ShowVBDWarningBox();
                    }
                }
            }
        }

      
    }
}
