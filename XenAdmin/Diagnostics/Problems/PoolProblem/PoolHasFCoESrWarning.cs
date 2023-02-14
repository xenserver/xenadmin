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
using System.Diagnostics;
using System.Reflection;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    internal class PoolHasFCoESrWarning : WarningWithMoreInfo
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly Pool _pool;
        private readonly bool _upgradingToVersionWithDeprecation;

        public override string Title => Check.Description;
        public override string HelpMessage => Messages.MORE_INFO;
        public override string LinkText => Messages.LEARN_MORE;
        public override string LinkData => InvisibleMessages.PV_GUESTS_CHECK_URL;
        public override string Message => string.Empty;

        public override string Description => string.Format(_upgradingToVersionWithDeprecation ? Messages.POOL_HAS_DEPRECATED_FCOE_WARNING : Messages.POOL_MAY_HAVE_DEPRECATED_FCOE_WARNING,
            _pool,
            BrandManager.ProductVersionPost82
        );

        public PoolHasFCoESrWarning(Check check, Pool pool, bool upgradingToVersionWithDeprecation) : base(check)
        {
            _pool = pool;
            _upgradingToVersionWithDeprecation = upgradingToVersionWithDeprecation;
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            try
            {
                Process.Start(InvisibleMessages.FCOE_SR_DEPRECATION_URL);
            }
            catch(Exception e)
            {
                Log.Error($"Error while attempting to open {InvisibleMessages.FCOE_SR_DEPRECATION_URL}.", e);
                using (var dlg = new WarningDialog(string.Format(Messages.COULD_NOT_OPEN_URL, InvisibleMessages.FCOE_SR_DEPRECATION_URL)))
                {
                    dlg.Show();
                }
            }
            
            cancelled = true;
            return null;
        }
    }
}