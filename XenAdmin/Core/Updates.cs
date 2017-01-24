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
using System.ComponentModel;
using System.Linq;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Network;
using System.Diagnostics;
using System.Windows.Forms;
using XenAdmin.Dialogs;

namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static event Action<bool, string> CheckForUpdatesCompleted;
        public static event Action CheckForUpdatesStarted;
        public static event Action RestoreDismissedUpdatesStarted;

        private static readonly object downloadedUpdatesLock = new object();
        private static List<XenServerVersion> XenServerVersionsForAutoCheck = new List<XenServerVersion>();
        private static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();
        private static List<XenCenterVersion> XenCenterVersions = new List<XenCenterVersion>();
        public static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();

        private static readonly object updateAlertsLock = new object();
        private static readonly ChangeableList<Alert> updateAlerts = new ChangeableList<Alert>();
        
        public static IEnumerable<Alert> UpdateAlerts
        {
            get { return updateAlerts; }
        }

        public static int UpdateAlertsCount
        {
            get { return updateAlerts.Count; }
        }

        private static void AddUpate(Alert update)
        {
            try
            {
                lock (updateAlertsLock)
                {
                    if(!updateAlerts.Contains(update))
                    {
                        updateAlerts.Add(update);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Failed to add update", e);
            }
        }

        public static void RemoveUpdate(Alert update)
        {
            try
            {
                lock (updateAlertsLock)
                {
                    if(updateAlerts.Contains(update))
                    {
                        updateAlerts.Remove(update);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Failed to remove update", e);
            }
        }

        /// <summary>
        /// Dismisses the updates in the given list i.e. they are added in the 
        /// other_config list of each pool and removed from the Updates.UpdateAlerts list.
        /// </summary>
        /// <param name="toBeDismissed"></param>
        public static void DismissUpdates(List<Alert> toBeDismissed)
        {
            if (toBeDismissed.Count == 0)
                return;

            foreach(IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!Alert.AllowedToDismiss(connection))
                    continue;

                XenAPI.Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool == null)
                    continue;

                Dictionary<string, string> other_config = pool.other_config;

                foreach (Alert alert in toBeDismissed)
                {
                  
                    if (alert is XenServerPatchAlert)
                    {

                        if (other_config.ContainsKey(IgnorePatchAction.IgnorePatchKey))
                        {
                            List<string> current = new List<string>(other_config[IgnorePatchAction.IgnorePatchKey].Split(','));
                            if (current.Contains(((XenServerPatchAlert)alert).Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                                continue;
                            current.Add(((XenServerPatchAlert)alert).Patch.Uuid);
                            other_config[IgnorePatchAction.IgnorePatchKey] = string.Join(",", current.ToArray());
                        }
                        else
                        {
                            other_config.Add(IgnorePatchAction.IgnorePatchKey, ((XenServerPatchAlert)alert).Patch.Uuid);
                        }
                    }
                    if (alert is XenServerVersionAlert)
                    {

                        if (other_config.ContainsKey(IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY))
                        {
                            List<string> current = new List<string>(other_config[IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                            if (current.Contains(((XenServerVersionAlert)alert).Version.VersionAndOEM))
                                continue;
                            current.Add(((XenServerVersionAlert)alert).Version.VersionAndOEM);
                            other_config[IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY] = string.Join(",", current.ToArray());
                        }
                        else
                        {
                            other_config.Add(IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY, ((XenServerVersionAlert)alert).Version.VersionAndOEM);
                        }                       
                    }
                    Updates.RemoveUpdate(alert);
                }

                XenAPI.Pool.set_other_config(connection.Session, pool.opaque_ref, other_config);
            }
        }

        private static Alert FindUpdate(Alert alert)
        {
            lock (updateAlertsLock)
                return FindUpdate(a => a.Equals(alert));
        }

        private static Alert FindUpdate(Predicate<Alert> predicate)
        {
            lock (updateAlertsLock)
                return updateAlerts.Find(predicate);
        }


        /// <summary>
        /// If AutomaticCheck is enabled it checks for updates regardless the
        /// value of the parameter force. If AutomaticCheck is disabled it only
        /// checks if force is true.
        /// </summary>
        public static void CheckForUpdates(bool force)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            if (Properties.Settings.Default.AllowXenCenterUpdates ||
                Properties.Settings.Default.AllowXenServerUpdates ||
                Properties.Settings.Default.AllowPatchesUpdates || force)
            {
                DownloadUpdatesXmlAction action = new DownloadUpdatesXmlAction(
                    Properties.Settings.Default.AllowXenCenterUpdates || force,
                    Properties.Settings.Default.AllowXenServerUpdates || force,
                    Properties.Settings.Default.AllowPatchesUpdates || force,
                    Updates.CheckForUpdatesUrl);

                action.Completed += actionCompleted;

                if (CheckForUpdatesStarted != null)
                    CheckForUpdatesStarted();

                action.RunAsync();
            }
        }

        /// <summary>
        /// It does exactly what CheckForUpdates(true) does, but this is sync and shows an ActionProgressDialog while running
        /// </summary>
        /// <returns>true if the action has succeeded</returns>
        public static bool CheckForUpdatesSync(Control parentForProgressDialog)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return false;

            DownloadUpdatesXmlAction action = new DownloadUpdatesXmlAction(true, true, true, Updates.CheckForUpdatesUrl);
            action.Completed += actionCompleted;

            if (CheckForUpdatesStarted != null)
                CheckForUpdatesStarted();

            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dialog.ShowDialog(parentForProgressDialog);

            return action.Succeeded;
        }

        public static string CheckForUpdatesUrl
        {
            get
            {
                return Registry.CustomUpdatesXmlLocation ?? Branding.CheckForUpdatesUrl;
            }
        }

        private static void actionCompleted(ActionBase sender)
        {
            Program.AssertOffEventThread();

            DownloadUpdatesXmlAction action = sender as DownloadUpdatesXmlAction;
            if (action == null)
                return;

            bool succeeded = action.Succeeded;
            string errorMessage = string.Empty;

            lock (updateAlertsLock)
                updateAlerts.Clear();

            if (succeeded)
            {
                lock (downloadedUpdatesLock)
                {
                    XenCenterVersions = action.XenCenterVersions;

                    XenServerVersionsForAutoCheck = action.XenServerVersionsForAutoCheck;

                    XenServerVersions = action.XenServerVersions;

                    XenServerPatches = action.XenServerPatches;
                }

                var xenCenterAlert = NewXenCenterUpdateAlert(XenCenterVersions, Program.Version);
                if (xenCenterAlert != null && !xenCenterAlert.IsDismissed())
                    updateAlerts.Add(xenCenterAlert);

                var xenServerUpdateAlert = NewXenServerVersionAlert(XenServerVersionsForAutoCheck);
                if (xenServerUpdateAlert != null && !xenServerUpdateAlert.CanIgnore)
                    updateAlerts.Add(xenServerUpdateAlert);

                var xenServerPatchAlerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
                if (xenServerPatchAlerts != null)
                    updateAlerts.AddRange(xenServerPatchAlerts.Where(alert => !alert.CanIgnore));
            }
            else
            {
                if (action.Exception != null)
                {
                    if (action.Exception is System.Net.Sockets.SocketException)
                    {
                        errorMessage = Messages.AVAILABLE_UPDATES_NETWORK_ERROR;
                    }
                    else
                    {
                        // Clean up and remove excess newlines, carriage returns, trailing nonsense
                        string errorText = action.Exception.Message.Trim();
                        errorText = System.Text.RegularExpressions.Regex.Replace(errorText, @"\r\n+", "");
                        errorMessage = string.Format(Messages.AVAILABLE_UPDATES_ERROR, errorText);
                    }
                }

                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = Messages.AVAILABLE_UPDATES_INTERNAL_ERROR;
            }

            if (CheckForUpdatesCompleted != null)
                CheckForUpdatesCompleted(succeeded, errorMessage);
        }


        public static void RegisterCollectionChanged(CollectionChangeEventHandler handler)
        {
            updateAlerts.CollectionChanged += handler;
        }

        public static void DeregisterCollectionChanged(CollectionChangeEventHandler handler)
        {
            updateAlerts.CollectionChanged -= handler;
        }


        public static XenCenterUpdateAlert NewXenCenterUpdateAlert(List<XenCenterVersion> xenCenterVersions, Version currentProgramVersion)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            XenCenterVersion toUse = null;
            if (xenCenterVersions.Count != 0 && currentProgramVersion != new Version(0, 0, 0, 0))
            {
                var latest = from v in xenCenterVersions where v.IsLatest select v;

                toUse = latest.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                        latest.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));
            }

            if (toUse == null)
                return null;

            if (toUse.Version > currentProgramVersion ||
                (toUse.Version == currentProgramVersion && toUse.Lang == Program.CurrentLanguage &&
                 !PropertyManager.IsCultureLoaded(Program.CurrentCulture)))
            {
                return new XenCenterUpdateAlert(toUse);
            }

            log.Info(string.Format("Not alerting XenCenter update - lastest = {0}, detected = {1}",
                                   toUse.VersionAndLang, Program.VersionAndLanguage));
            return null;
        }

        public static List<XenServerPatchAlert> NewXenServerPatchAlerts(List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var alerts = new List<XenServerPatchAlert>();

            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                Host master = Helpers.GetMaster(xenConnection);
                Pool pool = Helpers.GetPoolOfOne(xenConnection);
                List<Host> hosts = xenConnection.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                var serverVersions = xenServerVersions.FindAll(version =>
                                                  {
                                                      if (version.BuildNumber != string.Empty)
                                                          return (master.BuildNumberRaw == version.BuildNumber);

                                                      return Helpers.HostProductVersionWithOEM(master) == version.VersionAndOEM
                                                             || (version.Oem != null && Helpers.OEMName(master).StartsWith(version.Oem)
                                                                 && Helpers.HostProductVersion(master) == version.Version.ToString());
                                                  });

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
                        var alert = new XenServerPatchAlert(xenServerPatch);
                        var existingAlert = alerts.Find(al => al.Equals(alert));

                        if (existingAlert != null)
                            alert = existingAlert;
                        else
                            alerts.Add(alert);

                        if (!xenConnection.IsConnected)
                            continue;

                        XenServerPatch serverPatch = xenServerPatch;

                        // A patch can be installed on a host if:
                        // 1. it is not already installed and
                        // 2. the host has all the required patches installed and
                        // 3. the host doesn't have any of the conflicting patches installed

                        var noPatchHosts = hosts.Where(host =>
                            {
                                bool elyOrGreater = Helpers.ElyOrGreater(host);
                                var appliedUpdates = host.AppliedUpdates();
                                var appliedPatches = host.AppliedPatches();

                                // 1. patch is not already installed 
                                if (elyOrGreater && appliedUpdates.Any(update => string.Equals(update.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
                                        return false;
                                else if (!elyOrGreater && appliedPatches.Any(patch => string.Equals(patch.uuid, serverPatch.Uuid, StringComparison.OrdinalIgnoreCase)))
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
                            });

                        if (noPatchHosts.Count() == hosts.Count)
                            alert.IncludeConnection(xenConnection);
                        else
                            alert.IncludeHosts(noPatchHosts);
                    }
                }
            }

            return alerts;
        }
        
        /// <summary>
        /// This method returns the minimal set of patches for a host if this class already has information about them. Otherwise it returns empty list.
        /// Calling this function will not initiate a download or update.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static List<XenServerPatch> RecommendedPatchesForHost(Host host)
        {
            var recommendedPatches = new List<XenServerPatch>();

            if (XenServerVersions == null)
                return recommendedPatches;

            var serverVersions = XenServerVersions.FindAll(version =>
            {
                if (version.BuildNumber != string.Empty)
                    return (host.BuildNumberRaw == version.BuildNumber);

                return Helpers.HostProductVersionWithOEM(host) == version.VersionAndOEM
                       || (version.Oem != null && Helpers.OEMName(host).StartsWith(version.Oem)
                           && Helpers.HostProductVersion(host) == version.Version.ToString());
            });

            if (serverVersions.Count != 0)
            {
                var minimumPatches = serverVersions[0].MinimalPatches;

                if (minimumPatches == null) //unknown
                    return recommendedPatches;

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

        public static UpgradeSequence GetUpgradeSequence(IXenConnection conn)
        {
            if (XenServerVersions == null)
                return null;

            Host master = Helpers.GetMaster(conn);
            if (master == null)
                return null;

            var version = GetCommonServerVersionOfHostsInAConnection(conn, XenServerVersions);

            if (version != null)
            {
                if (version.MinimalPatches == null)
                    return null;

                var uSeq = new UpgradeSequence();
                uSeq.MinimalPatches = version.MinimalPatches;

                List<Host> hosts = conn.Cache.Hosts.ToList();
                
                foreach (Host h in hosts)
                {
                    uSeq[h] = GetUpgradeSequenceForHost(h, uSeq.MinimalPatches);
                }

                return uSeq;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a XenServerVersion if all hosts of the pool have the same version
        /// Returns null if it is unknown or they don't match
        /// </summary>
        /// <returns></returns>
        private static XenServerVersion GetCommonServerVersionOfHostsInAConnection(IXenConnection connection, List<XenServerVersion> xsVersions)
        {
            XenServerVersion commonXenServerVersion = null;

            if (connection == null)
                return null;
            
            List<Host> hosts = connection.Cache.Hosts.ToList();

            foreach (Host host in hosts)
            {
                var hostVersions = xsVersions.FindAll(version =>
                {
                    if (version.BuildNumber != string.Empty)
                        return (host.BuildNumberRaw == version.BuildNumber);

                    return Helpers.HostProductVersionWithOEM(host) == version.VersionAndOEM
                           || (version.Oem != null && Helpers.OEMName(host).StartsWith(version.Oem)
                               && Helpers.HostProductVersion(host) == version.Version.ToString());
                });

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

        private static List<XenServerPatch> GetUpgradeSequenceForHost(Host h, List<XenServerPatch> latestPatches)
        {
            var sequence = new List<XenServerPatch>();
            var appliedUpdateUuids = new List<string>();

            bool elyOrGreater = Helpers.ElyOrGreater(h);

            if (elyOrGreater)
            {
                appliedUpdateUuids = h.AppliedUpdates().Select(u => u.uuid).ToList();
            }
            else
            {
                appliedUpdateUuids = h.AppliedPatches().Select(p => p.uuid).ToList();
            }

            var neededPatches = new List<XenServerPatch>(latestPatches);

            //Iterate through latestPatches once; in each iteration, move the first item from L0 that has its dependencies met to the end of the Update Schedule (US)
            for (int ii = 0; ii < neededPatches.Count; ii++)
            {
                var p = neededPatches[ii];

                //checking requirements
                if (//not applied yet
                    !appliedUpdateUuids.Any(apu => string.Equals(apu, p.Uuid, StringComparison.OrdinalIgnoreCase))
                    // and either no requirements or they are meet
                    && (p.RequiredPatches == null
                    || p.RequiredPatches.Count == 0
                    // all requirements met?
                    || p.RequiredPatches.All(
                        rp =>
                            //sequence already has the required-patch
                            (sequence.Count != 0 && sequence.Any(useqp => string.Equals(useqp.Uuid, rp, StringComparison.OrdinalIgnoreCase)))

                            //the required-patch has already been applied
                            || (appliedUpdateUuids.Count != 0 && appliedUpdateUuids.Any(apu => string.Equals(apu, rp, StringComparison.OrdinalIgnoreCase)))
                        )
                    ))
                {
                    // this patch can be added to the upgrade sequence now
                    sequence.Add(p);

                    // by now the patch has either been added to the upgrade sequence or something already contains it among the installed patches
                    neededPatches.RemoveAt(ii);

                    //resetting position - the loop will start on 0. item
                    ii = -1;
                }
            }

            return sequence;
        }

        public class UpgradeSequence : Dictionary<Host, List<XenServerPatch>>
        {
            private IEnumerable<XenServerPatch> AllPatches
            {
                get
                {
                    foreach (var patches in this.Values)
                        foreach(var patch in patches)
                            yield return patch;
                }
            }

            public List<XenServerPatch> UniquePatches
            {
                get
                {
                    var uniquePatches = new List<XenServerPatch>();

                    foreach (var mp in MinimalPatches)
                    {
                        if (AllPatches.Any(p => p.Uuid == mp.Uuid))
                        {
                            uniquePatches.Add(mp);
                        }
                    }

                    return uniquePatches;
                }
            }

            public bool AllHostsUpToDate
            {
                get
                {
                    if (this.Count == 0)
                        return false;

                    foreach (var host in this.Keys)
                    {
                        if (this[host].Count > 0)
                            return false;
                    }

                    return true;
                }
            }

            public List<XenServerPatch> MinimalPatches
            {
                set;
                get;
            }
        }

        public static XenServerVersionAlert NewXenServerVersionAlert(List<XenServerVersion> xenServerVersions)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var latestVersion = xenServerVersions.FindAll(item => item.Latest).OrderByDescending(v => v.Version).FirstOrDefault();
            if (latestVersion == null)
                return null;

            var alert = new XenServerVersionAlert(latestVersion);

            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                if (!xc.IsConnected)
                    continue;

                Host master = Helpers.GetMaster(xc);
                Pool pool = Helpers.GetPoolOfOne(xc);
                List<Host> hosts = xc.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                var outOfDateHosts = hosts.Where(host => new Version(Helpers.HostProductVersion(host)) < latestVersion.Version);

                if (outOfDateHosts.Count() == hosts.Count)
                    alert.IncludeConnection(xc);
                else
                    alert.IncludeHosts(outOfDateHosts);
            }

            return alert;
        }


        public static void CheckServerVersion()
        {
            var alert = NewXenServerVersionAlert(XenServerVersionsForAutoCheck);
            if (alert == null)
                return;

            CheckUpdate(alert);
        }

        public static void CheckServerPatches()
        {
            var alerts = NewXenServerPatchAlerts(XenServerVersions, XenServerPatches);
            if (alerts == null)
                return;

            foreach (var alert in alerts)
                CheckUpdate(alert);
        }

        private static void CheckUpdate(XenServerUpdateAlert alert)
        {
            var existingAlert = FindUpdate(alert);

            if (existingAlert != null && alert.CanIgnore)
                RemoveUpdate(existingAlert);
            else if (existingAlert != null)
                ((XenServerUpdateAlert)existingAlert).CopyConnectionsAndHosts(alert);
            else if (!alert.CanIgnore)
                AddUpate(alert);
        }

        public static void RestoreDismissedUpdates()
        {
            var actions = new List<AsyncAction>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                actions.Add(new RestoreDismissedUpdatesAction(connection));

            var action = new ParallelAction(Messages.RESTORE_DISMISSED_UPDATES, Messages.RESTORING, Messages.COMPLETED, actions, true, false);
            action.Completed += action_Completed;

            if (RestoreDismissedUpdatesStarted != null)
                RestoreDismissedUpdatesStarted();

            action.RunAsync();
        }

        private static void action_Completed(ActionBase action)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                Properties.Settings.Default.LatestXenCenterSeen = "";
                Settings.TrySaveSettings();

                CheckForUpdates(true);
            });
        }
    }
}
