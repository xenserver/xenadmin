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
        private bool loading = false;

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

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            VM template = SelectedTemplate;
            bool installMethodIsNetwork = SelectedInstallMethod == InstallMethod.Network;

            if (template == Template && InstallMethodIsNetwork == installMethodIsNetwork)
                return;

            loading = true;
            Template = template;

            InstallMethodIsNetwork = installMethodIsNetwork;
            if ((!Template.DefaultTemplate() && !Template.HasAtLeastOneDisk())
                || (Template.IsHVM() && InstallMethodIsNetwork)) // CA-46213 The default should be "diskless" if the install method is "boot from network"
            {
                DisklessVMRadioButton.Checked = true;
            }
            else
                DisksRadioButton.Checked = true;

            LoadDisks();
            loading = false;
            UpdateEnablement(true);
        }

        public override void SelectDefaultControl()
        {
            DisksGridView.Select();
        }

        private void LoadDisks()
        {
            DisksGridView.Rows.Clear();
            var rowList = new List<DataGridViewRow>();

            XmlNode provision = Template.ProvisionXml();
            if (provision != null)
            {
                foreach (XmlNode diskNode in provision.ChildNodes)
                {
                    var device = new VBD
                    {
                        userdevice = diskNode.Attributes["device"].Value,
                        bootable = diskNode.Attributes["bootable"].Value == "true",
                        mode = vbd_mode.RW
                    };

                    var diskSize = long.Parse(diskNode.Attributes["size"].Value);
                    SR srUuid = Connection.Cache.Find_By_Uuid<SR>(diskNode.Attributes["sr"].Value);
                    SR sr = GetBestDiskStorage(Connection, diskSize, Affinity, srUuid);

                    var disk = new VDI
                    {
                        name_label = string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_VDINAME, SelectedName, device.userdevice),
                        name_description = Messages.NEWVMWIZARD_STORAGEPAGE_DISK_DESCRIPTION,
                        virtual_size = diskSize,
                        type = (vdi_type)Enum.Parse(typeof(vdi_type), diskNode.Attributes["type"].Value),
                        read_only = false,
                        SR = new XenRef<SR>(sr != null ? sr.opaque_ref : Helper.NullOpaqueRef)
                    };

                    rowList.Add(new DiskGridRowItem(Connection, disk, device, DiskSource.FromDefaultTemplate));
                }
            }
            else
            {
                var vbds = Connection.ResolveAll(Template.VBDs);
                foreach (VBD vbd in vbds)
                {
                    if (vbd.type != vbd_type.Disk)
                        continue;

                    VDI vdi = Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    var device = new VBD
                    {
                        userdevice = vbd.userdevice,
                        bootable = vbd.bootable,
                        mode = vbd.mode
                    };

                    SR sr = GetBestDiskStorage(Connection, vdi.virtual_size, Affinity, Connection.Resolve(vdi.SR));

                    var disk = new VDI
                    {
                        name_label = vdi.name_label,
                        name_description = vdi.name_description,
                        virtual_size = vdi.virtual_size,
                        type = vdi.type,
                        read_only = vdi.read_only,
                        sm_config = vdi.sm_config,
                        SR = new XenRef<SR>(sr != null ? sr.opaque_ref : Helper.NullOpaqueRef)
                    };

                    rowList.Add(new DiskGridRowItem(Connection, disk, device, DiskSource.FromCustomTemplate));
                }
            }

            DisksGridView.Rows.AddRange(rowList.ToArray());
        }

        /// <summary>
        /// Tries to find the best SR for the given VDI considering first the
        /// suggestedSR then the pool's default SR, then other SRs.
        /// </summary>
        /// <returns>The SR if a suitable one is found, otherwise null</returns>
        private static SR GetBestDiskStorage(IXenConnection connection, long diskSize, Host affinity, SR suggestedSR)
        {
            if (suggestedSR != null && suggestedSR.CanBeSeenFrom(affinity) &&
                suggestedSR.VdiCreationCanProceed(diskSize))
                return suggestedSR;

            SR defaultSR = connection.Resolve(Helpers.GetPoolOfOne(connection).default_SR);
            if (defaultSR != null && defaultSR.CanBeSeenFrom(affinity) &&
                defaultSR.VdiCreationCanProceed(diskSize))
                return defaultSR;

            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.CanCreateVmOn() &&
                    sr.CanBeSeenFrom(affinity) && sr.VdiCreationCanProceed(diskSize))
                    return sr;
            }

            return null;
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
            using (var dialog = new NewDiskDialog(Connection, Template, Affinity, SrPicker.SRPickerType.LunPerVDI, null,
                true, 0, AddedVDIs) {DontCreateVDI = true})
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                DisksGridView.Rows.Add(new DiskGridRowItem(Connection, dialog.NewDisk(), dialog.NewDevice(), DiskSource.New));
                UpdateEnablement();
            }
        }

        private void DisksRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DisksRadioButton.Checked)
                UpdateEnablement();
        }

        private void DisklessVMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DisklessVMRadioButton.Checked)
                UpdateEnablement();
        }

        private void UpdateEnablement(bool pageLoad = false)
        {
            if (loading)
                return;

            AddButton.Enabled = DisksRadioButton.Checked && DisksGridView.Rows.Count < Template.MaxVBDsAllowed() - 1;
            EditButton.Enabled = DisksRadioButton.Checked && DisksGridView.SelectedRows.Count > 0;
            DeleteButton.Enabled = DisksRadioButton.Checked && DisksGridView.SelectedRows.Count > 0 && ((DiskGridRowItem)DisksGridView.SelectedRows[0]).CanDelete;
            DisksGridView.Enabled = DisksRadioButton.Checked;
            DisklessVMRadioButton.Enabled = Template.IsHVM() && InstallMethodIsNetwork;

            CheckForOverCommit();

            CloneCheckBox.Enabled = false;

            if (!Template.DefaultTemplate())
            {
                foreach (DiskGridRowItem row in DisksGridView.Rows)
                {
                    if (!row.CanDelete && row.SourceSR != null && row.Disk != null)
                    {
                        SR dest = Connection.Resolve(row.Disk.SR);

                        if (row.SourceSR.Equals(dest))
                        {
                            CloneCheckBox.Enabled = true;

                            if (pageLoad)
                                CloneCheckBox.Checked = true;

                            break;
                        }
                    }
                }
            }

            if (!CloneCheckBox.Enabled)
                CloneCheckBox.Checked = false;

            OnPageUpdated();
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

                if(sr.HBALunPerVDI()) //No over commit in this case
                    continue;

                if (totalDiskSize.ContainsKey(sr.opaque_ref))
                    totalDiskSize[sr.opaque_ref] += item.Disk.virtual_size;
                else
                    totalDiskSize[sr.opaque_ref] = item.Disk.virtual_size;

                if (totalDiskInitialAllocation.ContainsKey(sr.opaque_ref))
                    totalDiskInitialAllocation[sr.opaque_ref] += item.Disk.virtual_size;
                else
                    totalDiskInitialAllocation[sr.opaque_ref] = item.Disk.virtual_size;
            }
            DiskOverCommit overcommitedDisk = DiskOverCommit.None;
            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                SR sr = Connection.Resolve(item.Disk.SR);

                if (sr == null)
                    continue;

                if (sr.HBALunPerVDI()) //No over commit in this case
                    continue;

                if (item.Disk.SR.opaque_ref != sr.opaque_ref)
                    continue;

                if (!sr.VdiCreationCanProceed(totalDiskInitialAllocation[sr.opaque_ref]))
                    overcommitedDisk = item.OverCommit = DiskOverCommit.Error;
                else if (sr.FreeSpace() < totalDiskInitialAllocation[sr.opaque_ref])
                    overcommitedDisk = item.OverCommit = DiskOverCommit.Warning;

                if (item.OverCommit != DiskOverCommit.None)
                {
                    item.ImageToolTip = 
                        string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_SROVERCOMMIT, 
                                                Helpers.GetName(sr), 
                                                Util.DiskSizeString(sr.FreeSpace()),
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

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (DisksGridView.SelectedRows.Count <= 0)
                return;

            DiskGridRowItem selectedItem = (DiskGridRowItem) DisksGridView.SelectedRows[0];

            using (var dialog = new NewDiskDialog(Connection, Template, Affinity, SrPicker.SRPickerType.LunPerVDI, 
                    selectedItem.Disk, selectedItem.CanResize, selectedItem.MinSize, AddedVDIs)
                {DontCreateVDI = true})
            {
                if (dialog.ShowDialog(ParentForm) != DialogResult.OK)
                    return;

                selectedItem.Disk = dialog.NewDisk();
                selectedItem.UpdateDetails();
                UpdateEnablement();
            }
        }

        #region Accessors

        /// <summary>
        /// When the VM is created by the New VM Wizard, VM.copy or VM.clone is
        /// used depending on what SRs the disks are on:
        /// - If the disks are all on the same SR, this SR is returned and VM.copy is used.
        /// - If at least one disk is on same SR as the source disk, this SR is returned and VM.copy is used.
        /// - Otherwise, this property returns null and VM.clone is used.
        /// </summary>
        public SR FullCopySR
        {
            get
            {
                if (!Template.DefaultTemplate() && !Template.is_a_snapshot && !CloneCheckBox.Checked)
                {
                    SR sr = null;
                    List<SR> targetSRs = new List<SR>();
                    foreach (DiskGridRowItem row in DisksGridView.Rows)
                    {
                        if (!row.CanDelete && row.SourceSR != null && row.Disk != null)
                        {
                            SR target = Connection.Resolve(row.Disk.SR);

                            if (sr == null && row.SourceSR.Equals(target))
                                sr = row.SourceSR;

                            if (!targetSRs.Contains(target))
                                targetSRs.Add(target);
                        }
                    }

                    return targetSRs.Count == 1 ? targetSRs[0] : sr;
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
        public readonly SR SourceSR;
        public VDI Disk;
        public readonly VBD Device;
        public readonly IXenConnection Connection;
        public readonly bool CanDelete;
        public readonly bool CanResize;
        public readonly long MinSize;
        public DiskOverCommit OverCommit = DiskOverCommit.None;
        public string ImageToolTip;

        private readonly DataGridViewImageCell ImageCell = new DataGridViewImageCell(false) {ValueType = typeof(Image)};
        private readonly DataGridViewTextBoxCell SizeCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell SrCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell SharedCell = new DataGridViewTextBoxCell();

        public DiskGridRowItem(IXenConnection connection, VDI vdi, VBD vbd, DiskSource src)
        {
            Disk = vdi;
            Device = vbd;
            Connection = connection;

            if (src == DiskSource.FromCustomTemplate)
            {
                SourceSR = Connection.Resolve(vdi.SR);
            }
            else
            {
                CanDelete = Disk.type == vdi_type.user;
                CanResize = true;
            }

            if (src == DiskSource.FromDefaultTemplate)
                MinSize = Disk.virtual_size;

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
                SizeCell.Value = sr.HBALunPerVDI() ? String.Empty : Util.DiskSizeString(Disk.virtual_size);

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
    }

    public enum DiskOverCommit
    {
        None = 0,
        Warning = 1,
        Error = 2
    }

    public enum DiskSource
    {
        New,
        FromDefaultTemplate,
        FromCustomTemplate
    }
}
