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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWName : XenTabPage
    {
        public NetWName()
        {
            InitializeComponent();
        }

        public override string Text { get { return Messages.NETW_NAME_TEXT; } }

        public override string PageTitle { get { return Messages.NETW_NAME_TITLE; } }

        public override bool EnableNext()
        {
            return txtName.Text.Trim().Length > 0;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            HelpersGUI.FocusFirstControl(Controls);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                CheckUniqueName(txtName.Text);

            base.PageLeave(direction, ref cancel);
        }

        public override void PopulatePage()
        {
            txtName.Text = Helpers.MakeUniqueName(GetNetworkName(SelectedNetworkType), GetExistingNetworkNames(Connection));
        }

        public NetworkTypes SelectedNetworkType { private get; set; }

        public string NetworkName
        {
            get
            {
                return this.txtName.Text;
            }
        }

        public string NetworkDescription
        {
            get
            {
                return this.txtDescription.Text;
            }
        }

        private string GetNetworkName(NetworkTypes network_type)
        {
            switch (network_type)
            {
                case NetworkTypes.External:
                    return Messages.NEWNETWORK_NAME;
                case NetworkTypes.Internal:
                case NetworkTypes.CHIN:
                    return Messages.NEWNETWORK_VNAME;
                default:
                    return "";
            }
        }

        private List<string> GetExistingNetworkNames(IXenConnection connection)
        {
            List<string> result = new List<string>();
            foreach (XenAPI.Network TheNetwork in connection.Cache.Networks)
            {
                result.Add(TheNetwork.Name);
            }
            return result;
        }

        //
        // If the network name is not unique, we will tack on " (n)" where
        // n equals 1 on the first copy and increments until unique.
        //
        private void CheckUniqueName(string networkName)
        {
            List<string> existing_names = GetExistingNetworkNames(Connection);

            if (existing_names.Contains(networkName))
            {
                int baseCharCount = networkName.IndexOf(@"(");
                if (baseCharCount == -1)
                {
                    baseCharCount = networkName.Length;
                }

                string basename = networkName.Substring(0, baseCharCount).TrimEnd(null);
                txtName.Text = Helpers.MakeUniqueName(basename, existing_names);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
