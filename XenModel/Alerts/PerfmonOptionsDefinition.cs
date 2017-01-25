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
using System.Text;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Collections;
using System.Xml;

using System.Diagnostics;

namespace XenAdmin.Alerts
{
    public class PerfmonOptionsDefinition
    {
        public const String MAIL_DESTINATION_KEY_NAME = "mail-destination";
        public const String SMTP_MAILHUB_KEY_NAME = "ssmtp-mailhub";

        private static readonly char[] mailHubDelim = new char[] { ':' };
        private readonly String mailHub;
        private readonly String mailDestination;

        public PerfmonOptionsDefinition(String mailHub, String mailDestination)
        {
            this.mailHub = mailHub;
            this.mailDestination = mailDestination;
        }

        public String MailHub
        {
            get
            {
                return mailHub;
            }
        }

        public String MailDestination
        {
            get
            {
                return mailDestination;
            }
        }

        public static String GetSmtpServerAddress(string mailHub)
        {
            try
            {
                string[] words = mailHub.Split(mailHubDelim);
                if (words.Length > 0)
                {
                    return words[0];
                }
            }
            catch { }
            return "";
        }

        public static String GetSmtpPort(string mailHub)
        {
            try
            {
                string[] words = mailHub.Split(mailHubDelim);
                if (words.Length > 1)
                {
                    return words[1];
                }
            }
            catch { }
            return "25";
        }

        public override string ToString()
        {
            return String.Format("mail-destination: {0} ssmtp-mailhub: {1}", MailDestination, MailHub);
        }

        public static PerfmonOptionsDefinition GetPerfmonOptionsDefinitions(IXenObject xmo)
        {
            if (xmo == null)
                return null;

            IXenConnection connection = xmo.Connection;
            if (connection == null)
                return null;

            string mailDestination = GetMailDestination(connection);
            string mailHub = GetSmtpMailHub(connection);

            if (mailDestination == null || mailDestination == null)
                return null;

            return new PerfmonOptionsDefinition(mailHub, mailDestination);
        }

        public static string GetMailDestination(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return null;

            Dictionary<String, String> other_config = Helpers.GetOtherConfig(pool);
            if (other_config == null)
                return null;

            if (!other_config.ContainsKey(MAIL_DESTINATION_KEY_NAME))
                return null;

            String mailAddress = other_config[MAIL_DESTINATION_KEY_NAME];
            if (mailAddress == null)
                return null;

            mailAddress.Trim();
            if (String.IsNullOrEmpty(mailAddress))
                return null;

            return mailAddress;
        }

        public static string GetSmtpMailHub(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return null;

            Dictionary<String, String> other_config = Helpers.GetOtherConfig(pool);
            if (other_config == null)
                return null;

            if (!other_config.ContainsKey(SMTP_MAILHUB_KEY_NAME))
                return null;

            String mailHub = other_config[SMTP_MAILHUB_KEY_NAME];
            if (mailHub == null)
                return null;

            mailHub.Trim();
            if (String.IsNullOrEmpty(mailHub))
                return null;

            return mailHub;
        }
    }
}
