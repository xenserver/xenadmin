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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Controls
{
    public class SrPicker : CustomTreeView
    {
        // Migrate is the live VDI move operation
        public enum SRPickerType { VM, InstallFromTemplate, Move, Copy, Migrate, LunPerVDI }

        #region Private fields

        private const int MAX_SCANS_PER_CONNECTION = 3;
        private readonly ChangeableList<SrRefreshAction> _refreshQueue = new ChangeableList<SrRefreshAction>();
        private SRPickerType _usage = SRPickerType.VM;
        private VDI[] _existingVDIs;
        private IXenConnection _connection;
        private Host _affinity;
        private SR defaultSR;
        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke;
        private readonly object _lock = new object();

        #endregion

        public SrPicker()
        {
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
            _refreshQueue.CollectionChanged += _refreshQueue_CollectionChanged;
        }

        #region Properties

        public override bool ShowCheckboxes => false;

        public override bool ShowDescription => true;

        public override bool ShowImages => true;

        public override int NodeIndent => 3;

        public SR SR => SelectedItem is SrPickerItem srpITem && srpITem.Enabled ? srpITem.TheSR : null;

        public bool ValidSelectionExists(out string invalidReason)
        {
            invalidReason = string.Empty;
            bool allScanning = true;

            foreach (SrPickerItem item in Items)
            {
                if (item != null)
                {
                    if (item.Enabled)
                        return true;

                    if (!item.Scanning)
                        allScanning = false;
                }
            }

            if (!allScanning)
                invalidReason = Messages.NO_VALID_DISK_LOCATION;

            return false;
        }

        #endregion

        public void PopulateAsync(SRPickerType usage, IXenConnection connection, Host affinity,
            SR preselectedSr, VDI[] existingVdis)
        {
            _usage = usage;
            _connection = connection;
            _affinity = affinity;
            _existingVDIs = existingVdis;

            if (_connection == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(_connection);
            if (pool != null)
            {
                defaultSR = _connection.Resolve(pool.default_SR);
                pool.PropertyChanged -= pool_PropertyChanged;
                pool.PropertyChanged += pool_PropertyChanged;
            }

            _connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

            foreach (var sr in _connection.Cache.SRs)
                AddNewSr(sr);

            _ = SelectSR(preselectedSr) || SelectDefaultSR() || SelectAnySR();
        }

        public void UpdateDisks(params VDI[] vdi)
        {
            Program.AssertOnEventThread();
            foreach (SrPickerItem node in Items)
                node.UpdateDisks(vdi);
        }

        private void AddNewSr(SR sr)
        {
            var item = SrPickerItem.Create(sr, _usage, _affinity, _existingVDIs);
            if (!item.Show)
                return;

            foreach (PBD pbd in sr.Connection.ResolveAll(sr.PBDs))
            {
                if (pbd != null)
                {
                    pbd.PropertyChanged -= pbd_PropertyChanged;
                    pbd.PropertyChanged += pbd_PropertyChanged;
                }
            }

            sr.PropertyChanged -= sr_PropertyChanged;
            sr.PropertyChanged += sr_PropertyChanged;

            item.ItemUpdated += Item_ItemUpdated;
            item.Scanning = true;
            AddNode(item);

            var srRefreshAction = new SrRefreshAction(item.TheSR, true);
            srRefreshAction.Completed += SrRefreshAction_Completed;

            lock (_lock)
                _refreshQueue.Add(srRefreshAction);
        }

        private void _refreshQueue_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            lock (_lock)
            {
                var srRefreshAction = _refreshQueue.FirstOrDefault(a => !a.StartedRunning && !a.IsCompleted);

                if (srRefreshAction != null && _refreshQueue.Count(a => a.StartedRunning && !a.IsCompleted) < MAX_SCANS_PER_CONNECTION)
                    srRefreshAction.RunAsync();
            }
        }

        private void Item_ItemUpdated(SrPickerItem item)
        {
            Invalidate();
        }

        private void SrRefreshAction_Completed(ActionBase obj)
        {
            if (!(obj is SrRefreshAction action))
                return;

            lock (_lock)
                _refreshQueue.Remove(action);

            Program.Invoke(this, () =>
            {
                foreach (var item in Items)
                {
                    if (item is SrPickerItem it && it.TheSR.opaque_ref == action.SR.opaque_ref)
                    {
                        it.Scanning = false;
                        break;
                    }
                }
            });
        }

        private void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Pool && e.PropertyName == "default_SR")
            {
                Program.Invoke(this, () =>
                {
                    foreach (var item in Items)
                    {
                        if (item is SrPickerItem it && !it.Scanning)
                            it.Update();
                    }
                });
            }
        }

        private void pbd_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is PBD pbd && e.PropertyName == "currently_attached")
            {
                Program.Invoke(this, () =>
                {
                    foreach (var item in Items)
                    {
                        if (item is SrPickerItem it && !it.Scanning && it.TheSR.opaque_ref == pbd.SR.opaque_ref)
                        {
                            it.Update();
                            break;
                        }
                    }
                });
            }
        }

        private void sr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is SR sr &&
                (e.PropertyName == "name_label" || e.PropertyName == "PBDs" ||
                 e.PropertyName == "physical_utilisation" || e.PropertyName == "virtual_allocation"))
            {
                Program.Invoke(this, () =>
                {
                    foreach (var item in Items)
                    {
                        if (item is SrPickerItem it && !it.Scanning && it.TheSR.opaque_ref == sr.opaque_ref)
                        {
                            it.Update();
                            break;
                        }
                    }
                });
            }
        }

        private void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add && e.Element is SR addedSr)
            {
                Program.Invoke(this, () =>
                {
                    foreach (var item in Items)
                    {
                        if (item is SrPickerItem it && it.TheSR.opaque_ref == addedSr.opaque_ref)
                            return;
                    }

                    AddNewSr(addedSr);
                });
                return;
            }
            
            if (e.Action == CollectionChangeAction.Remove)
            {
                var removedSrs = new List<string>();

                if (e.Element is SR storage)
                    removedSrs.Add(storage.opaque_ref);
                else if (e.Element is List<SR> range)
                    removedSrs = range.Select(sr => sr.opaque_ref).ToList();
                else
                    return;

                Program.Invoke(this, () =>
                {
                    var itemsToRemove = new List<SrPickerItem>();

                    foreach (var item in Items)
                    {
                        if (item is SrPickerItem it && removedSrs.Contains(it.TheSR.opaque_ref))
                        {
                            foreach (var pbdRef in it.TheSR.PBDs)
                            {
                                var pbd = it.TheSR.Connection.Resolve(pbdRef);
                                if (pbd != null)
                                    pbd.PropertyChanged -= pbd_PropertyChanged;
                            }
                            it.TheSR.PropertyChanged -= sr_PropertyChanged;

                            it.ItemUpdated -= Item_ItemUpdated;
                            itemsToRemove.Add(it);
                            break;
                        }
                    }

                    foreach (var item in itemsToRemove)
                        RemoveNode(item);
                });
            }
        }

        private void UnregisterHandlers()
        {
            if (_connection == null)
                return;

            var pool = Helpers.GetPoolOfOne(_connection);
            if (pool != null)
                pool.PropertyChanged -= pool_PropertyChanged;

            foreach (var sr in _connection.Cache.SRs)
            {
                foreach (var pbd in sr.Connection.ResolveAll(sr.PBDs))
                {
                    if (pbd != null)
                        pbd.PropertyChanged -= pbd_PropertyChanged;
                }
                sr.PropertyChanged -= sr_PropertyChanged;
            }

            _connection.Cache.DeregisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);
        }

        private bool SelectDefaultSR()
        {
            if (defaultSR == null)
                return false;

            foreach (SrPickerItem item in Items)
            {
                if (item.TheSR != null && item.TheSR.opaque_ref == defaultSR.opaque_ref && item.Enabled)
                {
                    SelectedItem = item;
                    return true;
                }
            }

            return false;
        }

        private bool SelectSR(SR sr)
        {
            if (sr == null)
                return false;

            foreach (SrPickerItem item in Items)
            {
                if (item.TheSR != null && item.TheSR.opaque_ref == sr.opaque_ref && item.Enabled)
                {
                    SelectedItem = item;
                    return true;
                }
            }

            return false;
        }

        private bool SelectAnySR()
        {
            foreach (SrPickerItem item in Items)
            {
                if (item != null && item.Enabled)
                {
                    SelectedItem = item;
                    return true;
                }
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    foreach (var action in _refreshQueue)
                    {
                        action.Completed -= SrRefreshAction_Completed;
                        if (!action.IsCompleted)
                            action.Cancel();
                    }
                }
                UnregisterHandlers();
            }

            base.Dispose(disposing);
        }
    }
}
