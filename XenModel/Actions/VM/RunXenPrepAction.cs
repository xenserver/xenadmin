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
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// This action installs the PV tools on a selected windows VM.
    /// </summary>
    public class RunXenPrepAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RunXenPrepAction(VM vm)
            : base(vm.Connection, string.Format(Messages.RUNXENPREP_TITLE, vm.Name))
        {
            VM = vm;
        }

        protected override void Run()
        {
            Description = Messages.ACTION_XENPREP_PREPARING;

            // Check that the VM has a cd-rom...
            XenAPI.VBD cdrom = VM.FindVMCDROM();
            if (cdrom == null)
            {
                Description = Messages.COULDNOTFIND_CD_ON_VM;
                return;
            }

            // Eject anything that's currently in the cd-rom...
            if (!cdrom.empty)
            {
                XenAPI.VBD.eject(Session, cdrom.opaque_ref);
            }

            VM.xenprep_start(Session, VM.opaque_ref);

            Description = Messages.ACTION_XENPREP_STARTED;
        }

    }
}
