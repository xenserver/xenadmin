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
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Commands;
using XenAdmin.Dialogs;


namespace XenAdmin.TabPages
{
    public partial class PhysicalStoragePage : BaseTabPage
    {
        private IXenConnection connection;
        private Host host;

        private bool NeedBuildList;
        private readonly ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

        private SelectionManager selectionManager;
        private bool showTrimButton;

        public PhysicalStoragePage()
        {
            InitializeComponent();

            listViewSrs.SmallImageList = Images.ImageList16;
            listViewSrs.ListViewItemSorter = lvwColumnSorter;
            listViewSrs.SelectedIndexChanged += new EventHandler(listViewSrs_SelectedIndexChanged);
            base.Text = Messages.STORAGE_TAB_TITLE;
            PBD_CollectionChangedWithInvoke=Program.ProgramInvokeHandler(PBD_CollectionChanged);

        }
       
        /// <summary>
        /// Make sure you set this before you set the connection, 
        /// as the connection is the one which rebuilds the list
        /// </summary>
        public Host Host
        {
            set
            {
                if (host != null)
                {
                    host.PropertyChanged -= host_PropertyChanged;
                }

                host = value;

                if (host != null)
                {
                    host.PropertyChanged += host_PropertyChanged;
                }
            }
        }

        private readonly CollectionChangeEventHandler PBD_CollectionChangedWithInvoke;
        public IXenConnection Connection
        {
            set
            {
                UnregisterHandlers();

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
                    connection.XenObjectsUpdated += XenObjectUpdated;

                    Pool pool = Helpers.GetPoolOfOne(connection);
                    if (pool != null)
                        pool.PropertyChanged += pool_PropertyChanged;
                }

                RefreshTrimButton();
                BuildList();
            }
        }

        internal void SetSelectionBroadcaster(SelectionBroadcaster selectionBroadcaster, IMainWindow mainWindow)
        {
            selectionBroadcaster.BindTo(newSRButton, mainWindow);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (listViewSrs.SelectedItems.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            contextMenuStrip.Items.Clear();

            SR sr = (SR)listViewSrs.SelectedItems[0].Tag;
            contextMenuStrip.Items.AddRange(Program.MainWindow.ContextMenuBuilder.Build(sr));
        }

        private void XenObjectUpdated(object sender, EventArgs e)
        {
            if (NeedBuildList)
            {
                BuildList();
            }
        }

        private void PBD_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            NeedBuildList = true;
            BuildList();
        }

        private void pool_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "default_SR")
                NeedBuildList = true;
        }

        private void host_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PBDs")
                NeedBuildList = true;
        }

        private void sr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PBDs")
            {
                NeedBuildList = true;
            }
            else
            {
                Program.Invoke(this, () => RefreshRowForSr((SR)sender));
            }
        }

        private void UnregisterHandlers()
        {
            if (connection == null) 
                return;

            connection.Cache.DeregisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
            connection.XenObjectsUpdated -= XenObjectUpdated;

            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
            }

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
                pool.PropertyChanged -= pool_PropertyChanged;
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        /// <summary>
        /// Finds the ListView row for the given SR, and calls RefreshRow on it.
        /// </summary>
        private void RefreshRowForSr(SR sr)
        {
            foreach (ListViewItem row in listViewSrs.Items)
            {
                if ((SR)row.Tag == sr)
                {
                    RefreshRow(row);
                    return;
                }
            }
        }

        /// <summary>
        /// Fills in the subitems for the given row based on the SR it is Tag'd with.
        /// </summary>
        /// <param name="row"></param>
        private void RefreshRow(ListViewItem row)
        {
            SR sr = (SR)row.Tag;

            // Work out percent usage
            int percent = 0;
            double ratio = 0;
            if (sr.physical_size > 0)
            {
                ratio = sr.physical_utilisation / (double)sr.physical_size;
                percent = (int)(100.0 * ratio);
            }
            string percentString = string.Format(Messages.DISK_PERCENT_USED, percent.ToString(), Util.DiskSizeString(sr.physical_utilisation));

            row.SubItems.Clear();
            row.SubItems.AddRange(new string[] {
                sr.Name,
                sr.Description,
                sr.FriendlyTypeName,
                sr.shared ? Messages.YES : Messages.NO,
                percentString,
                Util.DiskSizeString(sr.physical_size),
                Util.DiskSizeString(sr.virtual_allocation)
            });
            // SubItems.Clear() always leaves 1 element in the collection,
            // so when we do the AddRange we're left with an unwanted element at the start
            // of the collection. Remove it below.
            row.SubItems.RemoveAt(0);

            // Tag for the benefit of our Comparison<ListViewItem.ListViewSubItem> below.
            row.SubItems[4].Tag = ratio;
            row.SubItems[5].Tag = sr.physical_size;
            row.SubItems[6].Tag = sr.virtual_allocation;

            row.ImageIndex = (int)sr.GetIcon;
        }

        private void BuildList()
        {
            if (!this.Visible)
                return;
            int selectedIndex = listViewSrs.SelectedIndices.Count == 1 ? listViewSrs.SelectedIndices[0] : -1;

            listViewSrs.BeginUpdate();

            try
            {
                listViewSrs.Items.Clear();

                if (connection == null)
                    return;

                List<PBD> pbds = host != null ? new List<PBD>(connection.ResolveAll(host.PBDs))
                    : new List<PBD>(connection.Cache.PBDs);

                List<String> srs = new List<String>();

                foreach (PBD pbd in pbds)
                {
                    SR sr = pbd.Connection.Resolve(pbd.SR);

                    if (sr == null || sr.IsToolsSR || !sr.Show(Properties.Settings.Default.ShowHiddenVMs))
                        continue;

                    // From MSDN:
                    // Returns the zero-based index of item in the sorted List<T>, if item is found;
                    // otherwise, a negative number that is the bitwise complement of the index of
                    // the next element that is larger than item or, if there is no larger element,
                    // the bitwise complement of Count. 
                    int index = srs.BinarySearch(sr.opaque_ref);
                    
                    // Don't allow duplicates
                    if (index >= 0)
                        continue;

                    sr.PropertyChanged -= sr_PropertyChanged;
                    sr.PropertyChanged += sr_PropertyChanged;

                    index = ~index;
                    srs.Insert(index, sr.opaque_ref);

                    ListViewItem item = new ListViewItem();
                    item.Tag = sr;
                    RefreshRow(item);
                    listViewSrs.Items.Add(item);
                }

                if (selectedIndex >= 0 && selectedIndex < listViewSrs.Items.Count)
                {
                    // Select previously selected item
                    listViewSrs.SelectedIndices.Clear();
                    listViewSrs.SelectedIndices.Add(selectedIndex);
                }
                else
                {
                    // Select first item
                    if (listViewSrs.Items.Count > 0)
                        listViewSrs.SelectedIndices.Add(0);
                }
            }
            finally
            {
                listViewSrs.EndUpdate();
            }
            RefreshButtons();
            NeedBuildList = false;
        }

        private void newSRButton_Click(object sender, EventArgs e)
        {
            new NewSRCommand(Program.MainWindow, connection).Execute();
        }

        /// <summary>
        /// Taken from http://support.microsoft.com/kb/319401.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewSrs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            if (4 <= e.Column && e.Column <= 6)
            {
                // Use a custom comparer that sorts by the subitem's tag
                lvwColumnSorter.Comparer = (Comparison<ListViewItem.ListViewSubItem>)delegate(ListViewItem.ListViewSubItem a, ListViewItem.ListViewSubItem b)
                {
                    return ((IComparable)a.Tag).CompareTo((IComparable)b.Tag);
                };
            }
            else
            {
                // Use the default comparer (Helpers.NatualCompare)
                lvwColumnSorter.Comparer = null;
            }

            // Perform the sort with these new sort options.
            listViewSrs.Sort();
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            if (listViewSrs.SelectedItems.Count != 1)
                return;

            SR sr = (SR)listViewSrs.SelectedItems[0].Tag;

            new PropertiesDialog(sr).ShowDialog(this);
        }

        void listViewSrs_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
            if (showTrimButton)
            {
                List<SelectedItem> selectedSRs = (from ListViewItem item in listViewSrs.SelectedItems select new SelectedItem((SR)item.Tag)).ToList();
                selectionManager.SetSelection(selectedSRs);
            }
        }

        private void RefreshButtons()
        {
            buttonProperties.Enabled = listViewSrs.SelectedItems.Count == 1;
        }

        private void RefreshTrimButton()
        {
            showTrimButton = connection != null && Helpers.CreedenceOrGreater(connection);
            if (showTrimButton)
            {
                trimButtonContainer.Visible = true;
                if (selectionManager == null)
                    selectionManager = new SelectionManager();
                selectionManager.BindTo(trimButton, Program.MainWindow);
            }
            else
            {
                trimButtonContainer.Visible = false;
                trimButton.SelectionBroadcaster = null;
            }
        }
    }
}
