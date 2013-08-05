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
using System.Linq;
using XenAdmin.Actions;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Network;


namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string LastSeenServerVersionKey = "XenCenter.LastSeenServerVersion";

        public static List<XenCenterVersion> XenCenterVersions = new List<XenCenterVersion>();
        public static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();
        public static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();

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

        private static void RunCheckForUpdates(Action<ActionBase> completedEvent)
        {
            DownloadUpdatesXmlAction action = new DownloadUpdatesXmlAction();
            action.Completed += completedEvent;
            action.RunAsync();
        }

        public static void CheckForUpdates(Action<ActionBase> completedEvent)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            RunCheckForUpdates(completedEvent);
        }

        public static void AutomaticCheckForUpdates()
        {
            if (!AllowUpdates)
                return;

            RunCheckForUpdates(action_Completed);
        }

        private static void action_Completed(ActionBase sender)
        {
            DownloadUpdatesXmlAction action = (DownloadUpdatesXmlAction)sender;
            if (!action.Succeeded)
                return;

            XenCenterVersions = action.XenCenterVersions;
            XenServerVersions = action.XenServerVersions;
            XenServerPatches = action.XenServerPatches;

            CheckXenCenterVersion();
            CheckServerPatches();
            CheckServerVersion();
        }

        private static XenCenterVersion GetLatestPublishedXenCenterVersion(List<XenCenterVersion> xenCenterVersions, Version programVersion)
        {
            if (xenCenterVersions.Count == 0 || programVersion == new Version(0, 0, 0, 0))
                return null;

            List<XenCenterVersion> latest = new List<XenCenterVersion>();
            foreach (XenCenterVersion v in xenCenterVersions)
                if (v.IsLatest)
                    latest.Add(v);

            XenCenterVersion to_use = latest.Find(new Predicate<XenCenterVersion>(delegate(XenCenterVersion xcv)
            {
                return xcv.Lang == Program.CurrentLanguage;
            }));

            if (to_use == null)
                to_use = latest.Find(new Predicate<XenCenterVersion>(delegate(XenCenterVersion xcv)
                {
                    return string.IsNullOrEmpty(xcv.Lang);
                }));

            return to_use;
        }

        private static void CheckXenCenterVersion()
        {
            if (!AllowUpdates || !Properties.Settings.Default.AllowXenCenterUpdates)
                return;

            XenCenterUpdateAlert alert = NewXenCenterVersionAlert(XenCenterVersions, true);
            if (alert != null)
                Alert.AddAlert(alert);
        }

        public static XenCenterUpdateAlert NewXenCenterVersionAlert(List<XenCenterVersion> xenCenterVersions,
            bool checkAlertIsAlreadyDismissed)
        {
            return NewXenCenterVersionAlert(xenCenterVersions, Program.Version, checkAlertIsAlreadyDismissed);
        }

        public static XenCenterUpdateAlert NewXenCenterVersionAlert(List<XenCenterVersion> xenCenterVersions, Version currentProgramVersion,
            bool checkAlertIsAlreadyDismissed)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            XenCenterVersion toUse = GetLatestPublishedXenCenterVersion(xenCenterVersions, currentProgramVersion);

            if (toUse == null)
                return null;

            if (checkAlertIsAlreadyDismissed && (toUse.VersionAndLang == Properties.Settings.Default.LatestXenCenterSeen))
            {
                log.Info(string.Format("Version {0} detected but already dismissed", toUse.VersionAndLang));
                return null;
            }

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
            List<XenServerPatch> xenServerPatches, bool checkAlertIsAlreadyDismissed)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            List<XenServerPatchAlert> alerts = GetServerPatchesAlerts(xenServerVersions, xenServerPatches,
                                                                      checkAlertIsAlreadyDismissed);
            return alerts.Where(alert => !alert.CanIgnore).ToList();
        }

        public static void CheckServerPatches()
        {
            if (!AllowUpdates || !Properties.Settings.Default.AllowPatchesUpdates)
                return;

            List<XenServerPatchAlert> alerts = GetServerPatchesAlerts(XenServerVersions, XenServerPatches, true);

            foreach (var alert in alerts)
            {
                Alert existingAlert = Alert.FindAlert(alert);
                if (existingAlert != null && alert.CanIgnore)
                    Alert.RemoveAlert(existingAlert);
                else if (existingAlert != null)
                    ((XenServerPatchAlert) existingAlert).CopyConnectionsAndHosts(alert);
                else if (!alert.CanIgnore)
                    Alert.AddAlert(alert);
            }
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
            List<XenServerPatch> xenServerPatches, bool checkAlertIsAlreadyDismissed)
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

                        if (checkAlertIsAlreadyDismissed && pool.other_config.ContainsKey(IgnorePatchAction.IgnorePatchKey))
                        {
                            List<string> ignorelist =
                                new List<string>(pool.other_config[IgnorePatchAction.IgnorePatchKey].Split(','));
                            if (ignorelist.Contains(xenServerPatch.Uuid))
                            {
                                // we dont want to show the alert
                                continue;
                            }
                        }

                        XenServerPatch serverPatch = xenServerPatch;
                        List<Host> noPatchHosts =
                            hosts.Where(host => !host.AppliedPatches().Any(patch => patch.uuid == serverPatch.Uuid)).ToList();

                        if (noPatchHosts.Count == hosts.Count)
                            alert.IncludeConnection(xenConnection);
                        else
                            alert.IncludeHosts(noPatchHosts);
                    }
                }
            }

            return alerts;
        }

        private static List<string> GetLatestSeenVersion(Pool pool)
        {
            if (!pool.other_config.ContainsKey(Updates.LastSeenServerVersionKey))
                return new List<string>();
            return new List<string>(pool.other_config[Updates.LastSeenServerVersionKey].Split(','));
        }

        public static void CheckServerVersion()
        {
            if (!AllowUpdates || !Properties.Settings.Default.AllowXenServerUpdates)
                return;

            XenServerUpdateAlert alert = NewServerVersionAlert(XenServerVersions, true);
            if (alert == null)
                return;
            
            Alert existingAlert = Alert.FindAlert(alert);
            if (existingAlert != null && alert.CanIgnore)
                Alert.RemoveAlert(existingAlert);
            else if (existingAlert != null)
                ((XenServerUpdateAlert)existingAlert).CopyConnectionsAndHosts(alert);
            else if (!alert.CanIgnore)
                Alert.AddAlert(alert);
        }

        public static XenServerUpdateAlert NewServerVersionAlert(List<XenServerVersion> xenServerVersions,
            bool checkAlertIsAlreadyDismissed)
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return null;

            List<XenServerVersion> latestVersions = xenServerVersions.FindAll(item => item.Latest);

            if (latestVersions.Count == 0)
                return null;

            XenServerVersion latestVersion = latestVersions[0];
            for (int i = 1; i < latestVersions.Count; i++)
            {
                XenServerVersion version = latestVersions[i];
                if (version.Version > latestVersion.Version)
                    latestVersion = version;
            }

            XenServerUpdateAlert alert = new XenServerUpdateAlert(latestVersion);

            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                Host master = Helpers.GetMaster(xc);
                Pool pool = Helpers.GetPoolOfOne(xc);
                List<Host> hosts = xc.Cache.Hosts.ToList();
                if (master == null || pool == null)
                    continue;

                //check if the latest version has been already dismissed
                if (checkAlertIsAlreadyDismissed && GetLatestSeenVersion(pool).Contains(latestVersion.VersionAndOEM))
                    return null;

                List<Host> outOfDateHosts =
                    hosts.Where(host => new Version(Helpers.HostProductVersion(host)) < latestVersion.Version).ToList();

                if (outOfDateHosts.Count == hosts.Count)
                    alert.IncludeConnection(xc);
                else
                    alert.IncludeHosts(outOfDateHosts);
            }

            if (alert.CanIgnore)
                return null;

            return alert;
        }

        /// <summary>
        /// Equivalent to CheckForUpdates().
        /// </summary>
        internal static void Tick(object sender, EventArgs e)
        {
            AutomaticCheckForUpdates();
        }
    }
}
