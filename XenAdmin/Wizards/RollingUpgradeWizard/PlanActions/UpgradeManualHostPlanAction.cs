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

using XenAPI;
using System;
using System.Timers;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using Timer = System.Timers.Timer;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    public abstract class UpgradeHostPlanAction : RebootPlanAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected bool rebooting;
        private readonly Timer timer;
        protected readonly Control invokingControl;
        private Version _upgradeVersion;
        
        protected UpgradeHostPlanAction(Host host, Control invokingControl)
            : base(host)
        {
            timer = new Timer { Interval = 20 * 60000, AutoReset = true };
            timer.Elapsed += timer_Elapsed;
            this.invokingControl = invokingControl;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!rebooting)
                return;

            ReplaceProgressStep(_upgradeVersion == null
                ? string.Format(Messages.ROLLING_UPGRADE_TIMEOUT, BrandManager.ProductBrand, CurrentHost.Name())
                : string.Format(Messages.ROLLING_UPGRADE_TIMEOUT_VERSION, BrandManager.ProductBrand, _upgradeVersion, CurrentHost.Name()));
        }

        protected void Upgrade(ref Session session, string upgradeVersion = null)
        {
            try
            {
                var hostObj = GetResolvedHost();

                if (hostObj.enabled)
                {
                    log.DebugFormat("Disabling host {0}", hostObj.Name());
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_ENTERING_MAINTENANCE_MODE, hostObj.Name()));
                    Host.disable(session, HostXenRef.opaque_ref);
                }

                Version.TryParse(upgradeVersion, out _upgradeVersion);

                timer.Start();
                rebooting = true;

                log.DebugFormat("Upgrading host {0}", hostObj.Name());
                AddProgressStep(_upgradeVersion == null
                    ? string.Format(Messages.PLAN_ACTION_STATUS_INSTALLING_XENSERVER, BrandManager.ProductBrand, hostObj.Name())
                    : string.Format(Messages.PLAN_ACTION_STATUS_INSTALLING_XENSERVER_VERSION, BrandManager.ProductBrand, _upgradeVersion, hostObj.Name()));

                log.DebugFormat("Waiting for host {0} to reboot", hostObj.Name());
                WaitForReboot(ref session, Host.BootTime, s => Host.async_reboot(s, HostXenRef.opaque_ref));

                AddProgressStep(Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE);
                foreach (var host in Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs(); // Wait for PBDs to become plugged on all hosts

                rebooting = false;
                log.DebugFormat("Host {0} rebooted", hostObj.Name());
            }
            finally
            {
                Connection.ExpectDisruption = false;
                timer.Stop();
            }
        }
    }

    public class UpgradeManualHostPlanAction : UpgradeHostPlanAction
    {
        public UpgradeManualHostPlanAction(Host host, Control invokingControl)
            : base(host, invokingControl)
        {
        }

        protected override void RunWithSession(ref Session session)
        {
            //Show dialog prepare host boot from CD or PXE boot and click OK to reboot

            Program.Invoke(invokingControl, () =>
            {
                using (var dialog = new InformationDialog(string.Format(Messages.ROLLING_UPGRADE_REBOOT_MESSAGE,
                            BrandManager.ProductBrand, GetResolvedHost().Name()),
                    new ThreeButtonDialog.TBDButton(Messages.REBOOT, DialogResult.OK),
                    new ThreeButtonDialog.TBDButton(Messages.SKIP_SERVER, DialogResult.Cancel))
                    {WindowTitle = Messages.ROLLING_POOL_UPGRADE})
                {
                    if (dialog.ShowDialog(invokingControl) != DialogResult.OK) // Cancel or Unknown
                    {
                        if (GetResolvedHost().IsCoordinator())
                        {
                            Error = new ApplicationException(Messages.EXCEPTION_USER_CANCELLED_COORDINATOR);
                            throw Error;
                        }

                        Error = new CancelledException();
                    }
                }
            });

            if (Error != null)
                return;

            var hostObj = GetResolvedHost();
            string beforeRebootProductVersion = hostObj.LongProductVersion();
            string hostName = hostObj.Name();

            do
            {
                if (Cancelling)
                    break;

                Upgrade(ref session);

                hostObj = GetResolvedHost();
                if (Helpers.SameServerVersion(hostObj, beforeRebootProductVersion))
                {
                    var obj = hostObj;
                    Program.Invoke(invokingControl, () =>
                    {
                        using (var dialog = new WarningDialog(string.Format(Messages.ROLLING_UPGRADE_REBOOT_AGAIN_MESSAGE, hostName),
                            new ThreeButtonDialog.TBDButton(Messages.REBOOT_AGAIN_BUTTON_LABEL, DialogResult.OK),
                            new ThreeButtonDialog.TBDButton(Messages.SKIP_SERVER, DialogResult.Cancel))
                            {WindowTitle = Messages.ROLLING_POOL_UPGRADE})
                        {
                            if (dialog.ShowDialog(invokingControl) == DialogResult.OK)
                                return;
                            
                            if (obj.IsCoordinator())
                            {
                                Error = new Exception(Messages.HOST_REBOOTED_SAME_VERSION);
                                throw Error;
                            }
                            Error = new CancelledException();
                            Cancel();
                        }
                    });
                }
                else
                {
                    break;
                }
            } while (true);
        }
    }
}
