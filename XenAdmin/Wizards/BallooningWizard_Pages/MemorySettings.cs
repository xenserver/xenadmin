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
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.BallooningWizard_Pages
{
    public partial class MemorySettings : XenTabPage
    {
        public event Action InstallTools;

        private VMMemoryControlsEdit memoryControls;
        private List<VM> _vms = new List<VM>();
        private bool _changed;

        public MemorySettings()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text => Messages.BALLOONING_PAGE_MEMORY_TEXT;

        public override string PageTitle => Messages.BALLOONING_PAGE_MEMORY_PAGETITLE;

        public override string HelpID => "Settings";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (!_changed)
                return;

            var connection = VMs?.FirstOrDefault()?.Connection;
            var ballooningInUse = VMs?.FirstOrDefault()?.UsesBallooning();

            if (!Helpers.FeatureForbidden(connection, Host.RestrictDMC) &&
                ballooningInUse.HasValue && ballooningInUse.Value)
            {
                memoryControlsBasic.Visible = false;
                memoryControls = memoryControlsAdvanced;
            }
            else
            {
                memoryControlsAdvanced.Visible = false;
                memoryControls = memoryControlsBasic;
            }

            memoryControls.VMs = VMs;
            memoryControls.Visible = true;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            memoryControls.UnfocusSpinners();
            cancel = !memoryControls.ChangeMemorySettings();
        }

        #endregion

        public List<VM> VMs
        {
            get => _vms;
            set
            {
                var newVMs = value == null ? new List<VM>() : new List<VM>(value);

                if (_vms.Count != newVMs.Count)
                    _changed = true;
                else if (_vms.Any(v => !newVMs.Contains(v)))
                    _changed = true;
                else
                    _changed = false;

                _vms = newVMs;
            }
        }

        private void memoryControlsBasic_InstallTools()
        {
            InstallTools?.Invoke();
        }
    }
}
