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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;
using System.Linq;
using CheckGroup = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<XenAdmin.Diagnostics.Checks.Check>>;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardPrecheckPage : PatchingWizard_PrecheckPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RollingUpgradeWizardPrecheckPage()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }


        private void AddEventHandlersToMasters()
        {
            foreach (Host master in SelectedMasters)
            {
                master.Connection.ConnectionStateChanged += connection_ConnectionChanged;
                master.Connection.CachePopulated += connection_ConnectionChanged;
            }
        }

        private void RemoveEventHandlersToMasters()
        {
            foreach (Host master in SelectedMasters)
            {
                master.Connection.ConnectionStateChanged -= connection_ConnectionChanged;
                master.Connection.CachePopulated -= connection_ConnectionChanged;
            }
        }

        private void connection_ConnectionChanged(object sender, EventArgs eventArgs)
        {
            Program.Invoke(this, RefreshRechecks);
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Back)
            {
                RefreshRechecks();
                return;
            }
            var selectedMasters = new List<Host>(SelectedMasters);
            ManualUpgrade = ManualModeSelected;
            RemoveEventHandlersToMasters();
            SelectedServers.Clear();
            foreach (Host selectedMaster in selectedMasters)
            {
                Host master = selectedMaster;
                if (master != null)
                {
                    Pool pool = Helpers.GetPoolOfOne(master.Connection);
                    if (pool != null)
                        SelectedServers.AddRange(pool.HostsToUpgrade());
                    else
                        SelectedServers.Add(master);
                }
            }
            AddEventHandlersToMasters();
            labelPrechecksFirstLine.Text = Messages.ROLLINGUPGRADE_PRECHECKS;
            RefreshRechecks();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            RemoveEventHandlersToMasters();
        }

        public override void PageCancelled()
        {
            RemoveEventHandlersToMasters();
            base.PageCancelled();
        }

        public override string PageTitle
        {
            get
            {
                return Messages.UPGRADE_PRECHECKS_TITLE;
            }
        }

        public override string Text
        {
            get
            {
                return Messages.UPGRADE_PRECHECKS_TEXT;
            }
        }

        public override string HelpID
        {
            get { return "Upgradeprechecks"; }
        }

        protected override List<CheckGroup> GenerateChecks(Pool_patch patch)
        {
            var groups = new List<CheckGroup>();

            //XenCenter version check (if any of the selected server version is not the latest)
            var latestCrVersion = Updates.XenServerVersions.FindAll(item => item.LatestCr).OrderByDescending(v => v.Version).FirstOrDefault();
            if (latestCrVersion != null &&
                SelectedServers.Any(host => new Version(Helpers.HostProductVersion(host)) < latestCrVersion.Version))
            {
                groups.Add(new CheckGroup(Messages.CHECKING_XENCENTER_VERSION, new List<Check> {new XenCenterVersionCheck(null)}));
            }

            //HostMaintenanceModeCheck checks
            var livenessChecks = new List<Check>();
            foreach (Host host in SelectedServers)
                livenessChecks.Add(new HostMaintenanceModeCheck(host));

            groups.Add(new CheckGroup(Messages.CHECKING_HOST_LIVENESS_STATUS, livenessChecks));

            //HA checks
            var haChecks = new List<Check>();
            foreach (Pool pool in SelectedPools)
            {
                Host host = pool.Connection.Resolve(pool.master);

                if (host == null)
                    continue;

                haChecks.Add(new HAOffCheck(host));
            }
            groups.Add(new CheckGroup(Messages.CHECKING_HA_STATUS, haChecks));

            //Checking can evacuate host
            var evacuateChecks = new List<Check>();
            foreach (Host host in SelectedServers)
                evacuateChecks.Add(new AssertCanEvacuateUpgradeCheck(host));

            groups.Add(new CheckGroup(Messages.CHECKING_CANEVACUATE_STATUS, evacuateChecks));

            //PBDsPluggedCheck
            var pbdChecks = new List<Check>();
            foreach (Host host in SelectedServers)
                pbdChecks.Add(new PBDsPluggedCheck(host));
            groups.Add(new CheckGroup(Messages.CHECKING_STORAGE_CONNECTIONS_STATUS, pbdChecks));


            //HotfixesCheck required for MNR, Cowley, Boston and Sanibel
            var hotfixChecks = new List<Check>();
            foreach (var host in SelectedServers)
            {
                if (new HotfixFactory().IsHotfixRequired(host) && !ManualUpgrade)
                    hotfixChecks.Add(new HostHasHotfixCheck(host));
            }
            if (hotfixChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_UPGRADE_HOTFIX_STATUS, hotfixChecks));


            //iSL (StorageLink) check - CA-223486: only for pre-Creedence
            var preCreedenceServers = SelectedServers.Where(h => !Helpers.CreedenceOrGreater(h)).ToList();
            if (preCreedenceServers.Any())
            {
                var srLinkChecks = new List<Check>();
                foreach (Host host in preCreedenceServers)
                    srLinkChecks.Add(new HostHasUnsupportedStorageLinkSRCheck(host));
                groups.Add(new CheckGroup(Messages.CHECKING_STORAGELINK_STATUS, srLinkChecks));
            }

            //SafeToUpgradeCheck - in automatic mode only
            if (!ManualUpgrade)
            {
                var upgradeChecks = new List<Check>();
                foreach (Host host in SelectedServers)
                    upgradeChecks.Add(new SafeToUpgradeCheck(host));

                groups.Add(new CheckGroup(Messages.CHECKING_SAFE_TO_UPGRADE, upgradeChecks));
            }

            //Checking automated updates are possible if apply updates checkbox is ticked
            if (ApplyUpdatesToNewVersion)
            {
                var automatedUpdateChecks = new List<Check>();
                foreach (var pool in SelectedPools)
                    automatedUpdateChecks.Add(new AutomatedUpdatesLicenseCheck(pool));

                automatedUpdateChecks.Add(new CfuAvailabilityCheck());

                groups.Add(new CheckGroup(Messages.CHECKING_AUTOMATED_UPDATES_POSSIBLE,
                    automatedUpdateChecks));
            }

            return groups;
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }
        public bool ManualModeSelected { private get; set; }
    }
}
