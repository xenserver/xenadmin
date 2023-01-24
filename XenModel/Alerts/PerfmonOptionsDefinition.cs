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
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenAdmin.Alerts
{
    public class PerfmonOptionsDefinition
    {
        public const String MAIL_DESTINATION_KEY_NAME = "mail-destination";
        public const String SMTP_MAILHUB_KEY_NAME = "ssmtp-mailhub";
        public const String MAIL_LANGUAGE_KEY_NAME = "mail-language";

        private class MailLanguageList
        {
            private Dictionary<String, String> _list;

            public MailLanguageList(Dictionary<String, String> initList)
            {
                _list = new Dictionary<String, String>(initList);
            }

            public String CodeFromName(String name)
            {
                String ret = null;

                if (null == name)
                    return ret;

                foreach(KeyValuePair<String, String> pair in _list)
                {
                    if(pair.Value == name)
                    {
                        ret = pair.Key;
                        break;
                    }
                }

                return ret;
            }

            public String NameFromCode(String code)
            {
                String ret = null;

                if (null == code)
                    return ret;

                foreach (KeyValuePair<String, String> pair in _list)
                {
                    if (pair.Key == code)
                    {
                        ret = pair.Value;
                        break;
                    }
                }

                return ret;
            }

            public bool HasCode(String code)
            {
                return null == code ? false : _list.ContainsKey(code);
            }

            public Dictionary<String, String> dataSource()
            {
                return new Dictionary<String, String>(_list);
            }
        }

        private static readonly char[] mailHubDelim = new char[] { ':' };
        private readonly String mailHub;
        private readonly String mailDestination;
        private readonly String mailLanguageCode;

        private static MailLanguageList ml_list = new MailLanguageList(new Dictionary<String, String>() {
            {Messages.MAIL_LANGUAGE_ENGLISH_CODE, Messages.MAIL_LANGUAGE_ENGLISH_NAME},
            {Messages.MAIL_LANGUAGE_CHINESE_CODE, Messages.MAIL_LANGUAGE_CHINESE_NAME},
            {Messages.MAIL_LANGUAGE_JAPANESE_CODE, Messages.MAIL_LANGUAGE_JAPANESE_NAME}
        });

        public PerfmonOptionsDefinition(String mailHub, String mailDestination, String mailLanguageCode)
        {
            this.mailHub = mailHub;
            this.mailDestination = mailDestination;
            this.mailLanguageCode = mailLanguageCode;
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

        public String MailLanguageCode
        {
            get
            {
                return mailLanguageCode;
            }
        }

        public String MailLanguageName
        {
            get
            {
                return ml_list.NameFromCode(mailLanguageCode);
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
            catch
            {
                // ignored
            }

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
            catch
            {
                // ignored
            }

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
            string mailLanguageCode = GetMailLanguageCode(connection);

            if (mailDestination == null || mailHub == null)
                return null;

            return new PerfmonOptionsDefinition(mailHub, mailDestination, mailLanguageCode);
        }

        public static string GetMailDestination(IXenConnection connection)
        {
            var pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return null;

            var otherConfig = Helpers.GetOtherConfig(pool);
            if (otherConfig == null)
                return null;

            if (!otherConfig.ContainsKey(MAIL_DESTINATION_KEY_NAME))
                return null;

            var mailAddress = otherConfig[MAIL_DESTINATION_KEY_NAME]?.Trim();
            return string.IsNullOrEmpty(mailAddress) ? null : mailAddress;
        }

        public static string GetSmtpMailHub(IXenConnection connection)
        {
            var pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return null;

            var otherConfig = Helpers.GetOtherConfig(pool);
            if (otherConfig == null)
                return null;

            if (!otherConfig.ContainsKey(SMTP_MAILHUB_KEY_NAME))
                return null;

            var mailHub = otherConfig[SMTP_MAILHUB_KEY_NAME]?.Trim();
            return string.IsNullOrEmpty(mailHub) ? null : mailHub;
        }

        public static string GetMailLanguageCode(IXenConnection connection)
        {
            var pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return null;

            var otherConfig = Helpers.GetOtherConfig(pool);
            if (otherConfig == null)
                return null;

            if (!otherConfig.ContainsKey(MAIL_LANGUAGE_KEY_NAME))
                return null;

            var mailLanguageCode = otherConfig[MAIL_LANGUAGE_KEY_NAME]?.Trim();
            return string.IsNullOrEmpty(mailLanguageCode) ? null : mailLanguageCode;
        }

        public static String MailLanguageNameFromCode(String code)
        {
            return ml_list.NameFromCode(code);
        }

        public static String MailLanguageCodeFromName(String name)
        {
            return ml_list.CodeFromName(name);
        }

        public static bool MailLanguageHasCode(String code)
        {
            return ml_list.HasCode(code);
        }

        public static object MailLanguageDataSource()
        {
            return PerfmonOptionsDefinition.ml_list.dataSource();
        }
    }
}
