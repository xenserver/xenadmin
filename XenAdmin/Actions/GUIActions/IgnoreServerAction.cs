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

using System;
using System.Collections.Generic;
using System.Text;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class IgnoreServerAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private XenServerVersion Version;

        public const string LAST_SEEN_SERVER_VERSION_KEY = "XenCenter.LastSeenServerVersion";

        public IgnoreServerAction(IXenConnection connection, XenServerVersion version)
            : base(connection, "ignore_patch", "ignore_patch", true)
        {
            Version = version;
        }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                return;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(LAST_SEEN_SERVER_VERSION_KEY))
            {
                List<string> current = new List<string>(other_config[LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                if (current.Contains(Version.Version.ToString()))
                    return;
                current.Add(Version.Version.ToString());
                other_config[LAST_SEEN_SERVER_VERSION_KEY] = string.Join(",", current.ToArray());
            }
            else
            {
                other_config.Add(LAST_SEEN_SERVER_VERSION_KEY, Version.Version.ToString());
            }

            XenAPI.Pool.set_other_config(Session, pool.opaque_ref, other_config);
        }
    }
}
