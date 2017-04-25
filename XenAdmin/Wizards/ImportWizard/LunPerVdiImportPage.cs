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
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using XenAdmin.Controls;
using XenAdmin.Mappings;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using XenOvf.Definitions;

namespace XenAdmin.Wizards.ImportWizard
{
    public class LunPerVdiImportPage : LunPerVdiMappingPage
    {
        public LunPerVdiImportPage()
        {
            VdiColumnTitle = Messages.LUNPERVDIPICKER_IMPORT_VDI_COLUMN_TITLE;
            VmMappings = new Dictionary<string, VmMapping>();
        }

        private Dictionary<string, VmMapping> mappingsToMap = new Dictionary<string, VmMapping>();
        /// <summary>
        /// Data to get the user to map
        /// </summary>
        public Dictionary<string, VmMapping> VmMappings
        {
            get
            {
                ConvertMappedData();
                return mappingsToMap;
            }
            set 
            { 
                mappingsToMap = value;
                ClearPickerData();
                PopulatePicker();
            }
        }

        /// <summary>
        /// Data mapped by the user
        /// </summary>
        private void ConvertMappedData()
        {
            if (PickerData.Count < 1)
                return;

            List<LunPerVdiImportPickerItem> mappedData = PickerData.ConvertAll(i => i as LunPerVdiImportPickerItem);

            foreach (KeyValuePair<string, VmMapping> pair in mappingsToMap)
            {
                string sysId = pair.Key;
                foreach (IStorageResource resourceData in ResourceData(sysId))
                {
                    List<LunPerVdiImportPickerItem> items = (from i in mappedData
                                                             where i.VmKey == sysId
                                                             select i).ToList();

                    foreach (LunPerVdiImportPickerItem item in items)
                    {
                        if(item.IsValidForMapping &&
                            !pair.Value.StorageToAttach.ContainsKey(item.SrKey) && 
                            pair.Value.Storage.ContainsKey(item.SrKey))
                        {
                            pair.Value.StorageToAttach.Add(item.SrKey, item.SelectedVdi);
                            pair.Value.Storage.Remove(item.SrKey);
                        }
                    }
                }
            }
        }

        public EnvelopeType SelectedOvfEnvelope { private get; set; }

        private StorageResourceContainer ResourceData(string sysId)
        {
            return new OvfStorageResourceContainer(SelectedOvfEnvelope, sysId);
        }

        protected override void PopulatePicker()
        {
            List<LunPerVdiPickerItem> datatoAdd = new List<LunPerVdiPickerItem>();

            foreach (KeyValuePair<string, VmMapping> pair in mappingsToMap)
            {
                string sysId = pair.Key;
                foreach (IStorageResource resourceData in ResourceData(sysId))
                {
                    if (!resourceData.CanCalculateDiskCapacity)
                        continue;

                    ulong requiredSize = resourceData.RequiredDiskCapacity;

                    foreach (KeyValuePair<string, SR> subPair in pair.Value.Storage)
                    {
                        datatoAdd.Add(new LunPerVdiImportPickerItem(subPair.Value, resourceData.DiskLabel,
                                                                    pair.Value.VmNameLabel, requiredSize) { SrKey = subPair.Key, VmKey = sysId });
                    }
                }
            }

            AddDataToPicker(datatoAdd);
        }

    }

    public class LunPerVdiImportPickerItem : LunPerVdiPickerItem
    {
        private readonly string vmName;
        private readonly string vdiName;

        public LunPerVdiImportPickerItem(SR LUNsourceSr, string vdiName, string vmName, ulong requiredSize) 
            : base(LUNsourceSr, null)
        {
            this.vmName = vmName;
            this.vdiName = vdiName;
            LunConstraints.Add(v => v.virtual_size < (long) requiredSize);
            ConstructCells();
        }

        public string SrKey { get; set; }
        public string VmKey { get; set; }

        public override bool Equals(object obj)
        {
            LunPerVdiImportPickerItem cf = obj as LunPerVdiImportPickerItem;
            if(cf == null)
                return base.Equals(obj);

            if(SrKey != null && cf.SrKey != null)
                return SrKey.Equals(cf.SrKey);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override string VdiColumnText
        {
            get { return string.Format(Messages.VALUE_HYPHEN_VALUE, vmName, vdiName); }
        }
    }
}
