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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Dialogs.HealthCheck;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class HealthCheckOverviewLauncher
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool healthCheckOverviewVisible;
        private HealthCheckOverviewDialog healthCheckOverviewDialog;
        private readonly IWin32Window parent;
        private DateTime LastCloseTime { get; set; }
        private readonly object healthCheckLock = new object();

        protected virtual DateTime ReferenceTime
        {
            get { return DateTime.Now; }
        }

        protected virtual TimeSpan TimeSinceLastClose
        {
            get { return ReferenceTime - LastCloseTime; }
        }

        protected virtual bool ModalDialogVisible
        {
            get { return Win32Window.ModalDialogIsVisible(); }
        }

        protected virtual DialogResult LaunchDialog(IEnumerable<IXenObject> selectedObjects)
        {
            if (healthCheckOverviewDialog == null)
                return DialogResult.None;

            return healthCheckOverviewDialog.ShowDialog(parent, selectedObjects.ToList());
        }

        protected virtual void RefreshDialog(IEnumerable<IXenObject> selectedObjects)
        {
            if (healthCheckOverviewDialog == null)
                return;

            healthCheckOverviewDialog.RefreshView(selectedObjects.ToList());
        }

        public HealthCheckOverviewLauncher(IWin32Window parent)
        {
            this.parent = parent;
            healthCheckOverviewVisible = false;
        }

        public bool HealthCheckDialogIsShowing
        {
            get { return healthCheckOverviewDialog != null; }
        }

        private void LoadHealthCheckOverviewDialog()
        {
            healthCheckOverviewDialog = new HealthCheckOverviewDialog();
        }

        public void LaunchIfRequired(bool nag, SelectedItemCollection selectedObjects)
        {
            if (selectedObjects != null && selectedObjects.AllItemsAre<IXenObject>(x => x is Pool || x is Host))
            {
                List<IXenObject> itemsSelected = selectedObjects.AsXenObjects<Pool>().ConvertAll(p => p as IXenObject);
                itemsSelected.AddRange(selectedObjects.AsXenObjects<Host>().Select
                    (host => Helpers.GetPoolOfOne(((IXenObject)host).Connection)).Cast<IXenObject>().Distinct());

                LaunchIfRequired(nag, itemsSelected);
            }
            else
                LaunchIfRequired(nag, new List<IXenObject>());
        }

        private void LaunchIfRequired(bool nag, IEnumerable<IXenObject> selectedObjects)
        {
            lock (healthCheckLock)
            {
                if (!healthCheckOverviewVisible)
                {
                    LoadHealthCheckOverviewDialog();
                    if (nag && TimeSinceLastClose < TimeSpan.FromSeconds(10))
                    {
                        // this nag was less than 10 seconds since we closed this dialog. Don't re-show.
                        return;
                    }

                    if (nag && ModalDialogVisible)
                    {
                        // if the add-server dialog is visible, then don't nag with the health check dialog as it
                        // will appear above it.
                        return;
                    }
                    healthCheckOverviewVisible = true;
                    log.Info("Health Check Overview not showing. Show it now.");

                    try
                    {
                        LaunchDialog(selectedObjects);
                    }
                    finally
                    {
                        healthCheckOverviewVisible = false;
                        LastCloseTime = ReferenceTime;
                        if(healthCheckOverviewDialog != null)
                        {
                            healthCheckOverviewDialog.Dispose();
                            healthCheckOverviewDialog = null;
                        }
                    }
                }
                else
                {
                    RefreshDialog(selectedObjects);
                }
            }
        }

        /// <summary>
        /// Call this to check the health check enrollment when a connection has been made or on periodic check.
        /// If not enrolled, the user is warned.
        /// </summary>
        /// <param name="connection">The connection to check enrollment on</param>
        internal bool CheckHealthCheckEnrollment(IXenConnection connection)
        {
            if (Helpers.FeatureForbidden(connection, Host.RestrictHealthCheck))
                return false;

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            if (pool.HealthCheckSettings.Status == HealthCheckStatus.Undefined)
            {
                Program.Invoke(Program.MainWindow, () => ShowHealthCheckOverview(pool));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shows the Health Check Overview dialog to the user
        /// </summary>
        /// <param name="selectedPool"> the pool that will be selected in the dialog</param>
        private void ShowHealthCheckOverview(Pool selectedPool)
        {
            Program.AssertOnEventThread();

            log.InfoFormat("Pool {0} not enrolled into Health Check. Show Health Check Overview if needed", selectedPool.Name);

            if (Program.RunInAutomatedTestMode)
                log.DebugFormat("In automated test mode: not showing Health Check dialog");
            else
                LaunchIfRequired(true, new List<IXenObject>() { selectedPool });
        }
    }
}
