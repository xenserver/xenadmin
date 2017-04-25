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
using XenAPI;


namespace XenAdmin.Actions
{
    public class CreateCdDriveAction : AsyncAction
    {
        private readonly bool InstallingTools;
        private Action _showMustRebootBoxCD;
        private Action _showVBDWarningBox;

        public CreateCdDriveAction(VM vm, bool installingTools, Action showMustRebootBoxCD, Action showVBDWarningBox)
            : base(vm.Connection, string.Format(Messages.NEW_DVD_DRIVE_CREATE_TITLE, vm.Name))
        {
            _showMustRebootBoxCD = showMustRebootBoxCD;
            _showVBDWarningBox = showVBDWarningBox;
            VM = vm;
            InstallingTools = installingTools;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            VBD cdrom = VM.FindVMCDROM();
            if (cdrom == null)
            {
                Description = Messages.NEW_DVD_DRIVE_CREATING;
                // could not find a cd, try and make one

                if (VM.VBDs.Count >= VM.MaxVBDsAllowed)
                {
                    throw new Exception(Messages.CDDRIVE_MAX_ALLOWED_VBDS);
                }

                List<String> allowedDevices = new List<String>(XenAPI.VM.get_allowed_VBD_devices(Session, VM.opaque_ref));

                if (allowedDevices == null || allowedDevices.Count == 0)
                {
                    throw new Exception(Messages.CDDRIVE_MAX_ALLOWED_VBDS);
                }

                XenAPI.VBD cdDrive = new XenAPI.VBD();
                cdDrive.VM = new XenAPI.XenRef<XenAPI.VM>(VM.opaque_ref);
                cdDrive.VDI = null;
                cdDrive.bootable = false;
                cdDrive.device = "";
                cdDrive.userdevice = allowedDevices.Contains("3") ? "3" : allowedDevices[0];
                cdDrive.empty = true;
                cdDrive.type = XenAPI.vbd_type.CD;
                cdDrive.mode = XenAPI.vbd_mode.RO;

                VbdSaveAndPlugAction cdCreate = new VbdSaveAndPlugAction(VM, cdDrive, Messages.DVD_DRIVE, Session, InstallingTools, true,_showMustRebootBoxCD,_showVBDWarningBox);
                cdCreate.RunExternal(Session);
                Description = Messages.NEW_DVD_DRIVE_DONE;
            }
        }
    }
}
