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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    internal class PoolHasVmsWithDmcProblem : ProblemWithMoreInfo
    {
        private readonly Pool _pool;
        private readonly List<VM> _vms;
        protected readonly Control Control;

        public PoolHasVmsWithDmcProblem(Control control, Check check, Pool pool, List<VM> vms)
            : base(check)
        {
            Control = control;
            _pool = pool;
            _vms = vms;
        }

        public override string Title => Description;

        public override string Description =>
            string.Format(Messages.DMC_REMOVAL_SHORT, _pool.Name(),
                string.Format(Messages.STRING_SPACE_STRING, BrandManager.ProductBrand, BrandManager.ProductVersionPost82));

        public override string Message =>
            string.Format(Messages.DMC_REMOVAL_LONG_ERROR,
                string.Format(Messages.STRING_SPACE_STRING, BrandManager.ProductBrand, BrandManager.ProductVersionPost82),
                BrandManager.BrandConsole);

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Control, delegate
            {
                using (var dlg = new ErrorDialog(Message,
                           new ThreeButtonDialog.TBDButton(Messages.DMC_DISABLE, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT),
                           new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, ThreeButtonDialog.ButtonType.CANCEL, true))
                {
                    LinkText = Messages.LEARN_MORE,
                    LinkData = InvisibleMessages.DMC_REMOVAL_URL,
                    ShowLinkLabel = true
                })
                {
                    if (dlg.ShowDialog(Control) == DialogResult.OK)
                        DisableDmc();
                }
            });

            cancelled = true;
            return null;
        }

        protected void DisableDmc()
        {
            var subActions = _vms.Select(DisableDmcPerVm).ToList();

            new ParallelAction(Messages.DMC_DISABLE_ACTION_TITLE, "", "",
                subActions, suppressHistory: true, showSubActionsDetails: true).RunAsync();
        }

        private AsyncAction DisableDmcPerVm(VM vm)
        {
            return new ChangeMemorySettingsAction(vm,
                string.Format(Messages.ACTION_CHANGE_MEMORY_SETTINGS, vm.Name()),
                vm.memory_static_min, vm.memory_dynamic_max, vm.memory_dynamic_max, vm.memory_dynamic_max,
                VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm, false);
        }
    }


    internal class PoolHasVmsWithDmcWarning : PoolHasVmsWithDmcProblem
    {
        public PoolHasVmsWithDmcWarning(Control control, Check check, Pool pool, List<VM> vms)
            : base(control, check, pool, vms)
        {
        }

        public override Image Image => Images.GetImage16For(Icons.Warning);

        public override string Message =>
            string.Format(Messages.DMC_REMOVAL_LONG_WARNING,
                string.Format(Messages.STRING_SPACE_STRING, BrandManager.ProductBrand, BrandManager.ProductVersionPost82),
                BrandManager.BrandConsole);

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Control, delegate
            {
                using (var dlg = new WarningDialog(Message,
                           new ThreeButtonDialog.TBDButton(Messages.DMC_DISABLE, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT),
                           new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, ThreeButtonDialog.ButtonType.CANCEL, true))
                {
                    LinkText = Messages.LEARN_MORE,
                    LinkData = InvisibleMessages.DMC_REMOVAL_URL,
                    ShowLinkLabel = true
                })
                {
                    if (dlg.ShowDialog(Control) == DialogResult.OK)
                        DisableDmc();
                }
            });

            cancelled = true;
            return null;
        }
    }
}
