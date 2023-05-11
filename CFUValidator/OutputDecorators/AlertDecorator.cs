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
using System.Text.RegularExpressions;
using XenAdmin.Alerts;


namespace CFUValidator.OutputDecorators
{
    public interface ISummaryGenerator
    {
        string GenerateSummary();
    }

    public class HeaderDecorator : ISummaryGenerator
    {
        private readonly string _serverVersion;
        private readonly string _xmlLocation;

        public HeaderDecorator(string serverVersion, string xmlLocation)
        {
            _serverVersion = serverVersion;
            _xmlLocation = xmlLocation;
        }

        public string GenerateSummary()
        {
            return $"Summary for server version {_serverVersion}, XML from {_xmlLocation}, generated at {DateTime.Now.ToLocalTime()}\n";
        }
    }

    public abstract class AlertDecorator : ISummaryGenerator
    {
        public string GenerateSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine(SummaryTitle);
            sb.AppendLine(GenerateSummaryCore());
            return sb.ToString();
        }

        protected abstract string GenerateSummaryCore();

        protected abstract string SummaryTitle { get; }
    }

    class XenServerUpdateDecorator : AlertDecorator
    {
        private readonly List<XenServerVersionAlert> _alerts;

        public XenServerUpdateDecorator(List<XenServerVersionAlert> alerts)
        {
            _alerts = alerts;
        }

        protected override string GenerateSummaryCore()
        {
            if (_alerts.Count == 0)
                return "None";

            var sb = new StringBuilder();
            foreach (XenServerVersionAlert alert in _alerts)
                sb.AppendLine(alert == null ? "XenServer update could not be found" : alert.Version.Name);
            return sb.ToString();
        }

        protected override string SummaryTitle => "XenServer updates required:";
    }


    class PatchAlertDecorator : AlertDecorator
    {
        private readonly List<XenServerPatchAlert> _alerts;
        private const string HOTFIX_REGEX = "XS[0-9]+E[A-Z]*[0-9]+";

        public PatchAlertDecorator(List<XenServerPatchAlert> alerts)
        {
            _alerts = alerts;
        }

        protected override string GenerateSummaryCore()
        {
            if (_alerts.Count == 0)
                return "None";

            var sb = new StringBuilder();

            foreach (XenServerPatchAlert alert in _alerts.OrderBy(a => a.Patch.Name))
            {
                string patchName = Regex.Match(alert.Patch.Name, HOTFIX_REGEX).Value;

                if (string.IsNullOrEmpty(patchName))
                    patchName = alert.Patch.Name.Trim();

                if (string.IsNullOrEmpty(patchName))
                    patchName = $"Name unknown (uuid={alert.Patch.Uuid})";

                sb.AppendLine(patchName);
            }

            return sb.ToString();
        }

        protected override string SummaryTitle => "Patches required:";
    }
}
