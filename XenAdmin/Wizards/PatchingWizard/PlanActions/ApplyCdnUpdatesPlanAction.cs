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

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class ApplyCdnUpdatesPlanAction : HostPlanAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CdnPoolUpdateInfo _updateInfo;

        public ApplyCdnUpdatesPlanAction(Host host, CdnPoolUpdateInfo updateInfo)
            : base(host)
        {
            _updateInfo = updateInfo;
        }

        protected override void RunWithSession(ref Session session)
        {
            var host = GetResolvedHost();

            if (host.enabled)
            {
                log.DebugFormat("Disabling host {0}", host.Name());
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_ENTERING_MAINTENANCE_MODE, host.Name()));
                Host.disable(session, HostXenRef.opaque_ref);
            }

            AddProgressStep(string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATES_FROM_CDN, host.Name()));
            new ApplyUpdatesFromCdnAction(host, _updateInfo).RunSync(session);
        }
    }


    class CheckForCdnUpdatesPlanAction : PlanActionWithSession
    {
        public CheckForCdnUpdatesPlanAction(IXenConnection connection)
            : base(connection)
        {
        }

        protected override void RunWithSession(ref Session session)
        {
            AddProgressStep(string.Format(Messages.UPDATES_WIZARD_REFRESHING_CDN_UPDATES_LIST, Connection.Name));
            Updates.CheckForCdnUpdates(Connection, true);
        }
    }
}
