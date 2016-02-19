﻿/* Copyright (c) Citrix Systems Inc. 
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

namespace XenAdmin.Actions
{
    public class GetVMSSAlertsAction : PureAsyncAction
    {
        public readonly VMSS VMSS;
        private int _hoursFromNow;
        public GetVMSSAlertsAction(VMSS vmss, int hoursfromNow)
            : base(vmss.Connection, "", true)
        {
            VMSS = vmss;
            _hoursFromNow = hoursfromNow;
        }

        protected override void Run()
        {
            var now = DateTime.Now;
            var messages = Pool.Connection.Cache.Messages;
            var listAlerts = new List<PolicyAlert>();

            /*for VMSS: Populate the alerts from Messages by filtering out the alerts for this schedule
                This is not required in VMPP as the VMPP record itself has the recentAlerts */
            foreach (var message in messages)
            {
                if (message.cls == cls.VMSS && message.obj_uuid == VMSS.uuid)
                {
                    listAlerts.Add(new PolicyAlert(message.priority, message.name, message.timestamp));
                }
            }

            VMSS.Alerts = new List<PolicyAlert>(listAlerts);
            Debug.WriteLine(string.Format("GetAlerts took: {0}", DateTime.Now - now));
        }

 
    }
}
