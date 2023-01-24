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
using System.Linq;
using System.Text;
using System.Xml;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class FailedLoginAttemptAlert : MessageAlert
    {
        private readonly Pool _pool;
        private readonly int _total;
        private readonly int _unknown;
        private readonly List<Offender> _topOffenders = new List<Offender>();

        public FailedLoginAttemptAlert(Message m)
            : base(m)
        {
            _pool = m.Connection.Cache.Pools.FirstOrDefault(p => p.uuid == m.obj_uuid);

            try
            {
                /* - Total and unknown are optional (unknown being absent implies that all the failed
                     logins came from known sources, and conversely total being absent implies that all
                     the failed logins came from unknown sources).
                   - There may be 0 or more known tags.
                   - Each known tag must contain date and number, but the other tags (ip, originator,
                     username, useragent) are all optional (though we expect at least one) */
                
                var doc = new XmlDocument();
                doc.LoadXml(m.body);
                
                var totalNodes = doc.GetElementsByTagName("total");
                if (totalNodes.Count > 0)
                    int.TryParse(totalNodes[0].InnerText, out _total);
                
                var unknownNodes = doc.GetElementsByTagName("unknown");
                if (unknownNodes.Count > 0)
                    int.TryParse(unknownNodes[0].InnerText, out _unknown);

                var knownNodes = doc.GetElementsByTagName("known");
                foreach (XmlNode node in knownNodes)
                {
                    var offender = new Offender();

                    foreach (XmlNode child in node.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case "number":
                                if (int.TryParse(child.InnerText, out var number))
                                    offender.Number = number;
                                break;
                            case "date":
                                if (Util.TryParseIso8601DateTime(child.InnerText, out var date))
                                    offender.Date = date;
                                break;
                            case "ip":
                                offender.Ip = child.InnerText;
                                break;
                            case "originator":
                                offender.Originator = child.InnerText;
                                break;
                            case "username":
                                offender.Username = child.InnerText;
                                break;
                            case "useragent":
                                offender.Useragent = child.InnerText;
                                break;
                        }
                    }

                    _topOffenders.Add(offender);
                }
            }
            catch
            {
                //ignore
            }
        }

        public override string Title => _pool == null
            ? base.Title
            : string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_TITLE, _pool.Name());

        public override string Description
        {
            get
            {
                if (_total == 0 && _unknown > 0)
                    return string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_DESCRIPTION_ALL_UNKNOWN, _unknown);

                var sb = new StringBuilder();

                if (_total > 0)
                    sb.AppendLine(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_DESCRIPTION_TOTAL, _total));
                
                if (_unknown > 0)
                    sb.AppendLine(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_DESCRIPTION_UNKNOWN, _unknown));

                if (_topOffenders.Count > 0)
                {
                    sb.AppendLine(Messages.ALERT_FAILED_LOGIN_ATTEMPT_DESCRIPTION_COMMON);

                    foreach (var offender in _topOffenders)
                        sb.AppendLine(offender.ToString());
                }

                return sb.Length > 0 ? sb.ToString() : base.Title;
            }
        }

        public override Action FixLinkAction => null;

        public override string FixLinkText => null;

        public override string HelpID => "FailedLoginAttemptAlert";

        public override string HelpLinkText => Messages.ALERT_GENERIC_HELP;


        private class Offender
        {
            /// <summary>
            /// Number of failed login attempts from this offender
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// DateTime of last failed login attempt from this offender
            /// </summary>
            public DateTime? Date { private get; set;}

            /// <summary>
            /// Offender IP address
            /// </summary>
            public string Ip { get; set;}

            public string Originator { get; set;}
            public string Username { get; set;}
            public string Useragent { get; set;}
            
            private string GetFriendlyDate()
            {
                string date = string.Empty;

                Program.Invoke(Program.MainWindow,
                    () =>
                    {
                        if (Date.HasValue)
                            date = HelpersGUI.DateTimeToString(Date.Value.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
                    });

                return date;
            }

            public override string ToString()
            {
                var entries = new List<string>();
                
                if (!string.IsNullOrEmpty(Ip))
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_IP, Ip));
                
                if (Number > 0)
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_NUMBER, Number));

                var friendlyDate = GetFriendlyDate();
                if (!string.IsNullOrEmpty(friendlyDate))
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_DATE, friendlyDate));
                
                if (!string.IsNullOrEmpty(Username))
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_USERNAME, Username));
                
                if (!string.IsNullOrEmpty(Originator))
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_ORIGINATOR, Originator));
                
                if (!string.IsNullOrEmpty(Useragent))
                    entries.Add(string.Format(Messages.ALERT_FAILED_LOGIN_ATTEMPT_OFFENDER_USERAGENT, Useragent));

                return $"- {string.Join(", ", entries)}";
            }
        }
    }
}
