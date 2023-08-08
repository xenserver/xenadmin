/* Copyright (c) Cloud Software Group, Inc. 
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Alerts.Types;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static event Action CheckForClientUpdatesStarted;
        public static event Action CheckForClientUpdatesCompleted;
        public static event Action CheckForServerUpdatesStarted;
        public static event Action<bool, string> CheckForServerUpdatesCompleted;
        public static event Action<CollectionChangeEventArgs> UpdateAlertCollectionChanged;
        public static event Action RestoreDismissedUpdatesStarted;
        public static event Action<IXenConnection> CdnUpdateInfoChanged;

        public static string UserAgent { get; } = $"{BrandManager.BrandConsole}/{Program.Version} ({IntPtr.Size * 8}-bit)";

        private static readonly object downloadedUpdatesLock = new object();
        private static readonly object updateAlertsLock = new object();
        private static readonly object _cdnUpdatesLock = new object();

        private static List<XenServerVersion> XenServerVersionsForAutoCheck = new List<XenServerVersion>();
        private static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();
        private static List<ClientVersion> ClientVersions = new List<ClientVersion>();
        public static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();

        private static readonly List<Alert> updateAlerts = new List<Alert>();
        private static readonly Dictionary<IXenConnection, CdnPoolUpdateInfo> _cdnUpdateInfoPerConnection = new Dictionary<IXenConnection, CdnPoolUpdateInfo>();

        /// <summary>
        /// Locks and creates a new list of the update alerts
        /// </summary>
        public static List<Alert> UpdateAlerts
        {
            get
            {
                lock (updateAlertsLock)
                    return updateAlerts.ToList();
            }
        }

        /// <summary>
        /// Locks and creates a new dictionary of the CDN update info per connection
        /// </summary>
        public static Dictionary<IXenConnection, CdnPoolUpdateInfo> CdnUpdateInfoPerConnection
        {
            get
            {
                lock (_cdnUpdatesLock)
                    return _cdnUpdateInfoPerConnection.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public static void RemoveCdnInfoForConnection(IXenConnection connection)
        {
            lock (_cdnUpdatesLock)
            {
                _cdnUpdateInfoPerConnection.Remove(connection);
            }

            CdnUpdateInfoChanged?.Invoke(connection);
        }

        public static void CheckForCdnUpdates(IXenConnection connection)
        {
            var pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return;

            if (Helpers.XapiEqualOrGreater_23_18_0(connection))
            {
                if (pool.last_update_sync == Util.GetUnixMinDateTime() ||
                    connection.Cache.Hosts.All(h => h.latest_synced_updates_applied == latest_synced_updates_applied_state.yes))
                    return;
            }
            else
            {
                if (pool.repositories.Count == 0)
                    return;
            }

            if (!pool.allowed_operations.Contains(pool_allowed_operations.get_updates))
                return;

            var action = new CheckForCdnUpdatesAction(connection);
            action.Completed += CheckForCdnUpdatesAction_Completed;
            action.RunAsync();
        }

        private static void CheckForCdnUpdatesAction_Completed(ActionBase sender)
        {
            if (!(sender is CheckForCdnUpdatesAction action))
                return;

            bool succeeded = action.Succeeded;

            if (succeeded)
            {
                lock (_cdnUpdatesLock)
                {
                    _cdnUpdateInfoPerConnection[action.Pool.Connection] = action.Updates;
                }
            }

            CdnUpdateInfoChanged?.Invoke(action.Pool.Connection);
        }

        public static bool CheckCanDownloadUpdates()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.FileServiceUsername) &&
                   !string.IsNullOrEmpty(Properties.Settings.Default.FileServiceClientId);
        }

        public static void RemoveUpdate(Alert update)
        {
            lock (updateAlertsLock)
                updateAlerts.Remove(update);

            UpdateAlertCollectionChanged?.Invoke(new CollectionChangeEventArgs(CollectionChangeAction.Remove, update));
        }

        /// <summary>
        /// If AutomaticCheck is enabled it checks for updates regardless the
        /// value of the parameter userRequested. If AutomaticCheck is disabled it checks
        /// for all update types if userRequested is true.
        /// </summary>
        public static void CheckForClientUpdates(bool userRequested = false)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            if (Properties.Settings.Default.AllowXenCenterUpdates || userRequested)
            {
                var action = new DownloadClientUpdatesXmlAction(
                    Properties.Settings.Default.AllowXenCenterUpdates || userRequested,
                    UserAgent,
                    XenAdminConfigManager.Provider.GetCustomClientUpdatesXmlLocation() ?? BrandManager.XcUpdatesUrl,
                    !userRequested);
                
                action.Completed += DownloadClientUpdatesXmlAction_Completed;
                CheckForClientUpdatesStarted?.Invoke();
                action.RunAsync();
            }
        }

        /// <summary>
        /// If AutomaticCheck is enabled it checks for updates regardless the
        /// value of the parameter userRequested. If AutomaticCheck is disabled it checks
        /// for all update types if userRequested is true.
        /// </summary>
        public static bool CheckForServerUpdates(bool userRequested = false, bool asynchronous = true, Control owner = null)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return false;

            if (Properties.Settings.Default.AllowPatchesUpdates ||
                Properties.Settings.Default.AllowXenServerUpdates || userRequested)
            {
                var action = new DownloadCfuAction(
                    Properties.Settings.Default.AllowXenServerUpdates || userRequested,
                    Properties.Settings.Default.AllowPatchesUpdates || userRequested,
                    UserAgent,
                    XenAdminConfigManager.Provider.GetCustomCfuLocation() ?? BrandManager.CfuUrl,
                    !userRequested);

                action.Completed += DownloadCfuAction_Completed;
                CheckForServerUpdatesStarted?.Invoke();

                if (asynchronous)
                {
                    action.RunAsync();
                }
                else if (owner == null)
                {
                    action.RunSync(action.Session);
                }
                else
                {
                    using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                        dialog.ShowDialog(owner);
                }

                return action.Succeeded;
            }

            return false;
        }

        private static void DownloadClientUpdatesXmlAction_Completed(ActionBase sender)
        {
            if (!(sender is DownloadClientUpdatesXmlAction action))
                return;

            bool succeeded = action.Succeeded;

            if (succeeded)
            {
                lock (downloadedUpdatesLock)
                {
                    ClientVersions = action.ClientVersions;
                }
            }

            lock (updateAlertsLock)
            {
                updateAlerts.Clear();

                if (succeeded)
                {
                    var clientUpdateAlerts = NewClientUpdateAlerts(ClientVersions, Program.Version);
                    updateAlerts.AddRange(clientUpdateAlerts.Where(a => !a.IsDismissed()));
                }

                var xenServerUpdateAlerts = NewXenServerVersionAlerts(XenServerVersionsForAutoCheck);
                updateAlerts.AddRange(xenServerUpdateAlerts.Where(a => !a.CanIgnore));

                var xenServerPatchAlerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
                updateAlerts.AddRange(xenServerPatchAlerts.Where(a => !a.CanIgnore));
            }

            UpdateAlertCollectionChanged?.Invoke(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, UpdateAlerts));

            CheckForClientUpdatesCompleted?.Invoke();
        }

        private static void DownloadCfuAction_Completed(ActionBase sender)
        {
            if (!(sender is DownloadCfuAction action))
                return;

            bool succeeded = action.Succeeded;

            if (succeeded)
            {
                lock (downloadedUpdatesLock)
                {
                    XenServerVersionsForAutoCheck = action.XenServerVersionsForAutoCheck;
                    XenServerVersions = action.XenServerVersions;
                    XenServerPatches = action.XenServerPatches;
                }
            }

            lock (updateAlertsLock)
            {
                updateAlerts.Clear();

                var clientUpdateAlerts = NewClientUpdateAlerts(ClientVersions, Program.Version);
                updateAlerts.AddRange(clientUpdateAlerts.Where(a => !a.IsDismissed()));

                if (succeeded)
                {
                    var xenServerUpdateAlerts = NewXenServerVersionAlerts(XenServerVersionsForAutoCheck);
                    updateAlerts.AddRange(xenServerUpdateAlerts.Where(a => !a.CanIgnore));

                    var xenServerPatchAlerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
                    updateAlerts.AddRange(xenServerPatchAlerts.Where(a => !a.CanIgnore));
                }
            }

            UpdateAlertCollectionChanged?.Invoke(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, UpdateAlerts));

            CheckForServerUpdatesCompleted?.Invoke(action.Succeeded, action.Exception?.Message);

            CheckHotfixEligibility();
        }

        public static List<ClientUpdateAlert> NewClientUpdateAlerts(List<ClientVersion> clientVersions,
            Version currentProgramVersion)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return new List<ClientUpdateAlert>();

            var alerts = new List<ClientUpdateAlert>();
            ClientVersion latest = null, latestCr = null;

            if (clientVersions.Count != 0 && currentProgramVersion != new Version(0, 0, 0, 0))
            {
                var latestVersions = from v in clientVersions where v.Latest select v;
                latest = latestVersions.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                         latestVersions.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));

                if (IsSuitableForClientUpdateAlert(latest, currentProgramVersion))
                    alerts.Add(new ClientUpdateAlert(latest));

                var latestCrVersions = from v in clientVersions where v.LatestCr select v;
                latestCr = latestCrVersions.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                           latestCrVersions.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));

                if (latestCr != latest && IsSuitableForClientUpdateAlert(latestCr, currentProgramVersion))
                    alerts.Add(new ClientUpdateAlert(latestCr));
            }

            if (alerts.Count == 0)
            {
                log.InfoFormat("Not alerting XenCenter update - latest = {0},  latestcr = {1}, detected = {2}",
                    latest != null ? latest.VersionAndLang : "", latestCr != null ? latestCr.VersionAndLang : "", Program.VersionAndLanguage);
            }

            return alerts;
        }

        private static bool IsSuitableForClientUpdateAlert(ClientVersion toUse, Version currentProgramVersion)
        {
            if (toUse == null)
                return false;

            if (!toUse.Name.Contains(BrandManager.BrandConsole))
                return false;

            return toUse.Version > currentProgramVersion ||
                   toUse.Version == currentProgramVersion && toUse.Lang == Program.CurrentLanguage &&
                   !FriendlyNameManager.IsCultureLoaded(Program.CurrentCulture);
        }

        public static List<XenServerPatchAlert> NewXenServerPatchAlerts(List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return new List<XenServerPatchAlert>();

            var alerts = new List<XenServerPatchAlert>();

            var xenServerVersionsAsUpdates = xenServerVersions.Where(v => v.IsVersionAvailableAsAnUpdate);

            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                Host coordinator = Helpers.GetCoordinator(xenConnection);
                Pool pool = Helpers.GetPoolOfOne(xenConnection);
                List<Host> hosts = xenConnection.Cache.Hosts.ToList();
                if (coordinator == null || pool == null)
                    continue;

                var serverVersions = new List<XenServerVersion>();
                foreach (Host host in hosts)
                {
                    var serverVersion = GetServerVersions(host, xenServerVersions);
                    serverVersions.AddRange(serverVersion);
                }
                serverVersions = serverVersions.Distinct().ToList();

                if (serverVersions.Count == 0)
                    continue;

                foreach (XenServerVersion xenServerVersion in serverVersions)
                {
                    XenServerVersion version = xenServerVersion;
                    List<XenServerPatch> patches = xenServerPatches.FindAll(patch => version.Patches.Contains(patch));

                    if (patches.Count == 0)
                        continue;

                    foreach (XenServerPatch xenServerPatch in patches)
                    {
                        XenServerVersion newServerVersion = xenServerVersionsAsUpdates.FirstOrDefault(newVersion => newVersion.PatchUuid.Equals(xenServerPatch.Uuid, StringComparison.OrdinalIgnoreCase));

                        var alert = new XenServerPatchAlert(xenServerPatch, newServerVersion);
                        var existingAlert = alerts.Find(al => al.Equals(alert));

                        if (existingAlert != null)
                            alert = existingAlert;
                        else
                            alerts.Add(alert);

                        if (!xenConnection.IsConnected)
                            continue;

                        XenServerPatch serverPatch = xenServerPatch;

                        var noPatchHosts = hosts.Where(host => PatchCanBeInstalledOnHost(serverPatch, host)).ToList();
        
                        if (noPatchHosts.Count == hosts.Count)
                            alert.IncludeConnection(xenConnection);
                        else
                            alert.IncludeHosts(noPatchHosts);
                    }
                }
            }

            return alerts;
        }

        private static bool PatchCanBeInstalledOnHost(XenServerPatch serverPatch, Host host)
        {
            Debug.Assert(serverPatch != null);
            Debug.Assert(host != null);

            // A patch is applicable to host if the patch is amongst the current version patches
            var patchIsApplicable = GetServerVersions(host, XenServerVersions).Any(v => v.Patches.Contains(serverPatch));
            if (!patchIsApplicable)
                return false;

            // A patch can be installed on a host if:
            // 1. it is not already installed and
            // 2. the host has all the required patches installed and
            // 3. the host doesn't have any of the conflicting patches installed

            bool elyOrGreater = Helpers.ElyOrGreater(host);
            var appliedUpdates = host.AppliedUpdates();
            var appliedPatches = host.AppliedPatches();

            // 1. patch is not already installed 
            if (elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (!elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
                return false;

            // 2. the host has all the required patches installed
            if (serverPatch.RequiredPatches != null && serverPatch.RequiredPatches.Count > 0 &&
                !serverPatch.RequiredPatches
                .All(requiredPatchUuid =>
                    elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, requiredPatchUuid, StringComparison.OrdinalIgnoreCase))
                    || !elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, requiredPatchUuid, StringComparison.OrdinalIgnoreCase))
                    )
                )
                return false;

            // 3. the host doesn't have any of the conflicting patches installed
            if (serverPatch.ConflictingPatches != null && serverPatch.ConflictingPatches.Count > 0 &&
                serverPatch.ConflictingPatches
                .Any(conflictingPatchUuid =>
                    elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, conflictingPatchUuid, StringComparison.OrdinalIgnoreCase))
                    || !elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, conflictingPatchUuid, StringComparison.OrdinalIgnoreCase))
                    )
                )
                return false;

            return true;
        }

        /// <summary>
        /// If parameter is null, it returns latestcr XenCenter version if it is greater than current XC version,
        /// or null, if the current XC version is latestcr.
        /// If parameter is not null, it returns the minimum XenCenter version if it is greater than the current XC version,
        /// or null, if the minimum XC version couldn't be found or the current XC version is enough.
        /// </summary>
        public static ClientVersion GetRequiredClientVersion(XenServerVersion serverVersion)
        {
            if (ClientVersions.Count == 0)
                return null;

            var currentProgramVersion = Program.Version;
            if (currentProgramVersion == new Version(0, 0, 0, 0))
                return null;

            if (serverVersion == null)
                return ClientVersions.FirstOrDefault(xcv => xcv.LatestCr && xcv.Version > currentProgramVersion);

            var minXcVersion = serverVersion.MinimumXcVersion;
            if (minXcVersion == null)
                return null;

            var minimumXcVersion = ClientVersions.FirstOrDefault(xcv => xcv.Version == minXcVersion);
            return minimumXcVersion != null && minimumXcVersion.Version > currentProgramVersion
                ? minimumXcVersion
                : null;
        }
        
        /// <summary>
        /// This method returns the minimal set of patches for a host if this class already has information about them.
        /// Otherwise it returns empty list.
        /// Calling this function will not initiate a download or update.
        /// </summary>
        public static List<XenServerPatch> RecommendedPatchesForHost(Host host)
        {
            var recommendedPatches = new List<XenServerPatch>();

            if (XenServerVersions == null)
                return null;

            var serverVersions = GetServerVersions(host, XenServerVersions);

            if (serverVersions.Count != 0)
            {
                var minimumPatches = serverVersions[0].MinimalPatches;

                if (minimumPatches == null) //unknown
                    return null;

                bool elyOrGreater = Helpers.ElyOrGreater(host);

                var appliedPatches = host.AppliedPatches();
                var appliedUpdates = host.AppliedUpdates();

                if (elyOrGreater)
                {
                    recommendedPatches = minimumPatches.FindAll(p => !appliedUpdates.Any(au => string.Equals(au.uuid, p.Uuid, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    recommendedPatches = minimumPatches.FindAll(p => !appliedPatches.Any(ap => string.Equals(ap.uuid, p.Uuid, StringComparison.OrdinalIgnoreCase)));
                }

            }

            return recommendedPatches;
        }

        public static List<XenServerPatch> GetMinimalPatches(IXenConnection conn)
        {
            var version = GetCommonServerVersionOfHostsInAConnection(conn, XenServerVersions);
            return GetMinimalPatches(version);
        }

        public static List<XenServerPatch> GetMinimalPatches(Host host)
        {
            if (host == null || host.Connection == null || XenServerVersions== null)
                return null;
            var hostVersions = GetServerVersions(host, XenServerVersions);
            return GetMinimalPatches(hostVersions.FirstOrDefault());
        }

        private static List<XenServerPatch> GetMinimalPatches(XenServerVersion version)
        {
            if (version == null || version.MinimalPatches == null)
                return null;

            var minimalPatches = new List<XenServerPatch>(version.MinimalPatches);

            // if there is a "new version" update in the update sequence, also add the minimal patches of this new version
            if (minimalPatches.Count > 0)
            {
                // assuming that the new version update (if there is one) is the last one in the minimal patches list
                var lastUpdate = minimalPatches[minimalPatches.Count - 1];
                    
                var newServerVersion = XenServerVersions.FirstOrDefault(
                    v => v.IsVersionAvailableAsAnUpdate && v.PatchUuid.Equals(lastUpdate.Uuid, StringComparison.OrdinalIgnoreCase));

                if (newServerVersion != null && newServerVersion.MinimalPatches != null)
                    minimalPatches.AddRange(newServerVersion.MinimalPatches);
            }

            return minimalPatches;
        }

        /// <summary>
        /// Gets an upgrade sequence that contains a version upgrade, optionally followed by the minimal patches for the new version
        /// </summary>
        /// <param name="alert">The alert that refers the version-update</param>
        /// <param name="updateTheNewVersion">Also add the minimum patches for the new version (true) or not (false).</param>
        public static List<XenServerPatch> GetMinimalPatches(XenServerPatchAlert alert, bool updateTheNewVersion)
        {
            Debug.Assert(alert != null);

            var minimalPatches = new List<XenServerPatch> {alert.Patch};

            // if it's a version upgrade the min sequence will be this patch (the upgrade) and the min patches for the new version
            if (updateTheNewVersion && alert.NewServerVersion != null && alert.NewServerVersion.MinimalPatches != null)
            {
                minimalPatches.AddRange(alert.NewServerVersion.MinimalPatches);
            }

            return minimalPatches;
        }

        /// <summary>
        /// Gets all the patches for the given connection
        /// </summary>
        public static List<XenServerPatch> GetAllPatches(IXenConnection conn)
        {
            var version = GetCommonServerVersionOfHostsInAConnection(conn, XenServerVersions);
            return GetAllPatches(version);
        }

        /// <summary>
        /// Gets an upgrade sequence that contains a version upgrade, optionally followed by all the patches for the new version
        /// </summary>
        public static List<XenServerPatch> GetAllPatches(XenServerPatchAlert alert, bool updateTheNewVersion)
        {
            Debug.Assert(alert != null);

            var allPatches = new List<XenServerPatch> { alert.Patch };

            // if it's a version upgrade the update sequence will be this patch (the upgrade) and the patches for the new version
            if (updateTheNewVersion && alert.NewServerVersion != null)
            {
                var newVersionPatches = GetAllPatches(alert.NewServerVersion);
                if (newVersionPatches != null)
                    allPatches.AddRange(newVersionPatches);
            }

            return allPatches;
        }

        /// <summary>
        /// Gets all the patches for the given server version, including the cumulative updates and the patches on those
        /// </summary>
        private static List<XenServerPatch> GetAllPatches(XenServerVersion version)
        {
            if (version == null || version.Patches == null)
                return null;

            // exclude patches that are new versions (we will include the cumulative updates later)
            var excludedUuids = XenServerVersions.Where(v => v.IsVersionAvailableAsAnUpdate).Select(v => v.PatchUuid);

            var allPatches = new List<XenServerPatch>(version.Patches.Where(p => !excludedUuids.Contains(p.Uuid)));
            
            // if there is a "new version" update in the minimal patches (e.g. a cumulative update), also add this new version update and all the patches on it
            if (version.MinimalPatches != null && version.MinimalPatches.Count > 0)
            {
                // assuming that the new version update (if there is one) is the last one in the minimal patches list
                var lastUpdate = version.MinimalPatches[version.MinimalPatches.Count - 1];

                var newServerVersion = XenServerVersions.FirstOrDefault(
                    v => v.IsVersionAvailableAsAnUpdate && v.PatchUuid.Equals(lastUpdate.Uuid, StringComparison.OrdinalIgnoreCase));

                if (newServerVersion != null)
                {
                    allPatches.Add(lastUpdate);
                    if (newServerVersion.Patches != null)
                        allPatches.AddRange(newServerVersion.Patches);
                }
            }

            return allPatches;
        }

        /// <summary>
        /// Returns a XenServerVersion if all hosts of the pool have the same version
        /// Returns null if it is unknown or they don't match
        /// </summary>
        private static XenServerVersion GetCommonServerVersionOfHostsInAConnection(IXenConnection connection, List<XenServerVersion> xsVersions)
        {
            if (connection == null || xsVersions == null)
                return null;

            XenServerVersion commonXenServerVersion = null;
            List<Host> hosts = connection.Cache.Hosts.ToList();

            foreach (Host host in hosts)
            {
                var hostVersions = GetServerVersions(host, xsVersions);

                var foundVersion = hostVersions.FirstOrDefault();

                if (foundVersion == null)
                {
                    return null;
                }
                else
                {
                    if (commonXenServerVersion == null)
                    {
                        commonXenServerVersion = foundVersion;
                    }
                    else
                    {
                        if (commonXenServerVersion != foundVersion)
                            return null;
                    }
                }
            }

            return commonXenServerVersion;
        }

        public static List<XenServerPatch> GetPatchSequenceForHost(Host h, List<XenServerPatch> minimalPatches)
        {
            if (minimalPatches == null)
                return null;

            var appliedUpdateUuids = Helpers.ElyOrGreater(h)
                ? h.AppliedUpdates().Select(u => u.uuid).ToList()
                : h.AppliedPatches().Select(p => p.uuid).ToList();

            var neededPatches = new List<XenServerPatch>(minimalPatches);
            var sequence = new List<XenServerPatch>();

            //Iterate through minimalPatches once; in each iteration, move the first item from L0
            //that has its dependencies met to the end of the update schedule
            for (int i = 0; i < neededPatches.Count; i++)
            {
                var p = neededPatches[i];

                if (appliedUpdateUuids.Any(apu => string.Equals(apu, p.Uuid, StringComparison.OrdinalIgnoreCase)))
                    continue; //the patch has been applied

                if (p.RequiredPatches == null || p.RequiredPatches.Count == 0 // no requirements
                    || p.RequiredPatches.All(rp => //all the required patches are already in the sequence or have already been applied
                            sequence.Any(useqp => string.Equals(useqp.Uuid, rp, StringComparison.OrdinalIgnoreCase))
                            || appliedUpdateUuids.Any(apu => string.Equals(apu, rp, StringComparison.OrdinalIgnoreCase))
                    )
                )
                {
                    // this patch can be added to the upgrade sequence now
                    sequence.Add(p);

                    // by now the patch has either been added to the upgrade sequence or something already contains it among the installed patches
                    neededPatches.RemoveAt(i);

                    //resetting position - the loop will start on 0. item
                    i = -1;
                }
            }

            return sequence;
        }

        public static List<XenServerVersionAlert> NewXenServerVersionAlerts(List<XenServerVersion> xenServerVersions)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return new List<XenServerVersionAlert>();

            var latestVersion = xenServerVersions.FindAll(item => item.Latest).OrderByDescending(v => v.Version).FirstOrDefault();
            var latestCrVersion = xenServerVersions.FindAll(item => item.LatestCr).OrderByDescending(v => v.Version).FirstOrDefault();

            List<XenServerVersionAlert> alerts = new List<XenServerVersionAlert>();

            if (latestVersion != null)
                alerts.Add(CreateAlertForXenServerVersion(latestVersion));

            if (latestCrVersion != null && latestCrVersion != latestVersion)
                alerts.Add(CreateAlertForXenServerVersion(latestCrVersion));

            return alerts;
        }

        private static XenServerVersionAlert CreateAlertForXenServerVersion(XenServerVersion version)
        {
            var alert = new XenServerVersionAlert(version);

            // the patch that installs this version, if any
            var patch  = XenServerPatches.FirstOrDefault(p => p.Uuid.Equals(version.PatchUuid, StringComparison.OrdinalIgnoreCase));

            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                if (!xc.IsConnected)
                    continue;

                Host coordinator = Helpers.GetCoordinator(xc);
                Pool pool = Helpers.GetPoolOfOne(xc);
                List<Host> hosts = xc.Cache.Hosts.ToList();
                if (coordinator == null || pool == null)
                    continue;

                // Show the Upgrade alert for a host if:
                // - the host version is older than this version AND
                // - there is no patch (amongst the current version patches) that can update to this version OR, if there is a patch, the patch cannot be installed

                var outOfDateHosts = hosts.Where(host => new Version(Helpers.HostProductVersion(host)) < version.Version
                    && (patch == null || !PatchCanBeInstalledOnHost(patch, host))).ToList();

                if (outOfDateHosts.Count == hosts.Count)
                    alert.IncludeConnection(xc);
                else
                    alert.IncludeHosts(outOfDateHosts);
            }

            return alert;
        }

        public static List<XenServerVersion> GetServerVersions(Host host, List<XenServerVersion> xenServerVersions)
        {
            var serverVersions = xenServerVersions.FindAll(version =>
            {
                if (version.BuildNumber != string.Empty)
                    return (host.BuildNumberRaw() == version.BuildNumber);

                return Helpers.HostProductVersion(host) == version.Version.ToString();
            });
            return serverVersions;
        }

        public static void RefreshUpdateAlerts(UpdateType flags)
        {
            var alerts = new List<XenServerUpdateAlert>();

            if (flags.HasFlag(UpdateType.ServerVersion))
                alerts.AddRange(NewXenServerVersionAlerts(XenServerVersionsForAutoCheck));

            if (flags.HasFlag(UpdateType.ServerPatches))
                alerts.AddRange(NewXenServerPatchAlerts(XenServerVersions, XenServerPatches));

            bool changed = false;

            try
            {
                lock (updateAlertsLock)
                {
                    foreach (var alert in alerts)
                    {
                        var existingAlert = updateAlerts.FirstOrDefault(a => a.Equals(alert));

                        if (existingAlert != null && alert.CanIgnore)
                        {
                            updateAlerts.Remove(existingAlert);
                            changed = true;
                        }
                        else if (existingAlert is XenServerUpdateAlert updAlert)
                        {
                            updateAlerts.Remove(updAlert);
                            updAlert.CopyConnectionsAndHosts(alert);
                            if (!updateAlerts.Contains(updAlert))
                                updateAlerts.Add(updAlert);
                            changed = true;
                        }
                        else if (!alert.CanIgnore && !updateAlerts.Contains(alert))
                        {
                            updateAlerts.Add(alert);
                            changed = true;
                        }
                    }
                }

                if (changed)
                    UpdateAlertCollectionChanged?.Invoke(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, UpdateAlerts));
            }
            catch (Exception e)
            {
                log.Error("Failed to refresh the updates", e);
            }
        }

        public static void RestoreDismissedUpdates()
        {
            var actions = new List<AsyncAction>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                actions.Add(new RestoreDismissedUpdatesAction(connection));

            var action = new ParallelAction(Messages.RESTORE_DISMISSED_UPDATES, Messages.RESTORING, Messages.COMPLETED,
                actions, suppressHistory: true, showSubActionsDetails: false);
            action.Completed += ParallelAction_Completed;

            RestoreDismissedUpdatesStarted?.Invoke();

            action.RunAsync();
        }

        private static void ParallelAction_Completed(ActionBase action)
        {
            Program.Invoke(Program.MainWindow, () => CheckForServerUpdates(userRequested: true));
        }

        private static XenServerPatchAlert FindPatchAlert(Predicate<XenServerPatch> predicate)
        {
            var existingAlert = UpdateAlerts.FirstOrDefault(a => a is XenServerPatchAlert patchAlert && predicate(patchAlert.Patch));
            if (existingAlert != null)
                return existingAlert as XenServerPatchAlert;

            if (XenServerPatches.Count == 0)
                return null;

            var xenServerPatch = XenServerPatches.FirstOrDefault(p => predicate(p));
            if (xenServerPatch == null)
                return null;

            var newServerVersion = XenServerVersions.FirstOrDefault(v => v.IsVersionAvailableAsAnUpdate &&
                    v.PatchUuid.Equals(xenServerPatch.Uuid, StringComparison.OrdinalIgnoreCase));

            return new XenServerPatchAlert(xenServerPatch, newServerVersion);
        }

        public static XenServerPatchAlert FindPatchAlertByUuid(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
                return null;
            return FindPatchAlert(p => p.Uuid.Equals(uuid, StringComparison.OrdinalIgnoreCase));
        }

        public static XenServerPatchAlert FindPatchAlertByName(string patchName)
        {
            if (string.IsNullOrEmpty(patchName))
                return null;
            return FindPatchAlert(p => p.Name.Equals(patchName, StringComparison.OrdinalIgnoreCase));
        }

        public static hotfix_eligibility HotfixEligibility(Host host, out XenServerVersion xenServerVersion)
        {
            xenServerVersion = null;
            if (XenServerVersions == null)
                return hotfix_eligibility.all;

            xenServerVersion = GetServerVersions(host, XenServerVersions).FirstOrDefault();

            return xenServerVersion?.HotfixEligibility ?? hotfix_eligibility.all;
        }

        public static void CheckHotfixEligibility(IXenConnection connection)
        {
            var coordinator = Helpers.GetCoordinator(connection);
            if (coordinator == null)
                return;

            var hotfixEligibility = HotfixEligibility(coordinator, out var xenServerVersion);

            if (!HotfixEligibilityAlert.IsAlertNeeded(hotfixEligibility, xenServerVersion, !coordinator.IsFreeLicenseOrExpired()))
            {
                Alert.RemoveAlert(a => a is HotfixEligibilityAlert && connection.Equals(a.Connection));
                return;
            }

            var alertIndex = Alert.FindAlertIndex(a => a is HotfixEligibilityAlert alert && connection.Equals(alert.Connection) && xenServerVersion == alert.Version);
            if (alertIndex == -1)
            {
                Alert.RemoveAlert(a => a is HotfixEligibilityAlert && connection.Equals(a.Connection)); // ensure that there is no other alert for this connection
                Alert.AddAlert(new HotfixEligibilityAlert(connection, xenServerVersion));
            }
            else
                Alert.RefreshAlertAt(alertIndex);
        }

        private static void CheckHotfixEligibility()
        {
            var alerts = new List<HotfixEligibilityAlert>();
         
            foreach (var connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected)
                    continue;

                var coordinator = Helpers.GetCoordinator(connection);
                if (coordinator == null)
                    continue;
                
                var hotfixEligibility = HotfixEligibility(coordinator, out var xenServerVersion);
                if (!HotfixEligibilityAlert.IsAlertNeeded(hotfixEligibility, xenServerVersion, !coordinator.IsFreeLicenseOrExpired()))
                    continue;

                alerts.Add(new HotfixEligibilityAlert(connection, xenServerVersion));
            }

            Alert.RemoveAlert(a => a is HotfixEligibilityAlert);
            Alert.AddAlertRange(alerts);
        }

        [Flags]
        public enum UpdateType : short
        {
            None = 0,
            ServerPatches = 1,
            ServerVersion = 2,
            XenCenterVersion = 4
        }
    }
}
