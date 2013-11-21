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
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Network;


namespace XenAdmin.Core
{
    public class Updates
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static event Action<bool, string> CheckForUpdatesCompleted;

        public const string LastSeenServerVersionKey = "XenCenter.LastSeenServerVersion";

        private static readonly List<Alert> updateAlerts = new List<Alert>();
        public static List<Alert> UpdateAlerts
        {
            get { return updateAlerts; }
        }


        private static void actionCompleted(ActionBase sender)
        {
            Program.AssertOffEventThread();
            DownloadUpdatesXmlAction action = sender as DownloadUpdatesXmlAction;

            bool succeeded = false;
            string errorMessage = string.Empty;

            if (action != null)
            {
                succeeded = action.Succeeded;
                updateAlerts.Clear();

                if (succeeded)
                {
                    var xenCenterAlert = NewXenCenterVersionAlert(action.XenCenterVersions, false);
                    if (xenCenterAlert != null)
                        updateAlerts.Add(xenCenterAlert);

                    var xenServerUpdateAlert = NewServerVersionAlert(action.XenServerVersions, false);
                    if (xenServerUpdateAlert != null)
                        updateAlerts.Add(xenServerUpdateAlert);

                    var xenServerPatchAlerts = NewServerPatchesAlerts(action.XenServerVersions, action.XenServerPatches, false);
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
                    {
                        errorMessage = Messages.AVAILABLE_UPDATES_INTERNAL_ERROR;
                    }
                }
            }

            if (CheckForUpdatesCompleted != null)
                CheckForUpdatesCompleted(succeeded, errorMessage);
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
 
        public static void CheckForUpdates()
        {
            if (Helpers.CommonCriteriaCertificationRelease)
                return;

            DownloadUpdatesXmlAction action = new DownloadUpdatesXmlAction();
            action.Completed += actionCompleted;
            action.RunAsync();
        }

        public static void AutomaticCheckForUpdates()
        {
            if (!AllowUpdates)
                return;

            CheckForUpdates();
        }

        private static XenCenterVersion GetLatestPublishedXenCenterVersion(List<XenCenterVersion> xenCenterVersions, Version programVersion)
        {
            if (xenCenterVersions.Count == 0 || programVersion == new Version(0, 0, 0, 0))
                return null;

            List<XenCenterVersion> latest = new List<XenCenterVersion>();
            foreach (XenCenterVersion v in xenCenterVersions)
                if (v.IsLatest)
                    latest.Add(v);

            return latest.Find(xcv => xcv.Lang == Program.CurrentLanguage) ??
                   latest.Find(xcv => string.IsNullOrEmpty(xcv.Lang));
        }

        private static XenCenterUpdateAlert NewXenCenterVersionAlert(List<XenCenterVersion> xenCenterVersions,
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
            if (!pool.other_config.ContainsKey(LastSeenServerVersionKey))
                return new List<string>();
            return new List<string>(pool.other_config[LastSeenServerVersionKey].Split(','));
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
    }
}
