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
            Program.Invoke(Program.MainWindow, RefreshRechecks);
        }

        public override void PageLoaded(PageLoadedDirection direction)
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
                        SelectedServers.AddRange(pool.HostsToUpgrade);
                    else
                        SelectedServers.Add(master);
                }
            }
            AddEventHandlersToMasters();
            labelPrechecksFirstLine.Text = Messages.ROLLINGUPGRADE_PRECHECKS;
            RefreshRechecks();
            return;
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            RemoveEventHandlersToMasters();
            base.PageLeave(direction, ref cancel);
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

        private static bool HostNeedsLicenseCheck(Host host)
        {
            var edition = Host.GetEdition(host.edition);
            return edition != Host.Edition.EnterpriseXD && edition != Host.Edition.XenDesktop;
        }

        protected override List<KeyValuePair<string, List<Check>>> GenerateChecks(Pool_patch patch)
        {
            List<KeyValuePair<string, List<Check>>> checks = new List<KeyValuePair<string, List<Check>>>();

            //HostMaintenanceModeCheck checks
            checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_HOST_LIVENESS_STATUS, new List<Check>()));
            List<Check> checkGroup = checks[checks.Count - 1].Value;
            foreach (Host host in SelectedServers)
            {
                checkGroup.Add(new HostMaintenanceModeCheck(host));
            }

            //HA checks
            checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_HA_STATUS, new List<Check>()));
            checkGroup = checks[checks.Count - 1].Value;
            foreach (Pool pool in SelectedPools)
            {
                Host host = pool.Connection.Resolve(pool.master);

                if(host == null) 
                    continue;

                checkGroup.Add(new HAOffCheck(host));
            }

            //Checking can evacuate host
            checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_CANEVACUATE_STATUS, new List<Check>()));
            checkGroup = checks[checks.Count - 1].Value;
            foreach (Host host in SelectedServers)
            {
                checkGroup.Add(new AssertCanEvacuateUpgradeCheck(host));
            }

            //PBDsPluggedCheck
            checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_STORAGE_CONNECTIONS_STATUS, new List<Check>()));
            checkGroup = checks[checks.Count - 1].Value;
            foreach (Host host in SelectedServers)
            {
                checkGroup.Add(new PBDsPluggedCheck(host));
            }

           
            //HotfixesCheck required for MNR, Cowley, Boston and Sanibel
            bool titleAdded = false;
            foreach (var host in SelectedServers)
            {
                if (new HotfixFactory().IsHotfixRequired(host) && !ManualUpgrade)
                {
                    if (!titleAdded)
                    {
                        checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_UPGRADE_HOTFIX_STATUS,
                                                                         new List<Check>()));
                        checkGroup = checks[checks.Count - 1].Value;
                        titleAdded = true;
                    }
                    checkGroup.Add(new HostHasHotfixCheck(host));
                }
            }

            //iSL (StorageLink) check - CA-223486: only for pre-Creedence
            var preCreedenceServers = SelectedServers.Where(h => !Helpers.CreedenceOrGreater(h)).ToList();
            if (preCreedenceServers.Any())
            {
                checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_STORAGELINK_STATUS, new List<Check>()));
                checkGroup = checks[checks.Count - 1].Value;
                foreach (Host host in preCreedenceServers)
                {
                    checkGroup.Add(new HostHasUnsupportedStorageLinkSRCheck(host));
                }
            }

            //Upgrading to Clearwater and above - license changes warning and deprecations
            var preClearwaterServers = SelectedServers.Where(h => !Helpers.ClearwaterOrGreater(h)).ToList();
            if(preClearwaterServers.Any())
            {
                var hostsNeedingLicenseCheck = preClearwaterServers.Where(HostNeedsLicenseCheck).ToList();
                if (hostsNeedingLicenseCheck.Any())
                {
                    checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_LICENSING_STATUS,
                        new List<Check>()));
                    checkGroup = checks[checks.Count - 1].Value;
                    foreach (Host host in hostsNeedingLicenseCheck)
                    {
                        checkGroup.Add(new UpgradingFromTampaAndOlderCheck(host));
                    }
                }

                //WSS removal
                checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_WSS_STATUS, new List<Check>()));
                checkGroup = checks[checks.Count - 1].Value;
                foreach (Host host in preClearwaterServers)
                {
                    checkGroup.Add(new HostHasWssCheck(host));
                }

                //VMP[RP]removal
                checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_VMPR_STATUS, new List<Check>()));
                checkGroup = checks[checks.Count - 1].Value;
                foreach (Host host in preClearwaterServers)
                {
                    checkGroup.Add(new VmprActivatedCheck(host));
                }
            }

            //SafeToUpgradeCheck - in automatic mode only
            if (!ManualUpgrade)
            {
                checks.Add(new KeyValuePair<string, List<Check>>(Messages.CHECKING_SAFE_TO_UPGRADE, new List<Check>()));
                checkGroup = checks[checks.Count - 1].Value;
 
                foreach (Host host in SelectedServers)
                {
                    checkGroup.Add(new SafeToUpgradeCheck(host));
                }
            }

            return checks;
            
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }
        public bool ManualModeSelected { private get; set; }
    }
}
