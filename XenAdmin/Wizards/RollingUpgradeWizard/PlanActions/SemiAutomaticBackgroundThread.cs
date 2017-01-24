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
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    class SemiAutomaticBackgroundThread : BackgroundThreadBase
    {
        public event Action<UpgradeManualHostPlanAction> ManageSemiAutomaticPlanAction;

        public SemiAutomaticBackgroundThread(IEnumerable<Host> mastersToUpgrade, IDictionary<Host, List<PlanAction>> planActions, PlanAction revertAction)
        {
            _planActions = planActions;
            _mastersToUpgrade = mastersToUpgrade;
            _revertAction = revertAction;
            _bw = new Thread(BworkerDoWork) { IsBackground = true };
        }

        private static List<Host> GetAllHosts(IEnumerable<Host> masters)
        {
            var hosts = new List<Host>();
            foreach (var master in masters)
            {
                hosts.Add(master);
                foreach (var host in master.Connection.Cache.Hosts)
                {
                    if (host != master)
                        hosts.Add(host);
                }
            }
            return hosts;
        }

        private static bool CheckMasterIsUpgraded(List<Host> hosts, int i)
        {
            string masterVersion = hosts[i].LongProductVersion;
            for (int j = i + 1; j < hosts.Count; j++)
            {
                if (hosts[j].LongProductVersion != masterVersion)
                    return true;
                if (hosts[j].IsMaster())
                    break;
            }
            return false;
        }

        private static int SkipSlaves(List<Host> hosts, int i)
        {
            for (int j = i + 1; j < hosts.Count; j++)
            {
                if (hosts[j].IsMaster())
                    break;
                i++;
            }
            return i;
        }

        private void BworkerDoWork()
        {
            Host currentHost = null;
            try
            {
                List<Host> hosts = GetAllHosts(_mastersToUpgrade);

                bool currentMasterFailed = false;
                string poolHigherProductVersion = string.Empty;
                for (int i = 0; i < hosts.Count; i++)
                {
                    if (_cancel)
                        return;
                    //Skip hosts already upgraded
                    var host=currentHost = hosts[i];
                    if (host.IsMaster())
                    {
                        poolHigherProductVersion = host.LongProductVersion;
                        if (CheckMasterIsUpgraded(hosts, i))
                        {
                            log.Debug(string.Format("Skipping master '{0}' because it is upgraded", host.Name));
                            continue;
                        }
                    }
                    else if (host.LongProductVersion == poolHigherProductVersion)
                    {
                        log.Debug(string.Format("Skipping host '{0}' because it is upgraded", host.Name));
                        continue;
                    }
                    log.Debug(string.Format("Starting to upgrade host '{0}'", host.Name));
                    //Add subtasks for the current host
                    bool allActionsDone = true;
                    foreach (var planAction in _planActions[host])
                    {
                        //if the wizard has been cancelled, skip this action, unless it is a BringBabiesBackAction
                        if (_cancel && !(planAction is BringBabiesBackAction)) 
                            continue;

                        OnReportRunning(planAction, host);

                        try
                        {
                            if (planAction is UpgradeManualHostPlanAction)
                            {
                                var upgradeAction = (UpgradeManualHostPlanAction)planAction;
                                
                                if (ManageSemiAutomaticPlanAction != null)
                                    ManageSemiAutomaticPlanAction(upgradeAction);
                                
                                if (host.IsMaster())
                                    poolHigherProductVersion = upgradeAction.Host.LongProductVersion;
                            }
                            else
                                planAction.Run();

                        }
                        catch (Exception excep)
                        {
                            log.Error(string.Format("Exception in host '{0}' while it was upgraded", host.Name), excep);
                            PlanAction action1 = planAction;

                            OnReportException(excep, action1, host);
                            
                            if (host.IsMaster())
                                currentMasterFailed = true;
                            allActionsDone = false;
                            break;
                        }
                    }

                    if (allActionsDone)
                        OnReportHostDone(host);

                    //Skip slaves if master failed
                    if (currentMasterFailed)
                    {
                        i = SkipSlaves(hosts, i);
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
