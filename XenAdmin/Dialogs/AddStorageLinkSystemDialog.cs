/* Copyright (c) Citrix Systems Inc. 
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
using System.Windows.Forms;
using XenAdmin.Network.StorageLink;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class AddStorageLinkSystemDialog : XenDialogBase
    {
        private readonly StorageLinkConnection _storageLinkConnection;

        public AddStorageLinkSystemDialog()
        {
            InitializeComponent();
        }
        
        public AddStorageLinkSystemDialog(StorageLinkConnection storageLinkConnection)
        {
            Util.ThrowIfParameterNull(storageLinkConnection, "storageLinkConnection");
            _storageLinkConnection = storageLinkConnection;

            InitializeComponent();

            StorageAdapterComboBox.Items.AddRange(Util.PopulateList<object>(storageLinkConnection.Cache.StorageAdapters).ToArray());

            if (StorageAdapterComboBox.Items.Count > 0)
            {
                StorageAdapterComboBox.SelectedIndex = 0;
            }
        }

        private bool ValidToSubmit()
        {
            StorageLinkAdapter adapter = (StorageLinkAdapter)StorageAdapterComboBox.SelectedItem;

            if (adapter == null)
            {
                return false;
            }
            if (PortNumberTextBox.Text.Trim().Length == 0 && adapter.RequiresPort)
            {
                return false;
            }
            if (StorageSystemAddress.Length == 0 && adapter.RequiresIPAddress)
            {
                return false;
            }
            if (StorageSystemUsername.Length == 0 && adapter.RequiresUsername)
            {
                return false;
            }
            if (StorageSystemPassword.Length == 0 && adapter.RequiresPassword)
            {
                return false;
            }
            if (StorageSystemNamespace.Length == 0 && adapter.RequiresNamespace)
            {
                return false;
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidToSubmit())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public StorageLinkAdapter StorageAdapter
        {
            get
            {
                return (StorageLinkAdapter)StorageAdapterComboBox.SelectedItem;
            }
        }

        public int StorageSystemPort
        {
            get
            {
                int res;
                if (int.TryParse(PortNumberTextBox.Text.Trim(), out res))
                {
                    return res;
                }
                return 0;
            }
        }

        public string StorageSystemAddress
        {
            get
            {
                return IPAddressTextBox.Text.Trim();
            }
        }

        public string StorageSystemUsername
        {
            get
            {
                return UsernameTextBox.Text.Trim();
            }
        }

        public string StorageSystemPassword
        {
            get
            {
                return PasswordTextBox.Text.Trim();
            }
        }

        public string StorageSystemNamespace
        {
            get
            {
                return NamespaceTextBox.Text.Trim();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void StorageAdapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!PortNumberTextBox.Enabled)
            {
                PortNumberTextBox.Text = string.Empty;
            }

            if (!NamespaceTextBox.Enabled)
            {
                NamespaceTextBox.Text = string.Empty;
            }
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            StorageLinkAdapter adapter = (StorageLinkAdapter)StorageAdapterComboBox.SelectedItem;

            PortNumberLabel.Enabled = adapter.SupportsPort;
            PortNumberTextBox.Enabled = adapter.SupportsPort;
            IPAddressLabel.Enabled = adapter.SupportsIPAddress;
            IPAddressTextBox.Enabled = adapter.SupportsIPAddress;
            UsernameLabel.Enabled = adapter.SupportsUsername;
            UsernameTextBox.Enabled = adapter.SupportsUsername;

            PasswordLabel.Enabled = adapter.SupportsPassword;
            PasswordTextBox.Enabled = adapter.SupportsPassword;
            NamespaceLabel.Enabled = adapter.SupportsNamespace;
            NamespaceTextBox.Enabled = adapter.SupportsNamespace;

            if (NamespaceTextBox.Enabled)
            {
                NamespaceTextBox.Text = !string.IsNullOrEmpty(adapter.DefaultNamespace) ? adapter.DefaultNamespace : "interop";
                PortNumberTextBox.Text = adapter.DefaultPort == 0 ? string.Empty : adapter.DefaultPort.ToString();
                IPAddressLabel.Text = Messages.ADD_STORAGE_LINK_SYSTEM_CIMOM_IP_ADDRESS_LABEL;
            }
            else
            {
                NamespaceTextBox.Text = string.Empty;
                PortNumberTextBox.Text = string.Empty;
                IPAddressLabel.Text = Messages.ADD_STORAGE_LINK_SYSTEM_IP_ADDRESS_LABEL;
            }
        }

        private void StorageAdapterComboBox_DropDown(object sender, EventArgs e)
        {
            int width = StorageAdapterComboBox.DropDownWidth;
            foreach(StorageLinkAdapter adapter in StorageAdapterComboBox.Items)
            {
                width = Math.Max(width, TextRenderer.MeasureText(adapter.ToString(), StorageAdapterComboBox.Font).Width);
            }

            StorageAdapterComboBox.DropDownWidth = width;
        }
    }
}