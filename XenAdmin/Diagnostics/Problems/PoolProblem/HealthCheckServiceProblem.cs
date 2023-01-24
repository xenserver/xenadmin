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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class HealthCheckServiceProblem : PoolProblem
    {
        private Pool _pool;

        public HealthCheckServiceProblem(Check check, Pool pool)
            : base(check, pool)
        {
            _pool = pool;
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return DisableHealthCheckAction.Create(_pool);
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_HEALTH_CHECK_SERVICE_DESCRIPTION, _pool, BrandManager.ProductBrand, BrandManager.ProductVersionPost82);

        public override string HelpMessage => Messages.PROBLEM_HEALTH_CHECK_HELP;
    }

    class HealthCheckServiceWarning : WarningWithMoreInfo
    {
        private readonly Pool pool;

        public HealthCheckServiceWarning(Check check, Pool pool)
            : base(check)
        {
            this.pool = pool;
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_HEALTH_CHECK_SERVICE_DESCRIPTION, pool, BrandManager.ProductBrand, BrandManager.ProductVersionPost82);

        public override string Message =>
            string.Format(Messages.WARNING_HEALTH_CHECK_SERVICE_INFO, BrandManager.ProductBrand, BrandManager.ProductVersionPost82);

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            AsyncAction action = null;
            Program.Invoke(Program.MainWindow, () =>
            {
                using (var dlg = new WarningDialog(Message,
                    new ThreeButtonDialog.TBDButton(Messages.PROBLEM_HEALTH_CHECK_HELP, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.No)))
                {
                    if (dlg.ShowDialog() == DialogResult.Yes)
                        action = DisableHealthCheckAction.Create(pool);
                }
            });
            cancelled = action == null;
            return action;
        }
    }
}
