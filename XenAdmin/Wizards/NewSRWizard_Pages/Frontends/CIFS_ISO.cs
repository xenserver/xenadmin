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
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Network;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CIFS_ISO : XenTabPage
    {
        #region Private fields
        private const string LOCATION = "location";
        private const string TYPE = "type";
        private const string CIFS = "cifs";
        private const string ISO_PATH = "iso_path";
        private const string USERNAME = "username";
        private const string CIFSPASSWORD = "cifspassword";

        private bool m_disasterRecoveryTask;
        private SR m_srToReattach;
        private List<String> my_srs = new List<String>();
        #endregion

        public CIFS_ISO()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string PageTitle { get { return Messages.NEWSR_PATH_ISO_CIFS; } }

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string HelpID { get { return "Location_CIFSISO"; } }

        public override bool EnableNext()
        {
            return SrWizardHelpers.ValidateCifsSharename(comboBoxCifsSharename.Text)
                && !(checkBoxUseDifferentUsername.Checked && String.IsNullOrEmpty(textBoxCifsUsername.Text))
                && !passwordFailure1.Visible;
        }

        public override bool EnablePrevious()
        {
            if (m_disasterRecoveryTask && m_srToReattach == null)
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
                        if ((!pbd.device_config.ContainsKey("type")) || pbd.device_config["type"] != "cifs")
                            continue;

                        String location = pbd.device_config["location"];
                        location = location.Replace("/", "\\");
                        if (c == Connection)
                            my_srs.Add(location);
                        else if (!add_srs.Contains(location))
                            add_srs.Add(location);
                    }
                }
            }
            // Remove all SRs that the current pool can see
            add_srs.RemoveAll(my_srs.Contains);
            this.comboBoxCifsSharename.Items.AddRange(add_srs.ToArray());
        }

        public override void SelectDefaultControl()
        {
            comboBoxCifsSharename.Select();
        }

        #endregion

        #region Accessors

        public SrWizardType SrWizardType
        {
            set
            {
                m_disasterRecoveryTask = value.DisasterRecoveryTask;
                m_srToReattach = value.SrToReattach;
            }
        }

        public Dictionary<string, string> DeviceConfig
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                dconf[LOCATION] = comboBoxCifsSharename.Text.Replace('\\', '/');
                dconf[TYPE] = CIFS;

                // location is now //server/share or //server/share/some/path (the validator assures
                // this).  We need to take the /some/path if present and put it into the iso_path field.
                String[] bits = dconf[LOCATION].Split('/');
                if (bits.Length > 4)
                {
                    dconf[LOCATION] = string.Format("//{0}/{1}", bits[2], bits[3]);
                    dconf[ISO_PATH] = "/" + string.Join("/", bits, 4, bits.Length - 4);
                }

                if (checkBoxUseDifferentUsername.Checked)
                {
                    dconf[USERNAME] = textBoxCifsUsername.Text;
                    dconf[CIFSPASSWORD] = textBoxCifsPassword.Text;
                }

                return dconf;
            }
        }

        public string SrDescription
        {
            get
            {
                return string.IsNullOrEmpty(comboBoxCifsSharename.Text)
                           ? null
                           : string.Format(Messages.NEWSR_CIF_DESCRIPTION, comboBoxCifsSharename.Text);
            }
        }

        #endregion

        #region Event handlers

        private void textBoxCifsSharename_TextChanged(object sender, EventArgs e)
        {
            passwordFailure1.PerformCheck(IsIsoStorageAlreadyAttached);

            OnPageUpdated();
        }

        private bool IsIsoStorageAlreadyAttached(out string error)
        {
            error = string.Empty;
            if (my_srs.Contains(comboBoxCifsSharename.Text))
            {
                error = string.Format(Messages.SMB_ISO_STORAGE_ALREADY_ATTACHED, Connection.FriendlyName);
                return false;
            }
            return true;
        }

        private void checkBoxUseDifferentUsername_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxLogin.Enabled = checkBoxUseDifferentUsername.Checked;
            labelCifsUsername.Enabled = checkBoxUseDifferentUsername.Checked;
            labelCifsPassword.Enabled = checkBoxUseDifferentUsername.Checked;

            textBoxCifsUsername.Enabled = checkBoxUseDifferentUsername.Checked;
            textBoxCifsPassword.Enabled = checkBoxUseDifferentUsername.Checked;

            OnPageUpdated();
        }

        private void textBoxCifsUsername_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        #endregion  
    }
}
