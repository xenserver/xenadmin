﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class EditVmHaPrioritiesDialog : XenDialogBase
    {
        /// <summary>
        /// Never null.
        /// </summary>
        private readonly Pool pool;

        /// <summary>
        /// The value of ha_host_failures_to_tolerate when the dialog was opened.
        /// </summary>
        private readonly long originalNtol;

        /// <param name="pool">May not be null. HA must be turned off on the pool.</param>
        public EditVmHaPrioritiesDialog(Pool pool)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");
            if (!pool.ha_enabled)
                throw new ArgumentException("Can only configure HA for pools that already have HA turned on");

            this.pool = pool;
            InitializeComponent();
            Text += string.Format(" - '{0}'", pool.Name().Ellipsise(30));
            assignPriorities.StatusChanged += assignPriorities_StatusChanged;

            pool.PropertyChanged += pool_PropertyChanged;
            originalNtol = pool.ha_host_failures_to_tolerate;

            Rebuild();
        }

        private void assignPriorities_StatusChanged(XenTabPage sender)
        {
            // Enable OK only when the ntol update has successfully returned (-1 indicates failure).
            // Also disable when assignPriorities is disabled => edits are disallowed (e.g. because of dead hosts)
            buttonOk.Enabled = assignPriorities.Enabled && assignPriorities.EnableNext();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            assignPriorities.Connection = pool.Connection;
            assignPriorities.PopulatePageControls();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            pool.PropertyChanged -= pool_PropertyChanged;

            assignPriorities.StopNtolUpdate();
            assignPriorities.StatusChanged -= assignPriorities_StatusChanged;

            base.OnClosing(e);
        }

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            // Check that all hosts are live
            bool deadHosts = Array.Find(pool.Connection.Cache.Hosts, h => !h.IsLive()) != null;

            assignPriorities.Enabled = !deadHosts;
            pictureBoxWarningIcon.Visible = deadHosts;
            
            if (deadHosts)
            {
                buttonOk.Enabled = false;
                labelBlurb.Text = String.Format(Messages.HA_CANNOT_EDIT_WITH_DEAD_HOSTS_SHORT, pool.Name().Ellipsise(30));
            }
            else
            {
                labelBlurb.Text = string.Empty;
            	m_panelTop.Visible = false;
            }
        }

        private void pool_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label")
            {
                Rebuild();
            }
            else if (e.PropertyName == "ha_enabled")
            {
                if (!pool.ha_enabled)
                {
                    Program.Invoke(this, delegate()
                    {
                        using (var dlg = new WarningDialog(string.Format(Messages.HA_WAS_DISABLED, pool.Name()))
                            {WindowTitle = Messages.HIGH_AVAILABILITY})
                        {
                            dlg.ShowDialog(this);
                        }
                        this.Close();
                    });
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Throw away any changes
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            long newNtol = assignPriorities.Ntol;                         

            // User has configured ntol to be zero. Check this is what they want (since it is practically useless).
            if (newNtol == 0)
            {
                DialogResult dialogResult;
                using (var dlg = new WarningDialog(Messages.HA_NTOL_ZERO_QUERY,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true)){WindowTitle = Messages.HIGH_AVAILABILITY})
                {
                    dialogResult = dlg.ShowDialog(this);
                }
                if (dialogResult == DialogResult.No)
                    return;
            }

            if (assignPriorities.ChangesMade || newNtol != originalNtol)
            {
                // Set the new restart priorities.
                // This includes a pool-wide database sync to ensure the changed HA settings are communicated to all hosts
                new SetHaPrioritiesAction(this.pool.Connection, assignPriorities.GetChangedStartupOptions(), newNtol, false).RunAsync();
            }
            this.Close();
        }
    }
}
