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
using System.Reflection;
using System.Threading;
using log4net;
using XenAdmin.Core;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    class AutomaticBackgroundThread
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Thread _bw;
        private IDictionary<Host, List<PlanAction>> _planActions;
        private PlanAction _revertAction;
        private IEnumerable<Host> _mastersToUpgrade;

        public event EventHandler<ReportHostDoneArgs> ReportHostDone;
        public event EventHandler<ReportRunningArgs> ReportRunning;
        public event EventHandler<ReportExceptionArgs> ReportException;
        public event EventHandler<ReportRevertDoneArgs> ReportRevertDone;
        public event EventHandler Completed;

        public AutomaticBackgroundThread(IEnumerable<Host> mastersToUpgrade, IDictionary<Host, List<PlanAction>> planActions, PlanAction revertAction)
        {
            _planActions = planActions;
            _mastersToUpgrade = mastersToUpgrade;
            _revertAction = revertAction;
            _bw = new Thread(BworkerDoWork) { IsBackground = true };
        }

        public void Start()
        {
            _bw.Start();
        }




        private void BworkerDoWork()
        {
            Host currentHost = null;
            try
            {
                foreach (var selectedMaster in _mastersToUpgrade)
                {
                    if (_cancel)
                        return;
                    Pool pool = Helpers.GetPoolOfOne(selectedMaster.Connection);
                    foreach (var host in pool.HostsToUpgrade)
                    {
                        log.InfoFormat("Host '{0}' upgrading from version '{1}'", host.Name, host.LongProductVersion);
                        currentHost = host;
                        bool allactionsDone = true;
                        foreach (var planAction in _planActions[host])
                        {
                            try
                            {
                                string hostVersion = host.LongProductVersion;
                                ReportRunning(this, new ReportRunningArgs(planAction, host));
                                planAction.Run();
                                if (_cancel)
                                    return;
                                if (planAction is UpgradeHostPlanAction)
                                {
                                    Host hostAfterReboot = host.Connection.Resolve(new XenRef<Host>(host.opaque_ref));
                                    if (Helpers.SameServerVersion(hostAfterReboot, hostVersion))
                                    {
                                        log.ErrorFormat("Host '{0}' rebooted with the same version '{1}'", hostAfterReboot.Name, hostAfterReboot.LongProductVersion);
                                        ReportException.Raise(this,
                                                                new ReportExceptionArgs(
                                                                    new Exception(Messages.REBOOT_WITH_SAME_VERSION),
                                                                    planAction, host));
                                        allactionsDone = false;
                                        if (hostAfterReboot.IsMaster())
                                            _cancel = true;
                                    }
                                    log.InfoFormat("Host '{0}' upgraded with version '{1}'", hostAfterReboot.Name, hostAfterReboot.LongProductVersion);
                                }

                            }
                            catch (Exception e)
                            {
                                ReportException.Raise(this, new ReportExceptionArgs(e, planAction, host));
                                allactionsDone = false;
                                _cancel = true;
                            }
                            if (_cancel)
                                return;
                        }
                        if (allactionsDone)
                            ReportHostDone.Raise(this, new ReportHostDoneArgs(host));
                    }

                }
            }
            catch (Exception excep)
            {
                log.Error("Upgrade thread error: ", excep);
            }
            finally
            {
                //Revert resolved prechecks
                try
                {
                    log.Debug("Reverting prechecks");
                    ReportRunning.Raise(this, () => new ReportRunningArgs(_revertAction, currentHost));
                    _revertAction.Run();
                    ReportRevertDone.Raise(this, () => new ReportRevertDoneArgs(_revertAction));
                }
                catch (Exception excep)
                {
                    log.Error("Exception reverting prechecks", excep);
                    ReportException.Raise(this, () => new ReportExceptionArgs(excep, _revertAction, currentHost));
                }

                Completed.Raise(this);
            }


            Completed.Raise(this);

        }

        private volatile bool _cancel = false;
        internal void Cancel()
        {
            _cancel = true;
        }
    }

}
