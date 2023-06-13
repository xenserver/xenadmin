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

using System;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Creates a VBD, then tries to plug it into a VM.
    /// </summary>
    public class VbdCreateAndPlugAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly VBD vbd;

        /// <summary>
        /// Subscribe to this even unless installing tools
        /// </summary>
        public event Action<string> ShowUserInstruction;

        public VbdCreateAndPlugAction(VM vm, VBD vbd, string vdiName, bool suppress)
            : base(vm.Connection, string.Format(Messages.ATTACHING_VIRTUAL_DISK, vdiName, vm.Name()), "", suppress)
        {
            VM = vm;
            this.vbd = vbd;

            ApiMethodsToRoleCheck.Add("vbd.create");

            if (VM.IsHVM() || !vbd.empty)
                ApiMethodsToRoleCheck.AddRange("vbd.get_allowed_operations", "vbd.async_plug");
        }

        protected override void Run()
        {
            string vbdServerRef = VBD.create(Session, vbd);

            if (!VM.IsHVM() && vbd.empty)
            {
                // PV guest require no further action 
                return;
            }

            // Then if we can plug the vbd in, do so...
            if (vbdServerRef != null && 
                VBD.get_allowed_operations(Session, vbdServerRef).Contains(vbd_operations.plug))
            {

                log.DebugFormat("Attempting to hot plug VBD {0}.", vbd.uuid);
                RelatedTask = VBD.async_plug(Session, vbdServerRef);
                PollToCompletion();
                Description = Messages.ATTACHDISKWIZARD_ATTACHED;
            }
            else
            {
                VM vm = Connection.Resolve(vbd.VM);
                if (vm != null && vm.power_state != vm_power_state.Halted)
                {
                    if (vbd.type == vbd_type.CD)
                        ShowUserInstruction?.Invoke(Messages.NEW_DVD_DRIVE_REBOOT);
                    else
                        ShowUserInstruction?.Invoke(Messages.NEWDISKWIZARD_MESSAGE);
                }
            }
        }

      
    }
}
