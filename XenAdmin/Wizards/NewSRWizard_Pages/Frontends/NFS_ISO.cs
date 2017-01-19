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
using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class NFS_ISO : XenTabPage
    {
        private const string LOCATION = "location";
        private const string TYPE = "type";
        private const string NFS = "nfs_iso";
        private const string NFSVERSION = "nfsversion";

        private SR _srToReattach;
        private bool _disasterRecoveryTask;
        private List<String> my_srs = new List<String>();

        public NFS_ISO()
        {
            InitializeComponent();
            passwordFailure1.Visible = false;
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string PageTitle { get { return Messages.NEWSR_PATH_ISO; } }

        public override string HelpID { get { return "Location_NFSISO"; } }

        public override bool EnableNext()
        {
            return !passwordFailure1.Visible && SrWizardHelpers.ValidateNfsSharename(NfsServerPathComboBox.Text);
        }

        public override bool EnablePrevious()
        {
            if (_disasterRecoveryTask && _srToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            var add_srs = new List<String>();
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (SR sr in c.Cache.SRs)
                {
                    if (sr.GetSRType(true) != SR.SRTypes.iso)
                        continue;
                    foreach (PBD pbd in c.ResolveAll(sr.PBDs))
                    {
                        if ((!pbd.device_config.ContainsKey("type")) || pbd.device_config["type"] != "nfs_iso")
                            continue;
                        String location = pbd.device_config["location"];
                        if (c == Connection)
                            my_srs.Add(location);
                        else if (!add_srs.Contains(location))
                            add_srs.Add(location);
                    }
                }
            }
            // Remove all SRs that the current pool can see
            add_srs.RemoveAll(s => my_srs.Contains(s));
            this.NfsServerPathComboBox.Items.AddRange(add_srs.ToArray());

            //Setting up visibility of the NFS Version controls
            nfsVersionLabel.Visible = nfsVersionTableLayoutPanel.Visible = Helpers.DundeeOrGreater(Connection);
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (direction == PageLoadedDirection.Forward)
                HelpersGUI.FocusFirstControl(Controls);
        }

        #endregion

        private bool IsSRNameUnique(out string error)
        {
            error = string.Empty;
            if (my_srs.Contains(NfsServerPathComboBox.Text))
            {
                error = string.Format(Messages.NFS_ISO_ALREADY_ATTACHED, Connection.FriendlyName);
                return false;
            }
            return true;
        }

        private void NfsServerPathTextBox_TextChanged(object sender, EventArgs e)
        {
            passwordFailure1.PerformCheck(IsSRNameUnique);
            OnPageUpdated();
        }

        #region Accessors

        public SrWizardType SrWizardType
        {
            set
            {
                _srToReattach = value.SrToReattach;
                _disasterRecoveryTask = value.DisasterRecoveryTask;
            }
        }

        public Dictionary<string, string> DeviceConfig
        {
            get
            {
                var dconf = new Dictionary<string, string>();
                dconf[LOCATION] = NfsServerPathComboBox.Text;
                dconf[TYPE] = NFS;

                if (nfsVersion4RadioButton.Checked)
                    dconf[NFSVERSION] = "4";

                return dconf;
            }
        }

        public string SrDescription
        {
            get
            {
                return string.IsNullOrEmpty(NfsServerPathComboBox.Text)
                           ? null
                           : string.Format(Messages.NEWSR_ISO_DESCRIPTION, NfsServerPathComboBox.Text);
            }
        }

        #endregion
    }
}
