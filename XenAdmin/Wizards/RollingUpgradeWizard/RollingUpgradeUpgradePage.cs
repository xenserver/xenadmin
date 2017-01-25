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

using System.Reflection;
using log4net;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Properties;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAPI;
using System.Collections.Generic;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using System;
using XenAdmin.Dialogs;
using System.Drawing;
using System.Linq;

using XenAdmin.Wizards.RollingUpgradeWizard.PlanActions;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeUpgradePage : XenTabPage
    {
        #region Private fields
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Bitmap animatedImage = Resources.ajax_loader;

        private Dictionary<Host, List<PlanAction>> planActions = new Dictionary<Host, List<PlanAction>>();
        private UnwindProblemsAction revertAction = null;
        private SemiAutomaticBackgroundThread bworker;
        private AutomaticBackgroundThread _workerAutomaticUpgrade;
        #endregion

        public RollingUpgradeUpgradePage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string PageTitle { get { return Messages.UPGRADE_PLAN; } }

        public override string Text { get { return Messages.UPGRADE_PLAN; } }

        public override string HelpID { get { return "Upgradepools"; } }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            UnregisterAllStatusUpdateActions();
            ImageAnimator.StopAnimate(animatedImage, onFrameChanged);
            if (direction == PageLoadedDirection.Back)
            {
                planActions.Clear();
            }
            base.PageLeave(direction, ref cancel);
        }

        public override void PageCancelled()
        {
            UnregisterAllStatusUpdateActions();
            UpgradeStatus = RollingUpgradeStatus.Cancelled;
            if (bworker != null)
                bworker.Cancel();
            if (_workerAutomaticUpgrade != null)
                _workerAutomaticUpgrade.Cancel();
        }

        public override bool EnableNext()
        {
            return UpgradeStatus == RollingUpgradeStatus.Completed;
        }

        public override bool EnablePrevious()
        {
            return false;
        }

        public override bool EnableCancel()
        {
            return UpgradeStatus == RollingUpgradeStatus.NotStarted || UpgradeStatus == RollingUpgradeStatus.Started;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            UpgradeStatus = RollingUpgradeStatus.NotStarted;
            ImageAnimator.Animate(animatedImage, onFrameChanged);
            if (direction == PageLoadedDirection.Forward && planActions.Count > 0)
                return;

            OnPageUpdated();

            dataGridView1.Rows.Clear();
            UnregisterAllStatusUpdateActions();
            planActions.Clear();

            //Add masters first
            var hostNeedUpgrade = new List<Host>();
            //Add rest of slaves not ugpraded
            foreach (var host in SelectedMasters)
            {
                Pool pool = Helpers.GetPoolOfOne(host.Connection);
                if (pool != null)
                    hostNeedUpgrade.AddRange(pool.HostsToUpgrade);
                else
                    hostNeedUpgrade.Add(host);
            }

            foreach (var host in hostNeedUpgrade)
            {
                planActions.Add(host, GetSubTasksFor(host));
                dataGridView1.Rows.Add(new DataGridViewRowUpgrade(host));
            }

            //Revert precheck changes
            revertAction = new UnwindProblemsAction(ProblemsResolvedPreCheck);
            dataGridView1.Rows.Add(new DataGridViewRowUpgrade(revertAction));

            labelOverallProgress.Text = string.Format(Messages.OVERALL_PROGRESS, 0, dataGridView1.Rows.Count - 1);
            RegisterStatusUpdateActions();
            StartUpgrade();
        }

        #endregion

        #region Accessors

        public ThreeButtonDialog Dialog { get; private set; }
        public IEnumerable<Host> SelectedMasters { private get; set; }
        public bool ManualModeSelected { private get; set; }
        public Dictionary<string, string> InstallMethodConfig { private get; set; }
        public List<Problem> ProblemsResolvedPreCheck { private get; set; }
        public RollingUpgradeStatus UpgradeStatus { get; private set; }

        #endregion

        private void onFrameChanged(object sender, EventArgs e)
        {
            try
            {
                ImageAnimator.UpdateFrames();
                Program.BeginInvoke(dataGridView1, () => dataGridView1.InvalidateColumn(1));
            }
            catch (Exception)
            {
            }
        }

        private void StartUpgrade()
        {
            if (ManualModeSelected)
            {
                StartSemiAutomaticUpgrade();
            }
            else
            {
                StartAutomaticUpgrade();
            }
        }

        private string completedTitleLabel = Messages.ROLLING_UPGRADE_UPGRADE_COMPLETED;

        private void StartAutomaticUpgrade()
        {
            _workerAutomaticUpgrade = new AutomaticBackgroundThread(SelectedMasters, planActions, revertAction);
            _workerAutomaticUpgrade.ReportRunning += ReportRunning;
            _workerAutomaticUpgrade.ReportException += ReportException;
            _workerAutomaticUpgrade.ReportHostDone += ReportHostDone;
            _workerAutomaticUpgrade.ReportRevertDone += ReportRevertDone;
            _workerAutomaticUpgrade.Completed += Completed;
            _workerAutomaticUpgrade.Start();

            UpgradeStatus = RollingUpgradeStatus.Started;
        }

        private void StartSemiAutomaticUpgrade()
        {
            bworker = new SemiAutomaticBackgroundThread(SelectedMasters, planActions, revertAction);
            bworker.ManageSemiAutomaticPlanAction += ManageSemiAutomaticPlanAction;
            bworker.ReportRunning += ReportRunning;
            bworker.ReportException += ReportException;
            bworker.ReportHostDone += ReportHostDone;
            bworker.ReportRevertDone += ReportRevertDone;
            bworker.Completed += Completed;
            bworker.Start();

            UpgradeStatus = RollingUpgradeStatus.Started;
        }

        private void ReportRevertDone()
        {
            Program.BeginInvoke(this, () =>
                                          {
                                              var row = (DataGridViewRowUpgrade)dataGridView1.Rows[dataGridView1.Rows.Count - 1];
                                              row.UpdateStatus(HostUpgradeState.Upgraded, Messages.COMPLETED);
                                          });
        }

        private void Completed()
        {
            Program.BeginInvoke(this, () =>
                                          {
                                              progressBar1.Value = 100;
                                              UpgradeStatus = RollingUpgradeStatus.Completed;
                                              labelTitle.Text = completedTitleLabel;
                                              OnPageUpdated();
                                          });
        }

        private void ReportRunning(PlanAction planAction, Host host)
        {
            Program.BeginInvoke(this, () =>
                                          {
                                              progressBar1.Value = progressBar1.Value < 100
                                                                       ? progressBar1.Value + 2
                                                                       : progressBar1.Value;
                                              var row = planAction is UnwindProblemsAction
                                                        ? FindRow(null)
                                                        : FindRow(host);
                                              if (row != null)
                                                  row.UpdateStatus(HostUpgradeState.Upgrading, planAction.TitlePlan);
                                          });
        }

        private void ManageSemiAutomaticPlanAction(UpgradeManualHostPlanAction planAction)
        {
            if (UpgradeStatus == RollingUpgradeStatus.Cancelled)
                return;

            var upgradeHostPlanAction = planAction;
            //Show dialog prepare host boot from CD or PXE boot and click OK to reboot
            string msg = string.Format(Messages.ROLLING_UPGRADE_REBOOT_MESSAGE, planAction.Host.Name);

            UpgradeManualHostPlanAction action = upgradeHostPlanAction;

            Program.Invoke(this, () =>
            {
                using (Dialog = new NotModalThreeButtonDialog(SystemIcons.Information, msg, Messages.REBOOT, Messages.SKIP_SERVER))
                {
                    Dialog.ShowDialog(this);

                    if (Dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
                    {
                        completedTitleLabel = Messages.ROLLING_UPGRADE_UPGRADE_NOT_COMPLETED;
                        if(action.Host.IsMaster())
                            throw new ApplicationException(Messages.EXCEPTION_USER_CANCELLED_MASTER);
                        
                        throw new ApplicationException(Messages.EXCEPTION_USER_CANCELLED);
                    }
                }
            });
            string beforeRebootProductVersion = upgradeHostPlanAction.Host.LongProductVersion;
            string hostName = upgradeHostPlanAction.Host.Name;
            upgradeHostPlanAction.Timeout += new EventHandler(upgradeHostPlanAction_Timeout);
            try
            {
                do
                {
                    if (UpgradeStatus == RollingUpgradeStatus.Cancelled)
                        break;

                    //Reboot with timeout of 20 min
                    upgradeHostPlanAction.Run();

                    //if comes back and does not have a different product version
                    if (Helpers.SameServerVersion(upgradeHostPlanAction.Host, beforeRebootProductVersion))
                    {
                        using (var dialog = new NotModalThreeButtonDialog(SystemIcons.Exclamation,
                            string.Format(Messages.ROLLING_UPGRADE_REBOOT_AGAIN_MESSAGE, hostName)
                            , Messages.REBOOT_AGAIN_BUTTON_LABEL, Messages.SKIP_SERVER))
                        {
                            Program.Invoke(this, () => dialog.ShowDialog(this));
                            if (dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
                                throw new Exception(Messages.HOST_REBOOTED_SAME_VERSION);
                            else
                                upgradeHostPlanAction = new UpgradeManualHostPlanAction(upgradeHostPlanAction.Host);
                        }
                    }

                } while (Helpers.SameServerVersion(upgradeHostPlanAction.Host, beforeRebootProductVersion));
            }
            finally
            {
                upgradeHostPlanAction.Timeout -= new EventHandler(upgradeHostPlanAction_Timeout);
            }
        }

        private void upgradeHostPlanAction_Timeout(object sender, EventArgs e)
        {
            var dialog = new NotModalThreeButtonDialog(SystemIcons.Exclamation, Messages.ROLLING_UPGRADE_TIMEOUT.Replace("\\n", "\n"), Messages.KEEP_WAITING_BUTTON_LABEL.Replace("\\n", "\n"), Messages.CANCEL);
            Program.Invoke(this, () => dialog.ShowDialog(this));
            if (dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
            {
                UpgradeHostPlanAction action = (UpgradeHostPlanAction)sender;
                action.Cancel();
            }
        }

        private void ReportException(Exception exception, PlanAction planAction, Host host)
        {
            Program.Invoke(this, () =>
                                    {
                                        if (host != null && !host.enabled && host.Connection != null && host.Connection.Session != null)
                                            try
                                            {
                                                new EnableHostAction(host, false,
                                                                AddHostToPoolCommand.EnableNtolDialog).RunExternal(host.Connection.Session);
                                            }
                                            catch (Exception e)
                                            {
                                                log.Error("Exception while trying to re-enable the host", e);
                                            }
                                    });
            Program.BeginInvoke(this, () =>
                                          {
                                              var row = planAction is UnwindProblemsAction
                                                        ? FindRow(null)
                                                        : FindRow(host);

                                              row.UpdateStatus(HostUpgradeState.Error, exception.Message);
                                              
                                              UpgradeProgress(row.Index + 1);

                                          });


        }

        private void ReportHostDone(Host host)
        {
            Program.BeginInvoke(this, () =>
                                          {
                                              var row = FindRow(host);
                                              row.UpdateStatus(HostUpgradeState.Upgraded, Messages.COMPLETED);

                                              labelOverallProgress.Text = string.Format(Messages.OVERALL_PROGRESS, row.Index + 1, dataGridView1.Rows.Count - 1);
                                              UpgradeProgress(row.Index + 1);
                                          });
        }

        private void UpgradeProgress(int numberDone)
        {
            int progressValue = (int)(((double)(numberDone) / (dataGridView1.Rows.Count - 1)) * 100);
            progressBar1.Value = progressValue >= 100 ? 90 : progressValue;
        }

        private DataGridViewRowUpgrade FindRow(Host host)
        {
            foreach (DataGridViewRowUpgrade row in dataGridView1.Rows)
            {
                if (host == null && row.Host == null)
                    return row;
                if (host != null && row.Host != null && host.Equals(row.Host))
                    return row;
            }
            throw new Exception("Row not found");
        }

        private void RegisterStatusUpdateActions()
        {
            if (planActions == null)
                return;

            foreach (KeyValuePair<Host, List<PlanAction>> kvp in planActions)
            {
                foreach (PlanAction planAction in kvp.Value)
                {
                    planAction.StatusChanged += planAction_StatusChanged;
                }
            }
        }

        private void UnregisterAllStatusUpdateActions()
        {
            if (planActions == null)
                return;

            foreach (KeyValuePair<Host, List<PlanAction>> kvp in planActions)
            {
                foreach (PlanAction planAction in kvp.Value)
                {
                    planAction.StatusChanged -= planAction_StatusChanged;
                }  
            }
            
        }

        private void planAction_StatusChanged(PlanAction plan, Host senderHost)
        {
            if(senderHost == null || plan == null)
                return;

            Program.Invoke(Program.MainWindow, () =>
            {
                List<DataGridViewRowUpgrade> rowsForHost = (from DataGridViewRowUpgrade row in dataGridView1.Rows
                                                           where row.RowIsForHost(senderHost)
                                                           select row).ToList();

                foreach (DataGridViewRowUpgrade row in rowsForHost)
                {
                    row.UpdateStatus(HostUpgradeState.Upgrading, plan.Status);
                }
            });
        }

        private List<PlanAction> GetSubTasksFor(Host host)
        {
            List<XenRef<VM>> runningVMs = RunningVMs(host);
            if (ManualModeSelected)
                return new List<PlanAction>
                           {
                               new EvacuateHostPlanAction(host),
                               new UpgradeManualHostPlanAction(host),
                               new BringBabiesBackAction(runningVMs, host, true)
                           };
            return new List<PlanAction>
                       {
                           new EvacuateHostPlanAction(host),
                           new UpgradeHostPlanAction(host, InstallMethodConfig),
                           new BringBabiesBackAction(runningVMs, host, true)
                       };
        }

        private static List<XenRef<VM>> RunningVMs(Host host)
        {
            var vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.is_a_real_vm)
                    continue;

                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.ViewLogFiles();
        }

        #region Nested classes

        private class NotModalThreeButtonDialog : ThreeButtonDialog
        {
            public NotModalThreeButtonDialog(Icon icon, string msg, string button1Text, string button2Text)
                : base(new Details(icon, msg),
                    new TBDButton(button1Text, DialogResult.OK),
                    new TBDButton(button2Text, DialogResult.Cancel))
            {
                Buttons[0].Click += new EventHandler(button1_Click);
                Buttons[1].Click += new EventHandler(button2_Click);
            }

            void button1_Click(object sender, EventArgs e)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            void button2_Click(object sender, EventArgs e)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private class DataGridViewRowUpgrade : DataGridViewRow
        {
            public readonly Host Host;
            private DataGridViewImageCell imageCell = new DataGridViewImageCell();
            private DataGridViewTextBoxCell taskCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell statusCell = new DataGridViewTextBoxCell();

            private DataGridViewRowUpgrade()
            {
                this.Cells.Add(taskCell);
                this.Cells.Add(imageCell);
                this.Cells.Add(statusCell);
            }
            public DataGridViewRowUpgrade(Host host)
                : this()
            {
                Host = host;
                taskCell.Value = string.Format(Host.IsMaster() ? Messages.UPGRADE_POOL_MASTER : Messages.UPGRADE_SLAVE, host.Name);
                UpdateStatus(HostUpgradeState.NotUpgraded, Messages.NOT_UPGRADED);

            }

            public readonly UnwindProblemsAction RevertAction;
            public DataGridViewRowUpgrade(UnwindProblemsAction action)
                : this()
            {
                RevertAction = action;
                taskCell.Value = Messages.REVERT_PRECHECK_ACTIONS;
                UpdateStatus(HostUpgradeState.NotUpgraded, Messages.NOT_UPGRADED);
            }

            public bool RowIsForHost(Host host)
            {
                return host == Host;
            }

            public void UpdateStatus(HostUpgradeState state, string value)
            {
                try 
                { 
                    switch (state)
                    {
                        case HostUpgradeState.Upgraded:
                            imageCell.Value = Resources._000_Tick_h32bit_16;
                            break;

                        case HostUpgradeState.Upgrading:
                            imageCell.Value = animatedImage;
                            break;

                        case HostUpgradeState.Error:
                            imageCell.Value = Resources._000_Abort_h32bit_16;
                            break;

                        case HostUpgradeState.NotUpgraded:
                            imageCell.Value = new Bitmap(1, 1);
                            break;
                    }
                }
                catch (Exception) { }
                statusCell.Value = value;
            }
        }

        private enum HostUpgradeState
        {
            NotUpgraded,
            Upgraded,
            Upgrading,
            Error
        }

        #endregion
    }
}
