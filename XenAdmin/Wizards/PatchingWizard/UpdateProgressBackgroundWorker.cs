using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    class UpdateProgressBackgroundWorker : BackgroundWorker
    {
        public List<PlanAction> PlanActions { get; private set; }
        public List<PlanAction> DelayedActions { get; private set; }
        private Host master;

        public List<PlanAction> FinsihedActions = new List<PlanAction>();
        public PlanAction FailedWithExceptionAction = null;
        public List<PlanAction> doneActions = new List<PlanAction>();
        public PlanAction InProgressAction { get; set; }


        private readonly List<string> avoidRestartHosts = new List<string>();
        
        /// <summary>
        /// This list lists uuids of hosts that does not need to be restarted
        /// </summary>
        public List<string> AvoidRestartHosts
        {
            get
            {
                return avoidRestartHosts;
            }
        }

        public UpdateProgressBackgroundWorker(Host master, List<PlanAction> planActions, List<PlanAction> delayedActions)
        {
            this.master = master;
            this.PlanActions = planActions;
            this.DelayedActions = delayedActions;
        }

        public List<PlanAction> AllActions 
        {
            get
            {
                return PlanActions.Concat(DelayedActions).ToList();
            }
        }

        public int TotalNumberOfActions
        {
            get
            {
                return PlanActions.Count + DelayedActions.Count;
            }
        }

        public int ProgressPercent
        {
            get
            {
                return (int)(1.0 / (double)TotalNumberOfActions * (double)(FinsihedActions.Count));
            }
        }

        public new void CancelAsync()
        {
            if (PlanActions != null)
                PlanActions.ForEach(pa => 
                {
                    if (!pa.IsComplete)
                        pa.Cancel();
                });

            base.CancelAsync();
        }
    }
}
