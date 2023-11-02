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
using System.Linq;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Diagnostics.Hotfixing
{
    internal class SingleHotfix : Hotfix
    {
        public override string UUID { get; set; }
        public override string Filename { get; set; }

        public sealed override void Apply(Host host, Session session)
        {
            UploadAndApplyUpdate(host, session);

            IXenObject patch;
            int numberRetries = 0;
            do
            {
                Thread.Sleep(500);
                patch = host.Connection.Cache.Find_By_Uuid<Pool_update>(UUID);
                numberRetries++;

            } while (patch == null && numberRetries < 10);
        }

        private void UploadAndApplyUpdate(Host host, Session session)
        {
            var update = host.Connection.Cache.Find_By_Uuid<Pool_update>(UUID);
            if (update == null)
            {
                var coordinator = Helpers.GetCoordinator(host.Connection);
                var filePath = Path.Combine(Program.AssemblyDir, $"{Filename}.iso");
                var action = new UploadUpdateAction(coordinator.Connection, new List<Host> { coordinator }, filePath, false);
                action.RunSync(session);
                update = action.PoolUpdate;
            }
            new ApplyUpdateAction(update, host, false).RunSync(session);
        }
        
        public override bool ShouldBeAppliedTo(Host host)
        {
            var updates = host.Connection.ResolveAll(host.updates);
            return !updates.Any(update => UUID.ToLowerInvariant().Contains(update.uuid.ToLowerInvariant()));
        }
    }

}
