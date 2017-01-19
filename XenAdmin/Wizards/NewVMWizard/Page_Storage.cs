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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions.VMActions;
using XenAdmin.Dialogs;
using XenAPI;
using System.Xml;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_Storage : XenTabPage
    {
        private VM Template;
        private bool InstallMethodIsNetwork;

        public Page_Storage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_STORAGEPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_STORAGEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "Storage"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            VM template = SelectedTemplate;
            bool installMethodIsNetwork = SelectedInstallMethod == InstallMethod.Network;

            if (template == Template && InstallMethodIsNetwork == installMethodIsNetwork)
                return;

            Template = template;

            InstallMethodIsNetwork = installMethodIsNetwork;
            if ((!Template.DefaultTemplate && !Template.HasAtLeastOneDisk)
                || (Template.IsHVM && InstallMethodIsNetwork)) // CA-46213 The default should be "diskless" if the install method is "boot from network"
            {
                DisklessVMRadioButton.Checked = true;
            }
            else
                DisksRadioButton.Checked = true;
            DisksGridView.Rows.Clear();
            LoadDisks();
            UpdateEnablement();
            UpdateCloneCheckboxEnablement(true);
        }

        public override void SelectDefaultControl()
        {
            DisksGridView.Select();
        }

        private void LoadDisks()
        {
            XmlNode provision = Template.ProvisionXml;
            if (provision != null)
            {
                foreach (XmlNode disk in provision.ChildNodes)
                {
                    DisksGridView.Rows.Add(new DiskGridRowItem(Connection, disk, SelectedName, Affinity));
                }
            }
            else
            {
                foreach (VBD vbd in Connection.ResolveAll(Template.VBDs))
                {
                    if (vbd.type != vbd_type.Disk)
                        continue;

                    VDI vdi = Connection.Resolve(vbd.VDI);
                    if (vdi != null)
                    {
                        DisksGridView.Rows.Add(new DiskGridRowItem(Connection, vdi, vbd, false, Affinity));
                    }
                }

            }
        }

        public override bool EnableNext()
        {
            return (DisklessVMRadioButton.Checked || (DisksGridView.Rows.Count > 0 && AllDisksHaveSRs())) && CheckForOverCommit() != DiskOverCommit.Error;
        }

        private bool AllDisksHaveSRs()
        {
            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                if (item.Disk.SR.opaque_ref == Helper.NullOpaqueRef)
                    return false;
            }

            return true;
        }

        private IEnumerable<VDI> AddedVDIs
        {
            get
            {
                return from DiskGridRowItem row in DisksGridView.Rows select row.Disk;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NewDiskDialog dialog = new NewDiskDialog(Connection, Template, SrPicker.SRPickerType.LunPerVDI, null, Affinity, true, 0, AddedVDIs);
            dialog.DontCreateVDI = true;
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            DisksGridView.Rows.Add(new DiskGridRowItem(Connection, dialog.NewDisk(), dialog.NewDevice(), true, Affinity));
            UpdateEnablement();
        }

        private void DisksRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void DisklessVMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            AddButton.Enabled = DisksRadioButton.Checked && DisksGridView.Rows.Count < Template.MaxVBDsAllowed - 1;
            PropertiesButton.Enabled = DisksRadioButton.Checked && DisksGridView.SelectedRows.Count > 0;
            DeleteButton.Enabled = DisksRadioButton.Checked && DisksGridView.SelectedRows.Count > 0 && ((DiskGridRowItem)DisksGridView.SelectedRows[0]).CanDelete;
            DisksGridView.Enabled = DisksRadioButton.Checked;
            DisklessVMRadioButton.Enabled = Template.IsHVM && InstallMethodIsNetwork;

            CheckForOverCommit();

            OnPageUpdated();
            UpdateCloneCheckboxEnablement(false);
        }

        private void UpdateCloneCheckboxEnablement(bool pageLoad)
        {
            CloneCheckBox.Enabled = false;

            if (!Template.DefaultTemplate)
            {
                foreach (DiskGridRowItem row in DisksGridView.Rows)
                {
                    if (!row.CanDelete)
                    {
                        SR src = row.SourceDisk == null ? null : Connection.Resolve<SR>(row.SourceDisk.SR);
                        SR dest = Connection.Resolve<SR>(row.Disk.SR);

                        if (src != null && src.Equals(dest))
                        {
                            CloneCheckBox.Enabled = true;

                            if (pageLoad)
                            {
                                CloneCheckBox.Checked = true;
                            }

                            break;
                        }
                    }
                }
            }

            if (!CloneCheckBox.Enabled)
            {
                CloneCheckBox.Checked = false;
            }
        }

        private DiskOverCommit CheckForOverCommit()
        {
            Dictionary<string, long> totalDiskSize = new Dictionary<string, long>(); // total size of the new disks on each SR (calculated using vdi.virtual_size)
            Dictionary<string, long> totalDiskInitialAllocation = new Dictionary<string, long>(); // total initial allocation of the new disks on each SR (calculated using vdi.InitialAllocation)

            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                item.OverCommit = DiskOverCommit.None; // reset all errors
                item.ImageToolTip = "";
                SR sr = Connection.Resolve(item.Disk.SR);

                if (sr == null) // no sr assigned
                    continue;

                if(sr.HBALunPerVDI) //No over commit in this case
                    continue;

                if (totalDiskSize.ContainsKey(sr.opaque_ref))
                    totalDiskSize[sr.opaque_ref] += item.Disk.virtual_size;
                else
                    totalDiskSize[sr.opaque_ref] = item.Disk.virtual_size;

                var initialSpace = Helpers.GetRequiredSpaceToCreateVdiOnSr(sr, item.Disk);
                if (totalDiskInitialAllocation.ContainsKey(sr.opaque_ref))
                    totalDiskInitialAllocation[sr.opaque_ref] +=  initialSpace;
                else
                    totalDiskInitialAllocation[sr.opaque_ref] = initialSpace;
            }
            DiskOverCommit overcommitedDisk = DiskOverCommit.None;
            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                SR sr = Connection.Resolve(item.Disk.SR);

                if (sr == null)
                    continue;

                if (sr.HBALunPerVDI) //No over commit in this case
                    continue;

                if (item.Disk.SR.opaque_ref != sr.opaque_ref)
                    continue;

                if (sr.FreeSpace < totalDiskInitialAllocation[sr.opaque_ref])
                    overcommitedDisk = item.OverCommit = DiskOverCommit.Error;

                if (item.OverCommit != DiskOverCommit.None)
                {
                    item.ImageToolTip = sr.IsThinProvisioned ? 
                        string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_SROVERCOMMIT_THIN,
                                                Helpers.GetName(sr),
                                                Util.DiskSizeString(sr.FreeSpace),
                                                Util.DiskSizeString(totalDiskSize[sr.opaque_ref]), 
                                                Util.DiskSizeString(totalDiskInitialAllocation[sr.opaque_ref])) :
                        string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_SROVERCOMMIT, 
                                                Helpers.GetName(sr), 
                                                Util.DiskSizeString(sr.FreeSpace),
                                                Util.DiskSizeString(totalDiskSize[sr.opaque_ref]));
                }
                item.UpdateDetails();
            }
            return overcommitedDisk;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (DisksGridView.SelectedRows.Count <= 0)
                return;

            DisksGridView.Rows.Remove(DisksGridView.SelectedRows[0]);
            UpdateEnablement();
        }

        private void PropertiesButton_Click(object sender, EventArgs e)
        {
            if (DisksGridView.SelectedRows.Count <= 0)
                return;

            DiskGridRowItem selectedItem = ((DiskGridRowItem)DisksGridView.SelectedRows[0]);

            NewDiskDialog dialog = new NewDiskDialog(Connection, Template, SrPicker.SRPickerType.LunPerVDI, selectedItem.Disk, Affinity, selectedItem.CanResize, selectedItem.MinSize, AddedVDIs);
            dialog.DontCreateVDI = true;
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            selectedItem.Disk = dialog.NewDisk();
            selectedItem.UpdateDetails();

            UpdateEnablement();
        }

        #region Accessors

        /// <summary>
        /// Gets the SR that should be used as the parameter for VM.copy when the VM is created by the New VM Wizard. If null
        /// is returned then VM.clone should be used.
        /// </summary>
        public SR FullCopySR
        {
            get
            {
                if (!Template.DefaultTemplate && !CloneCheckBox.Checked)
                {
                    // if target disks are all on the same SR then use that SR
                    // otherwise iterate through disks and find first target disks that is on same SR as source disk

                    SR sr = null;
                    List<SR> targetSRs = new List<SR>();
                    foreach (DiskGridRowItem row in DisksGridView.Rows)
                    {
                        if (!row.CanDelete && row.SourceDisk != null && row.Disk != null)
                        {
                            SR src = Connection.Resolve<SR>(row.SourceDisk.SR);
                            SR target = Connection.Resolve<SR>(row.Disk.SR);

                            if (sr == null && src != null && src.Equals(target))
                            {
                                sr = src;
                            }

                            if (!targetSRs.Contains(target))
                            {
                                targetSRs.Add(target);
                            }
                        }
                    }

                    if (targetSRs.Count == 1)
                    {
                        return targetSRs[0];
                    }
                    return sr;
                }
                return null;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                var sum = new List<KeyValuePair<string, string>>();

                foreach (DiskDescription d in SelectedDisks)
                {
                    sum.Add(new KeyValuePair<string, string>(
                        string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_DISK, Helpers.GetName(d.Disk).Ellipsise(30)),
                        Util.DiskSizeString(d.Disk.virtual_size)));
                }
                return sum;
            }
        }

        public List<DiskDescription> SelectedDisks
        {
            get
            {
                if (DisklessVMRadioButton.Checked)
                    return new List<DiskDescription>();

                List<DiskDescription> disks = new List<DiskDescription>();
                foreach (DiskGridRowItem item in DisksGridView.Rows)
                {
                    disks.Add(new DiskDescription(item.Disk, item.Device));
                }

                return disks;
            }
        }

        public VM SelectedTemplate { private get; set; }
        public string SelectedName { private get; set; }
        public InstallMethod SelectedInstallMethod { private get; set; }
        public Host Affinity { private get; set; }

        #endregion

        private void DisksGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }
    }

    public class DiskGridRowItem : DataGridViewRow
    {
        public readonly VDI SourceDisk;
        public VDI Disk;
        public readonly VBD Device;
        public readonly IXenConnection Connection;
        public readonly bool CanDelete;
        public readonly bool CanResize;
        public readonly long MinSize;
        public DiskOverCommit OverCommit = DiskOverCommit.None;
        public string ImageToolTip;

        private DataGridViewImageCell ImageCell;
        private DataGridViewTextBoxCell SizeCell;
        private DataGridViewTextBoxCell NameCell;
        private DataGridViewTextBoxCell SrCell;
        private DataGridViewTextBoxCell SharedCell;

        public DiskGridRowItem(IXenConnection connection, XmlNode diskNode, string vmName, Host affinity)
        {
            Disk = new VDI();
            Device = new VBD();
            Connection = connection;

            Disk.virtual_size = long.Parse(diskNode.Attributes["size"].Value);
            SR sruuid = connection.Cache.Find_By_Uuid<SR>(diskNode.Attributes["sr"].Value);
            SR sr = GetBeskDiskStorage(Connection, Disk, affinity, sruuid == null ? null : sruuid);
            Disk.SR = new XenRef<SR>(sr != null ? sr.opaque_ref : Helper.NullOpaqueRef);
            Disk.type = (vdi_type)Enum.Parse(typeof(vdi_type), diskNode.Attributes["type"].Value);
            Device.userdevice = diskNode.Attributes["device"].Value;
            Device.bootable = diskNode.Attributes["bootable"].Value == "true";

            Disk.name_label = string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_VDINAME, vmName, Device.userdevice); //Device.userdevice;
            Disk.read_only = false;
            Disk.name_description = Messages.NEWVMWIZARD_STORAGEPAGE_DISK_DESCRIPTION;
            Device.mode = vbd_mode.RW;

            CanDelete = Disk.type == vdi_type.user;
            CanResize = true;
            MinSize = Disk.virtual_size;

            AddCells();
        }

        public DiskGridRowItem(IXenConnection connection, VDI vdi, VBD vbd, bool isNew, Host affinity)
        {
            SourceDisk = vdi;
            Disk = new VDI();
            Device = new VBD();
            Connection = connection;

            Disk.virtual_size = vdi.virtual_size;
            SR sr = GetBeskDiskStorage(Connection, vdi, affinity, Connection.Resolve(vdi.SR));
            Disk.SR = new XenRef<SR>(sr != null ? sr.opaque_ref : Helper.NullOpaqueRef);
            Disk.type = vdi.type;
            Device.userdevice = vbd.userdevice;
            Device.bootable = vbd.bootable;

            Disk.name_label = vdi.name_label;
            Disk.read_only = vdi.read_only;
            Disk.name_description = vdi.name_description;
            Disk.sm_config = vdi.sm_config;
            Device.mode = vbd.mode;

            CanDelete = Disk.type == vdi_type.user && isNew;
            CanResize = isNew;
            MinSize = 0;

            AddCells();
        }

        private void AddCells()
        {
            ImageCell = new DataGridViewImageCell(false) {ValueType = typeof(Image)};
            NameCell = new DataGridViewTextBoxCell();
            SizeCell = new DataGridViewTextBoxCell();
            SrCell = new DataGridViewTextBoxCell();
            SharedCell = new DataGridViewTextBoxCell();

            Cells.AddRange(ImageCell, NameCell, SrCell, SizeCell, SharedCell);

            UpdateDetails();
        }

        public void UpdateDetails()
        {
            switch (OverCommit)
            {
                case DiskOverCommit.None:
                    ImageCell.Value = Properties.Resources._000_VirtualStorage_h32bit_16;
                    break;
                case DiskOverCommit.Warning:
                    ImageCell.Value = Properties.Resources._000_Alert2_h32bit_16;
                    break;
                case DiskOverCommit.Error:
                    ImageCell.Value = Properties.Resources._000_error_h32bit_16;
                    break;
            }
            ImageCell.ToolTipText = ImageToolTip;
            SizeCell.ToolTipText = ImageToolTip;
            NameCell.ToolTipText = ImageToolTip;
            SrCell.ToolTipText = ImageToolTip;
            SharedCell.ToolTipText = ImageToolTip;

            SR sr = Connection.Resolve(Disk.SR);
            if(sr == null)
                SizeCell.Value = Util.DiskSizeString(Disk.virtual_size);
            else
                SizeCell.Value = sr.HBALunPerVDI ? String.Empty : Util.DiskSizeString(Disk.virtual_size);

            NameCell.Value = Helpers.GetName(Disk);

            if (Disk.SR.opaque_ref != Helper.NullOpaqueRef)
            {
                SrCell.Value = Helpers.GetName(sr);
                SharedCell.Value = sr.shared ? Messages.TRUE : Messages.FALSE;
            }
            else
            {
                SrCell.Value = Messages.NEWVMWIZARD_STORAGEPAGE_NOSTORAGE;
                SharedCell.Value = "";
            }
        }

        /// <summary>
        /// Tries to find the best SR for the given VDI considering the suggestedSR which has priority over other SRs in this check.
        /// SuggestedSR, default SR, other SRs are checked.
        /// Returns first suitable SR or NULL.
        /// </summary>
        private static SR GetBeskDiskStorage(IXenConnection connection, VDI disk, Host affinity, SR suggestedSR)
        {
            // try suggestion
            if (suggestedSR != null && suggestedSR.CanBeSeenFrom(affinity) && IsSufficientFreeSpaceAvailableOnSrForVdi(suggestedSR, disk))
                return suggestedSR;

            // try default sr
            SR defaultSR = connection.Resolve(Helpers.GetPoolOfOne(connection).default_SR);
            if (defaultSR != null && defaultSR.CanBeSeenFrom(affinity) && IsSufficientFreeSpaceAvailableOnSrForVdi(defaultSR, disk))
                return defaultSR;

            // pick an sr
            foreach (SR sr in connection.Cache.SRs)
            {
                if (!sr.CanCreateVmOn())
                    continue;

                if (sr.CanBeSeenFrom(affinity) && IsSufficientFreeSpaceAvailableOnSrForVdi(sr, disk))
                    return sr;
            }

            // there has been no suitable SR found
            return null; 
        }


        /// <summary>
        /// Checks whether there is enough space available on the SR to accommodate a VDI.
        /// </summary>
        private static bool IsSufficientFreeSpaceAvailableOnSrForVdi(SR sr, VDI disk)
        {
            return sr != null && !sr.IsFull && sr.FreeSpace > Helpers.GetRequiredSpaceToCreateVdiOnSr(sr, disk);
        }
    }

    public enum DiskOverCommit : int
    {
        None = 0,
        Warning = 1,
        Error = 2
    }
}
