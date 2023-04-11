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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class VtpmManagementPage : UserControl, VerticalTabs.IVerticalTab
    {
        public event Action<VtpmManagementPage> RemoveButtonClicked;

        public VtpmManagementPage(VTPM vtpm)
        {
            InitializeComponent();
            buttonReset.Visible = false;
            Vtpm = vtpm;
            RegisterEvents();
        }

        public override string Text => Messages.VTPM;
        public VTPM Vtpm { get; }
        public string SubText { get; }
        public Image Image => Images.StaticImages.tpm;
        public Rectangle DeleteIconBounds { get; set; }

        private void RegisterEvents()
        {
            Vtpm.PropertyChanged += Vtpm_PropertyChanged;
        }

        private void UnregisterEvents()
        {
            Vtpm.PropertyChanged -= Vtpm_PropertyChanged;
        }

        public void Repopulate()
        {
            UpdateProperties();
            UpdateButtons();
        }

        private void UpdateProperties()
        {
            labelProtectedValue.Text = Vtpm.is_protected.ToYesNoStringI18n();
            labelUniqueValue.Text = Vtpm.is_unique.ToYesNoStringI18n();
        }

        private void UpdateButtons()
        {
            buttonRemove.Enabled = CanRemoveVtpm(out string cannotReason);

            if (buttonRemove.Enabled)
                toolTipContainerRemove.RemoveAll();
            else if (!string.IsNullOrEmpty(cannotReason))
                toolTipContainerRemove.SetToolTip(cannotReason);
        }

        private bool CanRemoveVtpm(out string cannotReason)
        {
            cannotReason = null;

            if (Helpers.XapiEqualOrGreater_23_10_0(Vtpm.Connection))
            {
                if (Vtpm.allowed_operations.Contains(vtpm_operations.destroy))
                    return true;

                cannotReason = Messages.VTPM_OPERATION_DISALLOWED_REMOVE;
                return false;
            }

            var vm = Vtpm.Connection.Resolve(Vtpm.VM);
            return vm != null && vm.CanRemoveVtpm(out cannotReason);
        }

        private void Vtpm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is VTPM vtpm))
                return;

            if (e.PropertyName == "is_protected" || e.PropertyName == "is_unique")
                Program.Invoke(this, UpdateProperties);

            if (e.PropertyName == "allowed_operations" && Helpers.XapiEqualOrGreater_23_10_0(vtpm.Connection))
                Program.Invoke(this, UpdateButtons);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveButtonClicked?.Invoke(this);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            //TODO:
        }
    }
}
