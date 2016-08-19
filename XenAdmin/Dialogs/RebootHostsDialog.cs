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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class RebootHostsDialog : XenDialogBase
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private BackgroundWorker actionsWorker = null;
        private IList<Host> hosts;

        /// <summary>
        /// Initializes a new instance of the <see cref="RebootVmsDialog"/> class.
        /// </summary>
        /// <param name="hosts">The hosts for which the licensing is to be applied.</param>
        public RebootHostsDialog(IList<Host> hosts, Pool_patch patch)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(hosts, "hosts");

            InitializeComponent();

            foreach (var host in hosts)
            {
                var hostCheckbox = new CheckBox()
                {
                    Name = host.Name,
                    Checked = true,
                    Text = host.Name
                };

                hostsLayoutPanel.Controls.Add(hostCheckbox);

                this.hosts = hosts;
            }
        }

        public IEnumerable<Host> GetSelectedHosts()
        {
            var selectedHostNames = new List<string>();

            foreach (var control in hostsLayoutPanel.Controls)
            {
                var controlAsCheckbox = control as CheckBox;
                if (controlAsCheckbox == null) {continue;}

                if (controlAsCheckbox.Checked)
                {
                    selectedHostNames.Add(controlAsCheckbox.Name);
                }
            }

            return hosts.Where(host => selectedHostNames.Contains(host.Name));
        }
    }
}