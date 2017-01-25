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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Picker for LunPerVDI SRs
    /// </summary>
    public partial class LunPerVdiPicker : UserControl
    {
        public LunPerVdiPicker()
        {
            InitializeComponent();
            RegisterEvents();
            VdiColumnTitle = Messages.LUNPERVDIPICKER_VDI_COLUMN_TITLE;
        }

        /// <summary>
        /// The user has made a selection in a dropdown cell
        /// </summary>
        public OnSelectionChangedHandler OnSelectionChanged;
        public delegate void OnSelectionChangedHandler(object sender, EventArgs e);

        private readonly List<LunPerVdiPickerItem> pendingMappings = new List<LunPerVdiPickerItem>();

        /// <summary>
        /// Add a range of items to the picker
        /// </summary>
        /// <param name="mappingsToAdd"></param>
        public void AddRange(List<LunPerVdiPickerItem> mappingsToAdd)
        {
            pendingMappings.AddRange(mappingsToAdd.Where(i => !pendingMappings.Contains(i)));
            PopulateDataGridView();
        }

        /// <summary>
        /// Items mapped by user
        /// </summary>
        public List<LunPerVdiPickerItem> MappedItems
        {
            get
            {
                List<LunPerVdiPickerItem> list = dataGridView.Rows.OfType<LunPerVdiPickerItem>().ToList();
                list.AddRange(unmappedItems);
                return list;
            }
        }

        /// <summary>
        /// If user has made a complete, valid selection
        /// </summary>
        public bool SelectionIsValid
        {
            get { 
                return dataGridView.Rows.Count > 0 && 
                       dataGridView.Rows.Cast<LunPerVdiPickerItem>().All(row => row != null && row.SelectedVdi != null); 
            }
        }

        public void Clear()
        {
            dataGridView.Rows.Clear();
            unmappedItems.Clear();
            pendingMappings.Clear();
        }

        private void RegisterEvents()
        {
            dataGridView.CellClick += dataGridView_CellClick;
            dataGridView.CurrentCellDirtyStateChanged += dataGridViewCell_CurrentCellDirtyStateChanged;
        }

        private readonly List<LunPerVdiPickerItem> unmappedItems = new List<LunPerVdiPickerItem>();

        private void PopulateDataGridView()
        {
            foreach (LunPerVdiPickerItem item in pendingMappings)
            {
                if (item != null && item.IsValidForMapping && !dataGridView.Rows.Contains(item))
                {
                    dataGridView.Rows.Add(item);
                }

                else if (item != null && !unmappedItems.Contains(item))
                    unmappedItems.Add(item);
            }
            pendingMappings.Clear();
        }

        public string VdiColumnTitle
        {
            set { dataGridView.Columns[VdiColumn.Index].HeaderText = value; }
        }

        /// <summary>
        /// Deal with the changes made to the dropdown cell by firing an event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewCell_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            foreach (LunPerVdiPickerItem row in dataGridView.SelectedRows)
            {
                LunComboBoxItem item = row.Cells[LunColumn.Index].Value as LunComboBoxItem;
                DisableOtherEquivalentValues(item, row.Index);
            }

            if (OnSelectionChanged != null)
                OnSelectionChanged(this, new EventArgs());
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex !=  LunColumn.Index || e.RowIndex < 0 || e.RowIndex >= dataGridView.RowCount)
                return;

            dataGridView.BeginEdit(false);

            DataGridViewEnableableComboBoxCell senderItem = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewEnableableComboBoxCell;
            if (senderItem == null)
                return;

            if (dataGridView.EditingControl != null && dataGridView.EditingControl is ComboBox)
            {
                EnableAllEquivalentValues(senderItem.Value as LunComboBoxItem);
                (dataGridView.EditingControl as ComboBox).DroppedDown = true;
            }
        }

        #region Make the selection unique
        private void DisableOtherEquivalentValues(LunComboBoxItem item, int excludingRowIndex)
        {
            foreach (LunPerVdiPickerItem row in dataGridView.Rows)
            {
                DataGridViewEnableableComboBoxCell cb = row.Cells[LunColumn.Index] as DataGridViewEnableableComboBoxCell;
                if (cb == null)
                    return;

                if (row.Index != excludingRowIndex)
                {
                    foreach (LunComboBoxItem i in cb.Items)
                    {
                        i.DisableItemMatching(item);
                    }
                }
            }
            dataGridView.Refresh();
        }

        private void EnableAllEquivalentValues(LunComboBoxItem item)
        {
            foreach (LunPerVdiPickerItem row in dataGridView.Rows)
            {
                DataGridViewEnableableComboBoxCell cb = row.Cells[LunColumn.Index] as DataGridViewEnableableComboBoxCell;
                if (cb == null)
                    return;

                foreach (LunComboBoxItem i in cb.Items)
                {
                    i.EnableItemMatching(item);
                }
            }
            dataGridView.Refresh();
        } 
        #endregion
    }

    /// <summary>
    /// Class used as a row for the LunPerVdi picker. Responsible for making
    /// the cells required and filling them with data
    /// </summary>
    public class LunPerVdiPickerItem : DataGridViewRow
    {
        public LunPerVdiPickerItem(SR LUNsourceSr, VDI vdi)
        {
            Vdi = vdi;
            Sr = LUNsourceSr;
            LunConstraints = new List<Predicate<VDI>> { v => Vdi != null && v.virtual_size < Vdi.virtual_size };
        }

        public SR Sr { get; private set; }
        public VDI Vdi { get; private set; }
        public VDI SelectedVdi 
        { 
            get
            {
                LunComboBoxItem item = Cells[LunColumn].Value as LunComboBoxItem;
                return item == null ? null : item.Vdi;
            } 
        }

        public bool IsValidForMapping
        {
            get
            {
                if(Sr == null)
                    return false;

                return Sr.HBALunPerVDI;
            }
        }

        private const int LunColumn = 1;

        protected virtual string VdiColumnText
        {
            get
            {
                if (Vdi == null)
                    return String.Empty;

                return Vdi.Name;
            }
        }

        protected virtual string SrColumnText
        {
            get
            {
                if (Sr == null)
                    return String.Empty;

                return Sr.Name;
            }
        }

        protected void ConstructCells()
        {
            var tbVDI = new DataGridViewTextBoxCell { Value = VdiColumnText };
            var tbSR = new DataGridViewTextBoxCell { Value = SrColumnText };
            var cbLUN = new DataGridViewEnableableComboBoxCell{FlatStyle = FlatStyle.Flat};
            foreach (VDI vdi in Sr.Connection.ResolveAll(Sr.VDIs))
            {
                cbLUN.Items.Add(new LunComboBoxItem(vdi) { AdditionalConstraints = LunConstraints });
            }

            cbLUN.Items.OfType<LunComboBoxItem>().OrderBy(i=>i.Enabled);
            Cells.AddRange(tbVDI, cbLUN, tbSR);
            Debug.Assert(cbLUN.Items.Count == Sr.VDIs.Count, "Not all combobox items were converted");
        }

        protected List<Predicate<VDI>> LunConstraints { get; private set; }

        public override bool Equals(object obj)
        {
            LunPerVdiPickerItem cf = obj as LunPerVdiPickerItem;
            if (cf == null)
                return false;

            if(Vdi == null || cf.Vdi == null)
                return VdiColumnText.Equals(cf.VdiColumnText);

            if ( !String.IsNullOrEmpty(Vdi.opaque_ref) && !String.IsNullOrEmpty(cf.Vdi.opaque_ref))
                return Vdi.Equals(cf.Vdi);

            return VdiColumnText.Equals(cf.VdiColumnText);
        }

        public override int GetHashCode()
        {
            return Vdi.GetHashCode();
        }
    }

    /// <summary>
    /// Class used as element in the ComboBox cell
    /// </summary>
    internal class LunComboBoxItem : IEnableableComboBoxItem
    {
        public VDI Vdi { get; private set; }
        public LunComboBoxItem(VDI vdi)
        {
            Debug.Assert(vdi != null, "VDI passed to combobox was null");
            Vdi = vdi;
            AdditionalConstraints = new List<Predicate<VDI>>();
        }

        public List<Predicate<VDI>> AdditionalConstraints { get; set; }

        private bool enabled = true;
        public bool Enabled
        {
            get
            {
                if(Vdi.VBDs.Count > 0)
                    return false;

                if (AdditionalConstraints.Any(constraint => constraint.Invoke(Vdi)))
                    return false;

                return enabled;
            }
        }

        public void DisableItemMatching(LunComboBoxItem itemToDisable)
        {
            SetMatchingItemEnabledState(itemToDisable, false);
        }

        public void EnableItemMatching(LunComboBoxItem itemToEnable)
        {
            SetMatchingItemEnabledState(itemToEnable, true);
        }

        private void SetMatchingItemEnabledState(LunComboBoxItem itemToMatch, bool enabledState)
        {
            if (itemToMatch == null)
                return;

            if (Vdi == itemToMatch.Vdi)
                enabled = enabledState;
        }

        public override string ToString()
        {
            return String.Format(Messages.VALUE_HYPHEN_VALUE, Vdi.name_label, Vdi.SizeText);
        }

        public override bool Equals(object obj)
        {
            LunComboBoxItem cf = obj as LunComboBoxItem;
            if(cf == null)
                return false;

            return Vdi.Equals(cf.Vdi);
        }

        public override int GetHashCode()
        {
            return Vdi.GetHashCode();
        }
    }
}
