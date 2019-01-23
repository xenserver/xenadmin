using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class HostMemoryPostUpgradeWarning : WarningWithMoreInfo
    {
        private readonly Host host;
        private readonly long dom0MemoryAfterUpgrade;
        private readonly string upgradeProductVersion;

        public HostMemoryPostUpgradeWarning(Check check, Host host, long dom0MemoryAfterUpgrade = 0, string upgradeProductVersion = null)
            : base(check)
        {
            this.host = host;
            this.upgradeProductVersion = upgradeProductVersion;
            this.dom0MemoryAfterUpgrade = dom0MemoryAfterUpgrade;
        }

        public override string Title => Description;

        public override string Description
        {
            get
            {
                if (dom0MemoryAfterUpgrade > 0)
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DOM0_MEMORY_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
                if (string.IsNullOrEmpty(upgradeProductVersion))
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DEFAULT_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
                return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_VERSION_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
            }
        }

        public override string Message
        {
            get
            {
                if (dom0MemoryAfterUpgrade > 0)
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DOM0_MEMORY_WARNING_LONG, Helpers.GetName(host), Util.MemorySizeStringSuitableUnits(dom0MemoryAfterUpgrade, true));
                if (string.IsNullOrEmpty(upgradeProductVersion))
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DEFAULT_WARNING_LONG, Helpers.GetName(host));
                return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_VERSION_WARNING_LONG, Helpers.GetName(host), upgradeProductVersion);
            }
        }
    }
}
