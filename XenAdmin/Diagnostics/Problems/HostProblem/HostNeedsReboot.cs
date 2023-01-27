﻿/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public class HostNeedsReboot : Information
    {
        private readonly Host host;
        private readonly bool livePatchingRestricted;
        private readonly bool livePatchingDisabled;

        public HostNeedsReboot(Check check, Host host, bool livePatchingRestricted = false, bool livePatchingDisabled = false)
            : base(check)
        {
            this.host = host;
            this.livePatchingRestricted = livePatchingRestricted;
            this.livePatchingDisabled = livePatchingDisabled;
        }

        public override string Title => Description;

        public override string Description => livePatchingRestricted
            ? string.Format(Messages.UPDATES_WIZARD_REBOOT_NEEDED_LIVEPATCH_RESTRICTED, host.name_label)
            : livePatchingDisabled
                ? string.Format(Messages.UPDATES_WIZARD_REBOOT_NEEDED_LIVEPATCH_DISABLED, host.name_label)
                : string.Format(Messages.UPDATES_WIZARD_REBOOT_NEEDED, host.name_label);
    }
}
