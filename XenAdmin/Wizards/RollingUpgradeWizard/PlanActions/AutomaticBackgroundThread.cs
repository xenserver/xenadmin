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
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    class AutomaticBackgroundThread : BackgroundThreadBase
    {
        public AutomaticBackgroundThread(IEnumerable<Host> mastersToUpgrade, IDictionary<Host, List<PlanAction>> planActions, PlanAction revertAction)
        {
            _planActions = planActions;
            _mastersToUpgrade = mastersToUpgrade;
            _revertAction = revertAction;
            _bw = new Thread(BworkerDoWork) { IsBackground = true };
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
                                OnReportRunning(planAction, host);
                                
                                planAction.Run();
                                
                                if (_cancel)
                                    return;
                                
                                if (planAction is UpgradeHostPlanAction)
                                {
                                    Host hostAfterReboot = (planAction as UpgradeHostPlanAction).Host;
                                    if (hostAfterReboot != null && Helpers.SameServerVersion(hostAfterReboot, hostVersion))
                                    {
                                        log.ErrorFormat("Host '{0}' rebooted with the same version '{1}'", hostAfterReboot.Name, hostAfterReboot.LongProductVersion);
                                        OnReportException(new Exception(Messages.REBOOT_WITH_SAME_VERSION),
                                                          planAction, host);
                                        allactionsDone = false;
                                        if (hostAfterReboot.IsMaster())
                                            _cancel = true;
                                    }
                                    if (hostAfterReboot != null)
                                        log.InfoFormat("Host '{0}' upgraded with version '{1}'", hostAfterReboot.Name, hostAfterReboot.LongProductVersion);
                                    else
                                        log.InfoFormat("Cannot check host's version after reboot because the host '{0}' cannot be resolved", host.Name);
                                   
                                }
                            }
                            catch (Exception e)
                            {
                                OnReportException(e, planAction, host);
                                allactionsDone = false;
                                _cancel = true;
                            }

                            if (_cancel)
                                return;
                        }

                        if (allactionsDone)
                            OnReportHostDone(host);
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
                    OnReportRunning(_revertAction, currentHost);
                    _revertAction.Run();

                    OnReportRevertDone();
                }
                catch (Exception excep)
                {
                    log.Error("Exception reverting prechecks", excep);
                    OnReportException(excep, _revertAction, currentHost);
                }

                OnCompleted();
            }
        }
    }
}
