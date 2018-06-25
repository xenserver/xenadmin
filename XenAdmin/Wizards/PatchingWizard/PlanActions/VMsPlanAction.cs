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
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class VMsPlanAction : PlanActionWithSession
    {
        private readonly List<XenRef<VM>> _vms;

        protected VMsPlanAction(List<XenRef<VM>> vms, IXenConnection connection)
            : base(connection)
        {
            this._vms = vms;
        }

        protected abstract XenRef<Task> DoPerVM(Session session, VM vm);

        protected override void RunWithSession(ref Session session)
        {
            List<VM> vmObjs = new List<VM>();
            foreach (XenRef<VM> vm in _vms)
                vmObjs.Add(Connection.TryResolveWithTimeout(vm));

            PBD.CheckAndPlugPBDsFor(vmObjs);

            int vmCount = _vms.Count;
            int i = 0;

            foreach (VM vm in vmObjs)
            {
                XenRef<Task> task = DoPerVM(session, vm);

                try
                {
                    PollTaskForResult(Connection, ref session, task, delegate(int progress)
                    {
                        PercentComplete = progress/vmCount + 100*i/vmCount;
                    });

                    i++;
                }
                finally
                {
                    Task.destroy(session, task);
                }
            }
        }
    }
}