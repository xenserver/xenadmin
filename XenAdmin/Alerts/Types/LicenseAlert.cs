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
using System.Text;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class LicenseAlert : Alert
    {
        private string hostName;
        private DateTime nowDate;
        private DateTime expiryDate;

        public LicenseAlert(string hostname, DateTime now, DateTime expiry)
        {
            hostName = hostname;
            nowDate = now;
            expiryDate = expiry;
            _timestamp = now;
        }

        public LicenseManagerLauncher LicenseManagerLauncher { get; set; }

        #region Overrides of Alert

        public override string Title
        {
            get { return Messages.NOTICE_LICENCE_TITLE; }
        }

        public override string Description
        {
            get
            {
                if (expiryDate < nowDate)
                    return string.Format(Messages.MAINWINDOW_EXPIRE_MESSAGE_TOO_LATE, hostName.Ellipsise(25));

                string timeleft = GetLicenseTimeLeftString(expiryDate.Subtract(nowDate), false);
                return string.Format(Messages.MAINWINDOW_EXPIRE_MESSAGE, hostName.Ellipsise(25), timeleft);
            }
        }

        public override AlertPriority Priority
        {
            get
            {
                return expiryDate < nowDate
                           ? AlertPriority.Priority2
                           : AlertPriority.Priority3;
            }
        }

        public override string AppliesTo
        {
            get { return hostName; }
        }

        public override string FixLinkText
        {
            get { return Messages.LAUNCH_LICENSE_MANAGER; }
        }

        public override Action FixLinkAction
        {
            get
            {
                return () =>
                    {
                        if (LicenseManagerLauncher != null)
                        {
                            LicenseManagerLauncher.Parent = Program.MainWindow;
                            LicenseManagerLauncher.LaunchIfRequired(false, ConnectionsManager.XenConnections);
                        }
                    };
            }
        }

        public override string HelpID
        {
            get { return "LicenseManager"; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns a string similar to "x days", "x minutes", "x hours", "x months"
        /// where x is the time till the host's license expires/needs reactivating.
        /// </summary>
        /// <param name="timeTillExpire"></param>
        /// <param name="capAtTenYears">Set to true will return Messages.UNLIMITED for timespans over 3653 days</param>
        /// <returns></returns>
        public static string GetLicenseTimeLeftString(TimeSpan timeTillExpire, bool capAtTenYears)
        {
            if (timeTillExpire.Ticks < 0)
                return "";
            
            if (capAtTenYears && timeTillExpire.TotalDays > 3653)
                return Messages.UNLIMITED;

            if (timeTillExpire.TotalDays > 60)
            {
                // Show remaining time in months
                return string.Format(Messages.TIME_MONTHS,
                    (long)(Math.Floor(timeTillExpire.TotalDays / 30)));
            }
            
            if (timeTillExpire.TotalDays > 2)
            {
                // Show remaining time in days
                return string.Format(Messages.TIME_DAYS,
                    (long)timeTillExpire.TotalDays);
            }
            
            if (timeTillExpire.TotalHours > 2)
            {
                // Show remaining time in hours
                return string.Format(Messages.TIME_HOURS,
                    (long)timeTillExpire.TotalHours);
            }
            
            // Show remaining time in minutes (round up so you never get 'in 0 minutes')
            return string.Format(Messages.TIME_MINUTES,
                                 (long)Math.Ceiling(timeTillExpire.TotalMinutes));
        }

        #endregion
    }
}
