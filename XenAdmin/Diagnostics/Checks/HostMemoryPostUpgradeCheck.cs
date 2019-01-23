using System.Collections.Generic;
using System.Web.Script.Serialization;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class HostMemoryPostUpgradeCheck : HostPostLivenessCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, string> installMethodConfig;

        public HostMemoryPostUpgradeCheck(Host host, Dictionary<string, string> installMethodConfig)
            : base(host)
        {
            this.installMethodConfig = installMethodConfig;
        }
        public override string Description => Messages.CHECKING_HOST_MEMORY_POST_UPGRADE_DESCRIPTION;

        protected override Problem RunHostCheck()
        {
            if (TryGetDom0MemoryPostUpgrade(out var dom0MemoryPostUpgrade))
            {
                var currentDom0Memory = Host.dom0_memory();
                // we know the Dom0 memory after the upgrade, check if it will greater then the current value
                if (dom0MemoryPostUpgrade > currentDom0Memory)
                {
                    // add warning that the the Dom0 memory will be changed after the upgrade 
                    return new HostMemoryPostUpgradeWarning(this, Host, dom0MemoryPostUpgrade);
                }
            }
            else
            {
                // we don't know the Dom0 memory after the upgrade, so add generic warning if they are upgrading from pre-Naples 
                if (Helpers.NaplesOrGreater(Host))
                    return null;

                string upgradePlatformVersion = null;
                string upgradeProductVersion = null;
                if (installMethodConfig != null)
                    TryGetUpgradeVersion(out upgradePlatformVersion, out upgradeProductVersion);
                
                if (string.IsNullOrEmpty(upgradePlatformVersion))
                {
                    // we don't know the upgrade version, so add generic warning (this is the case of the manual upgrade or when the rpu plugin doesn't have the function)
                    return new HostMemoryPostUpgradeWarning(this, Host);
                }

                // we know the upgrade version
                if (Helpers.NaplesOrGreater(upgradePlatformVersion))
                {
                    // we know that they are upgrading to Naples or greater, so add specific warning
                    return new HostMemoryPostUpgradeWarning(this, Host, 0, upgradeProductVersion);
                }
            }
            return null;
        }

        public bool TryGetDom0MemoryPostUpgrade(out long dom0MemoryAfterUpgrade)
        {
            dom0MemoryAfterUpgrade = 0;

            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "getDom0DefaultMemory", installMethodConfig);
                if (long.TryParse(result, out dom0MemoryAfterUpgrade))
                    return true;
                return false;
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call prepare_host_upgrade.getDom0DefaultMemory on {0} failed with {1}", Host.Name(), failure.Message);
                return false;
            }
        }

        public bool TryGetUpgradeVersion(out string platformVersion, out string productVersion)
        {
            platformVersion = productVersion = null;
            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "getVersion", installMethodConfig);
                var serializer = new JavaScriptSerializer();
                var res = (Dictionary<string, object>)serializer.DeserializeObject(result);
                platformVersion = res.ContainsKey("platform-version") ? (string)res["platform-version"] : null;
                productVersion = res.ContainsKey("product-version") ? (string)res["product-version"] : null;
                if (platformVersion == null && productVersion == null)
                    return false;
                return true;
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call prepare_host_upgrade.getVersion on {0} failed with {1}", Host.Name(), failure.Message);
                
                return false;
            }
        }
    }
}
