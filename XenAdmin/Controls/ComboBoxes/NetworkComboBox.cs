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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Controls
{
    public class NetworkComboBox : LongStringComboBox
    {
        public NetworkComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowPoolName { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ExcludeDisconnectedNetworks { get; set; }
        
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ExcludeNetworksWithoutIpAddresses { get; set; }

        public XenAPI.Network SelectedNetwork => (SelectedItem as NetworkComboBoxItem)?.Network;

        /// <summary>
        /// Get the uuid of the selected item along with its name in a pair
        /// </summary>
        public KeyValuePair<string, string> SelectedNetworkUuid
        {
            get
            {
                var selectedNetwork = SelectedNetwork;
                return selectedNetwork == null
                    ? new KeyValuePair<string, string>(string.Empty, string.Empty)
                    : new KeyValuePair<string, string>(selectedNetwork.uuid, selectedNetwork.Name());
            }
        }


        public void PopulateComboBox(IXenConnection connection, Func<NetworkComboBoxItem, bool> matchSelectionCriteria)
        {
            Items.Clear();

            var pifArray = connection.Cache.PIFs;

            foreach (var pif in pifArray)
            {
                var curPif = pif;

                if (Items.Cast<NetworkComboBoxItem>().Any(i => i.Network.opaque_ref == curPif.network.opaque_ref))
                    continue;
                if (ExcludeDisconnectedNetworks && curPif.LinkStatus() != PIF.LinkState.Connected)
                    continue;
                if (ExcludeNetworksWithoutIpAddresses && !pif.IsManagementInterface(false))
                    continue;

                var item = new NetworkComboBoxItem(curPif, ShowPoolName);
                Items.Add(item);

                if (SelectedItem == null && matchSelectionCriteria(item))
                    SelectedItem = item;
            }
        }

        public void SelectItem(Func<NetworkComboBoxItem, bool> matchSelectionCriteria)
        {
            SelectedItem = Items.Cast<NetworkComboBoxItem>().FirstOrDefault(matchSelectionCriteria);
        }


        public class NetworkComboBoxItem
        {
            private readonly string _name;

            public bool IsManagement { get; }
            public XenAPI.Network Network { get; }

            public NetworkComboBoxItem(PIF pif, bool showPoolName)
            {
                IsManagement = pif.management;
                Network = pif.Connection.Resolve(pif.network);

                var pool = Helpers.GetPoolOfOne(pif.Connection);
                var showPool = showPoolName && pool != null;

                if (IsManagement && showPool)
                    _name = string.Format(Messages.MANAGEMENT_NETWORK_WITH_POOL, Network.Name(), pool.Name());
                else if (IsManagement && !showPool)
                    _name = string.Format(Messages.MANAGEMENT_NETWORK, Network.Name());
                else if (!IsManagement && showPool)
                    _name = string.Format(Messages.NETWORK_WITH_POOL, Network.Name(), pool.Name());
                else
                    _name = Network.Name();
            }

            public override string ToString()
            {
                return _name;
            }
        }
    }
}
