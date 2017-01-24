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
using XenAdmin.Actions.VMActions;
using XenAdmin.Controls;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.NewVMWizard
{
    public class LunPerVdiNewVMMappingPage : LunPerVdiMappingPage
    {
        private List<DiskDescription> disksToMap;
        public List<DiskDescription> DisksToMap
        {
            private get { return disksToMap; }
            set
            {
                disksToMap = value;
                PopulatePicker();
            }
        }

        /// <summary>
        /// Output of the users selection
        /// </summary>
        public List<DiskDescription> MappedDisks
        {
            get
            {
                List<DiskDescription> mapping = new List<DiskDescription>();
                foreach (DiskDescription disk in DisksToMap)
                {
                    DiskDescription closureDisk = disk;
                    VBD device = disk.Device;
                    device.type = vbd_type.Disk;
                    LunPerVdiPickerItem item = PickerData.FirstOrDefault(d => d.Vdi == closureDisk.Disk);
                    if(item.IsValidForMapping)
                    {
                        mapping.Add(new DiskDescription
                                        {
                                            Disk = item.SelectedVdi,
                                            Device = device,
                                            Type = DiskDescription.DiskType.Existing
                                        });
                    }
                    else
                    {
                        disk.Type = DiskDescription.DiskType.New;
                        mapping.Add(disk);
                    }
                   
                }
                return mapping;
            }
        }

        protected override void PopulatePicker()
        {
            if (DisksToMap == null)
                return;

            List<LunPerVdiPickerItem> data = new List<LunPerVdiPickerItem>();
            foreach (DiskDescription d in DisksToMap)
            {
                data.Add(new LunPerVdiNewVMPickerItem(Connection.Resolve(d.Disk.SR), d.Disk));
            }
            AddDataToPicker(data);
        }
    }

    public class LunPerVdiNewVMPickerItem : LunPerVdiPickerItem
    {
        public LunPerVdiNewVMPickerItem(SR LUNsourceSr, VDI vdi)
            : base(LUNsourceSr, vdi)
        {
            ConstructCells();
        }
    }
}
