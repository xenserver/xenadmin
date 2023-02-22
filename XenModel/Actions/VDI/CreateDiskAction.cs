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
    public class CreateDiskAction : AsyncAction
    {
        private readonly VDI _disk;
        private readonly VBD _device;

        public CreateDiskAction(VDI disk)
            : base(disk.Connection, "", "")
        {
            _disk = disk;

            Title = string.Format(Messages.ACTION_VDI_CREATING_TITLE, disk.Name(),
                disk.Connection.Resolve(disk.SR).NameWithoutHost());
            Description = Messages.ACTION_VDI_CREATING;
        }

        public CreateDiskAction(VDI disk, VBD device, VM vm)
            : this(disk)
        {
            _device = device;
            VM = vm;
        }

        protected override void Run()
        {
            if (_device == null)
            {
                new SaveChangesAction(_disk, true).RunSync(Session);
            }
            else
            {
                // CA-44959: only make the disk bootable if there aren't any other bootable VBDs.
                var alreadyHasBootableDisk = HasBootableDisk();

                // Get legitimate unused user device numbers
                string[] uds = VM.get_allowed_VBD_devices(Session, VM.opaque_ref);
                if (uds.Length == 0)
                    throw new Exception(FriendlyErrorNames.VBDS_MAX_ALLOWED);

                string ud = uds[0];
                string vdiRef = VDI.create(Session, _disk);

                _device.VDI = new XenRef<VDI>(vdiRef);
                _device.VM = new XenRef<VM>(VM);
                _device.bootable = ud == "0" && !alreadyHasBootableDisk;
                _device.userdevice = ud;
            }

            Description = Messages.ACTION_VDI_CREATED;
        }

        private bool HasBootableDisk()
        {
            foreach (XenRef<VBD> vbdRef in VM.VBDs)
            {
                var vbd = Connection.Resolve(vbdRef);

                if (vbd != null && !vbd.IsCDROM() && !vbd.IsFloppyDrive() && vbd.bootable)
                {
                    VDI vdi = Connection.Resolve(vbd.VDI);

                    if (vdi != null)
                    {
                        SR sr = Connection.Resolve(vdi.SR);
                        if (sr != null && sr.IsToolsSR())
                            continue;
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
