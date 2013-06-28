using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    public class MissingMultipleFCSRsProblem : Problem
    {
        private readonly Pool pool;
        private readonly List<SRDeviceConfig> srDeviceConfigList;

        public MissingMultipleFCSRsProblem(Check check, Pool pool, List<SRDeviceConfig> srDeviceConfigList)
            : base(check)
        {
            this.pool = pool;
            this.srDeviceConfigList = srDeviceConfigList;
        }

        private bool EmptySRDeviceConfigList
        {
            get { return srDeviceConfigList == null || srDeviceConfigList.Count == 0; }
        }

        public override string Title
        {
            get { return Check.Description; }
        }

        public override string Description
        {
            get
            {
                if (EmptySRDeviceConfigList)
                    return Messages.DR_WIZARD_PROBLEM_MISSING_MULTIPLE_SRS_NO_INFO;

                return string.Format(Messages.DR_WIZARD_PROBLEM_MISSING_MULTIPLE_SRS, srDeviceConfigList.Count);
            }
        }

        public override string HelpMessage
        {
            get
            {
                if (EmptySRDeviceConfigList)
                    return string.Empty;

                return Messages.DR_WIZARD_PROBLEM_MISSING_MULTIPLE_SRS_HELPMESSAGE;
            }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();
            cancelled = false;

            if (EmptySRDeviceConfigList)
                return null;

            Host master = pool.Connection.Resolve(pool.master);
            if (master == null)
                return null;

            List<AsyncAction> subActions = new List<AsyncAction>();
            foreach (SRDeviceConfig item in srDeviceConfigList)
            {
                if (item.DeviceConfig == null)
                    continue;

                ScannedDeviceInfo deviceInfo = new ScannedDeviceInfo(item.SR.GetSRType(true),
                                                                     item.DeviceConfig,
                                                                     item.SR.uuid);
                subActions.Add(new DrTaskCreateAction(pool.Connection, deviceInfo));
            }

            if (subActions.Count == 0)
                return null;

            return subActions.Count == 1
                       ? subActions[0]
                       : new ParallelAction(pool.Connection, Messages.ACTION_MULTIPLE_DR_TASK_CREATE_TITLE,
                                            Messages.ACTION_MULTIPLE_DR_TASK_CREATE_START,
                                            Messages.ACTION_MULTIPLE_DR_TASK_CREATE_END, subActions);
        }
    }

    public struct SRDeviceConfig
    {
        public SR SR { get; private set; }
        public Dictionary<string, string> DeviceConfig { get; private set; }

        public SRDeviceConfig(SR sr, Dictionary<string, string> deviceConfig)
            : this()
        {
            SR = sr;
            DeviceConfig = deviceConfig;
        }
    }
}
