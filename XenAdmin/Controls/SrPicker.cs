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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    public partial class SrPicker : UserControl
    {
        // Migrate is the live VDI move operation
        public enum SRPickerType { VM, InstallFromTemplate, MoveOrCopy, Migrate, LunPerVDI };
        private SRPickerType usage = SRPickerType.VM;
        
        //Used in the MovingVDI usage
        private VDI[] existingVDIs;
        public void SetExistingVDIs(VDI[] vdis)
        {
            existingVDIs = vdis;
        }

        private IXenConnection connection;

        private Host affinity;
        private SrPickerItem LastSelectedItem;
        public event Action ItemSelectionNull;
        public event Action ItemSelectionNotNull;
        public event EventHandler DoubleClickOnRow;
        public long DiskSize = 0;
        public long? OverridenInitialAllocationRate = null;

        public SrPicker(IXenConnection connection, SRPickerType usage) : this(connection)
        {
            this.usage = usage;
        }

        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke;
        public SrPicker(IXenConnection connection)
        {
            this.connection = connection;
            InitializeComponent();

            srListBox.ShowCheckboxes = false;
            srListBox.ShowDescription = true;
            srListBox.ShowImages = true;
            srListBox.NodeIndent = 3;
            srListBox.SelectedIndexChanged += srListBox_SelectedIndexChanged;
            srListBox.DoubleClickOnRow += srListBox_DoubleClickOnRow;

            SrHint.Text = usage == SRPickerType.MoveOrCopy ?
                Messages.IMPORT_WIZARD_TEMPLATE_SR_HINT_TEXT :
                Messages.IMPORT_WIZARD_VM_SR_HINT_TEXT;

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
            {
                pool.PropertyChanged -= Server_PropertyChanged;
                pool.PropertyChanged += Server_PropertyChanged;
            }
            SR_CollectionChangedWithInvoke=Program.ProgramInvokeHandler(SR_CollectionChanged);
            connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

            refresh();
        }

        public SrPicker()
        {
            InitializeComponent();
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
        }

        public SRPickerType Usage
        {
            set { usage = value; }
        }

        /// <summary>
        /// For new disk dialog only
        /// </summary>
        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                if (value == null)
                    return;
                connection = value;

                srListBox.ShowCheckboxes = false;
                srListBox.ShowDescription = true;
                srListBox.ShowImages = true;
                srListBox.NodeIndent = 3;
                srListBox.SelectedIndexChanged += srListBox_SelectedIndexChanged;
                srListBox.DoubleClickOnRow += srListBox_DoubleClickOnRow;

                SrHint.Text = Messages.NEW_DISK_DIALOG_SR_HINT_TEXT;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool != null)
                {
                    pool.PropertyChanged -= Server_PropertyChanged;
                    pool.PropertyChanged += Server_PropertyChanged;
                }

                connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

                refresh();
            }
        }

        private void srListBox_DoubleClickOnRow(object sender, EventArgs e)
        {
            if (DoubleClickOnRow != null)
                DoubleClickOnRow(sender, e);
        }

        void srListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            onItemSelect();
        }

        private void onItemSelect()
        {
            SrPickerItem item = srListBox.SelectedItem as SrPickerItem;
            if (item == null || !item.Enabled)
            {
                if (ItemSelectionNull != null)
                    ItemSelectionNull();
                return;
            }

            if (ItemSelectionNotNull != null)
                ItemSelectionNotNull();

            if (!item.Enabled && LastSelectedItem != null && LastSelectedItem.TheSR.opaque_ref != item.TheSR.opaque_ref)
                srListBox.SelectedItem = LastSelectedItem;
            else if (!item.Enabled && LastSelectedItem != null && LastSelectedItem.TheSR.opaque_ref == item.TheSR.opaque_ref)
            {
                SrPickerItem first = srListBox.Items[0] as SrPickerItem;
                if (first != null && first.Enabled)
                    srListBox.SelectedItem = first;
                else
                    srListBox.SelectedItem = null;
            }
            else
                LastSelectedItem = item;

                            

        }

        public SR SR
        {
            get
            {
                return srListBox.SelectedItem is SrPickerItem && (srListBox.SelectedItem as SrPickerItem).Enabled ? (srListBox.SelectedItem as SrPickerItem).TheSR : null;
            }
        }

        public SR DisabledSelectedSR
        {
            get 
            {
                return srListBox.SelectedItem is SrPickerItem && !(srListBox.SelectedItem as SrPickerItem).Enabled ? (srListBox.SelectedItem as SrPickerItem).TheSR : null;
            }
        }

        public SR DefaultSR = null;

		public void SetAffinity(Host host)
		{
			affinity = host;
			refresh();
		}

        /// <summary>
        /// Returns how much disk space is required to create the disk on an SR
        /// This depends on whether the SR is thin provisioned and on what the initial allocation rate is
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        private long GetRequiredDiskSizeForSR(SR sr)
        {
            long allocationRate = DiskSize;

            if (DiskSize > sr.physical_size)
                return DiskSize;

            if (sr != null && sr.IsThinProvisioned)
            {
                if (OverridenInitialAllocationRate.HasValue)
                {
                    allocationRate = OverridenInitialAllocationRate.Value;
                }
                else
                {
                    long temp = 0;

                    if (sr.sm_config != null && sr.sm_config.ContainsKey("initial_allocation") && long.TryParse(sr.sm_config["initial_allocation"], out temp))
                    {
                        allocationRate = temp;
                    }
                }
            }

            return Math.Min(allocationRate, DiskSize);
        }

        private readonly SrPickerItemFactory itemFactory = new SrPickerItemFactory();

    	public void refresh()
        {
            Program.AssertOnEventThread();

            SR selectedSr = SR;
            bool selected = false;
            srListBox.BeginUpdate();
            try
            {
                srListBox.ClearAllNodes();

                foreach (SR sr in connection.Cache.SRs)
                {
                    SrPickerItem item = itemFactory.PickerItem(sr, usage, affinity, GetRequiredDiskSizeForSR(sr), existingVDIs);
                    if (item.Show)
                        srListBox.AddNode(item);
                    foreach (PBD pbd in sr.Connection.ResolveAll(sr.PBDs))
                    {
                        if (pbd != null)
                        {
                            pbd.PropertyChanged -= Server_PropertyChanged;
                            pbd.PropertyChanged += Server_PropertyChanged;
                        }
                    }
                    sr.PropertyChanged -= Server_PropertyChanged;
                    sr.PropertyChanged += Server_PropertyChanged;
                }
            }
            finally
            {
                srListBox.EndUpdate();
            }

            if (selectedSr != null)
            {
                foreach (SrPickerItem node in srListBox.Items)
                {
                    if (node.TheSR != null && node.TheSR.uuid == selectedSr.uuid)
                    {
                        srListBox.SelectedItem = node;
                        onItemSelect();
                        selected = true;
                        break;
                    }
                }
            }

            if (!selected && srListBox.Items.Count > 0)
            {// If no selection made, select default SR
                if (!selectDefaultSR())
                {
                    // If no default SR, select first entry in list
                    srListBox.SelectedIndex = 0;
                    onItemSelect();
                }
            }
        }

        public void UpdateDiskSize()
        {
            Program.AssertOnEventThread();
            try
            {
                foreach (SrPickerItem node in srListBox.Items)
                {
                    node.UpdateDiskSize(GetRequiredDiskSizeForSR(node.TheSR));
                }
            }
            finally
            {
                srListBox.Refresh();
                onItemSelect();
            }
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "PBDs" || e.PropertyName == "physical_utilisation" || e.PropertyName == "currently_attached" || e.PropertyName == "default_SR")
                Program.Invoke(this, refresh);
        }

        void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, refresh);
        }

        private void UnregisterHandlers()
        {
            if (connection == null)
                return;

            var pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
                pool.PropertyChanged -= Server_PropertyChanged;

            foreach (var sr in connection.Cache.SRs)
            {
                foreach (var pbd in sr.Connection.ResolveAll(sr.PBDs))
                {
                    if (pbd != null)
                        pbd.PropertyChanged -= Server_PropertyChanged;
                }
                sr.PropertyChanged -= Server_PropertyChanged;
            }

            connection.Cache.DeregisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);
        }

        /// <summary>
        /// Selects the default SR, if it exists.
        /// </summary>
        /// <returns>true if the default SR was selected, otherwise false.</returns>
        public bool selectDefaultSR()
        {
            if (DefaultSR == null)
                return false;

            foreach (SrPickerItem node in srListBox.Items)
            {
                if (node.TheSR != null && node.TheSR == DefaultSR && node.Enabled)
                {
                    srListBox.SelectedItem = node;
                    return true;
                }
            }
            return false;
        }

        internal void selectSRorNone(SR TheSR)
        {
            foreach (SrPickerItem node in srListBox.Items)
            {
                if (node.TheSR != null && node.TheSR.opaque_ref == TheSR.opaque_ref)
                {
                    srListBox.SelectedItem = node;
                    return;
                }
            }

            if (ItemSelectionNull != null)
                ItemSelectionNull();
        }

        internal void selectDefaultSROrAny()
        {
            if (selectDefaultSR())
                return;
            foreach (SrPickerItem item in srListBox.Items)
            {
                if (item == null)
                    continue;
                if (item.Enabled)
                {
                    selectSRorNone(item.TheSR);
                    return;
                }
            }
            if (ItemSelectionNull != null)
                ItemSelectionNull();
        }

        public void selectSRorDefaultorAny(SR sr)
        {
            if (sr != null)
            {
                foreach (SrPickerItem node in srListBox.Items)
                {
                    if (node.TheSR != null && node.TheSR.opaque_ref == sr.opaque_ref)
                    {
                        srListBox.SelectedItem = node;
                        return;
                    }
                }
            }
            selectDefaultSROrAny();
        }

        public bool ValidSelectionExists
        {
            get
            {
                foreach (SrPickerItem item in srListBox.Items)
                {
                    if (item == null)
                        continue;
                    if (item.Enabled)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }


    
}
