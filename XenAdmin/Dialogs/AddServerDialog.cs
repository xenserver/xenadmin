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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenCenterLib;

namespace XenAdmin.Dialogs
{
    public partial class AddServerDialog : XenDialogBase
    {
        private readonly bool _changedPass;

        public event Action<IXenConnection> CachePopulated;

        /// <summary>
        /// Dialog with defaults taken from an existing IXenConnection
        /// </summary>
        /// <param name="connection">The IXenConnection from which the values will be taken.  May be null, in which case an appropriate new
        /// connection will be created when the dialog is completed.</param>
        /// <param name="changedPass"></param>
        public AddServerDialog(IXenConnection connection, bool changedPass)
            : base(connection)
        {
            _changedPass = changedPass;

            InitializeComponent();

            var history = Settings.GetServerHistory().ToArray();
            Array.Sort(history, StringUtility.NaturalCompare);
            ServerNameComboBox.Items.AddRange(history.Where(s => s != null).Cast<object>().ToArray());

            if (connection != null)
            {
                ServerNameComboBox.Text = connection.HostnameWithPort;
                UsernameTextBox.Text = connection.Username;
                PasswordTextBox.Text = connection.Password ?? "";
            }
        }

        private void OnCachePopulated(IXenConnection conn)
        {
            var handler = CachePopulated;
            if (handler != null)
                handler(conn);
        }

        private void AddServerDialog_Load(object sender, EventArgs e)
        {
            UpdateText();
            CenterToParent();
        }

        private void AddServerDialog_Shown(object sender, EventArgs e)
        {
            //Set focus to the password field if there's a connection and a username.
            //CA-68596: Focus has to be set when the dialog is shown rather than loaded
            //(which happens before it's shown), so a warning can pop up if CapsLock is on

            if (!ServerNameComboBox.Enabled && connection != null && !string.IsNullOrEmpty(connection.Username))
            {
                Win32.SetFocus(PasswordTextBox.Handle);
            }
        }

        private void UpdateText()
        {
            if (connection == null)
            {
                Text = Messages.ADD_NEW_CONNECT_TO;
                labelInstructions.Text = Messages.ADD_NEW_ENTER_CREDENTIALS;
                labelError.Text = "";
                ServerNameComboBox.Enabled = true;
                AddButton.Text = Messages.ADD;
            }
            else if (!_changedPass)
                return;
            else if (connection.Password == null)
            {
                Text = Messages.CONNECT_TO_SERVER;
                labelInstructions.Text = Messages.CONNECT_TO_SERVER_BLURB;
                labelError.Text = "";
                ServerNameComboBox.Enabled = false;
                AddButton.Text = Messages.CONNECT;
            }
            else if (connection.ExpectPasswordIsCorrect)
            {
                // This situation should be rare, it normally comes from logging in a new session after an existing one has been made
                // We now use duplicate sessions instead most of the time which don't log in again.
                Text = Messages.CONNECT_TO_SERVER;
                labelInstructions.Text = string.Format(Messages.ADDSERVER_PASS_NEW, BrandManager.BrandConsole);
                labelError.Text = "";
                ServerNameComboBox.Enabled = false;
                AddButton.Text = Messages.OK;
            }
            else  // the password probably hasn't actually changed but we do know the user has typed it in wrong
            {
                Text = Messages.CONNECT_TO_SERVER;
                labelInstructions.Text = string.Format(Messages.ERROR_CONNECTING_BLURB, BrandManager.BrandConsole);
                labelError.Text = Messages.ADD_NEW_INCORRECT;
                ServerNameComboBox.Enabled = false;
                AddButton.Text = Messages.CONNECT;
            }

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string hostnameAndPort = ServerNameComboBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Text;

            string[] multipleHosts;

            if (TryGetMultipleHosts(hostnameAndPort, out multipleHosts))
            {
                foreach (string h in multipleHosts)
                {
                    ConnectToServer(null, h, ConnectionsManager.DEFAULT_XEN_PORT, username, password, string.Empty);
                }
            }
            else
            {
                string hostname;
                int port;

                if (!StringUtility.TryParseHostname(hostnameAndPort, ConnectionsManager.DEFAULT_XEN_PORT, out hostname, out port))
                {
                    hostname = hostnameAndPort;
                    port = ConnectionsManager.DEFAULT_XEN_PORT;
                }
                ConnectToServer(connection, hostname, port, username, password, string.Empty);
            }

            Close();
        }

        private void ConnectToServer(IXenConnection conn, string hostname, int port, string username, string password, string version)
        {
            if (conn == null)
            {
                conn = new XenConnection { fromDialog = true };
                conn.CachePopulated += conn_CachePopulated;
            }
            else if (!_changedPass)
            {
                conn.EndConnect(); // in case we're already connected
            }

            conn.Hostname = hostname;
            conn.Port = port;
            conn.Username = username;
            conn.Password = password;
            conn.ExpectPasswordIsCorrect = false;
            conn.Version = version;

            if (!_changedPass)
                XenConnectionUI.BeginConnect(conn, true, Owner, false);
        }

        private void conn_CachePopulated(IXenConnection conn)
        {
            conn.CachePopulated -= conn_CachePopulated;
            OnCachePopulated(conn);
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TextFields_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            AddButton.Enabled = OKButtonEnabled();
        }

        private bool OKButtonEnabled()
        {
            return ServerNameComboBox.Text.Trim().Length > 0 && UsernameTextBox.Text.Trim().Length > 0;
        }

        /// <summary>
        /// Used for testing. Parses text for a semi-colon separated list of files that are loaded by DbProxy.
        /// </summary>
        /// <param name="text">The text to be parsed</param>
        /// <param name="hosts">The parse results</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        private static bool TryGetMultipleHosts(string text, out string[] hosts)
        {
            hosts = new string[0];
            string[] splitStr = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitStr.Length > 0)
            {
                List<string> hostList = new List<string>();
                foreach (string element in splitStr)
                {
                    string host = element.Trim();
                    if (string.IsNullOrEmpty(host))
                        // ignore "empty" entry
                        continue;

                    if (Regex.IsMatch(host, @"^[A-Za-z]\:\\") && File.Exists(host))
                    {
                        // is a file
                        hostList.Add(host);
                        continue;
                    }
                    if (Regex.IsMatch(host, @"^http\:|^[A-Za-z]\:"))
                    {
                        Uri uri;
                        if (Uri.TryCreate(host, UriKind.Absolute, out uri))
                        {
                            hostList.Add(host);
                            continue;
                        }
                    }
                    else
                    {
                        // some sort of ip address or hostname, just add
                        hostList.Add(host);
                    }
                }

                if (hostList.Count < 2)
                {
                    return false;
                }

                hosts = hostList.ToArray();
                return true;
            }

            return false;
        }

        private void labelError_TextChanged(object sender, EventArgs e)
        {
            pictureBoxError.Visible = labelError.Visible = (labelError.Text != "");
        }
    }
}
