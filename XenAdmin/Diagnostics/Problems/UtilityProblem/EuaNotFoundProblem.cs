﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.UtilityProblem
{
    internal class EuaNotFoundProblem : Problem
    {
        private readonly List<IXenObject> _hosts;
        private readonly Check _check;
        public EuaNotFoundProblem(Check check, List<IXenObject> hosts)
            : base(check)
        {
            _check = check;
            _hosts = hosts;
        }

        public override string Description => string.Format(Messages.EUA_NOT_FOUND_PROBLEM_DESCRIPTION, BrandManager.BrandConsole);

        public sealed override string Title => string.Empty;

        public override string HelpMessage => Messages.MORE_INFO;

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                using (var dlg = new InformationDialog(Messages.PROBLEM_PREPARE_TO_UPGRADE))
                    dlg.ShowDialog();
            });

            cancelled = true;
            return null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EuaNotFoundProblem item))
            {
                return false;
            }

            return _hosts.SequenceEqual(item._hosts);
        }

        public override int GetHashCode()
        {
            return _hosts.GetHashCode() ^ base.GetHashCode();
        }
    }
}