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
using System.IO;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions.Updates
{
    public class ApplyRpuHotfixAction : AsyncAction
    {
        private readonly Host _host;
        private readonly RpuHotfix _hotfix;
        private readonly string _hotfixDir;

        public ApplyRpuHotfixAction(Host host, RpuHotfix hotfix, string hotfixDir)
            : base(host.Connection, string.Format(Messages.APPLYING_HOTFIX_TO_HOST, host), true)
        {
            _host = host;
            _hotfix = hotfix;
            _hotfixDir = hotfixDir;
        }

        protected override void Run()
        {
            var update = _host.Connection.Cache.Find_By_Uuid<Pool_update>(_hotfix.Uuid);

            if (update == null)
            {
                var coordinator = Helpers.GetCoordinator(_host.Connection);
                var path = Path.Combine(_hotfixDir, $"{_hotfix.Name}.iso");
                var action = new UploadUpdateAction(coordinator.Connection, new List<Host> { coordinator }, path, false);
                action.RunSync(Session);
                update = action.PoolUpdate;
            }

            new ApplyUpdateAction(update, _host, false).RunSync(Session);

            _host.Connection.WaitFor(() => _host.Connection.Cache.Find_By_Uuid<Pool_update>(_hotfix.Uuid) != null, null);
        }
    }
}
