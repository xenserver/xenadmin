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
using XenAdmin.Alerts;
using XenAPI;
using System.Diagnostics;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAdmin.Actions
{
    public class LoadVmssAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoadVmssAction(IXenConnection connection)
            : base(connection, "", true)
        {
            ServerLocalTime = null;
            SnapshotSchedules = new Dictionary<VMSS, List<Message>>();
        }

        public DateTime? ServerLocalTime { get; private set; }

        public Dictionary<VMSS, List<XenAPI.Message>> SnapshotSchedules { get; private set; }

        protected override void Run()
        {
            try
            {
                ServerLocalTime = Host.get_server_localtime(Connection.Session, Helpers.GetMaster(Connection).opaque_ref);
            }
            catch (Exception e)
            {
                log.Error("An error occurred while obtaining VMPP date time: ", e);
                ServerLocalTime = null;
            }

            var schedules = Connection.Cache.VMSSs;
            var messages = Pool.Connection.Cache.Messages;

            var allVmssMessages = (from XenAPI.Message msg in messages
                    where msg.cls == cls.VMSS
                    group msg by msg.obj_uuid
                    into g
                    let gOrdered = g.OrderByDescending(m => m.timestamp).ToList()
                    select new {PolicyUuid = g.Key, PolicyMessages = gOrdered})
                .ToDictionary(x => x.PolicyUuid, x => x.PolicyMessages);

            var filteredVmssMessages = new Dictionary<VMSS, List<XenAPI.Message>>();
            foreach (var schedule in schedules)
            {
                List<XenAPI.Message> value;
                if (!allVmssMessages.TryGetValue(schedule.uuid, out value))
                    value = new List<XenAPI.Message>();

                filteredVmssMessages[schedule] = value;
            }
            
            SnapshotSchedules = filteredVmssMessages;
        }
    }
}
