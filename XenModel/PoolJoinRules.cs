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
using XenAdmin.Network;
using XenAPI;
using System.Linq;

namespace XenAdmin.Core
{
    /// <summary>
    /// Encapsulates whether a supporter can join a given pool
    /// </summary>
    public static class PoolJoinRules
    {
        // The order of the enum determines the order we sort the hosts in, in the New Pool Wizard
        public enum Reason
        {
            WillBeCoordinator,
            Allowed,
            Connecting,
            CoordinatorNotConnected,
            CoordinatorConnecting,
            DifferentAdConfig,
            HasRunningVMs,
            HasSharedStorage,
            IsAPool,
            LicenseRestriction,
            NotSameLinuxPack,
            LicensedHostUnlicensedCoordinator,
            UnlicensedHostLicensedCoordinator,
            LicenseMismatch,
            CoordinatorPoolMaxNumberHostReached,
            WillExceedPoolMaxSize,
            DifferentServerVersion,
            DifferentHomogeneousUpdatesFromCoordinator,
            DifferentHomogeneousUpdatesFromPool,
            DifferentCPUs,
            DifferentNetworkBackends,
            CoordinatorHasHA,
            NotPhysicalPif,
            NonCompatibleManagementInterface,
            WrongRoleOnCoordinator,
            WrongRoleOnSupporter,
            HasClusteringEnabled,
            WrongNumberOfIpsCluster,
            WrongNumberOfIpsBond,
            NotConnected,
            TlsVerificationOnlyOnPool,
            TlsVerificationOnlyOnPoolJoiner,
            TlsVerificationOnlyOnCoordinator,
            TlsVerificationOnlyOnCoordinatorJoiner
        }

        // The order of if's in CanJoinPool() determines which reason we display if there is more than one.
        // At least as far as WillBeCoordinator, some callers may rely on the order.
        // Also, some of the private functions in this file rely on previous tests (especially for
        // null-ness and connectedness) having been done first.

        /// <summary>
        /// Whether a server can join a pool (or form a pool with a standalone server)
        /// </summary>
        /// <param name="supporterConnection">The connection of the server that wants to join the pool</param>
        /// <param name="coordinatorConnection">The connection of the existing pool or of the proposed coordinator of a new pool</param>
        /// <param name="allowLicenseUpgrade">Whether we can upgrade a free host to a v6 license of the pool it's joining</param>
        /// <param name="allowCpuLevelling">Whether we can apply CPU levelling to the supporter before it joins the pool</param>
        /// <returns>The reason why the server can't join the pool, or Reason.Allowed if it's OK</returns>
        public static Reason CanJoinPool(IXenConnection supporterConnection, IXenConnection coordinatorConnection, bool allowLicenseUpgrade, bool allowCpuLevelling, bool allowSupporterAdConfig, int poolSizeIncrement = 1)
        {
            if (supporterConnection == null || !supporterConnection.IsConnected)
                return Reason.NotConnected;

            Host supporterHost = Helpers.GetCoordinator(supporterConnection);
            if (supporterHost == null)
                return Reason.Connecting;

            if (LicenseRestriction(supporterHost))
                return Reason.LicenseRestriction;

            if (IsAPool(supporterConnection))
                return Reason.IsAPool;

            if (coordinatorConnection == null || !coordinatorConnection.IsConnected)
                return Reason.CoordinatorNotConnected;

            Host coordinatorHost = Helpers.GetCoordinator(coordinatorConnection);
            
            if (coordinatorHost == null)
                return Reason.CoordinatorConnecting;

            if (WillBeCoordinator(supporterConnection, coordinatorConnection))
                return Reason.WillBeCoordinator;

            if (!RoleOK(coordinatorConnection))
                return Reason.WrongRoleOnCoordinator;

            if (!CompatibleCPUs(supporterHost, coordinatorHost, allowCpuLevelling))
                return Reason.DifferentCPUs;

            if (DifferentServerVersion(supporterHost, coordinatorHost))
                return Reason.DifferentServerVersion;

            if (DifferentHomogeneousUpdates(supporterHost, coordinatorHost))
                return coordinatorHost.Connection.Cache.Hosts.Length > 1 ? Reason.DifferentHomogeneousUpdatesFromPool : Reason.DifferentHomogeneousUpdatesFromCoordinator;

            if (FreeHostPaidCoordinator(supporterHost, coordinatorHost, allowLicenseUpgrade))
                return Reason.UnlicensedHostLicensedCoordinator;

            if (PaidHostFreeCoordinator(supporterHost, coordinatorHost))
                return Reason.LicensedHostUnlicensedCoordinator;

            if (LicenseMismatch(supporterHost, coordinatorHost))
                return Reason.LicenseMismatch;

            if (CoordinatorPoolMaxNumberHostReached(coordinatorConnection))
                return Reason.CoordinatorPoolMaxNumberHostReached;

            if (WillExceedPoolMaxSize(coordinatorConnection, poolSizeIncrement))
                return Reason.WillExceedPoolMaxSize;
            
            if (!SameLinuxPack(supporterHost, coordinatorHost))
                return Reason.NotSameLinuxPack;

            if (!RoleOK(supporterConnection))
                return Reason.WrongRoleOnSupporter;

            if (HasSharedStorage(supporterConnection))
                return Reason.HasSharedStorage;

            if (HasRunningVMs(supporterConnection))
                return Reason.HasRunningVMs;

            if (DifferentNetworkBackends(supporterHost, coordinatorHost))
                return Reason.DifferentNetworkBackends;

            if (!CompatibleAdConfig(supporterHost, coordinatorHost, allowSupporterAdConfig))
                return Reason.DifferentAdConfig;

            if (HaEnabled(coordinatorConnection))
                return Reason.CoordinatorHasHA;

            if (Helpers.FeatureForbidden(supporterConnection, Host.RestrictManagementOnVLAN) && HasSupporterAnyNonPhysicalPif(supporterConnection))
                return Reason.NotPhysicalPif;

            if (!Helpers.FeatureForbidden(supporterConnection, Host.RestrictManagementOnVLAN) && !HasCompatibleManagementInterface(supporterConnection))
                return Reason.NonCompatibleManagementInterface;

            if (supporterHost?.Connection?.Cache.Clusters.FirstOrDefault() != null)
                return Reason.HasClusteringEnabled;

            if (!HasIpForClusterNetwork(coordinatorConnection, supporterHost, out var clusterHostInBond))
                return clusterHostInBond ? Reason.WrongNumberOfIpsBond : Reason.WrongNumberOfIpsCluster;

            var coordinatorPool = Helpers.GetPool(coordinatorConnection);
            var coordinatorPoolOfOne = Helpers.GetPoolOfOne(coordinatorConnection);
            var supporterPoolOfOne = Helpers.GetPoolOfOne(supporterConnection);

            if (coordinatorPoolOfOne.tls_verification_enabled && !supporterPoolOfOne.tls_verification_enabled)
                return coordinatorPool == null ? Reason.TlsVerificationOnlyOnCoordinator : Reason.TlsVerificationOnlyOnPool;
            
            if (!coordinatorPoolOfOne.tls_verification_enabled && supporterPoolOfOne.tls_verification_enabled)
                return coordinatorPool == null ? Reason.TlsVerificationOnlyOnCoordinatorJoiner : Reason.TlsVerificationOnlyOnPoolJoiner;

            return Reason.Allowed;
        }

        public static string ReasonMessage(Reason reason)
        {
            switch (reason)
            {
                case Reason.WillBeCoordinator:
                    return Messages.COORDINATOR;
                case Reason.Allowed:
                    return "";
                case Reason.Connecting:
                    return Messages.CONNECTING;
                case Reason.CoordinatorNotConnected:
                    return Messages.NEWPOOL_COORDINATOR_DISCONNECTED;
                case Reason.CoordinatorConnecting:
                    return Messages.NEWPOOL_COORDINATOR_CONNECTING;
                case Reason.DifferentAdConfig:
                    return Messages.NEWPOOL_DIFFERING_AD_CONFIG;
                case Reason.HasRunningVMs:
                    return Messages.NEWPOOL_HAS_RUNNING_VMS;
                case Reason.HasSharedStorage:
                    return Messages.NEWPOOL_HAS_SHARED_STORAGE;
                case Reason.IsAPool:
                    return Messages.NEWPOOL_IS_A_POOL;
                case Reason.LicenseRestriction:
                    return Messages.NEWPOOL_POOLINGRESTRICTED;
                case Reason.NotSameLinuxPack:
                    return Messages.NEWPOOL_LINUXPACK;
                case Reason.LicensedHostUnlicensedCoordinator:
                    return Messages.NEWPOOL_LICENSED_HOST_UNLICENSED_COORDINATOR;
                case Reason.UnlicensedHostLicensedCoordinator:
                    return Messages.NEWPOOL_UNLICENSED_HOST_LICENSED_COORDINATOR;
                case Reason.LicenseMismatch:
                    return Messages.NEWPOOL_LICENSEMISMATCH;
                case Reason.CoordinatorPoolMaxNumberHostReached:
                    return Messages.NEWPOOL_MAX_NUMBER_HOST_REACHED;
                case Reason.WillExceedPoolMaxSize:
                    return Messages.NEWPOOL_WILL_EXCEED_POOL_MAX_SIZE;
                case Reason.DifferentServerVersion:
                    return Messages.NEWPOOL_DIFF_SERVER;
                case Reason.DifferentHomogeneousUpdatesFromCoordinator:
                    return Messages.NEWPOOL_DIFFERENT_HOMOGENEOUS_UPDATES_FROM_COORDINATOR;
                case Reason.DifferentHomogeneousUpdatesFromPool:
                    return Messages.NEWPOOL_DIFFERENT_HOMOGENEOUS_UPDATES_FROM_POOL;
                case Reason.DifferentCPUs:
                    return Messages.NEWPOOL_DIFF_HARDWARE;
                case Reason.DifferentNetworkBackends:
                    return Messages.NEWPOOL_DIFFERENT_NETWORK_BACKENDS;
                case Reason.NotConnected:
                    return Messages.DISCONNECTED;
                case Reason.CoordinatorHasHA:
                    return Messages.POOL_JOIN_FORBIDDEN_BY_HA;
                case Reason.NotPhysicalPif :
                    return Messages.POOL_JOIN_NOT_PHYSICAL_PIF;
                case Reason.NonCompatibleManagementInterface :
                    return Messages.POOL_JOIN_NON_COMPATIBLE_MANAGEMENT_INTERFACE;
                case Reason.WrongRoleOnCoordinator:
                    return Messages.NEWPOOL_COORDINATOR_ROLE;
                case Reason.WrongRoleOnSupporter:
                    return Messages.NEWPOOL_SUPPORTER_ROLE;
                case Reason.HasClusteringEnabled:
                    return Messages.NEW_POOL_CLUSTERING_ENABLED;
                case Reason.WrongNumberOfIpsCluster:
                    return Messages.NEWPOOL_IP_COUNT_CLUSTER;
                case Reason.WrongNumberOfIpsBond:
                    return Messages.NEWPOOL_IP_COUNT_BOND;
                case Reason.TlsVerificationOnlyOnPool:
                    return Messages.POOL_JOIN_CERTIFICATE_CHECKING_ONLY_ON_POOL;
                case Reason.TlsVerificationOnlyOnPoolJoiner:
                    return Messages.POOL_JOIN_CERTIFICATE_CHECKING_ONLY_ON_POOL_JOINER;
                case Reason.TlsVerificationOnlyOnCoordinator:
                    return Messages.POOL_JOIN_CERTIFICATE_CHECKING_ONLY_ON_COORDINATOR;
                case Reason.TlsVerificationOnlyOnCoordinatorJoiner:
                    return Messages.POOL_JOIN_CERTIFICATE_CHECKING_ONLY_ON_COORDINATOR_JOINER;
                default:
                    System.Diagnostics.Trace.Assert(false, "Unknown reason");
                    return "";
            }
        }

        private static bool WillBeCoordinator(IXenConnection supporter, IXenConnection coordinator)
        {
            // Assume we have already tested that the connection has no other reason why it can't be a coordinator
            // (e.g., connected, licensing restrictions, ...)
            return supporter == coordinator;
        }

        private static bool HasSharedStorage(IXenConnection connection)
        {
            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.shared && !sr.IsToolsSR())
                    return true;
            }
            return false;
        }

        private static bool HasRunningVMs(IXenConnection connection)
        {
            foreach (VM vm in connection.Cache.VMs)
            {
                if (vm.IsRealVm() && vm.power_state == XenAPI.vm_power_state.Running)
                    return true;
            }
            return false;
        }

        private static bool IsAPool(IXenConnection connection)
        {
            return Helpers.GetPool(connection) != null;
        }

        private static bool LicenseRestriction(Host host)
        {
            return Host.RestrictPooling(host);
        }

        // If CompatibleCPUs(supporter, coordinator, false) is true, the CPUs can be pooled without masking first.
        // If CompatibleCPUs(supporter, coordinator, true) is true but CompatibleCPUs(supporter, coordinator, false) is false,
        // the CPUs can be pooled but only if they are masked first.
        public static bool CompatibleCPUs(Host supporter, Host coordinator, bool allowCpuLevelling)
        {
            if (supporter == null || coordinator == null)
                return true;

            Dictionary<string, string> supporter_cpu_info = supporter.cpu_info;
            Dictionary<string, string> coordinator_cpu_info = coordinator.cpu_info;
            if (!Helper.AreEqual2(supporter_cpu_info, null) && !Helper.AreEqual2(coordinator_cpu_info, null))
            {
                // Host.cpu_info is supported
                // From this point on, it is only necessary to have matching vendor and features
                // (after masking the supporter, if allowed).
                if (supporter_cpu_info["vendor"] != coordinator_cpu_info["vendor"])
                    return false;

                // As of Dundee, feature levelling makes all CPUs from the same vendor compatible
                if (Helpers.DundeeOrGreater(coordinator) || Helpers.DundeeOrGreater(supporter))
                    return true;

                if (supporter_cpu_info["features"] == coordinator_cpu_info["features"])
                    return true;

                if (allowCpuLevelling)
                {
                    string cpuid_feature_mask = null;
                    Pool pool = Helpers.GetPoolOfOne(coordinator.Connection);
                    if (pool != null && pool.other_config.ContainsKey("cpuid_feature_mask"))
                        cpuid_feature_mask = pool.other_config["cpuid_feature_mask"];

                    return MaskableTo(supporter_cpu_info["maskable"], supporter_cpu_info["physical_features"], coordinator_cpu_info["features"], cpuid_feature_mask);
                }

                return false;    
            }

            // Host.cpu_info not supported: use the old method which compares vendor, family, model and flags
            foreach (Host_cpu cpu1 in supporter.Connection.Cache.Host_cpus)
            {
                foreach (Host_cpu cpu2 in coordinator.Connection.Cache.Host_cpus)
                {
                    if (cpu1.vendor != cpu2.vendor ||
                        cpu1.family != cpu2.family ||
                        cpu1.model != cpu2.model ||
                        cpu1.flags != cpu2.flags)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can the features represented by "from" be masked to become the features represented by "to"?
        /// </summary>
        /// <param name="mask_type">"no", "base" (can only mask first 64 bits) or "full"</param>
        /// <param name="from">The unmasked features, as a set of 32 hex digits, possibly separated by spaces or dashes</param>
        /// <param name="to">The target features, as a set of 32 hex digits, possibly separated by spaces or dashes</param>
        /// <param name="feature_mask">A mask of features to ignore for the comparison</param>
        public static bool MaskableTo(string mask_type, string from, string to, string feature_mask)
        {
            if (mask_type == "no")
                return false;

            string from2 = from.Replace(" ", "");
            from2 = from2.Replace("-", "");
            string to2 = to.Replace(" ", "");
            to2 = to2.Replace("-", "");
            if (from2.Length != 32 || to2.Length != 32)
                return false;

            string fm2;
            if (feature_mask == null)
                fm2 = "ffffffffffffffffffffffffffffffff";
            else
            {
                fm2 = feature_mask.Replace(" ", "");
                fm2 = fm2.Replace("-", "");
                if (fm2.Length != 32)
                    fm2 = "ffffffffffffffffffffffffffffffff";
            }

            // Compare int64-by-int64. AMD can mask all the bits; some Intel chips can only mask the first half.
            for (int i = 0; i < 2; ++i)
            {
                string fromPart = from2.Substring(16 * i, 16);
                string toPart = to2.Substring(16 * i, 16);
                string fmPart = fm2.Substring(16 * i, 16);

                ulong fromInt = Convert.ToUInt64(fromPart, 16);
                ulong toInt = Convert.ToUInt64(toPart, 16);
                ulong fmInt = Convert.ToUInt64(fmPart, 16);
                fromInt &= fmInt;
                toInt &= fmInt;

                if (i == 1 && mask_type == "base")  // must be an exact match
                {
                    if (fromInt != toInt)
                        return false;
                }
                else  // "from" must be maskable to "to"
                {
                    if ((fromInt & toInt) != toInt)
                        return false;
                }
            }

            return true;
        }

        private static bool DifferentServerVersion(Host supporter, Host coordinator)
        {
            if (supporter.API_version_major != coordinator.API_version_major ||
                supporter.API_version_minor != coordinator.API_version_minor)
                return true;

            if (Helpers.FalconOrGreater(supporter) && string.IsNullOrEmpty(supporter.GetDatabaseSchema()))
                return true;
            if (Helpers.FalconOrGreater(coordinator) && string.IsNullOrEmpty(coordinator.GetDatabaseSchema()))
                return true;

            if (supporter.GetDatabaseSchema() != coordinator.GetDatabaseSchema())
                return true;

            return
                !Helpers.ElyOrGreater(coordinator) && !Helpers.ElyOrGreater(supporter) && supporter.BuildNumber() != coordinator.BuildNumber() ||
                supporter.PlatformVersion() != coordinator.PlatformVersion() ||
                supporter.ProductBrand() != coordinator.ProductBrand();
        }

        /// <summary>
        /// Check whether all updates that request homogeneity are in fact homogeneous
        /// between coordinator and supporter. This is used in CanJoinPool and prevents the pool from being created
        /// </summary>
        private static bool DifferentHomogeneousUpdates(Host supporter, Host coordinator)
        {
            if (supporter == null || coordinator == null)
                return false;

            if (!Helpers.ElyOrGreater(supporter) || !Helpers.ElyOrGreater(coordinator))
                return false;

            var coordinatorUpdates = coordinator.AppliedUpdates().Where(update => update.EnforceHomogeneity()).Select(update => update.uuid).ToList();
            var supporterUpdates = supporter.AppliedUpdates().Where(update => update.EnforceHomogeneity()).Select(update => update.uuid).ToList();

            return coordinatorUpdates.Count != supporterUpdates.Count || !coordinatorUpdates.All(supporterUpdates.Contains);
        }

        private static bool SameLinuxPack(Host supporter, Host coordinator)
        {
            return supporter.LinuxPackPresent() == coordinator.LinuxPackPresent();
        }

        public static bool CompatibleAdConfig(Host supporter, Host coordinator, bool allowSupporterConfig)
        {
            if (supporter == null || coordinator == null)
                return false;

            // CA-30223: There is no need to check the config of the auth services are the same, as xapi only relies on these two values being equal
            if (supporter.external_auth_type != coordinator.external_auth_type ||
                supporter.external_auth_service_name != coordinator.external_auth_service_name)
            {
                // if the supporter is AD free and we are allowing the configure then we can solve this
                if (supporter.external_auth_type == Auth.AUTH_TYPE_NONE && allowSupporterConfig)
                    return true;

                return false;
            }
            return true;
        }

        public static bool FreeHostPaidCoordinator(Host supporter, Host coordinator, bool allowLicenseUpgrade)
        {
            if (supporter == null || coordinator == null)
                return false;
            
            return supporter.IsFreeLicense() && !coordinator.IsFreeLicense() && !allowLicenseUpgrade;
        }

        private static bool PaidHostFreeCoordinator(Host supporter, Host coordinator)
        {
            if (supporter == null || coordinator == null)
                return false;

            return !supporter.IsFreeLicense() && coordinator.IsFreeLicense();
        }

        private static bool LicenseMismatch(Host supporter, Host coordinator)
        {
            Host.Edition supporterEdition = Host.GetEdition(supporter.edition);
            Host.Edition coordinatorEdition = Host.GetEdition(coordinator.edition);

            return supporterEdition != Host.Edition.Free && coordinatorEdition != Host.Edition.Free && supporterEdition != coordinatorEdition;
        }

        private static bool CoordinatorPoolMaxNumberHostReached(IXenConnection connection)
        {
            return Helpers.FeatureForbidden(connection, Host.RestrictPoolSize) && connection.Cache.HostCount > 2;
        }

        public static bool WillExceedPoolMaxSize(IXenConnection connection, int poolSizeIncrement)
        {
            return Helpers.FeatureForbidden(connection, Host.RestrictPoolSize) && connection.Cache.HostCount + poolSizeIncrement > 3;
        }

        private static bool HaEnabled(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            return (pool != null && pool.ha_enabled);
        }

        private static bool RoleOK(IXenConnection connection)
        {
            return Role.CanPerform(new RbacMethodList("pool.join"), connection, out _);
        }

        private static bool DifferentNetworkBackends(Host supporter, Host coordinator)
        {
            if (supporter.software_version.ContainsKey("network_backend") &&
                coordinator.software_version.ContainsKey("network_backend") &&
                supporter.software_version["network_backend"] != coordinator.software_version["network_backend"])
                return true;

            var coordinatorPool = Helpers.GetPoolOfOne(coordinator.Connection);
            var supporterPool = Helpers.GetPoolOfOne(supporter.Connection);

            return coordinatorPool != null && supporterPool != null &&
                   coordinatorPool.vSwitchController() && supporterPool.vSwitchController() &&
                   coordinatorPool.vswitch_controller != supporterPool.vswitch_controller;
        }

        /// <summary>
        /// Check whether all supp packs that request homogeneity are in fact homogeneous
        /// across the proposed pool. This is only a warning; it does not prevent the pool being created;
        /// so it is not used by CanJoinPool.
        /// </summary>
        /// <returns>A list of strings: each string is the name of a bad supp pack with a reason in brackets</returns>
        public static List<string> HomogeneousSuppPacksDiffering(List<Host> supporters, IXenObject poolOrCoordinator)
        {
            List<Host> allHosts = new List<Host>(supporters);
            allHosts.AddRange(poolOrCoordinator.Connection.Cache.Hosts);

            // Make a dictionary of all supp packs that should be homogeneous
            Dictionary<string, string> homogeneousPacks = new Dictionary<string, string>();
            foreach (Host host in allHosts)
            {
                var suppPacks = host.SuppPacks();
                foreach (Host.SuppPack suppPack in suppPacks)
                {
                    if (suppPack.Homogeneous)
                        homogeneousPacks[suppPack.OriginatorAndName] = suppPack.Description;
                }
            }

            // Now for each such supp pack, see whether it is in fact homogeneous.
            // If not, add it to the list of bad packs with a reason.

            List<string> badPacks = new List<string>();

            foreach (string pack in homogeneousPacks.Keys)
            {
                // Is it present on all hosts, with the same version?
                List<Host> missingHosts = new List<Host>();
                string expectedVersion = null;
                bool versionsDiffer = false;

                foreach (Host host in allHosts)
                {
                    Host.SuppPack matchingPack = host.SuppPacks().Find(sp => (sp.OriginatorAndName == pack));
                    if (matchingPack == null)
                        missingHosts.Add(host);
                    else if (expectedVersion == null)
                        expectedVersion = matchingPack.Version;
                    else if (matchingPack.Version != expectedVersion)
                        versionsDiffer = true;
                }

                if (missingHosts.Count != 0)
                {
                    badPacks.Add(string.Format(Messages.SUPP_PACK_MISSING_ON,
                        homogeneousPacks[pack],  // the Description field from the pack, which we cunningly remembered to save above
                        string.Join("\n", missingHosts.ConvertAll(host => host.ToString()).ToArray())));  // comma-separated list of hosts
                }
                else if (versionsDiffer)
                {
                    badPacks.Add(string.Format(Messages.SUPP_PACK_VERSIONS_DIFFER, homogeneousPacks[pack]));
                }
            }

            return badPacks;
        }

        /// <summary>
        /// Check whether the host that joins the pool has a more extensive feature set than the pool (as long as the CPU vendor is common)
        /// In this case the host will transparently be down-levelled to the pool level (without needing reboots)
        /// </summary>
        public static bool HostHasMoreFeatures(Host supporter, Pool pool)
        {
            if (supporter == null || pool == null)
                return false;

            Dictionary<string, string> supporter_cpu_info = supporter.cpu_info;
            Dictionary<string, string> pool_cpu_info = pool.cpu_info;
            if (!Helper.AreEqual2(supporter_cpu_info, null) && !Helper.AreEqual2(pool_cpu_info, null))
            {
                // if pool has less features than supporter, then supporter will be down-levelled
                return FewerFeatures(pool_cpu_info, supporter_cpu_info);
            }
            return false;
        }

        /// <summary>
        /// Check whether the host that joins the pool has a less extensive feature set than the pool (as long as the CPU vendor is common)
        /// In this case the pool is transparently down-levelled to the new host's level (without needing reboots)
        /// </summary>
        public static bool HostHasFewerFeatures(Host supporter, Pool pool)
        {
            if (supporter == null || pool == null)
                return false;

            Dictionary<string, string> supporter_cpu_info = supporter.cpu_info;
            Dictionary<string, string> pool_cpu_info = pool.cpu_info;
            if (!Helper.AreEqual2(supporter_cpu_info, null) && !Helper.AreEqual2(pool_cpu_info, null))
            {
                // if supporter has less features than pool, then pool will be down-levelled
                return FewerFeatures(supporter_cpu_info, pool_cpu_info);
            }
            return false;
        }

        /// <summary>
        /// Check whether first CPU has fewer features than the second. 
        /// It returns true if the first feature set is less than the second one in at least one bit
        /// </summary>
        public static bool FewerFeatures(Dictionary<string, string> cpu_infoA, Dictionary<string, string> cpu_infoB)
        {
            if (cpu_infoA.ContainsKey("features_hvm") && cpu_infoB.ContainsKey("features_hvm") &&
                FewerFeatures(cpu_infoA["features_hvm"], cpu_infoB["features_hvm"]))
                return true;
            if (cpu_infoA.ContainsKey("features_pv") && cpu_infoB.ContainsKey("features_pv") &&
                FewerFeatures(cpu_infoA["features_pv"], cpu_infoB["features_pv"]))
                return true;

            return false;
        }

        public static bool FewerFeatures(string featureSetA, string featureSetB)
        {
            if (string.IsNullOrEmpty(featureSetA) || string.IsNullOrEmpty(featureSetB))
                return false;

            string stringA = featureSetA.Replace(" ", "");
            stringA = stringA.Replace("-", "");
            string stringB = featureSetB.Replace(" ", "");
            stringB = stringB.Replace("-", "");

            if (stringA.Length < stringB.Length)
                stringA = stringA.PadRight(stringB.Length, '0');
            if (stringB.Length < stringA.Length)
                stringB = stringB.PadRight(stringA.Length, '0');
            
            for (int i = 0; i < stringA.Length / 8; ++i)
            {
                uint intA = Convert.ToUInt32(stringA.Substring(8 * i, 8), 16);
                uint intB = Convert.ToUInt32(stringB.Substring(8 * i, 8), 16);

                if ((intA & intB) != intB)
                    return true;
            }
            return false;
        }

        public static bool HasSupporterAnyNonPhysicalPif(IXenConnection supporterConnection)
        {
            return
                supporterConnection.Cache.PIFs.Any(p => !p.physical);
        }

        public static bool HasCompatibleManagementInterface(IXenConnection supporterConnection)
        {
            /* if there are non physical pifs present then the supporter should have 
             * only one VLAN and it has to be the management interface. 
             * Bonds and cross server private networks are not allowed */

            int numberOfNonPhysicalPifs = supporterConnection.Cache.PIFs.Count(p => !p.physical);
            
            /* allow the case where there are only physical pifs */
            if (numberOfNonPhysicalPifs == 0)
                return true;

            if(numberOfNonPhysicalPifs != 1)
                return false;
            else
                return supporterConnection.Cache.PIFs.Any(p => !p.physical && p.management && p.VLAN != -1);
        }

        public static bool HasIpForClusterNetwork(IXenConnection coordinatorConnection, Host supporterHost, out bool clusterHostInBond)
        {
            clusterHostInBond = false;
            var clusterHost = coordinatorConnection.Cache.Cluster_hosts.FirstOrDefault();

            if (clusterHost == null)
                return true;

            var clusterHostPif = clusterHost.Connection.Resolve(clusterHost.PIF);

            if (clusterHostPif == null)
                return true;

            // if this PIF is a VLAN, then use the tagged_PIF field of the VLAN
            if (clusterHostPif.VLAN >= 0)
            {
                var vlan = coordinatorConnection.Resolve(clusterHostPif.VLAN_master_of);

                if (vlan != null)
                {
                    var taggedPif = coordinatorConnection.Resolve(vlan.tagged_PIF);
                    if (taggedPif != null)
                        clusterHostPif = taggedPif;
                }
            }

            clusterHostInBond = clusterHostPif.IsBondNIC();

            var pifsWithIPAddress = 0;

            List<string> ids = new List<string>();

            if (clusterHostInBond)
            {

                List<PIF> members = new List<PIF>();

                var bonds = coordinatorConnection.ResolveAll(clusterHostPif.bond_master_of);

                foreach (var bond in bonds)
                {
                    members.AddRange(coordinatorConnection.ResolveAll(bond.slaves));
                }

                ids.AddRange(members.Select(member => member.device));
            }
            else
            {
                ids.Add(clusterHostPif.device);
            }

            var pifs = supporterHost.Connection.ResolveAll(supporterHost.PIFs);

            foreach (var pif in pifs)
            {
                if (pif.IsManagementInterface(false) && ids.Contains(pif.device))
                {
                    pifsWithIPAddress += 1;
                }
            }

            return pifsWithIPAddress == 1;

        }
    }
}
