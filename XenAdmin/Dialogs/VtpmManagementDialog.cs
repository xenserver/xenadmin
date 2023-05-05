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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class VtpmManagementDialog : VerticallyTabbedDialog
    {
        private readonly VM _vm;
        private bool _toolTipVisible;
        private XenRef<VTPM> _selectedVtpm;

        public VtpmManagementDialog(VM vm)
        {
            InitializeComponent();
            _vm = vm;
            RegisterEvents();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Text = string.Format(Text, _vm.Name().Ellipsise(50));
            BlurbLabel.Text = string.Format(BlurbLabel.Text, _vm.Name().Ellipsise(80));
            RefreshTabs();
        }

        private void RegisterEvents()
        {
            _vm.PropertyChanged += _vm_PropertyChanged;
        }

        private void UnregisterEvents()
        {
            _vm.PropertyChanged -= _vm_PropertyChanged;
        }

        private VtpmManagementPage CreateVtpmPage(VTPM vtpm)
        {
            var page = new VtpmManagementPage(vtpm)
            {
                Parent = ContentPanel,
                TabStop = true,
                Dock = DockStyle.Fill
            };
            page.RemoveButtonClicked += Page_RemoveButtonClicked;
            return page;
        }

        private void RefreshTabs()
        {
            if (verticalTabs.SelectedItem is VtpmManagementPage p)
                _selectedVtpm = new XenRef<VTPM>(p.Vtpm.opaque_ref);
            else
                _selectedVtpm = null;

            var tabsToRemove = (from object tab in verticalTabs.Items
                let page = tab as VtpmManagementPage
                where page != null && _vm.VTPMs.All(t => t.opaque_ref != page.Vtpm.opaque_ref)
                select page).ToList();

            foreach (var tab in tabsToRemove)
            {
                verticalTabs.Items.Remove(tab);
                ContentPanel.Controls.Remove(tab);
            }

            var newVtpmPages = new List<object>();

            foreach (var vtpmRef in _vm.VTPMs)
            {
                var page = FindTabFromVtpm(vtpmRef);
                if (page == null)
                {
                    VTPM vtpm = _vm.Connection.Resolve(vtpmRef);
                    if (vtpm != null)
                    {
                        page = CreateVtpmPage(vtpm);
                        newVtpmPages.Add(page);
                    }
                }
                
                page?.Repopulate();
            }

            verticalTabs.Items.AddRange(newVtpmPages.ToArray());
            ResizeVerticalTabs();

            VtpmManagementPage selectedPage = null;
            if (_selectedVtpm != null)
                selectedPage = FindTabFromVtpm(_selectedVtpm);
            else if (verticalTabs.Items.Count > 0)
                selectedPage = verticalTabs.Items[0] as VtpmManagementPage;

            verticalTabs.SelectedItem = selectedPage;

            blueBorder.Visible = verticalTabs.SelectedItem != null;
            labelNoVtpms.Visible = verticalTabs.SelectedItem == null;

            addButton.Enabled = _vm.CanAddVtpm(out var cannotReason);
            if (string.IsNullOrEmpty(cannotReason))
                toolTipContainerAdd.RemoveAll();
            else
                toolTipContainerAdd.SetToolTip(cannotReason);
        }

        private VtpmManagementPage FindTabFromVtpm(XenRef<VTPM> vtpmRef)
        {
            foreach (var tab in verticalTabs.Items)
            {
                if (tab is VtpmManagementPage page && page.Vtpm.opaque_ref == vtpmRef.opaque_ref)
                    return page;
            }

            return null;
        }

        private void AddVtpm()
        {
            var action = new NewVtpmAction(_vm.Connection, _vm);
            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog(this);
        }

        private void RemoveVtpm(VTPM vtpm)
        {
            HideTooltip();

            using (var dlg = new WarningDialog(Messages.VTPM_REMOVE_WARNING, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
            {
                if (dlg.ShowDialog() != DialogResult.Yes)
                    return;
            }

            var action = new RemoveVtpmAction(vtpm.Connection, vtpm, _vm);
            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog(this);
        }

        private bool CanRemoveVtpm(VTPM vtpm, out string cannotReason)
        {
            cannotReason = null;

            if (Helpers.XapiEqualOrGreater_23_11_0(vtpm.Connection))
            {
                if (vtpm.allowed_operations.Contains(vtpm_operations.destroy))
                    return true;

                cannotReason = Messages.VTPM_OPERATION_DISALLOWED_REMOVE;
                return false;
            }

            return _vm.CanRemoveVtpm(out cannotReason);
        }

        private void ResizeVerticalTabs()
        {
            int maxHeight = splitContainer.Panel1.Height - toolTipContainerAdd.Height;
            verticalTabs.Height = Math.Min(maxHeight, verticalTabs.Items.Count * verticalTabs.ItemHeight);
            toolTipContainerAdd.Top = verticalTabs.Top + verticalTabs.Height;
        }

        private void ShowTooltip(VtpmManagementPage page, Point location)
        {
            if (!_toolTipVisible)
            {
                Cursor = Cursors.Hand;
                var msg = CanRemoveVtpm(page.Vtpm, out var cannotReason) ? Messages.VTPM_REMOVE : cannotReason;

                if (!string.IsNullOrEmpty(cannotReason))
                {
                    toolTip.Show(msg, verticalTabs, location.X, location.Y + 20);
                    _toolTipVisible = true;
                }
            }
        }

        private void HideTooltip()
        {
            toolTip.Hide(verticalTabs);
            _toolTipVisible = false;
            Cursor = Cursors.Default;
        }

        #region Event handlers

        private void _vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is VM))
                return;

            if (e.PropertyName != "power_state" && e.PropertyName != "VTPMs")
                return;

            Program.Invoke(this, RefreshTabs);
        }

        private void splitContainer_Panel1_Resize(object sender, EventArgs e)
        {
            ResizeVerticalTabs();
        }

        private void verticalTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            int pageIndex = e.Index;
            if (pageIndex < 0 || verticalTabs.Items.Count <= pageIndex)
                return;

            if (!(verticalTabs.Items[pageIndex] is VtpmManagementPage page))
                return;

            var deleteIcon = CanRemoveVtpm(page.Vtpm, out _) ? Images.StaticImages._000_Abort_h32bit_16 : Images.StaticImages._000_Abort_gray_h32bit_16;

            page.DeleteIconBounds = new Rectangle(e.Bounds.Right - deleteIcon.Width - (32 - deleteIcon.Width) / 2,
                e.Bounds.Y + (32 - deleteIcon.Height) / 2, deleteIcon.Width, deleteIcon.Height);

            e.Graphics.DrawImage(deleteIcon, page.DeleteIconBounds);
        }

        private void verticalTabs_MouseMove(object sender, MouseEventArgs e)
        {
            int pageIndex = verticalTabs.IndexFromPoint(e.Location);
            if (pageIndex < 0 || verticalTabs.Items.Count <= pageIndex)
                return;

            if (!(verticalTabs.Items[pageIndex] is VtpmManagementPage page))
                return;

            if (page.DeleteIconBounds.Contains(e.Location))
                ShowTooltip(page, e.Location);
            else
                HideTooltip();
        }

        private void verticalTabs_MouseClick(object sender, MouseEventArgs e)
        {
            int pageIndex = verticalTabs.IndexFromPoint(e.Location);
            if (pageIndex < 0 || verticalTabs.Items.Count <= pageIndex)
                return;

            if (!(verticalTabs.Items[pageIndex] is VtpmManagementPage page))
                return;

            if (page.DeleteIconBounds.Contains(e.Location) && CanRemoveVtpm(page.Vtpm, out _))
                RemoveVtpm(page.Vtpm);
        }

        private void Page_RemoveButtonClicked(VtpmManagementPage page)
        {
            RemoveVtpm(page.Vtpm);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddVtpm();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}