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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class InstallToolsWarningDialog : XenDialogBase
    {
        List<IXenConnection> SRCheckConnections = new List<IXenConnection>();

        public InstallToolsWarningDialog(IXenConnection connection)
            : this(connection, false, null)
        {
            pictureBox1.Image = SystemIcons.Warning.ToBitmap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallToolsWarningDialog"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="multipleVMs">if set to <c>true</c> then the message will be pluralised.</param>
        /// <param name="additionalConnections">if from a multiselect action spanning multiple connections, provide them here for the SR check</param>
        public InstallToolsWarningDialog(IXenConnection connection, bool multipleVMs, List<IXenConnection> additionalConnections)
            : base(connection)
        {
            InitializeComponent();
            if (additionalConnections != null)
            SRCheckConnections.AddRange(additionalConnections);
            if (connection != null) 
                SRCheckConnections.Add(connection);
                
            if (multipleVMs) // multiple VMs
            {
                label1.Text = Messages.XS_TOOLS_MESSAGE_MORE_THAN_ONE_VM;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.HelpManager.Launch(Name);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // check all connections to make sure they don't have any borked SRs. If we find one tell the user we are going to fix it.
            foreach (IXenConnection c in SRCheckConnections)
            {
                foreach (SR sr in c.Cache.SRs)
                {
                    if (sr.IsToolsSR && sr.IsBroken())
                    {
                        Hide();
                        DialogResult dialogResult;
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.BROKEN_TOOLS_PROMPT,
                                Messages.INSTALL_XS_TOOLS),
                            ThreeButtonDialog.ButtonOK,
                            ThreeButtonDialog.ButtonCancel))
                        {
                            dialogResult = dlg.ShowDialog(this);
                        }
                        if (dialogResult != DialogResult.OK)
                        {
                            DialogResult = DialogResult.No;
                            Close();
                            return;
                        }
                        else
                        {
                            DialogResult = DialogResult.Yes;
                            Close();
                            return;
                        }
                    }
                }
            }

            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
