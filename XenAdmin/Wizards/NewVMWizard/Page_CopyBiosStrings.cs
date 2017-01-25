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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CopyBiosStrings : XenTabPage
    {
        private Host _affinity;
        private VM _template;

        public Page_CopyBiosStrings()
        {
            InitializeComponent();
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            VM template = SelectedTemplate;
            
            if (!template.Equals(_template))
            {
                _template = template;
                ServersGridView.Rows.Clear();

                if (template.DefaultTemplate)
                {
                    List<Host> hosts = new List<Host>(Connection.Cache.Hosts);
                    hosts.Sort();
                    foreach (Host host in hosts)
                    {
                        ServerGridRow row = new ServerGridRow(host, false);
                        ServersGridView.Rows.Add(row);

                        if (host == _affinity)
                        {
                            row.Selected = true;
                        }
                    }
                }
                ServersGridView.Enabled = template.DefaultTemplate;
            }
        }

        public override string Text
        {
            get
            {
                return Messages.NEWVMWIZARD_COPY_BIOS_STRINGS_PAGE_NAME;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.NEWVMWIZARD_COPY_BIOS_STRINGS_PAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "BIOSStrings"; }
        }

        public VM SelectedTemplate { private get; set; }

        public Host Affinity
        {
            get
            {
                return _affinity;
            }
            set
            {
                _affinity = value;
            }
        }

        public Host CopyBiosStringsFrom
        {
            get
            {
                foreach (ServerGridRow row in ServersGridView.Rows)
                {
                    if (row.Selected)
                    {
                        return row.Server;
                    }
                }
                return null;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> d = new List<KeyValuePair<string, string>>();

                if (CopyBiosStringsFrom != null)
                {
                    d.Add(new KeyValuePair<string,string>(Messages.NEWVMWIZARD_COPY_BIOS_STRINGS_PAGE_HOST, CopyBiosStringsFrom.Name));
                }
                return d;
            }
        }

        private void ServersGridView_Paint(object sender, PaintEventArgs e)
        {
            if (!ServersGridView.Enabled)
            {
                TextRenderer.DrawText(e.Graphics, Messages.NEW_VM_WIZARD_BIOS_STRINGS_CANNOT_BE_CHANGED, ServersGridView.Font, ServersGridView.ClientRectangle, Color.Black, 
                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.WordBreak);
            }
        }
    }
}
