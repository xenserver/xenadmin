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

namespace XenAdmin.Actions
{
    public class GetVMPPAlertsAction : PureAsyncAction
    {
        public readonly VMPP VMPP;
        private int _hoursFromNow;
        public GetVMPPAlertsAction(VMPP vmpp,int hoursfromNow)
            : base(vmpp.Connection, "", true)
        {
            VMPP = vmpp;
            _hoursFromNow = hoursfromNow;
        }

        protected override void Run()
        {
            var now = DateTime.Now;
            var result = new List<string>(VMPP.get_alerts(VMPP.Connection.Session, VMPP.opaque_ref, _hoursFromNow));
            
            var listAlerts=new List<PolicyAlert>();
            foreach(string item in result)
            {
                listAlerts.Add(new PolicyAlert(VMPP.Connection,item));
            }
            VMPP.Alerts=new List<PolicyAlert>(listAlerts);
            Debug.WriteLine(string.Format("GetAlerts took: {0}", DateTime.Now - now));
        }

 
    }
}
