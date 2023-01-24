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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class SrProbeAction : AsyncAction
    {
        private readonly Host host;
        private readonly Dictionary<string, string> dconf;
        private readonly Dictionary<string, string> smconf;

        public SR.SRTypes SrType { get; }
        public List<SR.SRInfo> SRs { get; private set; }

        /// <summary>
        /// Won't appear in the program history (SuppressHistory == true).
        /// </summary>
        public SrProbeAction(IXenConnection connection, Host host, SR.SRTypes srType,
            Dictionary<string, string> dconf, Dictionary<string, string> smconf = null)
            : base(connection, string.Format(Messages.ACTION_SCANNING_SR_FROM, Helpers.GetName(connection)), null, true)
        {
            this.host = host;
            this.SrType = srType;
            this.dconf = dconf;

            string target;
            switch (srType)
            {
                case SR.SRTypes.nfs:
                    target = dconf["server"];
                    break;
                case SR.SRTypes.lvmoiscsi:
                    target = dconf["target"];
                    break;
                case SR.SRTypes.lvmohba:
                case SR.SRTypes.lvmofcoe:
                    target = dconf.ContainsKey("device") ? dconf["device"] : dconf["SCSIid"];
                    break;
                case SR.SRTypes.gfs2:
                    target = dconf.ContainsKey("target") ? dconf["target"] : dconf["SCSIid"];
                    break;
                default:
                    target = Messages.REPAIRSR_SERVER;
                    break;
            }

            Description = string.Format(Messages.ACTION_SR_SCANNING, SR.GetFriendlyTypeName(srType), target);

            this.smconf = smconf ?? new Dictionary<string, string>();

            #region RBAC Dependencies

            if (SrType == SR.SRTypes.gfs2)
            {
                ApiMethodsToRoleCheck.Add("sr.probe_ext");
            }
            else
            {
                ApiMethodsToRoleCheck.Add("sr.probe");
                ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            }

            #endregion
        }

        protected override void Run()
        {
            if (SrType != SR.SRTypes.gfs2)
            {
                RelatedTask = SR.async_probe(Session, host.opaque_ref,
                    dconf, SrType.ToString().ToLowerInvariant(), smconf);
                PollToCompletion();
                SRs = SR.ParseSRListXML(Result);
            }
            else
            {
                try
                {
                    var result = SR.probe_ext(this.Session, host.opaque_ref,
                        dconf, SrType.ToString().ToLowerInvariant(), smconf);
                    SRs = SR.ParseSRList(result);
                }
                catch (Failure f)
                {
                    if (f.ErrorDescription.Count > 1 && f.ErrorDescription[0].StartsWith("SR_BACKEND_FAILURE") &&
                        f.ErrorDescription[1] == "DeviceNotFoundException")
                    {
                        //Ignore: special treatment of case where gfs2 cannot see the same devices as lvmohba (CA-335356)
                    }
                    else if (f.ErrorDescription.Count > 1 && f.ErrorDescription[0] == "ISCSILogin" &&
                             dconf.ContainsKey("chapuser") && dconf.ContainsKey("chappassword"))
                    {
                        //Ignore: special treatment of gfs2 chap authentication failure (CA-337280)
                    }
                    else
                        throw;
                }
            }

            Description = Messages.ACTION_SR_SCAN_SUCCESSFUL;
        }
    }
}
