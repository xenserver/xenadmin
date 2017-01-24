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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Network;

namespace XenAdmin.Dialogs
{
    public class ConnectionLostDialogLauncher
    {
        public ConnectionLostDialogLauncher()
        {
            FireOnce = true;
        }

        private static readonly string DefaultMessage = Messages.CONNECTION_WAS_LOST;
        private string displayMessage = DefaultMessage;
        /// <summary>
        /// Message to display - default provided
        /// PLEASE_RECONNECT_HOST will be appended to this message
        /// </summary>
        private string DisplayMessage
        {
            set { displayMessage = value; }
            get { return string.Format(Messages.PLEASE_RECONNECT_HOST, displayMessage); }
        }

        private bool fired;
        /// <summary>
        /// Launch the dialog the first time it's called only
        /// </summary>
        public bool FireOnce { set; private get; }

        /// <summary>
        /// Check connection is connected and launch dialog if not
        /// </summary>
        /// <param name="connection">connection to check</param>
        /// <returns>If connection is connected at time of checking</returns>
        public bool IsStillConnected(IXenConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection", "Could not check if this connection was connected as a null was provided");

            if(!string.IsNullOrEmpty(connection.Hostname))
                DisplayMessage = string.Format(Messages.NEW_SR_CONNECTION_LOST, connection.Hostname);

            bool isConnected = connection.IsConnected;
            if(!isConnected)
            {
                if(FireOnce && !fired)
                    LaunchDialog();
                if(!FireOnce)
                    LaunchDialog();
            }

            return isConnected;
        }

        private void LaunchDialog()
        {
            fired = true;
            Program.Invoke(Program.MainWindow, () => 
            {    using (var dlg = new ThreeButtonDialog( new ThreeButtonDialog.Details( SystemIcons.Error, DisplayMessage, Messages.XENCENTER), 
                                   new ThreeButtonDialog.TBDButton(Messages.OK, DialogResult.OK)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
            });
        }
    }
}
