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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Core;
using XenAdmin.Network;

using XenAPI;
using XenAdmin.Properties;
using XenAdmin.Actions;


namespace XenAdmin.SettingsPanels
{
    public partial class HomeServerEditPage : UserControl, IEditPage
    {
        private VM vm;

        public HomeServerEditPage()
        {
            InitializeComponent();
            Text = Messages.HOME_SERVER;
            picker.AutoSelectAffinity = false;
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            Host aff = picker.SelectedAffinity;
            string affinity = aff == null ?
                Helper.NullOpaqueRef : aff.opaque_ref;

            return new DelegatedAsyncAction(
                vm.Connection,
                Messages.ACTION_CHANGE_HOME_SERVER,
                string.Format(Messages.ACTION_CHANGING_HOME_SERVER_FOR, vm),
                null,
                delegate(Session session) { VM.set_affinity(session, vm.opaque_ref, affinity); },
                true,
                "vm.set_affinity"
            );
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            vm = clone as VM;
            if (vm == null)
                return;

            Host currentAffinity = vm.Connection.Resolve(vm.affinity);
            picker.SetAffinity(vm.Connection, currentAffinity, null);
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                if (picker.SelectedAffinity == null && vm.Connection.Resolve(vm.affinity) == null)
                    return false;

                if (picker.SelectedAffinity != null && vm.affinity != null &&
                    picker.SelectedAffinity.opaque_ref == vm.affinity.opaque_ref)
                    return false;

                return true;
            }
        }

        public string SubText
        {
            get
            {
                Host host = picker.SelectedAffinity;
                if (picker.SelectedAffinity == null)
                    return Messages.NONE_DEFINED;

                return host.Name;
            }
        }

        public Image Image
        {
            get { return Resources._000_ServerHome_h32bit_16; }
        }

        #endregion
    }
}
