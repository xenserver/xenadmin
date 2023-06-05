/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;
using System.Linq;
using XenAdmin.Network;
using CheckGroup = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<XenAdmin.Diagnostics.Checks.Check>>;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardPrecheckPage : PatchingWizard_PrecheckPage
    {
        public RollingUpgradeWizardPrecheckPage()
        {
            InitializeComponent();
            ManualUpgrade = true;
        }

        private void AddEventHandlersToCoordinators()
        {
            foreach (var c in SelectedCoordinators.Select(c => c.Connection))
            {
                c.ConnectionStateChanged += connection_ConnectionChanged;
                c.CachePopulated += connection_CachePopulated;
            }
        }

        private void RemoveEventHandlersToCoordinators()
        {
            foreach (var c in SelectedCoordinators.Select(c => c.Connection))
            {
                c.ConnectionStateChanged -= connection_ConnectionChanged;
                c.CachePopulated -= connection_CachePopulated;
            }
        }

        private void connection_ConnectionChanged(IXenConnection conn)
        {
            Program.Invoke(this, RefreshRechecks);
        }

        private void connection_CachePopulated(IXenConnection conn)
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
            var selectedCoordinators = new List<Host>(SelectedCoordinators);
            RemoveEventHandlersToCoordinators();
            SelectedServers.Clear();
            foreach (var selectedCoordinator in selectedCoordinators)
            {
                if (selectedCoordinator != null)
                {
                    var pool = Helpers.GetPoolOfOne(selectedCoordinator.Connection);
                    if (pool != null)
                        SelectedServers.AddRange(pool.HostsToUpgrade());
                    else
                        SelectedServers.Add(selectedCoordinator);
                }
            }
            AddEventHandlersToCoordinators();
            labelPrechecksFirstLine.Text = Messages.ROLLINGUPGRADE_PRECHECKS;
            RefreshRechecks();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            RemoveEventHandlersToCoordinators();
        }

        public override void PageCancelled(ref bool cancel)
        {
            base.PageCancelled(ref cancel);
            if (cancel)
                return;
            RemoveEventHandlersToCoordinators();
        }

        public override string PageTitle => Messages.UPGRADE_PRECHECKS_TITLE;

        public override string Text => Messages.UPGRADE_PRECHECKS_TEXT;

        public override string HelpID => "Upgradeprechecks";

        public override string NextText(bool isLastPage)
        {
            return Messages.START_UPGRADE;
        }

        protected override List<CheckGroup> GenerateChecks(Pool_patch patch)
        {
            var groups = new List<CheckGroup>();

            var hostsToUpgrade = new List<Host>();
            var hostsToUpgradeOrUpdate = new List<Host>();
            foreach (var pool in SelectedPools)
            {
                var poolHostsToUpgrade = pool.HostsToUpgrade();
                hostsToUpgrade.AddRange(poolHostsToUpgrade);
                hostsToUpgradeOrUpdate.AddRange(poolHostsToUpgrade);
            }

            //XenCenter version check (if any of the selected server version is not the latest)
            var latestCrVersion = Updates.XenServerVersions.FindAll(item => item.LatestCr).OrderByDescending(v => v.Version).FirstOrDefault();
            if (latestCrVersion != null &&
                hostsToUpgradeOrUpdate.Any(host => new Version(Helpers.HostProductVersion(host)) < latestCrVersion.Version))
            {
                groups.Add(new CheckGroup(string.Format(Messages.CHECKING_XENCENTER_VERSION, BrandManager.BrandConsole),
                    new List<Check> {new ClientVersionCheck(null)}));
            }

            //HostMaintenanceModeCheck checks - for hosts that will be upgraded or updated
            var livenessChecks = new List<Check>();
            foreach (var host in hostsToUpgradeOrUpdate)
                livenessChecks.Add(new HostLivenessCheck(host, hostsToUpgrade.Contains(host)));
            groups.Add(new CheckGroup(Messages.CHECKING_HOST_LIVENESS_STATUS, livenessChecks));

            //HotfixesCheck - for hosts that will be upgraded
            var hotfixChecks = new List<Check>();
            foreach (var host in hostsToUpgrade)
            {
                if (HotfixFactory.IsHotfixRequired(host) && !ManualUpgrade)
                    hotfixChecks.Add(new HostHasHotfixCheck(host));
            }
            if (hotfixChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_UPGRADE_HOTFIX_STATUS, hotfixChecks));

            if (!ManualUpgrade)
            {
                // EUA check
                var euaCheck = GetPermanentCheck("EUA",
                new UpgradeRequiresEua(this, hostsToUpgrade, InstallMethodConfig));
                var euaChecks = new List<Check> { euaCheck };
                groups.Add(new CheckGroup(Messages.ACCEPT_EUA_CHECK_GROUP_NAME, euaChecks));
            } 

            //SafeToUpgrade- and PrepareToUpgrade- checks - in automatic mode only, for hosts that will be upgraded
            if (!ManualUpgrade)
            {
                var safeToUpgradeChecks = (from Host host in hostsToUpgrade
                    let check = new SafeToUpgradeCheck(host, InstallMethodConfig)
                    where check.CanRun()
                    select check as Check).ToList();

                if (safeToUpgradeChecks.Count > 0)
                    groups.Add(new CheckGroup(Messages.CHECKING_SAFE_TO_UPGRADE, safeToUpgradeChecks));

                var prepareToUpgradeChecks = (from Host host in hostsToUpgrade
                    select new PrepareToUpgradeCheck(host, InstallMethodConfig) as Check).ToList();

                if (prepareToUpgradeChecks.Count > 0)
                    groups.Add(new CheckGroup(Messages.CHECKING_PREPARE_TO_UPGRADE, prepareToUpgradeChecks));
            }

            //vSwitch controller check - for each pool
            var vSwitchChecks = (from Host server in SelectedCoordinators
                let check = new VSwitchControllerCheck(server, InstallMethodConfig, ManualUpgrade)
                where check.CanRun()
                select check as Check).ToList();

            if (vSwitchChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_VSWITCH_CONTROLLER_GROUP, vSwitchChecks));

            //Health Check check - for each pool
            var hcChecks = (from Pool pool in SelectedPools
                let check = new HealthCheckServiceCheck(pool, InstallMethodConfig)
                where check.CanRun()
                select check as Check).ToList();

            if (hcChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_HEALTH_CHECK_SERVICE, hcChecks));

            //protocol check - for each pool
            var sslChecks = (from Host server in SelectedCoordinators
                let check = new PoolLegacySslCheck(server, InstallMethodConfig, ManualUpgrade)
                where check.CanRun()
                select check as Check).ToList();

            if (sslChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_SECURITY_PROTOCOL_GROUP, sslChecks));

            //certificate key length - for each host
            var certKeyLengthChecks = (from Host server in hostsToUpgradeOrUpdate
                let check = new CertificateKeyLengthCheck(server, ManualUpgrade, InstallMethodConfig)
                where check.CanRun()
                select check as Check).ToList();

            if (certKeyLengthChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CERTIFICATE_KEY_LENGTH_CHECK_GROUP, certKeyLengthChecks));

            //power on mode check - for each host
            var iloChecks = (from Host server in hostsToUpgradeOrUpdate
                let check = new PowerOniLoCheck(server, InstallMethodConfig, ManualUpgrade)
                where check.CanRun()
                select check as Check).ToList();

            if (iloChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_POWER_ON_MODE_GROUP, iloChecks));

            //Checking DMC
            var dmcChecks = (from Pool pool in SelectedPools
                let check = new DmcCheck(this, pool, InstallMethodConfig, ManualUpgrade)
                where check.CanRun()
                select check as Check).ToList();

            if (dmcChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.DMC_CHECK_ENABLED, dmcChecks));

            //Checking PV guests - for hosts that have any PV guests and warn the user before the upgrade.
            var pvChecks = (from Host server in SelectedCoordinators
                let check = new PVGuestsCheck(server, ManualUpgrade, InstallMethodConfig)
                where check.CanRun()
                select check as Check).ToList();

            if (pvChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_PV_GUESTS, pvChecks));

            //HA checks - for each pool
            var haChecks = (from Host server in SelectedCoordinators
                select new HAOffCheck(server) as Check).ToList();


            if (haChecks.Count > 0) 
                groups.Add(new CheckGroup(Messages.CHECKING_HA_STATUS, haChecks));

            //Checking can evacuate host - for hosts that will be upgraded or updated
            var evacuateChecks = (from Host host in hostsToUpgradeOrUpdate
                select new AssertCanEvacuateUpgradeCheck(host) as Check).ToList();

            if (evacuateChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_CANEVACUATE_STATUS, evacuateChecks));

            //PBDsPluggedCheck -  for hosts that will be upgraded or updated
            var pbdChecks = (from Host host in hostsToUpgradeOrUpdate
                select new PBDsPluggedCheck(host) as Check).ToList();

            if(pbdChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_STORAGE_CONNECTIONS_STATUS, pbdChecks));

            //HostMemoryPostUpgradeCheck - for hosts that will be upgraded
            var mostMemoryPostUpgradeChecks = (from Host host in hostsToUpgrade
                select new HostMemoryPostUpgradeCheck(host, InstallMethodConfig) as Check).ToList();

            if (mostMemoryPostUpgradeChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_HOST_MEMORY_POST_UPGRADE, mostMemoryPostUpgradeChecks));

            //PoolHasGFS2SR checks 
            var gfs2Checks = (from Host host in SelectedCoordinators
                let check = new PoolHasGFS2SR(host)
                where check.CanRun()
                select check as Check).ToList();

            if (gfs2Checks.Count > 0) 
                groups.Add(new CheckGroup(Messages.CHECKING_CLUSTERING_STATUS, gfs2Checks));

            //Deprecated SRs checks 
            var deprecatedSRsChecks = (from Host host in SelectedCoordinators
                let check = new PoolHasDeprecatedSrsCheck(host, InstallMethodConfig, ManualUpgrade)
                where check.CanRun()
                select check as Check).ToList();

            if (deprecatedSRsChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_DEPRECATED_SRS, deprecatedSRsChecks));

            return groups;
        }

        public IEnumerable<Host> SelectedCoordinators { private get; set; }
        public bool ManualUpgrade { set; private get; }

        public Dictionary<string, string> InstallMethodConfig { private get; set; }

        #region private methods
        public static List<Host> HostsToUpgradeOrUpdate(Pool pool)
        {
            var result = new List<Host>();

            if (pool == null)
                return result;

            var coordinator = Helpers.GetCoordinator(pool);
            if (coordinator == null)
                return result;

            if (pool.IsCoordinatorUpgraded())
            {
                foreach (var h in pool.Connection.Cache.Hosts)
                {
                    if (h.LongProductVersion() != coordinator.LongProductVersion()) // host needs to be upgraded
                        result.Add(h); // host 
                    else
                    {
                        //check update sequence for already-upgraded hosts
                        var us = Updates.GetPatchSequenceForHost(h, Updates.GetMinimalPatches(coordinator));
                        if (us != null && us.Count > 0)
                        {
                            result.Add(h);
                        }
                    }
                }
            }
            else
                result.AddRange(pool.Connection.Cache.Hosts);

            result.Sort();

            return result;
        }
        #endregion

    }
}
