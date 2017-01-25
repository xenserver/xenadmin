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
using XenAdmin.Core;


namespace XenAdmin.Alerts
{
    public static class AlertExtensions
    {
        public static string GetAlertDetailsCSVQuotes(this Alert a)
        {
            string date = String.Empty;
            string description = String.Empty;

            Program.Invoke(Program.MainWindow, delegate
            {
                date = HelpersGUI.DateTimeToString(
                    a.Timestamp.ToLocalTime(),
                    Messages.DATEFORMAT_DMY_HM, true);
                description = a.Description;
            });

            return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                 a.Title.EscapeQuotes(),
                                 a.Priority.GetString().EscapeQuotes(),
                                 description.EscapeQuotes(),
                                 a.AppliesTo.EscapeQuotes(),
                                 date.EscapeQuotes());
        }

        public static string GetUpdateDetailsCSVQuotes(this Alert a)
        {
            string date = String.Empty;
            string description = String.Empty;

            Program.Invoke(Program.MainWindow, delegate
            {
                date = HelpersGUI.DateTimeToString(
                    a.Timestamp.ToLocalTime(),
                    Messages.DATEFORMAT_DMY_HM, true);
                description = a.Description;
            });

            return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                 a.Title.EscapeQuotes(),
                                 description.EscapeQuotes(),
                                 a.AppliesTo.EscapeQuotes(),
                                 date.EscapeQuotes(),
                                 a.WebPageLabel.EscapeQuotes());
        }
    }
}
