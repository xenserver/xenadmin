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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class SrProbeAction : PureAsyncAction
    {
        private readonly Host host;
        private readonly SR.SRTypes srType;
        private readonly Dictionary<String, String> dconf;
        public const String DEVICE = "device";
        public const String SCSIid = "SCSIid";
        public const String PATH = "path";

        private readonly Dictionary<String, String> smconf;
        /// <summary>
        /// Won't appear in the program history (SuppressHistory == true).
        /// </summary>
        /// <param name="masterUuid">The UUID of the host from which to perform the probe (usually the pool master).</param>
        /// <param name="srType">netapp or iscsi</param>
        public SrProbeAction(IXenConnection connection, Host host, SR.SRTypes srType, Dictionary<String, String> dconf)
            : base(connection, string.Format(Messages.ACTION_SCANNING_SR_FROM, Helpers.GetName(connection)), null, true)
        {
            this.host = host;
            this.srType = srType;
            this.dconf = dconf;

            switch (srType) {
                case XenAPI.SR.SRTypes.nfs:
                    Description = string.Format(Messages.ACTION_SR_SCANNING,
                        XenAPI.SR.getFriendlyTypeName(srType), dconf["server"]);
                    break;
                case XenAPI.SR.SRTypes.lvmoiscsi:
                    Description = string.Format(Messages.ACTION_SR_SCANNING,
                        XenAPI.SR.getFriendlyTypeName(srType), dconf["target"]);
                    break;
                case XenAPI.SR.SRTypes.lvmohba:
                case XenAPI.SR.SRTypes.lvmofcoe:
                    String device = dconf.ContainsKey(DEVICE) ?
                        dconf[DEVICE] : dconf[SCSIid];
                    Description = string.Format(Messages.ACTION_SR_SCANNING,
                        XenAPI.SR.getFriendlyTypeName(srType), device);
                    break;
                default:
                    Description = string.Format(Messages.ACTION_SR_SCANNING,
                        XenAPI.SR.getFriendlyTypeName(srType), Messages.REPAIRSR_SERVER); // this is a bit minging: CA-22111
                    break;
            }
            smconf = new Dictionary<string, string>();
        }

        public SrProbeAction(IXenConnection connection, Host host, SR.SRTypes srType, Dictionary<String, String> dconf, Dictionary<String, String> smconf)
            : this(connection, host, srType, dconf)
        {
            this.smconf = smconf;
        }

        protected override void Run()
        {
            RelatedTask = XenAPI.SR.async_probe(this.Session, host.opaque_ref,
                dconf, srType.ToString().ToLowerInvariant(), smconf);
            PollToCompletion();
            Description = Messages.ACTION_SR_SCAN_SUCCESSFUL;
        }
    }
}
