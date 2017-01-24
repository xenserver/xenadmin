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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class PvsCacheConfigurationPage : UserControl, IEditPage
    {
        protected internal PVS_site PvsSite;
        protected internal Rectangle DeleteIconBounds;

        private IXenConnection connection;
        private List<string> knownSiteNames;
        private List<PvsCacheStorageRow> rows = new List<PvsCacheStorageRow>();

        public event EventHandler Changed;
        public event EventHandler DeleteButtonClicked;

        public PvsCacheConfigurationPage(IXenConnection connection, List<string> knownSiteNames)
        {
            this.connection = connection;
            this.knownSiteNames = knownSiteNames;
            InitializeComponent();
        }

        public Image Image
        {
            get { return Images.GetImage16For(Icons.PvsSite); }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            PvsSite = (PVS_site) clone;
            if (PvsSite != null)
            {
                textBox1.Text = PvsSite.Name;
                pvsConfigInfoIcon.Visible = pvsConfigInfoLabel.Visible = string.IsNullOrEmpty(PvsSite.PVS_uuid);
            }
            else
            {
                // Generate list of all taken PVS_site names
                List<string> takenNames = new List<PVS_site>(connection.Cache.PVS_sites).ConvertAll(s => s.Name);
                takenNames.AddRange(knownSiteNames);

                // Generate a unique suggested name for the new site
                textBox1.Text = Helpers.MakeUniqueName(Messages.PVS_SITE_NAME, takenNames);
            }

            LoadServers();
            viewPvsServersButton.Enabled = PvsSite != null && PvsSite.servers.Count > 0;
            cacheStorageInUseInfoIcon.Visible = cacheStorageInUseInfoLabel.Visible = rows.Any(row => row.ReadOnly);
            memoryOnlyInfoIcon.Visible = memoryOnlyInfoLabel.Visible = rows.Any(row => !row.ReadOnly);
        }

        public bool ValidToSave
        {
            get { return !string.IsNullOrEmpty(textBox1.Text.Trim()) && rows.All(r => r.ValidToSave); }
        }

        public sealed override string Text
        {
            get { return textBox1.Text; }
            set { base.Text = value; }
        }

        public String SubText
        {
            get
            {
                var configuredRows = rows.Where(r => r.CacheSr != null).ToList();

                if (configuredRows.Count == 0)
                    return Messages.PVS_CACHE_NOT_CONFIGURED;

                return configuredRows.Any(row => row.CacheSr.GetSRType(false) != SR.SRTypes.tmpfs) 
                    ? Messages.PVS_CACHE_MEMORY_AND_DISK 
                    : Messages.PVS_CACHE_MEMORY_ONLY;
            }
        }

        private void LoadServers()
        {
            hostsPanel.SuspendLayout();
            hostsPanel.Controls.Clear();
            var hosts = connection.Cache.Hosts.ToList();
            hosts.Sort();
            // start from the last element in the list, because when each control is docked to top in this order
            for (var index = hosts.Count - 1; index >= 0; index--)
            {
                var host = hosts[index];
                var row = new PvsCacheStorageRow(host, PvsSite)
                {
                    ShowHeader = index == 0,
                    Dock = DockStyle.Top,
                    TabIndex = index
                };
                row.Changed += SomethingChanged;
                rows.Add(row);
                hostsPanel.Controls.Add(row);
            }
            hostsPanel.ResumeLayout();
            hostsPanel.Refresh();
        }

        public AsyncAction SaveSettings()
        {
            if (!ValidToSave)
                return null;

            var newPvsCacheStorages = new List<PVS_cache_storage>();
            foreach (var row in rows.Where(r => r.HasChanged))
            {
                var pvsCacheStorage = new PVS_cache_storage
                {
                    site = PvsSite != null ? new XenRef<PVS_site>(PvsSite) : null,
                    host = new XenRef<Host>(row.Host),
                    SR =  row.CacheSr != null ? new XenRef<SR>(row.CacheSr) : null,
                    size = row.CacheSize
                };

                newPvsCacheStorages.Add(pvsCacheStorage);
            }

            if (newPvsCacheStorages.Count > 0 || NameHasChanged)
                return new ConfigurePvsSiteAction(connection, textBox1.Text, PvsSite, newPvsCacheStorages);
            return null;
        }

        public void ShowLocalValidationMessages()
        { }

        public void Cleanup()
        { }

        public bool HasChanged
        {
            get { return NameHasChanged || rows.Any(r => r.HasChanged); }
        }

        private bool NameHasChanged
        {
            get { return PvsSite == null || textBox1.Text != PvsSite.Name; }
        }
        
        private void SomethingChanged(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (DeleteButtonClicked != null)
                DeleteButtonClicked(this, e);
        }

        private void viewServersButton_Click(object sender, EventArgs e)
        {
            if (PvsSite == null)
                return;
            using (var dialog = new PvsSiteDialog(PvsSite))
                dialog.ShowDialog(this);
        }
    }
}
