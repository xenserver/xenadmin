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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Alerts;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAPI;
using System.Linq;

namespace XenAdmin.Core
{
    public static class HelpersGUI
    {
        static HelpersGUI()
        {
        }

        public static bool WindowIsOnScreen(Point location, Size size)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    Rectangle r = new Rectangle(location, size);
                    return screen.WorkingArea.Contains(r);
                }
            }

            return false;
        }

        public static int BALLOON_DURATION = 5000;
        public static void ShowBalloonMessage(Control control, ToolTip toolTip, string caption = " ")
        {
            toolTip.Hide(control);
            toolTip.RemoveAll();
            toolTip.IsBalloon = true;
            toolTip.Active = true;
            toolTip.SetToolTip(control, caption); // required to improve the balloon position.
            toolTip.Show(caption, control, BALLOON_DURATION);
        }

        /// <summary>
        /// Brings the specified form to the front.
        /// </summary>
        /// <param name="f">The form to be brought to the front.</param>
        public static void BringFormToFront(Form f)
        {
            if (f.WindowState == FormWindowState.Minimized)
            {
                f.WindowState = FormWindowState.Normal;
            }

            f.BringToFront();
            f.Activate();
        }

        public static Dictionary<Host, Host> CheckHostIQNsDiffer()
        {
            Dictionary<Host, string> hosts = new Dictionary<Host, string>();
            Dictionary<Host, string> hostduplicates = new Dictionary<Host, string>();
            Dictionary<Host, Host> output = new Dictionary<Host, Host>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host host in connection.Cache.Hosts)
                {
                    var iscsiIqn = host.GetIscsiIqn();
                    if (!hosts.ContainsValue(iscsiIqn) || iscsiIqn == "")
                        hosts.Add(host, iscsiIqn);
                    else
                        hostduplicates[host] = iscsiIqn;
                }
            }

            foreach (Host host in hostduplicates.Keys)
            {
                foreach (Host host2 in hosts.Keys)
                {
                    if (host.GetIscsiIqn() == host2.GetIscsiIqn())
                        output.Add(host, host2);
                }
            }
            return output;
        }

        public static List<Host> CheckHostIQNsExist()
        {
            List<Host> badHosts = new List<Host>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host host in connection.Cache.Hosts)
                {
                    Host_metrics metrics = connection.Resolve<Host_metrics>(host.metrics);
                    if (metrics != null && metrics.live && host.GetIscsiIqn() == "")
                        badHosts.Add(host);
                }
            }
            return badHosts;
        }

        /// <summary>
        /// Finds the EnableHAAction or DisableHAAction in progress that pertains to the given connection, or null
        /// if there is no such action.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveHaAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                EnableHAAction enableAction = action as EnableHAAction;
                if (enableAction != null && !enableAction.Cancelled && enableAction.Connection == connection)
                    return enableAction;

                DisableHAAction disableAction = action as DisableHAAction;
                if (disableAction != null && !disableAction.Cancelled && disableAction.Connection == connection)
                    return disableAction;
            }
            return null;
        }

        /// <summary>
        /// Does the given VM have any running tasks?  Must be called on the event thread.
        /// </summary>
        internal static bool HasRunningTasks(VM vm)
        {
            Program.AssertOnEventThread(); // Program.History is event-thread only

            if (vm == null)
                return false;

            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                if (!vm.Equals(action.VM))
                    continue;

                return true;
            }

            return false;
        }


        /// <summary>
        /// WLB Optimize Pool
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveOptAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted || action.Connection != connection)
                    continue;

                if (action is WlbOptimizePoolAction optAction)
                    return optAction;

                if (action is WlbRetrieveRecommendationsAction optRecAction)
                    return optRecAction;
            }
            return null;
        }

        /// <summary>
        /// Finds the WLBAction in progress that pertains to the given connection, or null
        /// if there is no such action.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="connection">May not be null.</param>
        /// <returns></returns>
        internal static AsyncAction FindActiveWLBAction(IXenConnection connection)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted)
                    continue;

                InitializeWLBAction configureAction = action as InitializeWLBAction;
                if (configureAction != null && !configureAction.Cancelled && configureAction.Connection == connection)
                    return configureAction;

                EnableWLBAction enableAction = action as EnableWLBAction;
                if (enableAction != null && !enableAction.Cancelled && enableAction.Connection == connection)
                    return enableAction;

                DisableWLBAction disableAction = action as DisableWLBAction;
                if (disableAction != null && !disableAction.Cancelled && disableAction.Connection == connection)
                    return disableAction;

                RetrieveWlbConfigurationAction retrieveAction = action as RetrieveWlbConfigurationAction;
                if (retrieveAction != null && !retrieveAction.Cancelled && retrieveAction.Connection == connection)
                    return retrieveAction;

                SendWlbConfigurationAction sendAction = action as SendWlbConfigurationAction;
                if (sendAction != null && !sendAction.Cancelled && sendAction.Connection == connection)
                    return sendAction;
            }
            return null;
        }

        /// <summary>
        /// Whether there is a HostAction in progress that pertains to the given host.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="host">May not be null.</param>
        /// <returns></returns>
        internal static bool HasActiveHostAction(Host host)
        {
            Program.AssertOnEventThread();
            foreach (ActionBase action in ConnectionsManager.History)
            {
                 HostAbstractAction hostAction = action as HostAbstractAction;
                if (hostAction != null && !hostAction.Cancelled && !hostAction.IsCompleted && host.Connection == hostAction.Connection)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool iSCSIisUsed()
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (SR sr in connection.Cache.SRs)
                {
                    if (sr.GetSRType(false) == SR.SRTypes.lvmoiscsi)
                        return true;
                }
            }
            return false;
        }

        public static bool HAEnabledOnAtLeastOnePool
        {
            get
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                    if (Helpers.HAEnabled(connection))
                        return true;

                return false;
            }
        }


        internal static void PerformIQNCheck()
        {
            List<Host> missingIQNs = HelpersGUI.CheckHostIQNsExist();
            Dictionary<Host, Host> repeatedIQNs = HelpersGUI.CheckHostIQNsDiffer();

            foreach (Host host in missingIQNs)
            {
                MissingIqnAlert alert = new MissingIqnAlert(host);
                if (Alert.FindAlert(alert) == null)
                    Alert.AddAlert(alert);
            }

            foreach (Host host in repeatedIQNs.Keys)
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    DuplicateIqnAlert alert = new DuplicateIqnAlert(host, repeatedIQNs);
                    if (Alert.FindAlert(alert) == null)
                        Alert.AddAlert(alert);
                });
            }

        }

        public static bool GetActionInProgress(SR sr)
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted)
                {
                    if (a is ISrAction && a.SR.opaque_ref == sr.opaque_ref)
                        return true;

                    if (a is EnableHAAction haAction && haAction.HeartbeatSRs.Contains(sr))
                        return true;
                }

            }
            return false;
        }

        public static bool BeingScanned(SR sr, out SrRefreshAction scanAction)
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted && a is SrRefreshAction refreshAction && a.SR.opaque_ref == sr.opaque_ref)
                {
                    scanAction = refreshAction;
                    return true;
                }
            }

            scanAction = null;
            return false;
        }

        /// <summary>
        /// A wrapper around DateTime.ToString(), making sure that the resultant string is localised if and only if necessary.
        /// (What do we mean by localised here? Unlocalised means: in invariant culture, so the same whatever the language of
        /// the program and the OS. Localised means: in the language of the program. Localised doesn't mean: in the language
        /// of the OS (see CA-46983 for the rationale).
        /// </summary>
        /// <param name="dt">The DateTime to be stringified</param>
        /// <param name="format">The format string</param>
        /// <param name="localise">Whether the output should be localised</param>
        /// <returns></returns>
        public static string DateTimeToString(DateTime dt, string format, bool localise)
        {
            if (localise)
            {
                Program.AssertOnEventThread();  // otherwise it won't get localised: see CA-46983
                // If English, check for 'bad' formats: i.e., "standard" (single letter) formats,
                // and months in digits (M or MM, but MMM and MMMM are OK).
                Trace.Assert(!InvisibleMessages.LOCALE.StartsWith("en-") ||
                    format.Length > 1 && (!format.Contains("M") || format.Contains("MMM")),
                    "Bad date format");
                return dt.ToString(format);
            }
            else
                return dt.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the full name of the specified DayOfWeek,
        /// making sure that the resultant string is localised if and only if necessary.
        /// Localised means: in the language of the program (not in the language of the OS).
        /// </summary>
        public static string DayOfWeekToString(DayOfWeek dayOfWeek, bool localise = true)
        {
            if (localise)
            {
                Program.AssertOnEventThread();
                // get the day of the week localized to culture of the current thread
                return DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
            }
            
            return dayOfWeek.ToString();
        }

        /// <summary>
        /// Returns the abbreviated name of the specified DayOfWeek,
        /// making sure that the resultant string is localised if and only if necessary.
        /// Localised means: in the language of the program (not in the language of the OS).
        /// </summary>
        public static string DayOfWeekToShortString(DayOfWeek dayOfWeek, bool localise = true)
        {
            if (localise)
            {
                Program.AssertOnEventThread();
                // get the day of the week localized to culture of the current thread
                return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(dayOfWeek);
            }
            
            return dayOfWeek.ToString();
        }

        public static DateTime RoundToNearestQuarter(DateTime time)
        {
            var baseTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);

            if (time < baseTime.AddMinutes(7.5))
                return baseTime;
            if (baseTime.AddMinutes(7.5) <= time && time < baseTime.AddMinutes(22.5))
                return baseTime.AddMinutes(15);
            if (baseTime.AddMinutes(22.5) <= time && time < baseTime.AddMinutes(37.5))
                return baseTime.AddMinutes(30);
            if (baseTime.AddMinutes(37.5) <= time && time < baseTime.AddMinutes(52.5))
                return baseTime.AddMinutes(45);
            else
                return baseTime.AddHours(1);
        }

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control control)
        {
            var msgSuspendUpdate = System.Windows.Forms.Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                                                                       IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumeDrawing(Control control)
        {
            IntPtr wparam = new IntPtr(1);
            var msgResumeUpdate = System.Windows.Forms.Message.Create(control.Handle, WM_SETREDRAW, wparam, IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
        }

        /// <summary>
        /// Remember to call this after populating the gridview
        /// </summary>
        public static void ResizeGridViewColumnToAllCells(DataGridViewColumn col)
        {
            //the last column of the gridviews used on these pages should be autosized to Fill, but should not
            //become smaller than a minimum width, which is chosen to be the column's contents (including header)
            //width. To find what this is set temporarily the column's autosize mode to AllCells.

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            int storedWidth = col.Width;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.MinimumWidth = storedWidth;
        }

        public static void ResizeGridViewColumnToHeader(DataGridViewTextBoxColumn col)
        {
            //the column should be autosized to Fill, but should not become smaller than a minimum
            //width, which here is chosen to be the column header width. To find what this width is 
            //set temporarily the column's autosize mode to ColumnHeader.

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            int storedWidth = col.Width;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.MinimumWidth = storedWidth;
        }

        /// <summary>
        /// Get the localized Search name
        /// </summary>
        public static string GetLocalizedSearchName(Search search)
        {
            return search.Query.QueryFilter is CustomFieldDateQuery && search.Grouping == null
                       ? DateTimeToString(((CustomFieldDateQuery) (search.Query.QueryFilter)).query,
                                          Messages.DATEFORMAT_DMY_HMS, true)
                       : search.Name;
        }

        public static bool GetPermissionForCpuFeatureLevelling(List<Host> hosts, Pool pool)
        {
            if (hosts == null || pool == null || !Helpers.DundeeOrGreater(pool.Connection))
                return true;

            List<Host> hostsWithFewerFeatures = hosts.Where(host => PoolJoinRules.HostHasFewerFeatures(host, pool)).ToList();
            List<Host> hostsWithMoreFeatures = hosts.Where(host => PoolJoinRules.HostHasMoreFeatures(host, pool)).ToList();

            if (hostsWithFewerFeatures.Count > 0 && hostsWithMoreFeatures.Count > 0)
            {
                var hosts1 = hostsWithFewerFeatures.Union(hostsWithMoreFeatures).ToList();

                string msg1 = string.Format(hosts1.Count == 1
                        ? Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_AND_HOST_MESSAGE
                        : Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_AND_HOST_MESSAGE_MULTIPLE,
                    string.Join("\n", hosts1.Select(h => h.Name())));

                using (var dlg = new WarningDialog(msg1, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {HelpNameSetter = "PoolJoinCpuMasking"})
                {
                    return dlg.ShowDialog(Program.MainWindow) == DialogResult.Yes;
                }
            }

            if (hostsWithFewerFeatures.Count > 0)
            {
                string msg2 = string.Format(hostsWithFewerFeatures.Count == 1
                        ? Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_MESSAGE
                        : Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_POOL_MESSAGE_MULTIPLE,
                    string.Join("\n", hostsWithFewerFeatures.Select(h => h.Name())));

                using (var dlg = new WarningDialog(msg2, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {HelpNameSetter = "PoolJoinCpuMasking"})
                {
                    return dlg.ShowDialog(Program.MainWindow) == DialogResult.Yes;
                }
            }

            if (hostsWithMoreFeatures.Count > 0)
            {
                string msg3 = string.Format(hostsWithMoreFeatures.Count == 1
                        ? Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_HOST_MESSAGE
                        : Messages.ADD_HOST_TO_POOL_CPU_DOWN_LEVEL_HOST_MESSAGE_MULTIPLE,
                    string.Join("\n", hostsWithMoreFeatures.Select(h => h.Name())));

                using (var dlg = new WarningDialog(msg3, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {HelpNameSetter = "PoolJoinCpuMasking"})
                {
                    return dlg.ShowDialog(Program.MainWindow) == DialogResult.Yes;
                }
            }

            return true;
        }
    }
}
