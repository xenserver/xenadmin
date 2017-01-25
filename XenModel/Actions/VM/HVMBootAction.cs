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

using XenAPI;


namespace XenAdmin.Actions
{
    public class HVMBootAction : PureAsyncAction
    {
        public HVMBootAction(VM vm)
            : base(vm.Connection, string.Format(Messages.STARTING_IN_RECOVERY_MODE_TITLE, vm.Name))
        {
            VM = vm;
        }

        private VM vmCopy;
        protected override void Run()
        {
            Description = Messages.STARTING_IN_RECOVERY_MODE;
            string oldPolicy = VM.HVM_boot_policy; 
            string oldOrder  = VM.BootOrder;

            vmCopy = (VM)VM.Clone();
            vmCopy.HVM_boot_policy = "BIOS order";
            vmCopy.BootOrder = "DN";

            VM.Locked = true;
            vmCopy.SaveChanges(Session);
            VM.Locked = false;

            XenAPI.VM.start(Session, VM.opaque_ref, false, false);

            vmCopy.HVM_boot_policy = oldPolicy;
            vmCopy.BootOrder = oldOrder;

            VM.Locked = true;
            vmCopy.SaveChanges(Session);
            VM.Locked = false;

            Description = Messages.STARTED_IN_RECOVERY_MODE;
        }

        protected override void Clean()
        {
            VM.Locked = false;
        }
    }
}
