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
using System.Timers;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin
{
    /// <summary>
    /// A custom Timer that checks at regular intervals if any server licenses have expired. Also contains logic
    /// for testing license state on connection as to whether we should warn about a soon to expire license.
    /// </summary>
    class LicenseTimer : System.Timers.Timer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly TimeSpan EXPIRED_REMINDER_FREQUENCY = new TimeSpan(0, 0, 30, 0); // How frequently to remind user when a license has expired
        private static readonly TimeSpan CONNECTION_WARN_THRESHOLD = new TimeSpan(29, 0, 0, 0); // When to start warning on connection 
        private static readonly TimeSpan RUNNING_WARN_FREQUENCY = new TimeSpan(1, 0, 0, 0); // How frequently to remind when XC is running

        private static DateTime lastPeriodicLicenseWarning;
        private readonly LicenseManagerLauncher licenseManagerLauncher;

        public LicenseTimer(LicenseManagerLauncher licenseManagerLauncher)
        {
            Elapsed += new ElapsedEventHandler(licenseTimerElapsed);
            AutoReset = true;
            Interval = EXPIRED_REMINDER_FREQUENCY.TotalMilliseconds;
            lastPeriodicLicenseWarning = DateTime.UtcNow;
            this.licenseManagerLauncher = licenseManagerLauncher;
            Start();
        }

        /// <summary>
        /// Call this to check the server licenses when a connection has been made.
        /// If a license has expired, the user is warned.
        /// </summary>
        internal void CheckActiveServerLicense(IXenConnection connection)
        {
            DateTime now = DateTime.UtcNow - connection.ServerTimeOffset;
            foreach (Host host in connection.Cache.Hosts)
            {
                if (host.IsXCP)
                    continue;

                DateTime expiryDate = host.LicenseExpiryUTC;
                TimeSpan timeToExpiry = expiryDate.Subtract(now);

                if (expiryDate < now)
                {
                    // License has expired already
                    Program.Invoke(Program.MainWindow, delegate {
                        showLicenseSummaryExpired(host, expiryDate);
                    });
                    return;
                }
                else if (timeToExpiry < CONNECTION_WARN_THRESHOLD)
                {
                    // If the license is sufficiently close to expiry warn now
                    Program.Invoke(Program.MainWindow, delegate {
                        showLicenseSummaryWarning(Helpers.GetName(host), now, expiryDate);
                    });
                    return;
                }  
            }
        }

        /// <summary>
        /// Check to see if any licenses have expired as the timer periodically elapses.
        /// </summary>
        private void licenseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (licenseManagerLauncher.LicenceDialogIsShowing)
                return;
            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                if (periodicCheck(xc))
                    return;
            }          
        }


        /// <summary>
        /// The logic for the periodic license warning check, only shows the less than 30 day warnings once every day XC is running.
        /// Invokes dialog shows as is called from background timer threads.
        /// </summary>
        /// <param name="xc">The connection to check licenses on</param>
        /// <returns></returns>
        private bool periodicCheck(IXenConnection xc)
        {
            DateTime now = DateTime.UtcNow - xc.ServerTimeOffset;
            foreach (Host host in xc.Cache.Hosts)
            {
                if (host.IsXCP)
                    continue;

                DateTime expiryDate = host.LicenseExpiryUTC;
                TimeSpan timeToExpiry = expiryDate.Subtract(now);
                if (expiryDate < now)
                {
                    // License has expired. Pop up the License Manager.
                    Program.Invoke(Program.MainWindow, delegate() {
                        showLicenseSummaryExpired(host, expiryDate);
                    });
                    return true;
                }
                else if (timeToExpiry < CONNECTION_WARN_THRESHOLD &&
                    DateTime.UtcNow.Subtract(lastPeriodicLicenseWarning) > RUNNING_WARN_FREQUENCY)
                {
                    // Check to see if XC has been open one day. If it has, show < 30 day expiry warnings
                    lastPeriodicLicenseWarning = DateTime.UtcNow;
                    Program.Invoke(Program.MainWindow, delegate() {
                        showLicenseSummaryWarning(Helpers.GetName(host), now, expiryDate);
                    });
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Shows the license summary dialog to the user as their license will soon expire.
        /// </summary>
        private void showLicenseSummaryWarning(String hostname, DateTime now, DateTime expiryDate)
        {
            Program.AssertOnEventThread();

            log.InfoFormat("Server {0} is within 30 days of expiry ({1}). Show License Summary if needed", hostname, HelpersGUI.DateTimeToString(expiryDate, Messages.DATEFORMAT_DMY_HMS, true));
            string timeleft = GetLicenseTimeLeftString(expiryDate.Subtract(now), false);
            string message = string.Format(Messages.MAINWINDOW_EXPIRE_MESSAGE, hostname.Ellipsise(25), timeleft);
            new ActionBase(ActionType.Alert, Messages.NOTICE_LICENCE_TITLE, message, false);

            if (Program.RunInAutomatedTestMode)
                log.DebugFormat("In automated test mode: quashing license expiry warning '{0}'", message);
            else
            {
                licenseManagerLauncher.Parent = Program.MainWindow;
                licenseManagerLauncher.LaunchIfRequired(true, ConnectionsManager.XenConnections);
            }
                
        }

        /// <summary>
        /// Returns a string similar to "x days", "x minutes", "x hours", "x months" where x is the time till the host's license expires/needs reactivating.
        /// </summary>
        /// <param name="timeTillExpire"></param>
        /// <param name="CapAtTenYears">Set to true will return Messages.UNLIMITED for timespans over 3653 days</param>
        /// <returns></returns>
        public static string GetLicenseTimeLeftString(TimeSpan timeTillExpire, bool CapAtTenYears)
        {
            if (timeTillExpire.Ticks < 0)
                return "";
            if (CapAtTenYears && timeTillExpire.TotalDays > 3653)
            {
                return Messages.UNLIMITED;
            }
            else if (timeTillExpire.TotalDays > 60)
            {
                // Show remaining time in months
                return string.Format(Messages.TIME_MONTHS,
                    (long)(Math.Floor(timeTillExpire.TotalDays / 30)));
            }
            else if (timeTillExpire.TotalDays > 2)
            {
                // Show remaining time in days
                return string.Format(Messages.TIME_DAYS,
                    (long)timeTillExpire.TotalDays);
            }
            else if (timeTillExpire.TotalHours > 2)
            {
                // Show remaining time in hours
                return string.Format(Messages.TIME_HOURS,
                    (long)timeTillExpire.TotalHours);
            }
            else
            {
                // Show remaining time in minutes (round up so you never get 'in 0 minutes')
                return string.Format(Messages.TIME_MINUTES,
                    (long)Math.Ceiling(timeTillExpire.TotalMinutes));
            }
        }

        /// <summary>
        /// Shows the license summary dialog to the user as their license has expired.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="expiryDate">Should be expressed in local time.</param>
        private void showLicenseSummaryExpired(Host host, DateTime expiryDate)
        {
            Program.AssertOnEventThread();

            log.InfoFormat("Server {0} has expired ({1}). Show License Summary if needed", host.Name, HelpersGUI.DateTimeToString(expiryDate, Messages.DATEFORMAT_DMY_HMS, true));
            if (Program.RunInAutomatedTestMode)
                log.DebugFormat("In automated test mode: quashing license expiry warning");
            else
                licenseManagerLauncher.LaunchIfRequired(true, ConnectionsManager.XenConnections);
        }
    }
}
