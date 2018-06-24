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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public class UpdateProgressBackgroundWorker : BackgroundWorker
    {
        public List<HostPlan> HostPlans { get; private set; }
        public List<PlanAction> FinalActions { get; private set; }
        public List<PlanAction> CleanupActions { get; private set; }
        public readonly List<PlanAction> DoneActions = new List<PlanAction>();
        public readonly List<PlanAction> InProgressActions = new List<PlanAction>();
        public string Name { get; set; }
        public int ProgressIncrement { get; set; }

        public UpdateProgressBackgroundWorker(List<HostPlan> planActions, List<PlanAction> finalActions)
        {
            HostPlans = planActions ?? new List<HostPlan>();
            FinalActions = finalActions ?? new List<PlanAction>();
            CleanupActions = FinalActions.Where(a => a is RemoveUpdateFileFromMasterPlanAction).ToList();
        }

        public new void CancelAsync()
        {
            if (HostPlans != null)
                HostPlans.ForEach(ha =>
                {
                    var cur = ha;
                    cur.InitialPlanActions.ForEach(pa =>
                    {
                        if (!pa.IsComplete)
                            pa.Cancel();
                    });

                    cur.UpdatesPlanActions.ForEach(pa =>
                    {
                        if (!pa.IsComplete)
                            pa.Cancel();
                    });
                });

            base.CancelAsync();
        }

        private int FinalActionsPercentage
        {
            get
            {
                if (FinalActions == null || FinalActions.Count < 1)
                    return 0;
                return 4;
            }
        }

        public int FinalActionsIncrement
        {
            get
            {
                if (FinalActions == null || FinalActions.Count < 1)
                    return 0;
                return FinalActionsPercentage / FinalActions.Count;
            }
        }

        public int InitialActionsIncrement(HostPlan hp)
        {
            return hp.InitialActionsIncrement / HostPlans.Count;
        }

        public int UpdatesActionsIncrement(HostPlan hp)
        {
            return hp.UpdatesActionsIncrement / HostPlans.Count;
        }

        public int DelayedActionsIncrement(HostPlan hp)
        {
            return hp.DelayedActionsIncrement / HostPlans.Count;
        }
    }

    public class HostPlan
    {
        public Host Host;
        public readonly List<PlanAction> InitialPlanActions;
        public readonly List<PlanAction> UpdatesPlanActions;
        public readonly List<PlanAction> DelayedPlanActions;

        public HostPlan(Host host, List<PlanAction> initialActions, List<PlanAction> updateActions, List<PlanAction> delayedActions)
        {
            Host = host;
            InitialPlanActions = initialActions ?? new List<PlanAction>();
            UpdatesPlanActions = updateActions ?? new List<PlanAction>();
            DelayedPlanActions = delayedActions ?? new List<PlanAction>();
        }

        private int InitialActionsPercentage
        {
            get
            {
                if (InitialPlanActions == null || InitialPlanActions.Count < 1)
                    return 0;
                return 30;
            }
        }

        private int UpdatesActionsPercentage
        {
            get
            {
                if (UpdatesPlanActions == null || UpdatesPlanActions.Count < 1)
                    return 0;
                return 90 - InitialActionsPercentage;
            }
        }

        private int DelayedActionsPercentage
        {
            get
            {
                if (DelayedPlanActions == null || DelayedPlanActions.Count < 1)
                    return 0;
                return 96 - InitialActionsPercentage - UpdatesActionsPercentage;
            }
        }

        public int InitialActionsIncrement
        {
            get
            {
                if (InitialPlanActions == null || InitialPlanActions.Count < 1)
                    return 0;
                return InitialActionsPercentage / InitialPlanActions.Count;
            }
        }

        public int UpdatesActionsIncrement
        {
            get
            {
                if (UpdatesPlanActions == null || UpdatesPlanActions.Count < 1)
                    return 0;
                return UpdatesActionsPercentage / UpdatesPlanActions.Count;
            }
        }

        public int DelayedActionsIncrement
        {
            get
            {
                if (DelayedPlanActions == null || DelayedPlanActions.Count < 1)
                    return 0;
                return DelayedActionsPercentage / DelayedPlanActions.Count;
            }
        }
    }
}
