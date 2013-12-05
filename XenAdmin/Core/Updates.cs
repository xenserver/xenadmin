/* Copyright (c) Citrix Systems Inc. 
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


namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static event Action<bool, string> CheckForUpdatesCompleted;
        public static event Action CheckForUpdatesStarted;

        public const string LastSeenServerVersionKey = "XenCenter.LastSeenServerVersion";

        private static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();
        private static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();

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
                    updateAlerts.Add(update);
            }
            catch (Exception e)
            {
                log.Error("Failed to add update", e);
            }
        }

        private static void RemoveUpdate(Alert update)
        {
            try
            {
                lock (updateAlertsLock)
                    updateAlerts.Remove(update);
            }
            catch (Exception e)
            {
                log.Error("Failed to remove update", e);
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
                XenServerVersions = action.XenServerVersions;
                XenServerPatches = action.XenServerPatches;

                var xenCenterAlert = NewXenCenterVersionAlert(action.XenCenterVersions, Program.Version);
                if (xenCenterAlert != null)
                    updateAlerts.Add(xenCenterAlert);

                var xenServerUpdateAlert = NewServerVersionAlert(action.XenServerVersions);
                if (xenServerUpdateAlert != null)
                    updateAlerts.Add(xenServerUpdateAlert);

                var xenServerPatchAlerts = NewServerPatchesAlerts(action.XenServerVersions, action.XenServerPatches);
                if (xenServerPatchAlerts != null)
                {
                    foreach (var xenServerPatchAlert in xenServerPatchAlerts)
                        updateAlerts.Add(xenServerPatchAlert);
                }
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

        private static bool AllowUpdates
        {
            get
            {
                return !Helpers.CommonCriteriaCertificationRelease &&
                       (Properties.Settings.Default.AllowXenCenterUpdates ||
                        Properties.Settings.Default.AllowXenServerUpdates ||
                        Properties.Settings.Default.AllowPatchesUpdates);
            }
        }

        /// <summary>
        /// If AutomaticCheck is enabled it checks for updates regardless the
        /// value of the parameter force. If AutomaticCheck is disabled it only
        /// checks if force is true.
        /// </summary>
        public static void CheckForUpdates(bool force)
        {
            if (!AllowUpdates && !force)
                return;

            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            DownloadUpdatesXmlAction action = new DownloadUpdatesXmlAction();
            action.Completed += actionCompleted;

            if (CheckForUpdatesStarted != null)
                CheckForUpdatesStarted();

            action.RunAsync();
        }

        private static XenCenterVersion GetLatestPublishedXenCenterVersion(List<XenCenterVersion> xenCenterVersions, Version programVersion)
        {
            if (xenCenterVersions.Count == 0 || programVersion == new Version(0, 0, 0, 0))
                return null;

            var latest = from v in xenCenterVersions where v.IsLatest select v;

            return latest.FirstOrDefault(xcv => xcv.Lang == Program.CurrentLanguage) ??
                   latest.FirstOrDefault(xcv => string.IsNullOrEmpty(xcv.Lang));
        }

        public static XenCenterUpdateAlert NewXenCenterVersionAlert(List<XenCenterVersion> xenCenterVersions, Version currentProgramVersion)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            XenCenterVersion toUse = GetLatestPublishedXenCenterVersion(xenCenterVersions, currentProgramVersion);

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

        public static List<XenServerPatchAlert> NewServerPatchesAlerts(List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var alerts = GetServerPatchesAlerts(xenServerVersions, xenServerPatches);
            return alerts.Where(alert => !alert.CanIgnore).ToList();
        }

        private static XenServerPatchAlert GetServerPatchAlert(List<XenServerPatchAlert> alerts, XenServerPatch patch)
        {
            XenServerPatchAlert alert = new XenServerPatchAlert(patch);
            XenServerPatchAlert existingAlert = alerts.Find(al => al.Equals(alert));
            if (existingAlert != null)
                alert = existingAlert;
            else
                alerts.Add(alert);

            return alert;
        }

        private static List<XenServerPatchAlert> GetServerPatchesAlerts(List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches)
        {
            List<XenServerPatchAlert> alerts = new List<XenServerPatchAlert>();

            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                Host master = Helpers.GetMaster(xenConnection);
                Pool pool = Helpers.GetPoolOfOne(xenConnection);
                List<Host> hosts = xenConnection.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                List<XenServerVersion> serverVersions =
                    xenServerVersions.FindAll(version =>
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
                        XenServerPatchAlert alert = GetServerPatchAlert(alerts, xenServerPatch);
                        XenServerPatch serverPatch = xenServerPatch;
                        var noPatchHosts = hosts.Where(host => !host.AppliedPatches().Any(patch => patch.uuid == serverPatch.Uuid));

                        if (noPatchHosts.Count() == hosts.Count)
                            alert.IncludeConnection(xenConnection);
                        else
                            alert.IncludeHosts(noPatchHosts);
                    }
                }
            }

            return alerts;
        }

        public static XenServerUpdateAlert NewServerVersionAlert(List<XenServerVersion> xenServerVersions)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            var latestVersion = xenServerVersions.FindAll(item => item.Latest).OrderByDescending(v => v.Version).FirstOrDefault();
            if (latestVersion == null)
                return null;
            
            XenServerUpdateAlert alert = new XenServerUpdateAlert(latestVersion);

            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
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

            if (alert.CanIgnore)
                return null;

            return alert;
        }

        public static void CheckServerVersion()
        {
            if (!AllowUpdates || !Properties.Settings.Default.AllowXenServerUpdates)
                return;

            var alert = NewServerVersionAlert(XenServerVersions);
            if (alert == null)
                return;

            var existingAlert = FindUpdate(alert);

            if (existingAlert != null && alert.CanIgnore)
                RemoveUpdate(existingAlert);
            else if (existingAlert != null)
                ((XenServerUpdateAlert)existingAlert).CopyConnectionsAndHosts(alert);
            else if (!alert.CanIgnore)
                AddUpate(alert);
        }

        public static void CheckServerPatches()
        {
            if (!AllowUpdates || !Properties.Settings.Default.AllowPatchesUpdates)
                return;

            var alerts = GetServerPatchesAlerts(XenServerVersions, XenServerPatches);

            foreach (var alert in alerts)
            {
                var existingAlert = FindUpdate(alert);
                
                if (existingAlert != null && alert.CanIgnore)
                    RemoveUpdate(existingAlert);
                else if (existingAlert != null)
                    ((XenServerPatchAlert)existingAlert).CopyConnectionsAndHosts(alert);
                else if (!alert.CanIgnore)
                    AddUpate(alert);
            }
        }
    }
}
