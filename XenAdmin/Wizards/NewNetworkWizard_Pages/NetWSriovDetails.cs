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
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Network;

namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWSriovDetails : XenTabPage
    {
        internal Host Host;

        public NetWSriovDetails()
        {
            InitializeComponent();
        }
        public override string Text { get { return Messages.NETW_DETAILS_TEXT; } }

        public override string PageTitle
        {
            get
            {
                return Messages.NETW_INTERNAL_DETAILS_TITLE;
            }
        }
        public override void PopulatePage()
        {
            PopulateHostNicList(Host, Connection);
        }
        private void PopulateHostNicList(Host host, IXenConnection conn)
        {
            comboBoxNicList.Items.Clear();
            foreach (PIF thePIF in conn.Cache.PIFs)
            {
                if (host != null && thePIF.host.opaque_ref == host.opaque_ref && thePIF.IsPhysical() && !thePIF.IsBondNIC() 
                    && thePIF.SriovCapable() && !thePIF.IsSriovPhysicalPIF())
                {
                    comboBoxNicList.Items.Add(thePIF);
                }
            }
            if (comboBoxNicList.Items.Count > 0)
                comboBoxNicList.SelectedIndex = 0;
        }

        public XenAPI.PIF SelectedHostNic
        {
            get { return (XenAPI.PIF)comboBoxNicList.SelectedItem; }
        }

        public bool isAutomaticAddNicToVM
        {
            get { return cbxAutomatic.Checked; }
        }

        public override bool EnableNext()
        {
            return SelectedHostNic != null;
        }
    }
}
