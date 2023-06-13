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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
    /// <summary>
    /// Class representing the page of the ImportAppliance wizard where the user specifies
    /// the targets where the VMs of the appliance will be imported
    /// </summary>
    internal abstract partial class SelectMultipleVMDestinationPage : XenTabPage
    {
        private Dictionary<string, VmMapping> m_vmMappings;
        private bool updatingDestinationCombobox;
        private bool restoreGridHomeServerSelection;
        private bool updatingHomeServerList;
        private bool m_buttonNextEnabled;
        protected List<IXenConnection> ignoredConnections = new List<IXenConnection>();
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private string _preferredHomeRef;
        private IXenObject _selectedTargetPool;
        private IXenObject _selectedTarget;

        #region Nested classes

        /// <summary>
        /// Combobox item that can run a command but also be an IEnableableComboBoxItem
        /// </summary>
        private class AddHostRunningComboBoxItem : IEnableableComboBoxItem
        {
            public override string ToString()
            {
                return Messages.ADD_POOL_OR_SERVER;
            }

            public void RunCommand(Control parent)
            {
                new AddHostCommand(Program.MainWindow, parent).Run();
            }

            public bool Enabled => true;
        }

        private class NoTargetServerPoolItem : IEnableableXenObjectComboBoxItem
        {
            private readonly Pool _pool;

            public NoTargetServerPoolItem(Pool pool)
            {
                _pool = pool;
            }

            public IXenObject Item => _pool;

            public bool Enabled => true;

            public override string ToString()
            {
                return Messages.DONT_SELECT_TARGET_SERVER;
            }
        }

        #endregion

        protected SelectMultipleVMDestinationPage()
        {
            InitializeComponent();
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged);
            ConnectionsManager.XenConnections.CollectionChanged += CollectionChanged;
            ShowWarning(null);
        }

        protected void InitializeText()
        {
            m_labelIntro.Text = InstructionText;
            label1.Text = TargetPoolText;
            label2.Text = TargetServerSelectionIntroText;
            m_colVmName.HeaderText = VmColumnHeaderText;
        }

        public IXenObject SelectedTargetPool
        {
            get => _selectedTargetPool;
            private set
            {
                var oldTargetPool = _selectedTargetPool;
                _selectedTargetPool = value;

                if (oldTargetPool?.opaque_ref != _selectedTargetPool?.opaque_ref)
                    OnSelectedTargetPoolChanged();
            }
        }

        public IXenObject SelectedTarget
        {
            get => _selectedTarget;
            set
            {
                var oldTarget = _selectedTarget;
                _selectedTarget = value;

                if (oldTarget?.opaque_ref != SelectedTarget?.opaque_ref)
                    OnSelectedTargetChanged();
            }
        }

        /// <summary>
        /// Text containing instructions for use - at the top of the page
        /// </summary>
        protected abstract string InstructionText { get; }

        /// <summary>
        /// Text specifying the label for the target pool or standalone server drop down
        /// </summary>
        protected abstract string TargetPoolText { get; }

        protected virtual string VmColumnHeaderText => Messages.VM;

        /// <summary>
        /// Text above the table containing a list of VMs and concomitant home server
        /// </summary>
        protected abstract string TargetServerSelectionIntroText { get; }

        protected virtual void OnSelectedTargetPoolChanged()
        { }

        protected virtual void OnSelectedTargetChanged()
        {

        }

        protected void ShowWarning(string warningText)
        {
            if (string.IsNullOrEmpty(warningText))
                tableLayoutPanelWarning.Visible = false;
            else
            {
                labelWarning.Text = warningText;
                tableLayoutPanelWarning.Visible = true;
            }
        }

        #region Base class (XenTabPage) overrides

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            SelectedTargetPool = null;
            restoreGridHomeServerSelection = direction == PageLoadedDirection.Back;
            PopulateComboBox();
        }

        public override void PageCancelled(ref bool cancel)
        {
            UnregisterHandlers();
            CancelFilters();
            ClearComboBox();
            ClearDataGridView();
            SelectedTargetPool = null;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (!PerformCheck())
            {
                cancel = true;
                SetButtonNextEnabled(false);
                return;
            }

            UnregisterHandlers();
            ClearComboBox();
        }

        public override void SelectDefaultControl()
        {
            m_comboBoxConnection.Select();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        #endregion

        #region Accessors

        public Dictionary<string, VmMapping> VmMappings
        {
            get
            {
                foreach (DataGridViewRow row in m_dataGridView.Rows)
                {
                    var sysId = (string)row.Cells[0].Tag;

                    if (m_vmMappings.ContainsKey(sysId))
                    {
                        var mapping = m_vmMappings[sysId];
                        var cbCell = row.Cells[m_colTarget.Index] as DataGridViewEnableableComboBoxCell;
                        System.Diagnostics.Debug.Assert(cbCell != null, "ComboBox cell was not found");

                        var selectedItem = cbCell.Value as IEnableableXenObjectComboBoxItem;
                        System.Diagnostics.Debug.Assert(selectedItem != null, "Vm has no target mapped");
                        var type = selectedItem.Item.GetType();

                        if (type == typeof(Pool))
                            mapping.XenRef = new XenRef<Pool>(selectedItem.Item.opaque_ref);
                        else if (type == typeof(Host))
                            mapping.XenRef = new XenRef<Host>(selectedItem.Item.opaque_ref);

                        mapping.TargetName = selectedItem.Item.Name();
                    }
                }

                return m_vmMappings;
            }
            set => m_vmMappings = value;
        }

        #endregion

        protected abstract DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem);

        #region Private methods

        private void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

        protected virtual bool PerformCheck()
        {
            return true;
        }

        private void ClearComboBox()
        {
            Program.AssertOnEventThread();

            foreach (var item in m_comboBoxConnection.Items)
            {
                if (item is DelayLoadingOptionComboBoxItem tempItem)
                    tempItem.ReasonUpdated -= DelayLoadedComboBoxItem_ReasonChanged;
            }
            m_comboBoxConnection.Items.Clear();
        }

        private void ClearDataGridView()
        {
            //Clear up comboboxes
            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                row.Cells[m_colTarget.Index].Dispose();
            }

            m_dataGridView.Rows.Clear();
            m_dataGridView.Refresh();
        }

        private void PopulateComboBox()
        {
            Program.AssertOnEventThread();

            ClearDataGridView();

            updatingDestinationCombobox = true;
            ClearComboBox();

            var targetConnections = ConnectionsManager.XenConnectionsCopy.Where(con => con.IsConnected).Except(ignoredConnections).ToList();
            foreach (var xenConnection in targetConnections)
            {
                DelayLoadingOptionComboBoxItem item = null;

                var pool = Helpers.GetPool(xenConnection);

                if (pool == null)
                {
                    var host = Helpers.GetCoordinator(xenConnection);

                    if (host != null)
                    {
                        item = CreateDelayLoadingOptionComboBoxItem(host);
                        m_comboBoxConnection.Items.Add(item);
                        item.ReasonUpdated += DelayLoadedComboBoxItem_ReasonChanged;
                        item.LoadAsync();
                        host.PropertyChanged -= PropertyChanged;
                        host.PropertyChanged += PropertyChanged;
                    }
                }
                else
                {
                    item = CreateDelayLoadingOptionComboBoxItem(pool);
                    m_comboBoxConnection.Items.Add(item);
                    item.ReasonUpdated += DelayLoadedComboBoxItem_ReasonChanged;
                    item.LoadAsync();
                    pool.PropertyChanged -= PropertyChanged;
                    pool.PropertyChanged += PropertyChanged;
                }

                if (item != null && SelectedTarget != null && item.Item.Connection == SelectedTarget.Connection)
                    _preferredHomeRef = item.Item.opaque_ref;

                xenConnection.ConnectionStateChanged -= xenConnection_ConnectionStateChanged;
                xenConnection.ConnectionStateChanged += xenConnection_ConnectionStateChanged;
                xenConnection.CachePopulated -= xenConnection_CachePopulated;
                xenConnection.CachePopulated += xenConnection_CachePopulated;
                xenConnection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
            }

            m_comboBoxConnection.Items.Add(new AddHostRunningComboBoxItem());
            updatingDestinationCombobox = false;
        }

        private bool MatchingWithXenRefObject(IEnableableXenObjectComboBoxItem item, object xenRef)
        {
            if (xenRef is XenRef<Host> hostRef)
                return hostRef.opaque_ref == item.Item.opaque_ref;

            if (xenRef is XenRef<Pool> poolRef)
                return poolRef.opaque_ref == item.Item.opaque_ref;

            return false;
        }

        private void RestoreGridHomeServerSelectionFromMapping()
        {
            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                var sysId = (string)row.Cells[m_colVmName.Index].Tag;

                if (m_vmMappings.TryGetValue(sysId, out var mapping) &&
                    row.Cells[m_colTarget.Index] is DataGridViewEnableableComboBoxCell cbCell)
                {
                    var item = cbCell.Items.OfType<IEnableableXenObjectComboBoxItem>()
                        .FirstOrDefault(cbi => MatchingWithXenRefObject(cbi, mapping.XenRef));
                    if (item != null)
                        cbCell.Value = item;
                }
            }
        }

        private void PopulateDataGridView()
        {
            Program.AssertOnEventThread();

            updatingHomeServerList = true;
            try
            {
                var target = m_comboBoxConnection.SelectedItem as DelayLoadingOptionComboBoxItem;

                m_dataGridView.SuspendLayout();

                ClearDataGridView();

                var hasPoolSharedStorage = target != null && HasPoolSharedStorage(target.Item.Connection);

                foreach (var kvp in m_vmMappings)
                {
                    var tb = new DataGridViewTextBoxCell { Value = kvp.Value.VmNameLabel, Tag = kvp.Key };
                    var cb = new DataGridViewEnableableComboBoxCell { FlatStyle = FlatStyle.Flat };

                    if (target != null)
                    {
                        if (hasPoolSharedStorage)
                        {
                            //there exists one pool per connection
                            var pools = target.Item.Connection.Cache.Pools;
                            if (pools.Length > 0)
                            {
                                var pool = pools.First();
                                var item = new NoTargetServerPoolItem(pool);
                                cb.Items.Add(item);

                                if ((SelectedTarget != null && SelectedTarget.opaque_ref == pool.opaque_ref) ||
                                    target.Item.opaque_ref == pool.opaque_ref)
                                    _preferredHomeRef = item.Item.opaque_ref;
                            }
                        }

                        var sortedHosts = new List<Host>(target.Item.Connection.Cache.Hosts);
                        sortedHosts.Sort();

                        foreach (var host in sortedHosts)
                        {
                            var filters = CreateTargetServerFilterList(host, new List<string> { kvp.Key });
                            var item = new DelayLoadingOptionComboBoxItem(host, filters);
                            cb.Items.Add(item);
                            item.ParentComboBox = cb;
                            if (SelectedTarget != null && SelectedTarget.opaque_ref == host.opaque_ref ||
                                target.Item.opaque_ref == host.opaque_ref)
                                _preferredHomeRef = item.Item.opaque_ref;
                            item.ReasonUpdated += DelayLoadedGridComboBoxItem_ReasonChanged;
                            item.LoadAsync();
                        }

                        if (cb.Items.Count == 1 && cb.Items[0] is DelayLoadingOptionComboBoxItem it)
                            _preferredHomeRef = it.Item.opaque_ref;
                    }

                    SetComboBoxPreSelection(cb);

                    var row = new DataGridViewRow();
                    row.Cells.AddRange(tb, cb);
                    m_dataGridView.Rows.Add(row);
                }

                HelpersGUI.ResizeGridViewColumnToAllCells(m_colTarget); //set properly the width of the last column

                if (restoreGridHomeServerSelection)
                {
                    RestoreGridHomeServerSelectionFromMapping();
                    restoreGridHomeServerSelection = false;
                }
            }
            finally
            {
                updatingHomeServerList = false;
                m_dataGridView.ResumeLayout();
            }
        }

        private void SetComboBoxPreSelection(DataGridViewEnableableComboBoxCell cb)
        {
            if (cb.Value == null)
            {
                var firstEnabled = cb.Items.OfType<IEnableableComboBoxItem>().FirstOrDefault(i => i.Enabled);
                if (firstEnabled != null)
                {
                    cb.Value = firstEnabled;
                    SetButtonNextEnabled(true);
                }
                else
                {
                    SetButtonNextEnabled(false);
                }
            }
            else
            {
                SetButtonNextEnabled(true);
            }
        }

        private static bool HasPoolSharedStorage(IXenConnection conn)
        {
            if (conn == null)
                return false;

            foreach (var pbd in conn.Cache.PBDs.Where(thePbd => thePbd.SR != null))
            {
                var sr = conn.Resolve(pbd.SR);

                if (sr != null && sr.SupportsVdiCreate() && sr.shared)
                    return true;
            }
            return false;
        }

        #endregion

        #region Event Handlers

        private void DelayLoadedComboBoxItem_ReasonChanged(DelayLoadingOptionComboBoxItem item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

            Program.Invoke(this, () =>
            {
                var index = m_comboBoxConnection.Items.IndexOf(item);
                if (index < 0 || index >= m_comboBoxConnection.Items.Count)
                    return;

                if (updatingDestinationCombobox || updatingHomeServerList)
                    return;

                var selectedIndex = m_comboBoxConnection.SelectedIndex;

                if (!(m_comboBoxConnection.Items[index] is DelayLoadingOptionComboBoxItem tempItem))
                    throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

                tempItem.CopyFrom(item);

                try
                {
                    m_comboBoxConnection.BeginUpdate();
                    m_comboBoxConnection.Items.RemoveAt(index);

                    if (updatingDestinationCombobox || updatingHomeServerList)
                        return;

                    m_comboBoxConnection.Items.Insert(index, tempItem);
                    m_comboBoxConnection.SelectedIndex = selectedIndex;

                    if (_preferredHomeRef == tempItem.Item.opaque_ref)
                        m_comboBoxConnection.SelectedItem = tempItem;
                }
                finally
                {
                    m_comboBoxConnection.EndUpdate();
                    item.ReasonUpdated -= DelayLoadedComboBoxItem_ReasonChanged;
                }
            });
        }

        private void DelayLoadedGridComboBoxItem_ReasonChanged(DelayLoadingOptionComboBoxItem item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

            if (!(item.ParentComboBox is DataGridViewEnableableComboBoxCell cb))
                return;

            Program.Invoke(this, () =>
            {
                try
                {
                    if (cb.DataGridView == null)
                        return;

                    var selectedValue = cb.Value;
                    cb.DataGridView.RefreshEdit();

                    if (item.Enabled && _preferredHomeRef == item.Item.opaque_ref)
                        cb.Value = item;
                    else
                        cb.Value = selectedValue;

                    cb.DataGridView.Refresh();
                    SetButtonNextEnabled(cb.Value is IEnableableComboBoxItem enableableComboBoxItem && enableableComboBoxItem.Enabled);
                }
                finally
                {
                    item.ReasonUpdated -= DelayLoadedGridComboBoxItem_ReasonChanged;
                }
            });
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "metrics" ||
                e.PropertyName == "enabled" || e.PropertyName == "live" || e.PropertyName == "patches")
                Program.Invoke(this, PopulateComboBox);
        }

        private void CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, PopulateComboBox);
        }

        private void xenConnection_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(this, PopulateComboBox);
        }

        private void xenConnection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.Invoke(this, PopulateComboBox);
        }

        #endregion

        #region Control event handlers

        private void m_comboBoxConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingHomeServerList)
                return;

            // when selecting a new destination pool, reset the target host selection
            if (SelectedTargetPool != null && !SelectedTargetPool.Equals(m_comboBoxConnection.SelectedItem))
            {
                SelectedTarget = null;
            }

            //If the item is delay loading and them item is disabled, null the selection made 
            //and clear the table containing server data
            var item = m_comboBoxConnection.SelectedItem as IEnableableXenObjectComboBoxItem;
            if (item != null && !item.Enabled)
            {
                m_comboBoxConnection.SelectedIndex = -1;
                m_dataGridView.Rows.Clear();
                SelectedTargetPool = null;
                return;
            }

            if (m_comboBoxConnection.SelectedItem is AddHostRunningComboBoxItem exeItem && !updatingDestinationCombobox)
                exeItem.RunCommand(this);

            else if (!updatingDestinationCombobox)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    SelectedTargetPool = item?.Item;
                    PopulateDataGridView();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }

            IsDirty = true;
        }

        /// <summary>
        /// Create a set of filters for the homeserver combo box selection
        /// </summary>
        /// <param name="xenObject">XenObject behind the selected item from the host combobox</param>
        /// <param name="vmOpaqueRefs">OpaqueRefs of VMs which need to apply those filters</param>
        /// <returns></returns>
        protected virtual List<ReasoningFilter> CreateTargetServerFilterList(IXenObject xenObject, List<string> vmOpaqueRefs)
        {
            return new List<ReasoningFilter>();
        }

        private void m_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != m_colTarget.Index || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
                return;

            m_dataGridView.BeginEdit(false);

            if (m_dataGridView.EditingControl is ComboBox editingControl)
                editingControl.DroppedDown = true;
        }

        private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            IsDirty = true;
            if (!m_buttonNextEnabled)
                SetButtonNextEnabled(true);
        }

        private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var cell = m_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.Value is IEnableableXenObjectComboBoxItem value)
            {
                SelectedTarget = value.Item;
            }
        }

        #endregion

        private void UnregisterHandlers()
        {
            ConnectionsManager.XenConnections.CollectionChanged -= CollectionChanged;
            foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                var pool = Helpers.GetPool(xenConnection);
                if (pool == null)
                {
                    var host = Helpers.GetCoordinator(xenConnection);
                    if (host != null)
                        host.PropertyChanged -= PropertyChanged;
                }
                else
                {
                    pool.PropertyChanged -= PropertyChanged;
                }

                xenConnection.ConnectionStateChanged -= xenConnection_ConnectionStateChanged;
                xenConnection.CachePopulated -= xenConnection_CachePopulated;
                xenConnection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
            }
        }

        private void CancelFilters()
        {
            foreach (var item in m_comboBoxConnection.Items)
            {
                if (item is DelayLoadingOptionComboBoxItem comboBoxItem)
                    comboBoxItem.CancelFilters();
            }
        }

    }
}
