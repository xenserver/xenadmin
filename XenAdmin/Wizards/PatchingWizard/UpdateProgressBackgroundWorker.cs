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
        public Dictionary<Host, List<PlanAction>> DelayedActionsByHost {get; private set; }
        public List<PlanAction> FinalActions { get; private set; }

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

        public UpdateProgressBackgroundWorker(Host master, List<PlanAction> planActions, Dictionary<Host, List<PlanAction>> delayedActionsByHost, 
            List<PlanAction> finalActions)
        {
            this.master = master;
            this.PlanActions = planActions;
            this.DelayedActionsByHost = delayedActionsByHost;
            this.FinalActions = finalActions;
        }

        public int ActionsCount
        {
            get
            {
                return PlanActions.Count + DelayedActionsByHost.Sum(kvp => kvp.Value.Count) + FinalActions.Count;
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
