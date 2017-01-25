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
using XenAdmin.Core;
using XenAPI;
using System.Collections.Generic;


namespace XenAdmin.Actions
{
    public enum SrActionKind { SetAsDefault, Detach, Forget, Destroy, UnplugAndDestroyPBDs, ConvertToThin };

    public class SrAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SrActionKind kind;
        private readonly Dictionary<string, string> parameters;

        public SrAction(SrActionKind kind, SR sr)
            : base(sr.Connection, GetTitle(kind, sr))
        {
            this.kind = kind;
            this.SR = sr;
            Pool pool = Helpers.GetPoolOfOne(sr.Connection);
            if (pool != null)
                Pool = pool;
            Host host = sr.GetStorageHost();
            if (host != null)
                Host = host;
        }

        public SrAction(SrActionKind kind, SR sr, Dictionary<string, string> parameters)
            : this(kind, sr)
        {
            this.parameters = parameters;
        }

        private static String GetTitle(SrActionKind kind, SR sr)
        {
            switch (kind)
            {
                case SrActionKind.SetAsDefault:
                    return String.Format(Messages.ACTION_SR_SETTING_DEFAULT,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Detach:
                case SrActionKind.UnplugAndDestroyPBDs:
                    return String.Format(Messages.ACTION_SR_DETACHING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Destroy:
                    return String.Format(Messages.ACTION_SR_DESTROYING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Forget:
                    return String.Format(Messages.ACTION_SR_FORGETTING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.ConvertToThin:
                    return String.Format(Messages.ACTION_SR_CONVERT_TO_THIN,
                        sr.NameWithLocation);
            }

            return "";
        }

        protected override void Run()
        {
            log.DebugFormat("Running SrActionKind.{0}", kind.ToString());

            int inc = SR.PBDs.Count > 0 ? 100 / (SR.PBDs.Count * 2) : 0;

            switch (kind)
            {
                case SrActionKind.Detach:
                    UnplugPBDs(ref inc);
                    Description = string.Format(Messages.ACTION_SR_DETACH_SUCCESSFUL, SR.NameWithoutHost);
                    break;

                case SrActionKind.Destroy:
                    if (!Helpers.TampaOrGreater(Connection))
                        UnplugPBDs(ref inc);
                    RelatedTask = XenAPI.SR.async_destroy(Session, SR.opaque_ref);
                    PollToCompletion(50, 100);
                    Description = Messages.ACTION_SR_DESTROY_SUCCESSFUL;
                    break;

                case SrActionKind.Forget:
                    Description = string.Format(Messages.FORGETTING_SR_0, SR.NameWithoutHost);
                    if (!Helpers.TampaOrGreater(Connection) && !SR.IsDetached && SR.IsDetachable())
                        UnplugPBDs(ref inc);
                    if (!SR.allowed_operations.Contains(storage_operations.forget))
                    {
                        Description = Messages.ERROR_DIALOG_FORGET_SR_TITLE;
                        break;
                    }
                        
                    RelatedTask = XenAPI.SR.async_forget(Session, SR.opaque_ref);
                    PollToCompletion();
                    Description = string.Format(Messages.SR_FORGOTTEN_0, SR.NameWithoutHost);
                    break;

                case SrActionKind.SetAsDefault:
                    XenRef<SR> r = new XenRef<SR>(SR);
                    Pool = Helpers.GetPoolOfOne(Connection);
                    Description = string.Format(Messages.ACTION_SR_SETTING_DEFAULT, SR, Pool);
                    Pool poolCopy = (Pool)Pool.Clone();
                    if (Pool != null)
                    {
                        poolCopy.crash_dump_SR = r;
                        poolCopy.default_SR = r;
                        poolCopy.suspend_image_SR = r;
                        try
                        {
                            Pool.Locked = true;
                            poolCopy.SaveChanges(Session);
                        }
                        finally
                        {
                            Pool.Locked = false;
                        }
                    }

                    Description = Messages.ACTION_SR_SET_DEFAULT_SUCCESSFUL;
                    break;

                case SrActionKind.UnplugAndDestroyPBDs:
                    UnplugPBDs(ref inc);
                    DestroyPBDs(ref inc);
                    Description = string.Format(Messages.ACTION_SR_DETACH_SUCCESSFUL, SR.NameWithoutHost);
                    break;

                case SrActionKind.ConvertToThin:
                    Description = string.Format(Messages.ACTION_SR_CONVERTING_TO_THIN, SR.NameWithLocation);

                    long initial_allocation = 0;
                    long allocation_quantum = 0;

                    if (parameters != null)
                    {
                        if (parameters.ContainsKey("initial_allocation"))
                            long.TryParse(parameters["initial_allocation"], out initial_allocation);

                        if (parameters.ContainsKey("allocation_quantum"))
                            long.TryParse(parameters["allocation_quantum"], out allocation_quantum);
                    }

                    LVHD.enable_thin_provisioning(Session, Host.opaque_ref, SR.opaque_ref, initial_allocation, allocation_quantum);

                    Description = string.Format(Messages.ACTION_SR_CONVERTED_TO_THIN, SR.NameWithLocation);
                    break;
            }
        }

        private void UnplugPBDs(ref int inc)
        {
            if (SR.PBDs.Count < 1)
                return;

            //CA-176935, CA-173497 - we need to run Unplug for the master last - creating a new list of hosts where the master is always the last
            var allPBDRefsToNonMaster = new List<XenRef<PBD>>();
            var allPBDRefsToMaster = new List<XenRef<PBD>>();

            var master = Helpers.GetMaster(Connection);

            foreach (var pbdRef in SR.PBDs)
            {
                var pbd = Connection.Resolve(pbdRef);
                if (pbd != null)
                {
                    if (pbd.host != null)
                    {
                        var host = Connection.Resolve(pbd.host);
                        if (master != null && host != null && host.uuid == master.uuid)
                        {
                            allPBDRefsToMaster.Add(pbdRef);
                        }
                        else
                        {
                            allPBDRefsToNonMaster.Add(pbdRef);
                        }
                    }
                }
            }

            var allPBDRefs = new List<XenRef<PBD>>();
            allPBDRefs.AddRange(allPBDRefsToNonMaster);
            allPBDRefs.AddRange(allPBDRefsToMaster);

            foreach (XenRef<PBD> pbd in allPBDRefs)
            {
                RelatedTask = PBD.async_unplug(Session, pbd.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }
        }

        private void DestroyPBDs(ref int inc)
        {
            if (SR.PBDs.Count < 1)
                return;

            foreach (XenRef<PBD> pbd in SR.PBDs)
            {
                RelatedTask = PBD.async_destroy(Session, pbd.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }
        }
    }
}
