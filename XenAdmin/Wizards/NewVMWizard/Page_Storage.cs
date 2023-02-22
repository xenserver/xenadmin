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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private VM _template;
        private InstallMethod _selectedInstallMethod;
        private Host _affinity;
        private bool _loadRequired = true;
        private bool loading;
        private SR _fullCopySR;
        private bool _canCreateFullCopy;

        public Page_Storage()
        {
            InitializeComponent();
        }

        public override string Text => Messages.NEWVMWIZARD_STORAGEPAGE_NAME;

        public override string PageTitle => Messages.NEWVMWIZARD_STORAGEPAGE_TITLE;

        public override string HelpID => "Storage";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (!_loadRequired)
                return;

            loading = true;

            var isDefaultTemplate = Template.DefaultTemplate();
            var isHvmTemplate = Template.IsHVM();

            DisklessVMRadioButton.Enabled = isHvmTemplate && SelectedInstallMethod == InstallMethod.Network;

            // CA-46213 The default should be "diskless" if the install method is "boot from network"
            if (!isDefaultTemplate && !Template.HasAtLeastOneDisk() ||
                isHvmTemplate && SelectedInstallMethod == InstallMethod.Network)
            {
                DisklessVMRadioButton.Checked = true;
            }
            else
            {
                DisksRadioButton.Checked = true;
            }

            CloneCheckBox.Visible = !isDefaultTemplate;

            LoadDisks();
            loading = false;
            _loadRequired = false;
            UpdateEnablement(true);
        }

        public override void SelectDefaultControl()
        {
            if (DisksRadioButton.Checked)
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

                    var disk = new VDI
                    {
                        name_label = string.Format(Messages.STRING_SPACE_STRING, SelectedName, device.userdevice),
                        name_description = Messages.NEWVMWIZARD_STORAGEPAGE_DISK_DESCRIPTION,
                        virtual_size = long.Parse(diskNode.Attributes["size"].Value),
                        type = (vdi_type)Enum.Parse(typeof(vdi_type), diskNode.Attributes["type"].Value),
                        read_only = false
                    };

                    SR srUuid = Connection.Cache.Find_By_Uuid<SR>(diskNode.Attributes["sr"].Value);
                    SR sr = GetBestDiskStorage(Connection, new[] { disk }, Affinity, srUuid, out Image icon,
                        out string tooltip);
                    disk.SR = new XenRef<SR>(sr != null ? sr.opaque_ref : Helper.NullOpaqueRef);

                    var row = new DiskGridRowItem(Connection, disk, device, DiskSource.FromDefaultTemplate);
                    row.UpdateStatus(icon, tooltip);
                    rowList.Add(row);
                }
            }
            else
            {
                _canCreateFullCopy = true;
                var vbds = Connection.ResolveAll(Template.VBDs);

                foreach (VBD vbd in vbds)
                {
                    if (vbd.type != vbd_type.Disk)
                        continue;

                    VDI vdi = Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    if (!vdi.allowed_operations.Contains(vdi_operations.copy))
                        _canCreateFullCopy = false;

                    var sourceSr = Connection.Resolve(vdi.SR);

                    var device = new VBD
                    {
                        userdevice = vbd.userdevice,
                        bootable = vbd.bootable,
                        mode = vbd.mode
                    };

                    SR sr = GetBestDiskStorage(Connection, new[] { vdi }, Affinity, Connection.Resolve(vdi.SR),
                        out Image icon, out string tooltip);

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

                    var row = new DiskGridRowItem(Connection, disk, device, DiskSource.FromCustomTemplate, sourceSr);
                    row.UpdateStatus(icon, tooltip);
                    rowList.Add(row);
                }
            }

            DisksGridView.Rows.AddRange(rowList.ToArray());
            UpdateStatusForEachDisk(true);
        }

        /// <summary>
        /// Tries to find the best SR for the given VDI considering first the
        /// suggestedSR then the pool's default SR, then other SRs.
        /// </summary>
        /// <returns>The SR if a suitable one is found, otherwise null</returns>
        private SR GetBestDiskStorage(IXenConnection connection, VDI[] vdis, Host affinity, SR suggestedSr,
            out Image icon, out string tooltip)
        {
            icon = Images.StaticImages._000_VirtualStorage_h32bit_16;
            tooltip = null;
            var sb = new StringBuilder();

            var suggestedSrVisible = suggestedSr != null && suggestedSr.CanBeSeenFrom(affinity);
            var suggestedSrHasSpace = suggestedSr != null && suggestedSr.CanFitDisks(out _, vdis);

            if (suggestedSrVisible && suggestedSrHasSpace)
                return suggestedSr;

            if (suggestedSrVisible)
                sb.AppendFormat(Messages.NEWVMWIZARD_STORAGEPAGE_SUGGESTED_NOSPACE, suggestedSr.Name().Ellipsise(50)).AppendLine();
            else if (suggestedSrHasSpace)
                sb.AppendFormat(Affinity == null
                        ? Messages.NEWVMWIZARD_STORAGEPAGE_SUGGESTED_LOCAL_NO_HOME
                        : Messages.NEWVMWIZARD_STORAGEPAGE_SUGGESTED_LOCAL,
                    suggestedSr.Name().Ellipsise(50)).AppendLine();

            SR defaultSr = connection.Resolve(Helpers.GetPoolOfOne(connection).default_SR);
            var defaultSrVisible = defaultSr != null && defaultSr.CanBeSeenFrom(affinity);
            var defaultSrHasSpace = defaultSr != null && defaultSr.CanFitDisks(out _, vdis);

            if (defaultSrVisible && defaultSrHasSpace)
            {
                if (suggestedSr != null)
                {
                    sb.AppendLine(string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_XC_SELECTION, BrandManager.BrandConsole));
                    tooltip = sb.ToString();
                    icon = Images.StaticImages._000_Alert2_h32bit_16;
                }
                return defaultSr;
            }

            if (defaultSrVisible && !defaultSr.Equals(suggestedSr))
                sb.AppendFormat(Messages.NEWVMWIZARD_STORAGEPAGE_DEFAULT_NOSPACE, defaultSr.Name().Ellipsise(50)).AppendLine();
            else if (defaultSrHasSpace && !defaultSr.Equals(suggestedSr))
                sb.AppendFormat(Affinity == null
                        ? Messages.NEWVMWIZARD_STORAGEPAGE_DEFAULT_LOCAL_NO_HOME
                        : Messages.NEWVMWIZARD_STORAGEPAGE_DEFAULT_LOCAL,
                    defaultSr.Name().Ellipsise(50)).AppendLine();

            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.SupportsVdiCreate() && !sr.IsBroken(false) &&
                    sr.CanBeSeenFrom(affinity) && sr.CanFitDisks(out _, vdis))
                {
                    if (suggestedSr != null || defaultSr != null)
                    {
                        sb.AppendLine(string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_XC_SELECTION, BrandManager.BrandConsole));
                        tooltip = sb.ToString();
                        icon = Images.StaticImages._000_Alert2_h32bit_16;
                    }
                    return sr;
                }
            }

            return null;
        }

        public override bool EnableNext()
        {
            if (DisklessVMRadioButton.Checked)
                return true;

            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                if (item.HasError)
                    return false;
            }

            return true;
        }

        private void UpdateEnablement(bool pageLoad = false)
        {
            if (loading)
                return;

            AddButton.Enabled = DisksGridView.Rows.Count < Template.MaxVBDsAllowed() - 1;
            EditButton.Enabled = DisksGridView.SelectedRows.Count == 1;
            DeleteButton.Enabled = DisksGridView.SelectedRows.Count == 1 && ((DiskGridRowItem)DisksGridView.SelectedRows[0]).CanDelete;

            _fullCopySR = null;

            if (!Template.DefaultTemplate())
            {
                if (_canCreateFullCopy)
                {
                    var targetSRs = new List<SR>();
                    var disks = DisksGridView.Rows.Cast<DiskGridRowItem>()
                        .Where(r => !r.CanDelete && r.SourceSR != null && r.Disk != null)
                        .Select(r => r.Disk).ToArray();

                    foreach (DiskGridRowItem row in DisksGridView.Rows)
                    {
                        if (row.CanDelete || row.SourceSR == null || row.Disk == null)
                            continue;

                        SR target = Connection.Resolve(row.Disk.SR);

                        if (_fullCopySR == null && row.SourceSR.Equals(target) &&
                            target.CanFitDisks(out _, disks))
                            _fullCopySR = target;

                        if (!targetSRs.Contains(target))
                            targetSRs.Add(target);
                    }

                    if (targetSRs.Count == 1 && targetSRs[0].CanFitDisks(out _, disks))
                        _fullCopySR = targetSRs[0];
                }

                CloneCheckBox.Enabled = _fullCopySR != null;

                if (_fullCopySR == null || pageLoad)
                    CloneCheckBox.Checked = true;
            }

            OnPageUpdated();
        }

        private void UpdateStatusForEachDisk(bool pageLoad = false)
        {
            // total size of the new disks on each SR (calculated using vdi.virtual_size)
            var disksPerSr = new Dictionary<string, List<VDI>>();

            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                SR sr = Connection.Resolve(item.Disk.SR);

                if (sr == null) // no sr assigned
                    continue;

                if (sr.HBALunPerVDI()) //No over commit in this case
                    continue;

                if (disksPerSr.ContainsKey(sr.opaque_ref))
                    disksPerSr[sr.opaque_ref].Add(item.Disk);
                else
                    disksPerSr[sr.opaque_ref] = new List<VDI> { item.Disk };
            }

            foreach (DiskGridRowItem item in DisksGridView.Rows)
            {
                SR sr = Connection.Resolve(item.Disk.SR);

                if (sr == null)
                    continue;

                if (sr.HBALunPerVDI()) //No over commit in this case
                    continue;

                if (!sr.CanFitDisks(out var toolTip, disksPerSr[sr.opaque_ref].ToArray()))
                {
                    item.UpdateStatus(Images.StaticImages._000_error_h32bit_16, toolTip);
                    continue;
                }

                var freeSpace = sr.FreeSpace();
                var totalVirtualSize = disksPerSr[sr.opaque_ref].Sum(v => v.virtual_size);

                if (freeSpace < totalVirtualSize)
                {
                    item.UpdateStatus(Images.StaticImages._000_Alert2_h32bit_16, string.Format(Messages.NEWVMWIZARD_STORAGEPAGE_SROVERCOMMIT,
                        Helpers.GetName(sr),
                        Util.DiskSizeString(sr.FreeSpace()),
                        Util.DiskSizeString(totalVirtualSize)));
                    continue;
                }
                
                if (!pageLoad)
                    item.UpdateStatus(Images.StaticImages._000_VirtualStorage_h32bit_16, "");
            }
        }

        #region Control event handlers

        private void DisksRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DisksRadioButton.Checked)
                OnPageUpdated();
        }

        private void DisklessVMRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DisklessVMRadioButton.Checked)
                OnPageUpdated();
        }


        private void AddButton_Click(object sender, EventArgs e)
        {
            DisksRadioButton.Checked = true;

            using (var dialog = new NewDiskDialog(Connection, Template, Affinity, SrPicker.SRPickerType.LunPerVDI, null,
                    true, 0, AddedVDIs) { DontCreateVDI = true })
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                DisksGridView.Rows.Add(new DiskGridRowItem(Connection, dialog.Disk, dialog.Device, DiskSource.New));
            }

            UpdateStatusForEachDisk();
            UpdateEnablement();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DisksRadioButton.Checked = true;

            if (DisksGridView.SelectedRows.Count <= 0)
                return;

            DisksGridView.Rows.Remove(DisksGridView.SelectedRows[0]);
            UpdateStatusForEachDisk();
            UpdateEnablement();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            DisksRadioButton.Checked = true;

            if (DisksGridView.SelectedRows.Count <= 0)
                return;

            DiskGridRowItem selectedItem = (DiskGridRowItem) DisksGridView.SelectedRows[0];

            using (var dialog = new NewDiskDialog(Connection, Template, Affinity, SrPicker.SRPickerType.LunPerVDI, 
                    selectedItem.Disk, selectedItem.CanResize, selectedItem.MinSize, AddedVDIs)
                {DontCreateVDI = true})
            {
                if (dialog.ShowDialog(ParentForm) != DialogResult.OK)
                    return;

                selectedItem.Disk = dialog.Disk;
                selectedItem.UpdateDetails();
            }

            UpdateStatusForEachDisk();
            UpdateEnablement();
        }


        private void DisksGridView_SelectionChanged(object sender, EventArgs e)
        {
            EditButton.Enabled = DisksGridView.SelectedRows.Count == 1;
            DeleteButton.Enabled = DisksGridView.SelectedRows.Count == 1 && ((DiskGridRowItem)DisksGridView.SelectedRows[0]).CanDelete;
        }

        private void DisksGridView_Enter(object sender, EventArgs e)
        {
            DisksRadioButton.Checked = true;
        }

        #endregion

        #region Accessors

        private IEnumerable<VDI> AddedVDIs =>
            from DiskGridRowItem row in DisksGridView.Rows select row.Disk;

        public SR FullCopySR =>
            Template.DefaultTemplate() || _fullCopySR == null || CloneCheckBox.Checked
                ? null
                : _fullCopySR;

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

        public VM Template
        {
            private get { return _template; }
            set
            {
                if (_template != value)
                    _loadRequired = true;

                _template = value;
            }
        }

        public string SelectedName { private get; set; }

        public InstallMethod SelectedInstallMethod
        {
            private get { return _selectedInstallMethod; }
            set
            {
                if (_selectedInstallMethod != value)
                    _loadRequired = true;

                _selectedInstallMethod = value;
            }
        }

        public Host Affinity
        {
            private get { return _affinity; }
            set
            {
                if (_affinity != value)
                    _loadRequired = true;

                _affinity = value;
            }
        }

        #endregion
    }

    public class DiskGridRowItem : DataGridViewRow
    {
        public readonly SR SourceSR;
        public VDI Disk;
        public readonly VBD Device;
        private readonly IXenConnection _connection;
        public readonly bool CanDelete;
        public readonly bool CanResize;
        public readonly long MinSize;
        
        private readonly DataGridViewImageCell ImageCell = new DataGridViewImageCell(false) {ValueType = typeof(Image)};
        private readonly DataGridViewTextBoxCell SizeCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell SrCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell SharedCell = new DataGridViewTextBoxCell();

        public bool HasError => Disk.SR.opaque_ref == Helper.NullOpaqueRef ||
                                Cells.Count > 0 && Cells[0].Value == Images.StaticImages._000_error_h32bit_16;

        public DiskGridRowItem(IXenConnection connection, VDI vdi, VBD vbd, DiskSource src, SR sourceSr = null)
        {
            _connection = connection;
            Disk = vdi;
            Device = vbd;
            SourceSR = sourceSr;

            if (src != DiskSource.FromCustomTemplate)
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
            SR sr = _connection.Resolve(Disk.SR);
            if (sr == null)
            {
                SizeCell.Value = Util.DiskSizeString(Disk.virtual_size);
                SrCell.Value = Messages.NEWVMWIZARD_STORAGEPAGE_NOSTORAGE;
                SharedCell.Value = "";
            }
            else
            {
                SizeCell.Value = sr.HBALunPerVDI() ? string.Empty : Util.DiskSizeString(Disk.virtual_size);
                SrCell.Value = Helpers.GetName(sr);
                SharedCell.Value = sr.shared ? Messages.TRUE : Messages.FALSE;
            }

            NameCell.Value = Helpers.GetName(Disk);
        }

        public void UpdateStatus(Image icon, string toolTipTExt)
        {
            ImageCell.Value = icon;

            foreach (DataGridViewCell cell in Cells)
                cell.ToolTipText = toolTipTExt;
        }
    }

    public enum DiskSource
    {
        New,
        FromDefaultTemplate,
        FromCustomTemplate
    }
}
