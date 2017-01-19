/* Copyright (c) Citrix Systems, Inc. All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met: 
 * 
 * *  Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer. 
 * *  Redistributions in binary form must reproduce the above 
 *    copyright notice, this list of conditions and the following
 *    disclaimer in the documentation and/or other materials provided
 *    with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    class BackgroundThreadBase
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected Thread _bw;
        protected IDictionary<Host, List<PlanAction>> _planActions;
        protected PlanAction _revertAction;
        protected IEnumerable<Host> _mastersToUpgrade;

        public event Action<Host> ReportHostDone;
        public event Action<PlanAction, Host> ReportRunning;
        public event Action<Exception, PlanAction, Host> ReportException;
        public event Action ReportRevertDone;
        public event Action Completed;

        public void Start()
        {
            _bw.Start();
        }

        protected volatile bool _cancel = false;
        internal void Cancel()
        {
            _cancel = true;
        }

        protected void OnReportHostDone(Host host)
        {
            if (ReportRevertDone != null)
                ReportHostDone(host);
        }

        protected void OnReportRunning(PlanAction planAction, Host host)
        {
            if (ReportRunning != null)
                ReportRunning(planAction, host);
        }

        protected void OnReportException(Exception ex, PlanAction planAction, Host host)
        {
            if (ReportException != null)
                ReportException(ex, planAction, host);
        }

        protected void OnReportRevertDone()
        {
            if (ReportRevertDone != null)
                ReportRevertDone();
        }

        protected void OnCompleted()
        {
            if (Completed != null)
                Completed();
        }
    }   
}
