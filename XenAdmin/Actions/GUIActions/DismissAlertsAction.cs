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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Alerts;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class DismissAlertsAction : AsyncAction
    {
        private readonly List<Alert> _alerts;

        public DismissAlertsAction(List<Alert> alerts, IXenConnection connection = null)
            : base(connection, GetActionTitle(connection, alerts.Count),
                Messages.ACTION_REMOVE_ALERTS_DESCRIPTION)
        {
            _alerts = alerts;

            if (connection != null)
            {
                Pool = Helpers.GetPoolOfOne(connection);
                Host = Helpers.GetCoordinator(connection);
            }
        }

        private static string GetActionTitle(IXenConnection connection, int alertCount)
        {
            if (connection == null)
                return alertCount == 1
                    ? Messages.ACTION_REMOVE_ALERTS_ON_CLIENT_TITLE_ONE
                    : string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CLIENT_TITLE, alertCount);

            return alertCount == 1
                ? string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CONNECTION_TITLE_ONE, Helpers.GetName(connection))
                : string.Format(Messages.ACTION_REMOVE_ALERTS_ON_CONNECTION_TITLE, alertCount, Helpers.GetName(connection));
        }

        protected override void Run()
        {
            LogDescriptionChanges = false;

            try
            {
                if (Connection != null && Helpers.XapiEqualOrGreater_22_19_0(Connection))
                {
                    var msgRefs = new List<XenRef<Message>>();
                    var msgAlerts = new List<MessageAlert>();
                    var otherAlerts = new List<Alert>();

                    foreach (var a in _alerts)
                    {
                        if (a is MessageAlert ma)
                        {
                            msgAlerts.Add(ma);
                            msgRefs.Add(new XenRef<Message>(ma.Message.opaque_ref));
                        }
                        else
                        {
                            otherAlerts.Add(a);
                        }
                    }

                    int midPoint = 0;
                    if (_alerts.Count > 0)
                        midPoint = 100 * msgAlerts.Count / _alerts.Count;

                    if (msgAlerts.Count > 0)
                    {
                        RelatedTask = Message.async_destroy_many(Session, msgRefs);
                        PollToCompletion(0, midPoint);
                        Alert.RemoveAlert(a => msgAlerts.Contains(a));
                    }

                    for (var i = 0; i < otherAlerts.Count; i++)
                    {
                        _alerts[i].Dismiss();
                        PercentComplete = midPoint + i * (100 - midPoint) / otherAlerts.Count;
                    }
                }
                else
                {
                    for (var i = 0; i < _alerts.Count; i++)
                    {
                        Description = string.Format(Messages.ACTION_REMOVE_ALERTS_PROGRESS_DESCRIPTION, i, _alerts.Count);
                        _alerts[i].Dismiss();
                        PercentComplete = i * 100 / _alerts.Count;
                    }
                }
            }
            finally
            {
                LogDescriptionChanges = true;
            }

            Description = _alerts.Count == 1
                ? Messages.ACTION_REMOVE_ALERTS_DONE_ONE
                : string.Format(Messages.ACTION_REMOVE_ALERTS_DONE, _alerts.Count);
        }
    }
}
