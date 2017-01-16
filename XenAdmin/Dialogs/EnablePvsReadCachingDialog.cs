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
using System.Diagnostics;
using System.Linq;
using XenAPI;
using XenAdmin.Actions;

namespace XenAdmin.Dialogs
{
    public partial class EnablePvsReadCachingDialog : XenDialogBase
    {
        private readonly IList<VM> _vms;

        public EnablePvsReadCachingDialog(IList<VM> vms)
        {
            InitializeComponent();

            _vms = vms;

            if (vms.Count > 1)
            {
                rubricLabel.Text = Messages.ENABLE_PVS_READ_CACHING_RUBRIC_MULTIPLE;
            }
            else
            {
                rubricLabel.Text = Messages.ENABLE_PVS_READ_CACHING_RUBRIC_SINGLE;
            }

            PopulateSiteList();
        }

        private void PopulateSiteList()
        {
            // We assume all VMs share a pool
            var vm = _vms[0];

            foreach (var site in vm.Connection.Cache.PVS_sites)
            {
                 var siteToAdd = new PvsSiteComboBoxItem(site);
                 pvsSiteList.Items.Add(siteToAdd);
            }

            if (pvsSiteList.Items.Count > 0)
            {
                pvsSiteList.SelectedIndex = 0;
            }
        }

        private void enableButton_Click(object sender, EventArgs e)
        {
            var siteItemSelected = (PvsSiteComboBoxItem) pvsSiteList.SelectedItem;
            var siteSelected = (PVS_site) siteItemSelected.Item;

            var actions = new List<AsyncAction>();

            foreach (var vm in _vms)
            {
                var action = GetAsyncActionForVm(vm, siteSelected);
                if (action != null)
                {
                    actions.Add(action);
                }
            }

            if (actions.Any())
            {
                if (actions.Count == 1)
                {
                    actions[0].RunAsync();
                }
                else
                {
                    new ParallelAction(
                        Messages.ACTION_ENABLE_PVS_READ_CACHING,
                        Messages.ACTION_ENABLING_PVS_READ_CACHING,
                        Messages.ACTION_ENABLED_PVS_READ_CACHING,
                        actions).RunAsync();
                }
            }
        }

        /// <summary>
        /// If the VM can have PVS read-caching enabled, returns an action to do so. Else returns null
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="siteSelected"></param>
        /// <returns></returns>
        private AsyncAction GetAsyncActionForVm(VM vm, PVS_site siteSelected)
        {
            if (vm.PvsProxy != null)
                return null; // PVS read caching already enabled

            var vif = GetVifForPvsProxy(vm);
            if (vif == null)
                return null; // No VIF with device = 0, so can't enable

            return new PvsProxyCreateAction(vm, siteSelected, vif);
        }
        
        /// <summary>
        /// The VIF for a PVS Proxy is the one with VIF.device=0
        /// </summary>
        /// <param name="vm"></param>
        private VIF GetVifForPvsProxy(VM vm)
        {
            var vifRefs = vm.VIFs;
            var vifs = vm.Connection.ResolveAll(vifRefs);

            return vifs.FirstOrDefault(vif => vif.device.Equals("0"));
        }
    }

    internal class PvsSiteComboBoxItem
    {
        private readonly PVS_site _site;

        public PvsSiteComboBoxItem(PVS_site site)
        {
            Debug.Assert(site != null, "site passed to combobox was null");
            _site = site;
        }

        public override string ToString()
        {
            return _site.NameWithWarning;
        }

        public IXenObject Item
        {
            get { return _site; }
        }
    }
}