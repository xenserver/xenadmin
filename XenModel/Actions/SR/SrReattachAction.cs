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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class SrReattachAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SR sr;
        private readonly String name;
        private readonly String description;
        private readonly Dictionary<String, String> dconf;

        /// <summary>
        /// RBAC dependencies needed to reattach SR.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies = new RbacMethodList("sr.set_name_label",
                                                                                 "sr.set_name_description",
                                                                                 "pbd.async_create",
                                                                                 "pbd.async_plug");

        public SrReattachAction(SR sr,
            String name, String description, Dictionary<String, String> dconf)
            : base(sr.Connection,
            string.Format(Messages.ACTION_SR_ATTACHING_TITLE, name, Helpers.GetName(sr.Connection)))
        {
            this.sr = sr;
            this.name = name;
            this.description = description;
            this.dconf = dconf;
        }

        protected override void Run()
        {
            log.Debug("Running SR Reconfigure Action");
            log.DebugFormat("SR uuid = '{0}'", sr.uuid);
            log.DebugFormat("name = '{0}'", name);
            log.DebugFormat("description = '{0}'", description);

            Description = Messages.ACTION_SR_ATTACHING;
                        
            // Repair the SR with new PBDs for each host in the pool
            PBD pbdTemplate = new PBD();
            pbdTemplate.currently_attached = false;
            pbdTemplate.device_config = dconf;
            pbdTemplate.SR = new XenRef<SR>(sr.opaque_ref);

            int delta = 100 / (Connection.Cache.HostCount * 2);
            foreach (Host host in Connection.Cache.Hosts)
            {
                // Create the PBD
                log.DebugFormat("Creating PBD for host {0}", host.Name);
                this.Description = String.Format(Messages.ACTION_SR_REPAIR_CREATE_PBD, Helpers.GetName(host));
                pbdTemplate.host = new XenRef<Host>(host.opaque_ref);
                RelatedTask = PBD.async_create(this.Session, pbdTemplate);
                PollToCompletion(PercentComplete, PercentComplete + delta);
                XenRef<PBD> pbdRef = new XenRef<PBD>(this.Result);

                // Now plug the PBD
                log.DebugFormat("Plugging PBD for host {0}", host.Name);
                this.Description = String.Format(Messages.ACTION_SR_REPAIR_PLUGGING_PBD, Helpers.GetName(host));
                RelatedTask = PBD.async_plug(this.Session, pbdRef);
                PollToCompletion(PercentComplete, PercentComplete + delta);
            }

            // Update the name and description of the SR
            XenAPI.SR.set_name_label(Session, sr.opaque_ref, name);
            XenAPI.SR.set_name_description(Session, sr.opaque_ref, description);

            Description = Messages.ACTION_SR_ATTACH_SUCCESSFUL;
        }
    }
}
