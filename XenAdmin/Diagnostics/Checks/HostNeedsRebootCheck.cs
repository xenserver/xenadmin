﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Collections.Generic;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Wizards.PatchingWizard;

namespace XenAdmin.Diagnostics.Checks
{
    public class HostNeedsRebootCheck : Check
    {
        private readonly Dictionary<string, livepatch_status> livePatchCodesByHost;
        private readonly List<after_apply_guidance> patchGuidance;
        private readonly List<update_after_apply_guidance> updateGuidance;

        private string successfulCheckDescription;

        public HostNeedsRebootCheck(Host host, List<update_after_apply_guidance> guidance, Dictionary<string, livepatch_status> livePatchCodesByHost)
            : base(host)
        {
            this.livePatchCodesByHost = livePatchCodesByHost;
            updateGuidance = guidance;
        }

        public HostNeedsRebootCheck(Host host, List<after_apply_guidance> guidance, Dictionary<string, livepatch_status> livePatchCodesByHost)
            : base(host)
        {
            this.livePatchCodesByHost = livePatchCodesByHost;
            patchGuidance = guidance;
        }

        protected override Problem RunCheck()
        {
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);

            // when livepatching is available, no restart is expected
            if (livePatchCodesByHost != null && livePatchCodesByHost.ContainsKey(Host.uuid) &&
                livePatchCodesByHost[Host.uuid] == livepatch_status.ok_livepatch_complete)
            {
                successfulCheckDescription = string.Format(Messages.UPDATES_WIZARD_NO_REBOOT_NEEDED_LIVE_PATCH, Host);
                return null;
            }
            
            if ((updateGuidance != null && updateGuidance.Contains(update_after_apply_guidance.restartHost))
                || (patchGuidance != null && patchGuidance.Contains(after_apply_guidance.restartHost)))
            {
                 return new HostNeedsReboot(this, Host);
            }

            successfulCheckDescription = string.Format(Messages.UPDATES_WIZARD_NO_REBOOT_NEEDED, Host);
            return null;
        }
        
        public override string Description
        {
            get { return Messages.HOST_NEEDS_REBOOT_CHECK_DESCRIPTION; }
        }

        public override string SuccessfulCheckDescription
        {
            get { return successfulCheckDescription; }
        }
    }
}