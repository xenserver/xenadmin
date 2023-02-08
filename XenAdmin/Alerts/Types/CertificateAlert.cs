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
using System.Xml;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class CertificateAlert : MessageAlert
    {
        private readonly DateTime? _certificateExpiryDate;

        public CertificateAlert(Message m)
            : base(m)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(m.body);
                var nodes = doc.GetElementsByTagName("date");

                if (nodes.Count > 0 && Util.TryParseIso8601DateTime(nodes[0].InnerText, out DateTime result))
                    _certificateExpiryDate = result;
            }
            catch
            {
                //ignore
            }
        }

        public override string Title
        {
            get
            {
                var objectName = XenObject == null ? Messages.UNKNOWN_OBJECT : XenObject.Name();

                switch (Message.Type)
                {
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRED:
                        var pool1 = Helpers.GetPoolOfOne(Connection);
                        return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRED_TITLE, objectName, pool1.Name());

                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_30:
                        var pool2 = Helpers.GetPoolOfOne(Connection);
                        if (_certificateExpiryDate.HasValue && _certificateExpiryDate.Value > Timestamp)
                        {
                            var eta = _certificateExpiryDate.Value - Timestamp;

                            if (eta.TotalDays >= 1)
                                return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRING_TITLE_DAYS, objectName,
                                    pool2.Name(), Math.Round(eta.TotalDays, MidpointRounding.AwayFromZero));

                            if (eta.TotalHours >= 1)
                                return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRING_TITLE_HOURS, objectName,
                                    pool2.Name(), Math.Round(eta.TotalHours, MidpointRounding.AwayFromZero));

                            if (eta.TotalMinutes >= 1)
                                return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRING_TITLE_MINUTES, objectName,
                                    pool2.Name(), Math.Round(eta.TotalMinutes, MidpointRounding.AwayFromZero));
                        }
                        return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRED_TITLE, objectName, pool2.Name());

                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRED_TITLE, objectName);

                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_30:
                        if (_certificateExpiryDate.HasValue && _certificateExpiryDate.Value > Timestamp)
                        {
                            var eta = _certificateExpiryDate.Value - Timestamp;

                            if (eta.TotalDays >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRING_TITLE_DAYS, objectName,
                                    Math.Round(eta.TotalDays, MidpointRounding.AwayFromZero));

                            if (eta.TotalHours >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRING_TITLE_HOURS, objectName,
                                    Math.Round(eta.TotalHours, MidpointRounding.AwayFromZero));

                            if (eta.TotalMinutes >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRING_TITLE_MINUTES, objectName,
                                    Math.Round(eta.TotalMinutes, MidpointRounding.AwayFromZero));
                        }
                        return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRED_TITLE, objectName);

                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRED_TITLE, objectName);

                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        if (_certificateExpiryDate.HasValue && _certificateExpiryDate.Value > Timestamp)
                        {
                            var eta = _certificateExpiryDate.Value - Timestamp;

                            if (eta.TotalDays >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRING_TITLE_DAYS, objectName,
                                    Math.Round(eta.TotalDays, MidpointRounding.AwayFromZero));

                            if (eta.TotalHours >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRING_TITLE_HOURS, objectName,
                                    Math.Round(eta.TotalHours, MidpointRounding.AwayFromZero));

                            if (eta.TotalMinutes >= 1)
                                return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRING_TITLE_MINUTES, objectName,
                                    Math.Round(eta.TotalMinutes, MidpointRounding.AwayFromZero));
                        }
                        return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRED_TITLE, objectName);

                    default:
                        return base.Title;
                }
            }
        }


        public override string Description
        {
            get
            {
                var objectName = XenObject == null ? Messages.UNKNOWN_OBJECT : XenObject.Name();

                switch (Message.Type)
                {
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRED:
                        var pool1 = Helpers.GetPoolOfOne(Connection);
                        return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRED_DESCIRPTION, objectName, pool1.Name(), GetExpiryDate());

                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.POOL_CA_CERTIFICATE_EXPIRING_30:
                        var pool2 = Helpers.GetPoolOfOne(Connection);
                        return string.Format(Messages.CERTIFICATE_CA_ALERT_EXPIRING_DESCRIPTION,
                            objectName, pool2.Name(), GetExpiryDate());

                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRED_DESCIRPTION, objectName, GetExpiryDate());

                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_INTERNAL_CERTIFICATE_EXPIRING_30:
                        return string.Format(Messages.CERTIFICATE_HOST_INTERNAL_ALERT_EXPIRING_DESCRIPTION,
                            objectName, GetExpiryDate());

                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                        return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRED_DESCIRPTION, objectName, GetExpiryDate());

                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        return string.Format(Messages.CERTIFICATE_HOST_ALERT_EXPIRING_DESCRIPTION,
                            objectName, GetExpiryDate());

                    default:
                        return base.Title;
                }
            }
        }

        public override Action FixLinkAction
        {
            get
            {
                if (!(XenObject is Host host))
                    return null;

                switch (Message.Type)
                {
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        var cmd = new InstallCertificateCommand(Program.MainWindow, host);
                        if (cmd.CanRun())
                            return () => cmd.Run();
                        return null;
                    default:
                        return null;
                }
            }
        }

        public override string FixLinkText
        {
            get
            {
                if (!(XenObject is Host))
                    return null;

                switch (Message.Type)
                {
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRED:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_07:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_14:
                    case Message.MessageType.HOST_SERVER_CERTIFICATE_EXPIRING_30:
                        return Messages.INSTALL_SERVER_CERTIFICATE_ACTION_LINK;
                    default:
                        return null;
                }
            }
        }

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
