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
using System.Diagnostics;
using System.Xml;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class CssExpiryAlert : MessageAlert
    {
        private readonly DateTime? _cssExpiryDate;
        private readonly string _title;
        private readonly string _description;

        public CssExpiryAlert(Message m)
            : base(m)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(m.body);
                var nodes = doc.GetElementsByTagName("date");

                if (nodes.Count > 0 && Util.TryParseIso8601DateTime(nodes[0].InnerText, out var result))
                    _cssExpiryDate = result;
            }
            catch
            {
                //ignore
            }

            switch (Message.Type)
            {
                case Message.MessageType.UPDATES_FEATURE_EXPIRED:
                    _title = Messages.ALERT_CSS_EXPIRED_TITLE;
                    _description = string.Format(Messages.ALERT_CSS_EXPIRED_DESCIRPTION, AlertExtensions.GetGuiDate(_cssExpiryDate));
                    break;

                case Message.MessageType.UPDATES_FEATURE_EXPIRING_CRITICAL:
                case Message.MessageType.UPDATES_FEATURE_EXPIRING_MAJOR:
                case Message.MessageType.UPDATES_FEATURE_EXPIRING_WARNING:

                    if (_cssExpiryDate.HasValue && _cssExpiryDate.Value > Timestamp)
                    {
                        var eta = _cssExpiryDate.Value - Timestamp;

                        if (eta.TotalDays >= 1)
                            _title = string.Format(Messages.ALERT_CSS_EXPIRING_TITLE_DAYS, Math.Round(eta.TotalDays, MidpointRounding.AwayFromZero));

                        else if (eta.TotalHours >= 1)
                            _title = string.Format(Messages.ALERT_CSS_EXPIRING_TITLE_HOURS, Math.Round(eta.TotalHours, MidpointRounding.AwayFromZero));

                        else if (eta.TotalMinutes >= 1)
                            _title = string.Format(Messages.ALERT_CSS_EXPIRING_TITLE_MINUTES, Math.Round(eta.TotalMinutes, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        _title = Messages.ALERT_CSS_EXPIRED_TITLE;
                    }

                    _description = string.Format(Messages.ALERT_CSS_EXPIRING_DESCRIPTION, AlertExtensions.GetGuiDate(_cssExpiryDate));
                    break;
            }
        }

        public override string Title => string.IsNullOrEmpty(_title) ? base.Title : _title;

        public override string Description => string.IsNullOrEmpty(_description) ? base.Title : _description;

        public override Action FixLinkAction => () => Process.Start(InvisibleMessages.CSS_URL);

        public override string FixLinkText => Messages.ALERT_CSS_EXPIRED_LINK_TEXT;

        public override string HelpID => "CssExpiryAlert";

        public override string HelpLinkText => Messages.ALERT_GENERIC_HELP;
    }
}
