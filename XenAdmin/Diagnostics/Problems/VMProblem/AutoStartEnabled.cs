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

using System;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class AutoStartEnabled : VMProblem
    {
        public AutoStartEnabled(Check check, VM vm)
            : base(check, vm)
        { }

        public override string Description
        {
            get { return string.Format(Messages.AUTOSTART_ENABLED_CHECK_DESCRIPTION, ServerName, Helpers.GetName(VM).Ellipsise(30)); }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new DelegatedAsyncAction(VM.Connection, Messages.ACTION_DISABLE_AUTOSTART_ON_VM, string.Format(Messages.ACTION_DISABLING_AUTOSTART_ON_VM, Helpers.GetName(VM)), null, ActionDelegate(false));
        }

        public override string HelpMessage
        {
            get { return Messages.DISABLE_NOAMP; }
        }

        private Action<Session> ActionDelegate(bool autostartValue)
        {
            return delegate(Session session)
                       {
                           var vmclone = (VM)VM.Clone();
                           VM.Locked = true;
                           vmclone.SetAutoPowerOn(autostartValue);
                           try
                           {
                               vmclone.SaveChanges(session);
                           }
                           finally
                           {
                               VM.Locked = false;
                           }
                           int wait = 5000; // wait up to 5 seconds for the cache to be updated
                           while (wait > 0 && VM.GetAutoPowerOn() != autostartValue)
                           {
                               Thread.Sleep(100);
                               wait -= 100;
                           }
                       };
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return new DelegatedAsyncAction(
               VM.Connection,
               Messages.ACTION_ENABLE_AUTOSTART_ON_VM,
               string.Format(Messages.ACTION_ENABLING_AUTOSTART_ON_VM, Helpers.GetName(VM)),
               null,
               ActionDelegate(true));
        }
    }
}
