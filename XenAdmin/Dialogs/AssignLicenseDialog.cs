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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AssignLicenseDialog : XenDialogBase
    {
        private readonly List<IXenObject> xos;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignLicenseDialog"/> class.
        /// </summary>
        /// <param name="hosts">The hosts for which the licensing is to be applied.</param>
        public AssignLicenseDialog(IEnumerable<IXenObject> xos, String firstHost, String firstPort)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(xos, "XenObjects");
            this.xos = new List<IXenObject>(xos);
            InitializeComponent();
            licenseServerNameTextBox.TextChanged += licenseServerPortTextBox_TextChanged;
            licenseServerPortTextBox.TextChanged += licenseServerNameTextBox_TextChanged;
            SetOptionsForClearwaterAndNewer();
            UpdateButtonEnablement();

            // if all the hosts have the same license server details then populate the textboxes.
            List<Host> hosts = CreateHostsList();

            if (hosts[0].license_server.ContainsKey("address") && hosts[0].license_server.ContainsKey("port") &&
                hosts[0].license_server["address"] != "localhost")
            {
                if (hosts.TrueForAll(delegate(Host h)
                {
                    return h.license_server.ContainsKey("address") &&
                        h.license_server.ContainsKey("port") &&
                        h.license_server["address"] == hosts[0].license_server["address"] &&
                        h.license_server["port"] == hosts[0].license_server["port"];
                }))
                {
                    licenseServerPortTextBox.Text = hosts[0].license_server["port"];
                    licenseServerNameTextBox.Text = hosts[0].license_server["address"];
                }
            }
            else if ((!String.IsNullOrEmpty(firstHost)) && (!String.IsNullOrEmpty(firstPort)))
            {
                licenseServerPortTextBox.Text = firstPort;
                licenseServerNameTextBox.Text = firstHost;
            }
        }

        private List<Host> CreateHostsList()
        {
            List<Host> hosts = new List<Host>();
            foreach (IXenObject xenObject in xos)
            {
                if(xenObject is Pool)
                {
                    Pool pool = xenObject as Pool;
                    hosts.Add(xenObject.Connection.Resolve(pool.master));
                }
                if(xenObject is Host)
                    hosts.Add(xenObject as Host);
            }
            return hosts;
        }


        /// <summary>
        /// Not all license types apply for post-clearwater hosts. Hide them to declutter the UI
        /// </summary>
        private void SetOptionsForClearwaterAndNewer()
        {
            if(xos.TrueForAll(x=> Helpers.ClearwaterOrGreater(x.Connection)))
            {
                platinumRadioButton.Visible = false;
                enterpriseRadioButton.Visible = false;
                advancedRadioButton.Visible = false;
                perSocketRadioButton.Text = String.Format(Messages.PERSOCKET_LICENSES_X_REQUIRED,
                                                          xos.Sum(x => x.Connection.Cache.Hosts.Sum(h=>h.CpuSockets)));
            }
            else
            {
                perSocketRadioButton.Visible = false;
                advancedRadioButton.Checked = true;
            }
        }

        private Host.Edition GetCheckedEdition()
        {
            if (xenDesktopEnterpriseRadioButton.Checked)
                return xos.TrueForAll(x=>Helpers.ClearwaterOrGreater(x.Connection)) ? Host.Edition.XenDesktop : Host.Edition.EnterpriseXD;

            if (platinumRadioButton.Checked)
                return Host.Edition.Platinum;

            if (enterpriseRadioButton.Checked)
                return Host.Edition.Enterprise;

            if(advancedRadioButton.Checked)
                return Host.Edition.Advanced;

            return Host.Edition.PerSocket;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

            ApplyLicenseEditionCommand command = new ApplyLicenseEditionCommand(Program.MainWindow.CommandInterface, xos, GetCheckedEdition(), licenseServerNameTextBox.Text, licenseServerPortTextBox.Text, this);

            command.Succedded += delegate
                                     {
                                         Program.Invoke(this, () =>
                                            {
                                                DialogResult = DialogResult.OK;
                                                Close();
                                            });
                                     };
            command.Execute();
        }

        private void licenseServerPortTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnablement();
        }

        private void licenseServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnablement();
        }

        private void UpdateButtonEnablement()
        {
            okButton.Enabled = licenseServerPortTextBox.Text.Length > 0 && licenseServerNameTextBox.Text.Length > 0 && Util.IsValidPort(licenseServerPortTextBox.Text);
        }
    }
}