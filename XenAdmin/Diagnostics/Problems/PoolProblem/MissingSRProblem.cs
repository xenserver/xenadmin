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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Wizards;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    public class MissingSRProblem : Problem
    {
        private readonly Pool pool;
        private readonly SR sr;
        private readonly Dictionary<string, string> device_config;

        public MissingSRProblem(Check check, Pool pool, SR sr, Dictionary<string, string> device_config)
            : base(check)
        {
            this.pool = pool;
            this.sr = sr;
            this.device_config = device_config;
        }

        public override string Title
        {
            get { return Check.Description; }
        }


        public override string Description
        {
            get
            {
                return String.Format(sr.shared ? Messages.DR_WIZARD_PROBLEM_MISSING_SR : Messages.DR_WIZARD_PROBLEM_LOCAL_STORAGE, sr.Name);
            }
        }

        public override string HelpMessage
        {
            get
            {
                return sr.shared ? Messages.DR_WIZARD_PROBLEM_MISSING_SR_HELPMESSAGE : "";
            }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();
            cancelled = false;

            if (!sr.shared)
            {
                return null;
            }

            if (device_config == null) //no device config, we need to run the New SR wizard
            {
                NewSRWizard wizard = new NewSRWizard(pool.Connection, sr, true);
                wizard.ShowDialog(Program.MainWindow);
                return wizard.FinalAction;
            }

            Host master = pool.Connection.Resolve(pool.master);
            if (master == null)
            {
                return null;
            }

            ScannedDeviceInfo deviceInfo = new ScannedDeviceInfo(sr.GetSRType(true), device_config, sr.uuid);

            return new DrTaskCreateAction(pool.Connection, deviceInfo);
        }
    }
}
