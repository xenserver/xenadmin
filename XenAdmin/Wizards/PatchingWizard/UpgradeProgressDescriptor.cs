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
    public class UpgradeProgressDescriptor
    {
        public List<PlanAction> planActions { get; private set; }
        public List<PlanAction> delayedActions { get; private set; }
        private Host master;
        public float ProgressPercent = 0;

        public PlanAction InProgressAction = null;
        public PlanAction FailedWithExceptionAction = null;
        public List<PlanAction> doneActions = new List<PlanAction>();

        public UpgradeProgressDescriptor(Host master, List<PlanAction> planActions, List<PlanAction> delayedActions)
        {
            this.master = master;
            this.planActions = planActions;
            this.delayedActions = delayedActions;
        }

        public int TotalNumberOfActions
        {
            get
            {
                return planActions.Count + delayedActions.Count;
            }
        }
    
    }
}
