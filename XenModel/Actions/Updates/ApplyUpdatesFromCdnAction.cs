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

using System;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class ApplyUpdatesFromCdnAction : AsyncAction
    {
        private readonly CdnPoolUpdateInfo _updateInfo;

        public ApplyUpdatesFromCdnAction(Host host, CdnPoolUpdateInfo updateInfo)
            : base(host.Connection, string.Empty)
        {
            _updateInfo = updateInfo;
            Host = host;
            Title = Description = string.Format(Messages.ACTION_APPLY_CDN_UPDATES_TITLE, host.Name());
            ApiMethodsToRoleCheck.Add("host.apply_updates");
        }

        protected override void Run()
        {
            try
            {
                RelatedTask = Host.async_apply_updates(Session, Host.opaque_ref, _updateInfo.Checksum);
                PollToCompletion();
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 1 &&
                    f.ErrorDescription[0] == Failure.UPDATES_REQUIRE_RECOMMENDED_GUIDANCE &&
                    Enum.TryParse(f.ErrorDescription[1], true, out CdnGuidance guidance))
                {
                    throw new Exception(string.Format(Messages.CDN_PENDING_GUIDANCES_FAILURE, Cdn.FriendlyInstruction(guidance)));
                }

                throw;
            }
        }
    }
}
