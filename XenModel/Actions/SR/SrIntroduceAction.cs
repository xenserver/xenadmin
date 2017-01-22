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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class SrIntroduceAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _srUuid;
        private readonly string _srName;
        private readonly string _srDescription;
        private readonly string _srContentType;
        private readonly bool _srIsShared;
        private readonly SR.SRTypes _srType;
        private readonly Dictionary<string, string> _dconf;

        public SrIntroduceAction(IXenConnection connection,
            String srUuid, String srName, String srDescription, SR.SRTypes srType, 
            String srContentType, Dictionary<String, String> dconf)
            : base(connection, string.Format(Messages.ACTION_SR_ATTACHING_TITLE, srName, Helpers.GetName(connection)))
        {
            _srUuid = srUuid;
            _srName = srName;
            _srDescription = srDescription;
            _srContentType = srContentType;
            _srType = srType;
            _srIsShared = true;  // used to depend on restrict_pool_attached_storage flag: now always true, but left in in case we want to create local SRs one day
            _dconf = dconf;
        }

        protected override void Run()
        {
            log.Debug("Running SR.Introduce");
            log.DebugFormat("SR uuid='{0}'", _srUuid);
            log.DebugFormat("name='{0}'", _srName);
            log.DebugFormat("description='{0}'", _srDescription);
            log.DebugFormat("type='{0}'", _srType);
            log.DebugFormat("content type='{0}'", _srContentType);
            log.DebugFormat("is shared='{0}'", _srIsShared);

            Description = Messages.ACTION_SR_ATTACHING;
            // If SR is already attached, forget it (it may be in a broken invisible state with no PBDs)
            try
            {
                log.Debug("Performing preemptive SR.forget()");
                RelatedTask = XenAPI.SR.async_forget(this.Session, XenAPI.SR.get_by_uuid(this.Session,
                    _srUuid).opaque_ref);
                PollToCompletion(0, 5);
            }
            catch (Failure)
            {
                // Allow failure
            }

            // Introduce the existing SR
            RelatedTask = XenAPI.SR.async_introduce(this.Session, _srUuid, _srName,
                _srDescription, _srType.ToString(), _srContentType,
                _srIsShared, new Dictionary<string, string>());
            PollToCompletion(5, 10);

            // cache result, in order to reassign it later
            string introducedSr = Result;

            // Now repair the SR with new PBDs for each host in the pool
            XenAPI.PBD pbdTemplate = new PBD();
            pbdTemplate.currently_attached = false;
            pbdTemplate.device_config = _dconf;
            pbdTemplate.SR = new XenRef<SR>(Result);
            int delta = 90 / Connection.Cache.HostCount / 2;
            foreach (Host host in Connection.Cache.Hosts)
            {
                // Create the PBD
                log.DebugFormat("Creating PBD for host {0}", host.Name);
                this.Description = string.Format(Messages.ACTION_SR_REPAIR_CREATE_PBD, Helpers.GetName(host));
                pbdTemplate.host = new XenRef<Host>(host.opaque_ref);
                RelatedTask = PBD.async_create(this.Session, pbdTemplate);
                PollToCompletion(PercentComplete, PercentComplete + delta);
                XenRef<PBD> pbdRef = new XenRef<PBD>(this.Result);

                // Now plug the PBD
                log.DebugFormat("Plugging PBD for host {0}", host.Name);
                this.Description = string.Format(Messages.ACTION_SR_REPAIR_PLUGGING_PBD, Helpers.GetName(host));
                RelatedTask = XenAPI.PBD.async_plug(this.Session, pbdRef);
                PollToCompletion(PercentComplete, PercentComplete + delta);
            }

            // reassign result
            Result = introducedSr;

            if (isFirstSharedNonISOSR())
            {
                SR new_sr = Connection.WaitForCache(new XenRef<SR>(Result), GetCancelling);
                if (Cancelling)
                    throw new CancelledException();
                if (new_sr == null)
                    throw new Failure(Failure.HANDLE_INVALID, "SR", Result);

                // Set this SR to be the default
                new SrAction(SrActionKind.SetAsDefault, new_sr).RunExternal(Session);
            }

            Description = Messages.ACTION_SR_ATTACH_SUCCESSFUL;
        }

        private bool isFirstSharedNonISOSR()
        {
            if (_srType == XenAPI.SR.SRTypes.iso || !_srIsShared)
                return false;
            foreach (SR sr in Connection.Cache.SRs)
            {
                if (sr.opaque_ref != Result &&
                    sr.GetSRType(false) != XenAPI.SR.SRTypes.iso &&
                    sr.shared)
                    return false;
            }
            return true;
        }
    }
}
