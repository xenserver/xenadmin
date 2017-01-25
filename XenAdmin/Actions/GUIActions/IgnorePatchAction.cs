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
using System.Collections.Generic;
using System.Text;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Linq;

namespace XenAdmin.Actions
{
    public class IgnorePatchAction : AsyncAction
    {
        public const string IgnorePatchKey = "XenCenter.IgnorePatches";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private XenServerPatch Patch;

        public IgnorePatchAction(IXenConnection connection, XenServerPatch patch)
            : base(connection, "ignore_patch", "ignore_patch", true)
        {
            Patch = patch;
        }

        protected override void Run()
        {
            XenAPI.Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                return;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(IgnorePatchKey))
            {
                List<string> current = new List<string>(other_config[IgnorePatchKey].Split(','));
                if (current.Contains(Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                    return;
                current.Add(Patch.Uuid);
                other_config[IgnorePatchKey] = string.Join(",", current.ToArray());
            }
            else
            {
                other_config.Add(IgnorePatchKey, Patch.Uuid);
            }

            XenAPI.Pool.set_other_config(Session, pool.opaque_ref, other_config);
        }
    }
}
