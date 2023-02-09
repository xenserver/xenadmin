/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Windows.Forms;
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions;
using XenAdmin.Core;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public class BrokenSR : HostProblem
    {
        public BrokenSR(Check check, SR sr, Host host)
            : base(check, host)
        {
            Sr = sr;
        }

        public SR Sr { get; }

        public override string Description =>
            string.Format(Messages.UPDATES_WIZARD_BROKEN_STORAGE, ServerName, Sr.NameWithoutHost());

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();

            RepairSRDialog dlg = new RepairSRDialog(Sr, false);
            if (dlg.ShowDialog(Program.MainWindow) == DialogResult.OK)
            {
                cancelled = false;
                return dlg.RepairAction;
            }

            cancelled = true;
            return null;
        }

        public override string HelpMessage => Messages.REPAIR_SR;
    }

    class BrokenSRWarning : Warning
    {
        private readonly Host host;
        private readonly SR sr;

        public BrokenSRWarning(Check check, Host host, SR sr)
            : base(check)
        {
            this.sr = sr;
            this.host = host;
        }

        public override string Title => Check.Description;

        public override string Description =>
            string.Format(Messages.UPDATES_WIZARD_BROKEN_SR_WARNING, Helpers.GetName(host).Ellipsise(30), sr);
    }
}
