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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class CertificateAlert : MessageAlert
    {
        private readonly Host _host;
        private readonly DateTime? _certificateExpiryDate;
        private static readonly Regex _dateRegex = new Regex(@"(\d{8}T\d{2}:\d{2}:\d{2}Z)");
        private const string DATE_FORMAT = "yyyyMMddTHH:mm:ssZ";

        public CertificateAlert(Message m)
            : base(m)
        {
            _host = m.Connection.Cache.Hosts.FirstOrDefault(h => h.uuid == m.obj_uuid);

            var match = _dateRegex.Match(m.body);

            if (match.Success && DateTime.TryParseExact(match.Value, DATE_FORMAT, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
                _certificateExpiryDate = result;
        }

        public override string Title
        {
            get
            {
                switch (Message.Type)
                {
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_ALERT_EXPIRED_TITLE, _host.Name());
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        if (_certificateExpiryDate.HasValue && _certificateExpiryDate.Value > Timestamp)
                        {
                            var eta = _certificateExpiryDate.Value - Timestamp;

                            if (eta.TotalDays >= 1)
                                return string.Format(Messages.CERTIFICATE_ALERT_EXPIRING_TITLE_DAYS, _host.Name(),
                                    Math.Round(eta.TotalDays, MidpointRounding.AwayFromZero));

                            if (eta.TotalHours >= 1)
                                return string.Format(Messages.CERTIFICATE_ALERT_EXPIRING_TITLE_HOURS, _host.Name(),
                                    Math.Round(eta.TotalHours, MidpointRounding.AwayFromZero));

                            if (eta.TotalMinutes >= 1)
                                return string.Format(Messages.CERTIFICATE_ALERT_EXPIRING_TITLE_MINUTES, _host.Name(),
                                    Math.Round(eta.TotalMinutes, MidpointRounding.AwayFromZero));
                        }

                        return string.Format(Messages.CERTIFICATE_ALERT_EXPIRED_TITLE, _host.Name());
                    default:
                        return base.Title;
                }
            }
        }


        public override string Description
        {
            get
            {
                if (_host == null)
                    return base.Title;

                switch (Message.Type)
                {
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_ALERT_EXPIRED_DESCIRPTION, _host.Name(), GetExpiryDate());
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        return string.Format(Messages.CERTIFICATE_ALERT_EXPIRING_DESCRIPTION,
                            _host.Name(), GetExpiryDate());
                    default:
                        return base.Title;
                }
            }
        }

        public override Action FixLinkAction
        {
            get
            {
                var cmd = new InstallCertificateCommand(Program.MainWindow, XenObject as Host);
                if (cmd.CanExecute())
                    return () => cmd.Execute();
                return null;
            }
        }

        public override string FixLinkText => Messages.INSTALL_SERVER_CERTIFICATE_ACTION_LINK;

        public override string HelpID => "CertificateAlert";

        public override string HelpLinkText => Messages.ALERT_GENERIC_HELP;

        private string GetExpiryDate()
        {
            string date = string.Empty;

            Program.Invoke(Program.MainWindow,
                () =>
                {
                    if (_certificateExpiryDate.HasValue)
                        date = HelpersGUI.DateTimeToString(_certificateExpiryDate.Value.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
                });

            return date;
        }
    }
}
