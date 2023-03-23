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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public interface ISrAction
    {
    }


    public class SetSrAsDefaultAction : AsyncAction, ISrAction
    {
        public SetSrAsDefaultAction(SR sr)
            : base(sr.Connection, string.Format(Messages.ACTION_SR_SETTING_DEFAULT, sr.Name(), Helpers.GetName(sr.Connection)))
        {
            SR = sr;
            Host = sr.GetStorageHost();
            ApiMethodsToRoleCheck.AddRange(
                "pool.set_crash_dump_SR",
                "pool.set_default_SR",
                "pool.set_suspend_image_SR");
        }

        protected override void Run()
        {
            XenRef<SR> srRef = new XenRef<SR>(SR);
            Pool = Helpers.GetPoolOfOne(Connection);

            if (Pool != null)
            {
                Description = string.Format(Messages.ACTION_SR_SETTING_DEFAULT, SR, Pool);
                Pool poolCopy = (Pool)Pool.Clone();
                poolCopy.crash_dump_SR = srRef;
                poolCopy.default_SR = srRef;
                poolCopy.suspend_image_SR = srRef;

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
        }
    }

 
    public class DetachSrAction : AsyncAction, ISrAction
    {
        private readonly bool _destroyPbds;

        public DetachSrAction(SR sr, bool destroyPbds = false)
            : base(sr.Connection, string.Format(Messages.ACTION_SR_DETACHING, sr.Name(), Helpers.GetName(sr.Connection)))
        {
            _destroyPbds = destroyPbds;

            SR = sr;
            Host = sr.GetStorageHost();
            
            ApiMethodsToRoleCheck.Add("PBD.async_unplug");
            if (_destroyPbds)
                ApiMethodsToRoleCheck.Add("PBD.async_destroy");
        }

        protected override void Run()
        {
            int inc = SR.PBDs.Count > 0 ? 100 / (SR.PBDs.Count * 2) : 0;
            
            UnplugPBDs(ref inc);

            if (_destroyPbds)
                DestroyPBDs(ref inc);

            Description = string.Format(Messages.ACTION_SR_DETACH_SUCCESSFUL, SR.NameWithoutHost());
        }

        private void UnplugPBDs(ref int inc)
        {
            if (SR.PBDs.Count < 1)
                return;

            //CA-176935, CA-173497 - we need to run Unplug for the coordinator last - creating a new list of hosts where the coordinator is always the last
            var allPBDRefsToNonCoordinator = new List<XenRef<PBD>>();
            var allPBDRefsToCoordinator = new List<XenRef<PBD>>();

            var coordinator = Helpers.GetCoordinator(Connection);

            foreach (var pbdRef in SR.PBDs)
            {
                var pbd = Connection.Resolve(pbdRef);
                if (pbd != null)
                {
                    if (pbd.host != null)
                    {
                        var host = Connection.Resolve(pbd.host);
                        if (coordinator != null && host != null && host.uuid == coordinator.uuid)
                        {
                            allPBDRefsToCoordinator.Add(pbdRef);
                        }
                        else
                        {
                            allPBDRefsToNonCoordinator.Add(pbdRef);
                        }
                    }
                }
            }

            var allPBDRefs = new List<XenRef<PBD>>();
            allPBDRefs.AddRange(allPBDRefsToNonCoordinator);
            allPBDRefs.AddRange(allPBDRefsToCoordinator);

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


    public class ForgetSrAction : AsyncAction, ISrAction
    {
        public ForgetSrAction(SR sr)
            : base(sr.Connection, string.Format(Messages.ACTION_SR_FORGETTING, sr.Name(), Helpers.GetName(sr.Connection)))
        {
            SR = sr;
            Host = sr.GetStorageHost();
            ApiMethodsToRoleCheck.Add("SR.async_forget");
        }

        protected override void Run()
        {
            Description = string.Format(Messages.FORGETTING_SR_0, SR.NameWithoutHost());
            if (!SR.allowed_operations.Contains(storage_operations.forget))
            {
                Description = Messages.ERROR_DIALOG_FORGET_SR_TITLE;
                return;
            }

            RelatedTask = SR.async_forget(Session, SR.opaque_ref);
            PollToCompletion();
            Description = string.Format(Messages.SR_FORGOTTEN_0, SR.NameWithoutHost());
        }
    }


    public class DestroySrAction : AsyncAction, ISrAction
    {
        public DestroySrAction(SR sr)
            : base(sr.Connection, string.Format(Messages.ACTION_SR_DESTROYING, sr.Name(), Helpers.GetName(sr.Connection)))
        {
            SR = sr;
            Host = sr.GetStorageHost();
            ApiMethodsToRoleCheck.Add("SR.async_destroy");
        }

        protected override void Run()
        {
            RelatedTask = SR.async_destroy(Session, SR.opaque_ref);
            PollToCompletion(50, 100);
            Description = Messages.ACTION_SR_DESTROY_SUCCESSFUL;
        }
    }
}
