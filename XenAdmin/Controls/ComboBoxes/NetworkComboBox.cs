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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Controls
{
    public class NetworkComboBox : EnableableComboBox
    {
        private List<PIF> _pifArray = new List<PIF>();

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
            UnRegisterEvents();

            _pifArray = new List<PIF>(connection.Cache.PIFs);
            RegisterEvents();

            foreach (var pif in _pifArray)
            {
                var curPif = pif;
                if (!CanShowItem(curPif, out _))
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

        private bool CanShowItem(PIF pif, out NetworkComboBoxItem existingItem)
        {
            existingItem = Items.Cast<NetworkComboBoxItem>().FirstOrDefault(i => i.Network.opaque_ref == pif.network.opaque_ref);
            
            if (existingItem != null)
                return false;

            if (ExcludeDisconnectedNetworks && pif.LinkStatus() != PIF.LinkState.Connected)
                return false;
            
            if (ExcludeNetworksWithoutIpAddresses && !pif.IsManagementInterface(false))
                return false;
            
            return true;
        }

        private void RegisterEvents()
        {
            foreach (var pif in _pifArray)
            {
                if (pif != null)
                    pif.PropertyChanged += PropertyChanged;
            }
        }

        private void UnRegisterEvents()
        {
            foreach (var pif in _pifArray)
            {
                if (pif != null)
                    pif.PropertyChanged -= PropertyChanged;
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is PIF pif) || e.PropertyName.ToLower() != "locked")
                return;

            Program.Invoke(this, () =>
            {
                var canShow = CanShowItem(pif, out var existingItem);
                
                if (existingItem == null)
                {
                    if (canShow)
                        Items.Add(new NetworkComboBoxItem(pif, ShowPoolName));
                }
                else
                {
                    if (canShow)
                    {
                        int index = Items.IndexOf(existingItem);
                        if (-1 < index && index < Items.Count)
                            RefreshItem(index);
                    }
                    else
                    {
                        //close the dropdown before removing the last item, otherwise it
                        //will crash if it loses focus while the dropdown is empty
                        if (Items.Count <= 1)
                            DroppedDown = false;
                        
                        Items.Remove(existingItem);

                        //the dropdown is not resized when items are removed,
                        //unlike when they are added
                        if (Items.Count > 0)
                            DropDownHeight = Items.Count * ItemHeight;
                    }
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            UnRegisterEvents();
            base.Dispose(disposing);
        }


        public class NetworkComboBoxItem : IEnableableComboBoxItem
        {
            private readonly string _name;

            public PIF Pif { get; }
            public bool IsManagement { get; }
            public XenAPI.Network Network { get; }
            
            public bool Enabled
            {
                get
                {
                    if (Pif != null && Pif.Locked)
                        return false;
                    if (Network != null && Network.Locked)
                        return false;
                    return true;
                }
            }

            public NetworkComboBoxItem(PIF pif, bool showPoolName)
            {
                Pif = pif;
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
                return Enabled ? _name : $"{_name} - {Messages.NETWORK_LOCKED}";
            }
        }
    }
}
