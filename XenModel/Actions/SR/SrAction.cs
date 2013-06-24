/* Copyright (c) Citrix Systems Inc. 
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


namespace XenAdmin.Actions
{
    public enum SrActionKind { SetAsDefault, Detach, Forget, Destroy, UpgradeLVM };

    public class SrAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SrActionKind kind;

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

        private static String GetTitle(SrActionKind kind, SR sr)
        {
            switch (kind)
            {
                case SrActionKind.SetAsDefault:
                    return String.Format(Messages.ACTION_SR_SETTING_DEFAULT,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Detach:
                    return String.Format(Messages.ACTION_SR_DETACHING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Destroy:
                    return String.Format(Messages.ACTION_SR_DESTROYING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.Forget:
                    return String.Format(Messages.ACTION_SR_FORGETTING,
                        sr.Name, Helpers.GetName(sr.Connection));

                case SrActionKind.UpgradeLVM:
                    return String.Format(Messages.ACTION_SR_UPGRADE,
                        sr.Name, Helpers.GetName(sr.Connection));
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
                    DoDetach(ref inc);
                    Description = string.Format(Messages.ACTION_SR_DETACH_SUCCESSFUL, SR.NameWithoutHost);
                    break;

                case SrActionKind.Destroy:

                    foreach (XenRef<PBD> pbd in SR.PBDs)
                    {
                        RelatedTask = PBD.async_unplug(Session, pbd.opaque_ref);
                        PollToCompletion(PercentComplete, PercentComplete + inc);
                    }

                    RelatedTask = XenAPI.SR.async_destroy(Session, SR.opaque_ref);
                    PollToCompletion(50, 100);
                    Description = Messages.ACTION_SR_DESTROY_SUCCESSFUL;
                    break;

                case SrActionKind.Forget:
                    Description = string.Format(Messages.FORGETTING_SR_0, SR.NameWithoutHost);
                    if (!SR.IsDetached && SR.IsDetachable())
                        DoDetach(ref inc);
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

                case SrActionKind.UpgradeLVM:
                    Description = Messages.ACTION_SR_UPGRADING;

                    XenAPI.SR.remove_from_sm_config(Session, SR.opaque_ref, XenAPI.SR.USE_VHD);
                    XenAPI.SR.add_to_sm_config(Session, SR.opaque_ref, XenAPI.SR.USE_VHD, "true");

                    Description = Messages.ACTION_SR_UPGRADED;
                    break;
            }
        }

        private void DoDetach(ref int inc)
        {
            if (SR.PBDs.Count < 1)
                return;

            foreach (XenRef<PBD> pbd in SR.PBDs)
            {
                RelatedTask = PBD.async_unplug(Session, pbd.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);

                RelatedTask = PBD.async_destroy(Session, pbd.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }
        }
    }
}
