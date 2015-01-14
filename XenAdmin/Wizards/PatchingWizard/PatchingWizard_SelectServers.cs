/* Copyright (c) Citrix Systems Inc. 
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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectServers : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int PLUS_MINUS_COL = 0;
        private const int POOL_CHECKBOX_COL = 1;
        private const int POOL_ICON_HOST_CHECKBOX_COL = 2;

        private const int UNCHECKED = 0;
        private const int CHECKED = 1;
        private const int INDETERMINATE = 2;

        public PatchingWizard_SelectServers()
        {
            InitializeComponent();
            dataGridViewHosts.CheckBoxClicked += dataGridViewHosts_CheckBoxClicked;
        }

        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTSERVERPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTSERVERPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "SelectServers"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            try
            {
                // catch selected servers, in order to restore selection after the dataGrid is reloaded
                List<Host> selectedServers = SelectedServers;

                dataGridViewHosts.Rows.Clear();
                List<IXenConnection> xenConnections = ConnectionsManager.XenConnectionsCopy;
                xenConnections.Sort();
                foreach (IXenConnection xenConnection in xenConnections)
                {
                    Pool pool = Helpers.GetPool(xenConnection);
                    bool hasPool = true;
                    if (pool != null)
                    {
                        int index = dataGridViewHosts.Rows.Add(new PatchingHostsDataGridViewRow(pool));
                        Host master = pool.Connection.Resolve(pool.master);
                        EnabledRow(master, SelectedUpdateType, index);
                    }
                    else
                    {
                        hasPool = false;
                    }
                    Host[] hosts = xenConnection.Cache.Hosts;
                    Array.Sort(hosts);
                    foreach (Host host in hosts)
                    {
                        int index = dataGridViewHosts.Rows.Add(new PatchingHostsDataGridViewRow(host, hasPool));
                        EnabledRow(host, SelectedUpdateType, index);
                    }
                }

                // restore server selection
                SelectServers(selectedServers);
            }
            catch (Exception e)
            {
                log.Error(e, e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        private void EnabledRow(Host host, UpdateType type, int index)
        {
            var row = (PatchingHostsDataGridViewRow)dataGridViewHosts.Rows[index];

            if(!host.CanApplyHotfixes)
            {
                row.Enabled = false;
                row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_HOST_UNLICENSED;
                return;
            }
                
            switch (type)
            {
                case UpdateType.NewOem:
                    if (!host.isOEM)
                        row.Enabled = false;
                    break;
                case UpdateType.NewRetail:
                    if (host.isOEM)
                        row.Enabled = false;
                    break;
                case UpdateType.Existing:
                    if (host.isOEM)
                        row.Enabled = false;
                    if (Patch.IsAppliedTo(host,ConnectionsManager.XenConnectionsCopy))
                    {
                        row.Enabled = false;
                        row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_PATCH_ALREADY_APPLIED;
                    }
                    break;
            }
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            try
            {
                if (direction == PageLoadedDirection.Forward)
                {
                    if (!AllSelectedHostsConnected())
                    {
                        foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                            row.UpdateIcon();
                        dataGridViewHosts.Invalidate();
                        cancel = true;
                        return;
                    }

                    //Upload the patches to the masters if it is necessary
                    List<Host> masters = SelectedMasters;

                    //Do RBAC check
                    foreach (Host master in masters)
                    {
                        if(Helpers.MidnightRideOrGreater(master.Connection)&&!(Role.CanPerform(new RbacMethodList("pool_patch.apply"),master.Connection)))
                        {
                            string nameLabel = master.Name;
                            Pool pool = Helpers.GetPoolOfOne(master.Connection);
                            if (pool != null)
                                nameLabel = pool.Name;

                            new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.RBAC_UPDATES_WIZARD, master.Connection.Username, nameLabel), Messages.UPDATES_WIZARD)).ShowDialog(this);
                            
                            cancel = true;
                            base.PageLeave(direction, ref cancel);
                            return;
                        }
                    }
                }
                base.PageLeave(direction, ref cancel);
            }
            catch (Exception e)
            {
                log.Debug(e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        private bool AllSelectedHostsConnected()
        {
            var disconnectedServerNames = new List<string>();

            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if ((int)row.Cells[POOL_CHECKBOX_COL].Value > UNCHECKED && ((row.Tag is Host && !row.HasPool) || row.Tag is Pool))
                {
                    IXenConnection connection = ((IXenObject) row.Tag).Connection;
                    if (connection == null || !connection.IsConnected)
                        disconnectedServerNames.Add(((IXenObject) row.Tag).Name);
                }
            }

            if (disconnectedServerNames.Count > 0)
            {
                new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        string.Format(Messages.UPDATES_WIZARD_DISCONNECTED_SERVER, Helpers.StringifyList(disconnectedServerNames)),
                        Messages.UPDATES_WIZARD)).ShowDialog(this);
                return false;
            }
            return true;
        }

        public override bool EnableNext()
        {
            bool clearAllButtonEnabled = false;
            bool selectAllButtonEnabled = false;

            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if ((row.Tag is Host && !row.HasPool) || row.Tag is Pool)
                {
                    int val = (int)row.Cells[POOL_CHECKBOX_COL].Value;
                    if (val > UNCHECKED)
                    {
                        clearAllButtonEnabled = true;
                        if (val == INDETERMINATE)
                            selectAllButtonEnabled = true;
                    }
                    else
                        selectAllButtonEnabled = true;
                }
            }

            buttonClearAll.Enabled = clearAllButtonEnabled;
            buttonSelectAll.Enabled = selectAllButtonEnabled;
            return clearAllButtonEnabled;
        }

        #region Accessors

        public Pool_patch Patch { private get; set; }

        public List<Host> SelectedMasters
        {
            get
            {
                List<Host> result = new List<Host>();
                foreach (Host selectedServer in SelectedServers)
                {
                    Host master = Helpers.GetMaster(selectedServer.Connection);
                    if (!result.Contains(master))
                        result.Add(master);
                }
                return result;
            }
        }

        public List<Host> SelectedServers
        {
            get
            {
                List<Host> hosts = new List<Host>();
                foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (row.Tag is Host)
                    {
                        if ((row.HasPool && ((int)row.Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) == CHECKED) || (!row.HasPool && ((int)row.Cells[POOL_CHECKBOX_COL].Value) == CHECKED))
                            hosts.Add((Host)row.Tag);
                    }
                }
                return hosts;
            }
        }

        public List<Pool> SelectedPools
        {
            get
            {
                List<Pool> pools = new List<Pool>();
                foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (row.Tag is Pool)
                    {
                        if (((int)row.Cells[POOL_CHECKBOX_COL].Value) != UNCHECKED)
                            pools.Add((Pool)row.Tag);
                    }
                    else if (row.Tag is Host)
                    {
                        if (((int)row.Cells[POOL_CHECKBOX_COL].Value) != UNCHECKED)
                        {
                            Host host = (Host)row.Tag;
                            Pool pool = Helpers.GetPoolOfOne(host.Connection);
                            if (pool != null)
                                pools.Add(pool);
                        }
                    }
                }
                return pools;
            }
        }

        public UpdateType SelectedUpdateType { private get; set; }

        public void SelectServers(List<Host> selectedServers)
        {
            if (selectedServers.Count > 0)
            {
                foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (row.Enabled && row.Tag != null && row.Tag is Host)
                    {
                        var host = (Host) row.Tag;
                        if (selectedServers.Contains(host))
                            dataGridViewHosts.CheckBoxChange(row.Index,
                                                             Helpers.GetPool(host.Connection) != null
                                                                 ? POOL_ICON_HOST_CHECKBOX_COL
                                                                 : POOL_CHECKBOX_COL);
                    }
                }
            }
        }

        public void DisableUnselectedServers()
        {
            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (row.Enabled && row.CheckValue == UNCHECKED)
                {
                    row.Enabled = false;
                }
            }
        }

        #endregion
       
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            CheckAllCheckBoxes(CHECKED);
        }

        private void CheckAllCheckBoxes(int value)
        {
            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (row.Tag is Host)
                {
                    if (row.HasPool && (int)row.Cells[POOL_ICON_HOST_CHECKBOX_COL].Value != value)
                        dataGridViewHosts.CheckBoxChange(row.Index, POOL_ICON_HOST_CHECKBOX_COL);
                    else if ((int)row.Cells[POOL_CHECKBOX_COL].Value != value)
                        dataGridViewHosts.CheckBoxChange(row.Index, POOL_CHECKBOX_COL);
                }
            }
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            CheckAllCheckBoxes(UNCHECKED);
        }

        private void dataGridViewHosts_CheckBoxClicked(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        #region Nested items

        private class PatchingHostsDataGridView : CollapsingPoolHostDataGridView
        {
            protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
            {
                base.OnCellPainting(e);

                if (e.RowIndex >= 0 && Rows[e.RowIndex].Tag is Host)
                {
                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)Rows[e.RowIndex];
                    if (row.HasPool && (e.ColumnIndex == POOL_CHECKBOX_COL || e.ColumnIndex == PLUS_MINUS_COL))
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (!row.HasPool && e.ColumnIndex == PLUS_MINUS_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (row.HasPool && !row.Enabled && e.ColumnIndex == POOL_ICON_HOST_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (!row.HasPool && !row.Enabled && e.ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }

                if (e.RowIndex >= 0 && Rows[e.RowIndex].Tag is Pool)
                {
                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)Rows[e.RowIndex];
                    if (!row.Enabled && e.ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }
            }

            public override void CheckBoxChange(int RowIndex, int ColumnIndex)
            {
                if (RowIndex >= 0 && !((PatchingHostsDataGridViewRow)Rows[RowIndex]).Enabled)
                    return;

                if (RowIndex >= 0 && Rows[RowIndex].Tag is Host)
                {
                    if (ColumnIndex == POOL_ICON_HOST_CHECKBOX_COL && Rows[RowIndex].Cells[ColumnIndex].Value is int) //Checkbox
                    {
                        int hostNewValue = ClickCheckBox(RowIndex, ColumnIndex);

                        PatchingHostsDataGridViewRow poolRow = FindPoolRow(RowIndex);
                        bool allHostSameValue = true;
                        bool atLeastOneHostChecked = false;
                        for (int i = poolRow.Index + 1; i < Rows.Count; i++)
                        {
                            if (Rows[i].Tag is Host &&
                                ((PatchingHostsDataGridViewRow)Rows[i]).HasPool)
                            {
                                if (((int)Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) == CHECKED)
                                {
                                    atLeastOneHostChecked = true;
                                }
                                if (((PatchingHostsDataGridViewRow)Rows[i]).Enabled && ((int)Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) != hostNewValue)
                                {
                                    allHostSameValue = false;
                                }
                            }
                            else
                                break;
                        }
                        if (allHostSameValue)
                        {
                            poolRow.Cells[POOL_CHECKBOX_COL].Value = hostNewValue;
                        }
                        else if (atLeastOneHostChecked)
                        {
                            poolRow.Cells[POOL_CHECKBOX_COL].Value = INDETERMINATE;
                        }
                    }
                    else if (ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        ClickCheckBox(RowIndex, ColumnIndex);
                    }
                }
                else if (RowIndex >= 0 && Rows[RowIndex].Tag is Pool)
                {
                    if (ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        ClickCheckBox(RowIndex, ColumnIndex);
                        for (int i = RowIndex + 1; i < Rows.Count; i++)
                        {
                            if (Rows[i].Tag is Host && ((PatchingHostsDataGridViewRow)Rows[i]).HasPool)
                            {
                                var value = (int)Rows[RowIndex].Cells[ColumnIndex].Value;

                                if (value == UNCHECKED || value == CHECKED)
                                    Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value = value;
                            }
                            else break;
                        }
                    }
                }
                OnCheckBoxClicked();
            }

            private PatchingHostsDataGridViewRow FindPoolRow(int rowIndex)
            {
                for (int i = rowIndex; i >= 0; i--)
                {
                    if (Rows[i].Tag is Pool)
                        return (PatchingHostsDataGridViewRow)Rows[i];
                }
                return null;
            }

            private int ClickCheckBox(int rowIndex, int columnIndex)
            {
                int value = (int)Rows[rowIndex].Cells[columnIndex].Value;
                if (value == CHECKED)
                {
                    Rows[rowIndex].Cells[columnIndex].Value = UNCHECKED;
                    return UNCHECKED;
                }
                else
                {
                    Rows[rowIndex].Cells[columnIndex].Value = CHECKED;
                    return CHECKED;
                }
            }
        }

        private class PatchingHostsDataGridViewRow : CollapsingPoolHostDataGridViewRow
        {
            private class DataGridViewNameCell : DataGridViewExNameCell
            {
                protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
                {
                    Pool pool = value as Pool;

                    if (pool != null)
                        base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                    else
                    {
                        Host host = value as Host;
                        if (host != null)
                        {
                            PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)this.DataGridView.Rows[this.RowIndex];
                            if (row.HasPool)
                            {
                                Image hostIcon = Images.GetImage16For(host);
                                base.Paint(graphics, clipBounds,
                                           new Rectangle(cellBounds.X + 16, cellBounds.Y, cellBounds.Width - 16,
                                                         cellBounds.Height), rowIndex, cellState, value, formattedValue,
                                           errorText, cellStyle, advancedBorderStyle, paintParts);

                                if ((cellState & DataGridViewElementStates.Selected) != 0 && row.Enabled)
                                {
                                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                                        graphics.FillRectangle(
                                            brush, cellBounds.X,
                                            cellBounds.Y, hostIcon.Width, cellBounds.Height);
                                }
                                else
                                {
                                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.BackColor))
                                        graphics.FillRectangle(brush,
                                                               cellBounds.X, cellBounds.Y, hostIcon.Width, cellBounds.Height);
                                }

                                if (row.Enabled)
                                    graphics.DrawImage(hostIcon, cellBounds.X, cellBounds.Y + 3, hostIcon.Width,
                                                       hostIcon.Height);
                                else
                                    graphics.DrawImage(hostIcon,
                                                       new Rectangle(cellBounds.X, cellBounds.Y + 3,
                                                                     hostIcon.Width, hostIcon.Height),
                                                       0, 0, hostIcon.Width, hostIcon.Height, GraphicsUnit.Pixel,
                                                       Drawing.GreyScaleAttributes);
                            }
                            else
                            {
                                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue,
                                           errorText, cellStyle, advancedBorderStyle, paintParts);
                            }
                        }
                    }
                }
            }

            private class DataGridViewIconCell : DataGridViewImageCell
            {
                protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
                {
                    Image icon = value as Image;
                    if (icon == null)
                        return;

                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)DataGridView.Rows[RowIndex];
                    if ((cellState & DataGridViewElementStates.Selected) != 0 && row.Enabled)
                    {
                        using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                            graphics.FillRectangle(
                                brush, cellBounds.X,
                                cellBounds.Y, cellBounds.Width, cellBounds.Height);
                    }
                    else
                    {
                        using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.BackColor))
                            graphics.FillRectangle(brush, cellBounds.X,
                                                   cellBounds.Y, cellBounds.Width, cellBounds.Height);
                    }

                    if (row.Enabled)
                        graphics.DrawImage(icon, cellBounds.X, cellBounds.Y + 3, icon.Width, icon.Height);
                    else
                        graphics.DrawImage(icon, new Rectangle(cellBounds.X, cellBounds.Y + 3, icon.Width, icon.Height),
                                           0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel,
                                           Drawing.GreyScaleAttributes);
                }
            }

            private DataGridViewCell _poolIconHostCheckCell;
            private DataGridViewTextBoxCell _versionCell;

            public PatchingHostsDataGridViewRow(Pool pool)
                : base(pool)
            {
                SetupCells();
            }

            public PatchingHostsDataGridViewRow(Host host, bool hasPool)
                : base(host, hasPool)
            {
                SetupCells();
            }

            public override bool IsCheckable
            {
                get { return !HasPool; }
            }

            public override bool Enabled
            {
                get
                {
                    return base.Enabled;
                }
                set
                {
                    base.Enabled = value;
                    UpdateDetails();
                }
            }

            public int CheckValue
            {
                get {
                    return Tag is Pool || (Tag is Host && !_hasPool)
                               ? (int) Cells[POOL_CHECKBOX_COL].Value
                               : (int) Cells[POOL_ICON_HOST_CHECKBOX_COL].Value;
                }
            }

            private void SetupCells()
            {
                _poolCheckBoxCell = new DataGridViewCheckBoxCell { ThreeState = true };

                _expansionCell = new DataGridViewImageCell();

                if (Tag is Pool || (Tag is Host && !_hasPool))
                    _poolIconHostCheckCell = new DataGridViewIconCell();
                else
                    _poolIconHostCheckCell = new DataGridViewCheckBoxCell();


                _nameCell = new DataGridViewNameCell();
                _versionCell = new DataGridViewTextBoxCell();



                Cells.AddRange(new[] { _expansionCell, _poolCheckBoxCell, _poolIconHostCheckCell, _nameCell, _versionCell });

                this.UpdateDetails();
            }

            private void UpdateDetails()
            {
                if (Tag is Pool)
                {
                    Pool pool = (Pool)Tag;
                    Host master = pool.Connection.Resolve(pool.master);
                    _poolCheckBoxCell.Value = 0;
                    _expansionCell.Value = Resources.tree_minus;
                    _poolIconHostCheckCell.Value = Images.GetImage16For(pool);
                    _nameCell.Value = pool;
                    _versionCell.Value = master.ProductVersionTextShort;
                }
                else if (Tag is Host)
                {
                    Host host = (Host)Tag;
                    _poolCheckBoxCell.Value = 0;
                    _expansionCell.Value = Resources.tree_plus;
                    if (_hasPool)
                        _poolIconHostCheckCell.Value = 0;
                    else
                        _poolIconHostCheckCell.Value = Images.GetImage16For(host);
                    _nameCell.Value = host;
                    _versionCell.Value = host.ProductVersionTextShort;
                }
            }

            internal void UpdateIcon()
            {
                if (_poolIconHostCheckCell is DataGridViewImageCell)
                {
                    _poolIconHostCheckCell.Value = Images.GetImage16For((IXenObject)Tag);
                }
            }
        }

        #endregion

    }
}
