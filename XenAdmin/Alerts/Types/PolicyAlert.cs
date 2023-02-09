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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Alerts
{
    public class PolicyAlert : MessageAlert
    {
        private readonly Regex VmErrorRegex = new Regex(@"^VM:(.*)UUID:(.*)Error:\[('.*',?)*\],?$");
        public readonly PolicyAlertType Type;
        public readonly string _description;
        public readonly string _title;
        public readonly DateTime Time;

        public PolicyAlert(Message msg)
            : base(msg)
        {
            var policyName = msg.GetXenObject() is VMSS vmss ? vmss.Name() : "";

            Time = msg.TimestampLocal();
            Type = FromPriority(msg.priority);

            if (Type != PolicyAlertType.Error)
            {
                _title = string.Format(Message.FriendlyBody(msg.name), policyName);
                _description = _title;
                return;
            }

            //Parse the message body to get the errors per VM

            var vmFailures = new List<string>();

            using (var sr = new StringReader(msg.body))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var match = VmErrorRegex.Match(line);
                    if (!match.Success)
                        continue;

                    var vmName = match.Groups[1].Value.Trim();
                    var errorCode = match.Groups[3].Value
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().TrimStart('\'').TrimEnd('\'')).FirstOrDefault();

                    if (!string.IsNullOrEmpty(errorCode))
                    {
                        string errorMessage = new Failure(errorCode).Message;
                        vmFailures.Add(string.Format(Messages.VMSS_ALERT_VM_ERROR_FORMAT, vmName, errorMessage));
                    }
                }
            }

            if (vmFailures.Count > 0)
            {
                _title = string.Format(Messages.VM_SNAPSHOT_SCHEDULE_FAILED,
                    Message.FriendlyName(Message.MessageType.VMSS_SNAPSHOT_FAILED.ToString()), vmFailures.Count);

                var sb = new StringBuilder();
                sb.AppendFormat(Message.FriendlyBody(msg.name), policyName);
                vmFailures.ForEach(f => sb.AppendLine().Append(f));
                _description = sb.ToString();
            }
            else
            {
                _title = Message.FriendlyName(Message.MessageType.VMSS_SNAPSHOT_FAILED.ToString());
                _description = string.Format(Message.FriendlyBody(msg.name), policyName);
            }
        }

        public override string Title => _title;

        public override string Description => _description;

        public override bool Equals(Alert other)
        {
            return other is PolicyAlert pa &&
                   Type == pa.Type &&
                   Time == pa.Time &&
                   Description == pa.Description &&
                   Title == pa.Title;
        }

        public static PolicyAlertType FromPriority(long priority)
        {
            //CA-343763: The following logic for assigning the PolicyAlert.Type works also
            //for pre-Stockholm servers where info was erroneously set to 4 and warn to 1.

            if (priority < 3)
                return PolicyAlertType.Error;

            if (priority == 3)
                return PolicyAlertType.Warn;

            return PolicyAlertType.Info;
        }
    }

    public enum PolicyAlertType
    {
        Error = 1,
        Warn = 3,
        Info = 5
    }

    public static class PolicyAlertTypeExtensions
    {
        public static string GetString(this PolicyAlertType paType)
        {
            switch (paType)
            {
                case PolicyAlertType.Error:
                    return Messages.FAILED;
                case PolicyAlertType.Warn:
                    return Messages.WARNING;
                case PolicyAlertType.Info:
                    return Messages.SUCCEEDED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paType), paType, null);
            }
        }

        public static Bitmap GetImage(this PolicyAlertType paType)
        {
            switch (paType)
            {
                case PolicyAlertType.Error:
                    return Images.StaticImages._000_error_h32bit_16;
                case PolicyAlertType.Warn:
                    return Images.StaticImages._075_WarningRound_h32bit_16;
                case PolicyAlertType.Info:
                    return Images.StaticImages._075_TickRound_h32bit_16;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paType), paType, null);
            }
        }
    }
}
