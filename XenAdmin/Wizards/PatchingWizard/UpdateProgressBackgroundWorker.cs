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
using System.ComponentModel;
using System.Linq;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using HostActionTuple = System.Tuple<XenAPI.Host, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>>;

namespace XenAdmin.Wizards.PatchingWizard
{
    class UpdateProgressBackgroundWorker : BackgroundWorker
    {
        private readonly int _actionsCount;
        public List<HostActionTuple> HostActions { get; private set; }
        public List<PlanAction> FinalActions { get; private set; }
        public readonly List<PlanAction> DoneActions = new List<PlanAction>();
        public readonly List<PlanAction> InProgressActions = new List<PlanAction>();

        public UpdateProgressBackgroundWorker(List<HostActionTuple> planActions, List<PlanAction> finalActions)
        {
            HostActions = planActions;
            FinalActions = finalActions;
            _actionsCount = HostActions.Sum(t => t.Item2.Count + t.Item3.Count) + FinalActions.Count;
        }

        public int ActionsCount
        {
            get { return _actionsCount; }
        }

        public new void CancelAsync()
        {
            if (HostActions != null)
                HostActions.ForEach(ha =>
                {
                    var cur = ha;
                    cur.Item2.ForEach(pa =>
                    {
                        if (!pa.IsComplete)
                            pa.Cancel();
                    });
                });

            base.CancelAsync();
        }
    }
}
