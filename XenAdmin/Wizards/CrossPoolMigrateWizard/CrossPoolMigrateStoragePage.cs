﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Controls;
using XenAdmin.Mappings;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    class CrossPoolMigrateStoragePage : SelectVMStorageWithMultipleVirtualDisksPage
    {
        private readonly bool templatesOnly = false;
        private readonly WizardMode wizardMode;

        public CrossPoolMigrateStoragePage(WizardMode wizardMode)
        {
            DisplayDiskCapacity = false;
            this.wizardMode = wizardMode;

            InitializeText();
        }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override bool SrIsSuitable(SR sr)
        {
            return sr != null && !sr.HBALunPerVDI();
        }

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_SELECT_STORAGE_PAGE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_SELECT_STORAGE_PAGE_TEXT; } }

        protected override string IntroductionText { get { return Messages.CPM_WIZARD_STORAGE_INSTRUCTIONS; } }
        protected override string AllOnSameSRRadioButtonText { get { return Messages.CPM_WIZARD_ALL_ON_SAME_SR_RADIO; } }
        protected override string OnSpecificSRRadioButtonText { get { return Messages.CPM_WIZARD_SPECIFIC_SR_RADIO; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return wizardMode == WizardMode.Copy ? "StorageCopyMode" : "Storage"; } }

        public override StorageResourceContainer ResourceData(string sysId)
        {
            VM vm = Connection.Resolve(new XenRef<VM>(sysId));

            if (vm == null)
                return null;

            List<VDI> vdis = Connection.ResolveAll(vm.VBDs).Select(v => vm.Connection.Resolve(v.VDI)).ToList();
            vdis.RemoveAll(vdi => vdi == null || Connection.Resolve(vdi.SR).GetSRType(true) == SR.SRTypes.iso);
            return new CrossPoolMigrationStorageResourceContainer(vdis);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (!CrossPoolMigrateWizard.AllVMsAvailable(VmMappings, Connection))
            {
                cancel = true;
                SetButtonNextEnabled(false);
                SetButtonPreviousEnabled(false);
            }

            base.PageLeave(direction, ref cancel);
        }

        protected override string VmDiskColumnHeaderText
        {
            get
            {
                return templatesOnly ? Messages.CPS_WIZARD_STORAGE_PAGE_DISK_COLUMN_HEADER_FOR_TEMPLATE : Messages.CPS_WIZARD_STORAGE_PAGE_DISK_COLUMN_HEADER_FOR_VM;
            }
        }

        /// <summary>
        /// Add storage mappings for snapshots
        /// </summary>
        /// <param name="vmMappings"></param>
        protected override void AddAditionalMappings(Dictionary<string, VmMapping> vmMappings)
        {
            foreach (var kvp in vmMappings)
            {
                var vm = Connection.Resolve(new XenRef<VM>(kvp.Key));
                if (vm == null)
                    continue;

                var vmMapping = kvp.Value;
                var suitableSr = vmMapping.Storage.Values.FirstOrDefault(); //an SR already selected for VM's disks

                foreach (var snapshot in Connection.ResolveAll(vm.snapshots))
                {
                    foreach (IStorageResource resourceData in ResourceData(snapshot.opaque_ref))
                    {
                        var snapshotVdi = Connection.Resolve(new XenRef<VDI>(resourceData.Tag.ToString()));
                        if (snapshotVdi == null)
                            continue;

                        // try to use the mapping of the original vdi; if this doesn't exist, then use another suitable SR
                        if (snapshotVdi.snapshot_of != null && vmMapping.Storage.ContainsKey(snapshotVdi.snapshot_of.opaque_ref))
                            vmMapping.Storage[snapshotVdi.opaque_ref] = vmMapping.Storage[snapshotVdi.snapshot_of.opaque_ref];
                        else if (suitableSr != null)
                            vmMapping.Storage[snapshotVdi.opaque_ref] = suitableSr;
                    }
                }
            }
        }
    }
}
