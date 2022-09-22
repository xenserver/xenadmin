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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    public class SrPicker : CustomTreeView
    {
        // Migrate is the live VDI move operation
        public enum SRPickerType { VM, InstallFromTemplate, Move, Copy, Migrate, LunPerVDI }

        #region Private fields

        private SRPickerType _usage = SRPickerType.VM;
        private VDI[] _existingVDIs;
        private IXenConnection _connection;
        private Host _affinity;
        private SR defaultSR;
        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke;
        private volatile int _scanCount;

        #endregion

        public SrPicker()
        {
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
        }

        #region Properties

        public override bool ShowCheckboxes => false;

        public override bool ShowDescription => true;

        public override bool ShowImages => true;

        public override int NodeIndent => 3;

        public SR SR => SelectedItem is SrPickerItem srpITem && srpITem.Enabled ? srpITem.TheSR : null;

        public bool ValidSelectionExists
        {
            get
            {
                foreach (SrPickerItem item in Items)
                {
                    if (item != null && item.Enabled)
                        return true;
                }

                return false;
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_scanCount > 0)
            {
                var size = e.Graphics.MeasureString(Messages.SR_REFRESH_ACTION_TITLE_MANY, Font);
                e.Graphics.DrawString(Messages.SR_REFRESH_ACTION_TITLE_MANY, Font, SystemBrushes.WindowText, (Width - size.Width) / 2, (Height - size.Height) / 2);
                return;
            }

            base.OnPaint(e);
        }

        public void PopulateAsync(SRPickerType usage, IXenConnection connection, Host affinity,
            SR preselectedSR, VDI[] existingVdis)
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
                pool.PropertyChanged -= Server_PropertyChanged;
                pool.PropertyChanged += Server_PropertyChanged;
            }

            _connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

            var items = GetSrPickerItems();
            _scanCount = items.Count;
            Invalidate();//force redrawing so as to show the scanning message

            foreach (var item in items)
            {
                var action = new SrRefreshAction(item.TheSR, true);
                action.Completed += obj =>
                {
                    _scanCount--;

                    if (_scanCount > 0)
                        return;

                    Program.Invoke(this, () => Rebuild(items, preselectedSR));
                };
                action.RunAsync();
            }
        }

        private List<SrPickerItem> GetSrPickerItems()
        {
            var items = new List<SrPickerItem>();

            foreach (SR sr in _connection.Cache.SRs)
            {
                var item = SrPickerItem.Create(sr, _usage, _affinity, _existingVDIs);
                if (item.Show)
                    items.Add(item);

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

            return items;
        }

        private void Rebuild(List<SrPickerItem> items = null, SR preselectedSr = null)
        {
            Program.AssertOnEventThread();
            Invalidate();

            SR selectedSr = preselectedSr ?? SR;
            var theItems = items ?? GetSrPickerItems();

            try
            {
                BeginUpdate();
                ClearAllNodes();

                foreach (var item in theItems)
                    AddNode(item);
            }
            finally
            {
                EndUpdate();
            }

            _ = SelectSR(selectedSr) || SelectDefaultSR() || SelectAnySR();
        }

        public void UpdateDisks(params VDI[] vdi)
        {
            Program.AssertOnEventThread();
            try
            {
                foreach (SrPickerItem node in Items)
                    node.UpdateDisks(vdi);
            }
            finally
            {
                Refresh();
            }
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_scanCount > 0)
                return;

            if (e.PropertyName == "name_label" || e.PropertyName == "PBDs" || e.PropertyName == "physical_utilisation" || e.PropertyName == "currently_attached" || e.PropertyName == "default_SR")
                Program.Invoke(this, () => Rebuild());
        }

        private void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (_scanCount > 0)
                return;

            Program.Invoke(this, () => Rebuild());
        }

        private void UnregisterHandlers()
        {
            if (_connection == null)
                return;

            var pool = Helpers.GetPoolOfOne(_connection);
            if (pool != null)
                pool.PropertyChanged -= Server_PropertyChanged;

            foreach (var sr in _connection.Cache.SRs)
            {
                foreach (var pbd in sr.Connection.ResolveAll(sr.PBDs))
                {
                    if (pbd != null)
                        pbd.PropertyChanged -= Server_PropertyChanged;
                }
                sr.PropertyChanged -= Server_PropertyChanged;
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
                UnregisterHandlers();

            base.Dispose(disposing);
        }
    }
}
