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

using System.Collections.Generic;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;

namespace XenAdmin.Diagnostics.Problems.UtilityProblem
{
    internal class EuaNotAcceptedProblem : Problem
    {
        private readonly List<string> _euas;
        private readonly Check _check;
        private readonly Control _control;
        public EuaNotAcceptedProblem(Control control, Check check, List<string> euas)
            : base(check)
        {
            _control = control;
            _check = check;
            _euas = euas;
        }

        public override string Description => Messages.ACCEPT_EUA_PROBLEM_DESCRIPTION;

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            using (var d = new AcceptEuaDialog(_euas))
            {
                _check.Completed = d.ShowDialog(_control) == DialogResult.Yes;
            }
            cancelled = true;
            return null;
        }

        public override string HelpMessage => Messages.ACCEPT_EUA_PROBLEM_HELP_MESSAGE;

        public sealed override string Title => string.Empty;

        public override bool Equals(object obj)
        {
            if (!(obj is EuaNotAcceptedProblem item))
            {
                return false;
            }

            return _euas.Equals(item._euas) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _euas.GetHashCode() ^ base.GetHashCode();
        }
    }
}
