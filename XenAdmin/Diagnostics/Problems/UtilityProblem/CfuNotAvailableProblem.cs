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

using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;

namespace XenAdmin.Diagnostics.Problems.UtilityProblem
{
    class CfuNotAvailableProblem : Problem
    {
        public CfuNotAvailableProblem(Check check)
            : base(check)
        {
        }

        public override string Description
        {
            get { return Messages.UPGRADEWIZARD_PROBLEM_CFU_STATUS; }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            using (var dlg = new ThreeButtonDialog(SystemIcons.Warning, Messages.UPDATE_SERVER_NOT_REACHABLE))
            {
                dlg.ShowDialog();
            }

            cancelled = true;
            return null;
        }

        public override string HelpMessage => Messages.MORE_INFO;

        public sealed override string Title
        {
            get { return string.Empty; }
        }

        public override bool IsFixable
        {
            get { return false; }
        }
    }
}
