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
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class DestroyHostCrashDumpAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DestroyHostCrashDumpAction(Host host)
            : base(host.Connection, string.Format(Messages.HOST_REMOVING_CRASHDUMPS_TITLE, host.Name))
        {
            Host = host;
        }

        protected override void Run()
        {
            string hostname = Helpers.GetName(Host);
            int max = Host.crashdumps.Count;
            if (max == 0)
            {
                Description = Messages.HOST_NO_CRASHDUMPS;
                return;
            }
            int i = 0;
            int delta = 100 / max;
            foreach (Host_crashdump dump in Host.Connection.ResolveAll(Host.crashdumps))
            {
                i++;
                Description = string.Format(Messages.HOST_REMOVING_CRASHDUMP, i, max, hostname);
                if (Cancelling)
                    return;

                try
                {
                    Host_crashdump.destroy(Session, dump.opaque_ref);
                }
                catch (Exception exn)
                {
                    log.Error(exn, exn);
                }

                PercentComplete += delta;
            }
            if (i == 1)
                Description = string.Format(Messages.HOST_REMOVED_ONE_CRASHDUMP, hostname);
            else
                Description = string.Format(Messages.HOST_REMOVED_MANY_CRASHDUMPS, i, hostname);
        }
    }
}
