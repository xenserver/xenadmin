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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;
using System.Net;


namespace XenAdmin.SettingsPanels
{
    public partial class StorageLinkEditPage : UserControl, IEditPage
    {
        private IXenObject _xenObjectCopy;
        private bool _hasChanged;

        public StorageLinkEditPage()
        {
            InitializeComponent();
            allServersCheckBox.Visible = GetValidConnections().Count > 1;
        }

        private static List<IXenConnection> GetValidConnections()
        {
            return ConnectionsManager.XenConnectionsCopy.FindAll(c => c.IsConnected && Helpers.GetPoolOfOne(c) != null && Helpers.MidnightRideOrGreater(c) && !Helpers.FeatureForbidden(c, Host.RestrictStorageChoices));
        }

        #region IEditPage Members

        public override string Text { get { return Messages.STORAGELINK_GATEWAY; } }

        public AsyncAction SaveSettings()
        {
            var connections = new List<IXenConnection> { _xenObjectCopy.Connection };

            if (allServersCheckBox.Checked)
            {
                connections = GetValidConnections();
            }

            return new SetCslgCredentialsAction(connections, textBoxHostAddress.Text.Trim(), textBoxUsername.Text.Trim(), textBoxPassword.Text.Trim());
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _xenObjectCopy = clone;
            allServersCheckBox.Checked = true;

            if (_xenObjectCopy != null && Helpers.MidnightRideOrGreater(_xenObjectCopy.Connection))
            {
                Pool pool = Helpers.GetPoolOfOne(_xenObjectCopy.Connection);

                if (pool != null)
                {
                    StorageLinkCredentials creds = pool.GetStorageLinkCredentials();

                    var otherConfig = new Dictionary<string, string>(pool.other_config);

                    // retrieve CSLG credentials from the server. 


                    // need to set the default check-state for the "all servers" checkbox.

                    // if the user has multiple valid sl connection creds, then set the checkbox to be unchecked.
                    // otherwise set it checked.

                    var credsList = new List<StorageLinkCredentials>();

                    foreach (Pool p in GetValidConnections().ConvertAll(c => Helpers.GetPoolOfOne(c)).FindAll(p => p != null))
                    {
                        credsList.Add(p.GetStorageLinkCredentials());
                        credsList.AddRange(Array.ConvertAll(p.Connection.Cache.PBDs, pbd => pbd.GetStorageLinkCredentials()));
                    }

                    credsList.RemoveAll(c => c == null || !c.IsValid);
                    allServersCheckBox.Checked = credsList.Count == 0 || credsList.TrueForAll(c => c.Host == credsList[0].Host);

                    textBoxHostAddress.Text = creds == null ? string.Empty : creds.Host;
                    textBoxUsername.Text = creds == null ? string.Empty : creds.Username;
                    textBoxPassword.Text = creds == null ? string.Empty : creds.Password;

                    if (textBoxUsername.Text.Length == 0)
                    {
                        textBoxUsername.Text = "admin";
                    }
                }

                _hasChanged = false;
            }
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
            get { return _hasChanged; }
        }

        #endregion

        #region VerticalTab Members

        public string SubText
        {
            get
            {
                if (textBoxUsername.Text.Length > 0 && textBoxPassword.Text.Length > 0 && textBoxHostAddress.Text.Length > 0)
                {
                    return Messages.CSLG_EDIT_DETAILS_ENTERED;
                }
                else
                {
                    return Messages.CSLG_EDIT_NO_DETAILS_ENTERED;
                }
            }
        }

        public Image Image
        {
            get { return Properties.Resources.sl_16; }
        }

        #endregion

        private void textBoxHostAddress_TextChanged(object sender, EventArgs e)
        {
            _hasChanged = true;
            UpdateTestButtonEnablement();
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            _hasChanged = true;
            UpdateTestButtonEnablement();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            _hasChanged = true;
        }

        private void UpdateTestButtonEnablement()
        {
            testConnectionButton.Enabled = textBoxHostAddress.Text.Trim().Length > 0 && textBoxUsername.Text.Trim().Length > 0;
        }

        private void testConnectionButton_Click(object sender, EventArgs e)
        {
            string password = textBoxPassword.Text.Trim();
            string address = textBoxHostAddress.Text.Trim();
            string username = textBoxUsername.Text.Trim();

            string title = string.Format(Messages.STORAGELINK_TEST_CONNECTION, address);

            DelegatedAsyncAction action = null;
            action = new DelegatedAsyncAction(_xenObjectCopy.Connection, title, "", "", s =>
                {
                    string secretUuid = Secret.CreateSecret(_xenObjectCopy.Connection.Session, password);
                    var scanAction = new SrCslgStorageSystemScanAction(Program.MainWindow,_xenObjectCopy.Connection,Program.StorageLinkConnections.GetCopy(), address, username, secretUuid);
                    scanAction.RunExternal(action.Session);

                    string secretRef = Secret.get_by_uuid(_xenObjectCopy.Connection.Session, secretUuid);
                    Secret.destroy(_xenObjectCopy.Connection.Session, secretRef);
                }, true);

            action.AppliesTo.Add(_xenObjectCopy.opaque_ref);

            new ActionProgressDialog(action, ProgressBarStyle.Marquee).ShowDialog(this);

            passFailLabel.Visible = true;
            passFailLabel.Text = action.Succeeded ? Messages.CSLG_EDIT_CONNECTION_PASSED : Messages.CSLG_EDIT_CONNECTION_FAILED;
            passFailPictureBox.Image = action.Succeeded ? Resources._000_Tick_h32bit_16 : Resources._000_Abort_h32bit_16;
        }

        private void allServersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allServersCheckBox.Checked)
            {
                _hasChanged = true;
            }
        }
    }
}
